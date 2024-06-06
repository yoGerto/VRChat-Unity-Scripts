
using Newtonsoft.Json.Linq;
using System.Collections;
using TMPro;
using UdonSharp;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using VRC.SDK3.ClientSim;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Serialization.OdinSerializer;

public class DoubleJump : UdonSharpBehaviour
{
    //if i want to add a double jump, i will need to figure out when the player has jumped
    //if the player presses space, can we assume they have jumped?
    //how can the player jump again whilst in the air?

    //private bool        canJumpAgain = false;
    private readonly float  jumpVelocity = 3.0f;
    private VRCPlayerApi    player;
    private bool            spaceIsHeld;
    bool                    rotating = false;
    bool                    bhop = false;
    bool                    airJumps = true;
    bool                    movementParamsGrounded = true;
    float                   playerMoveHorizontal = 0.0f;
    float                   playerMoveVertical = 0.0f;
    float[]                 defaultMovementSettings = new float[4];

    //Vector2 player2DMovement = new Vector2(0.0f, 0.0f);

    float timer = 0.0f;

    public TextMeshProUGUI  rotationText;
    Vector3 currentRotation;
    Vector3 previousRotation;

    void Start()
    {
        player = Networking.LocalPlayer;
        player.SetGravityStrength(1);

        defaultMovementSettings[0] = 3; //for some reason getting the default is not working here
        defaultMovementSettings[1] = 3;
        defaultMovementSettings[2] = 4;
        defaultMovementSettings[3] = 3;

        player.SetJumpImpulse(defaultMovementSettings[0]);
        player.SetWalkSpeed(defaultMovementSettings[1]);
        player.SetRunSpeed(defaultMovementSettings[2]);
        player.SetStrafeSpeed(defaultMovementSettings[3]);
    }

    
    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        spaceIsHeld = value;
    }

    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
        playerMoveHorizontal = value;
    }

    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        playerMoveVertical = value;
    }

    public void SetMovementParams(bool state)
    {
        if (state) //true means player is grounded
        {
            player.SetJumpImpulse(defaultMovementSettings[0]);
            player.SetWalkSpeed(defaultMovementSettings[1]);
            player.SetRunSpeed(defaultMovementSettings[2]);
            player.SetStrafeSpeed(defaultMovementSettings[3]);
            player.SetGravityStrength(1);
        }
        else
        {
            player.SetJumpImpulse(0);
            player.SetWalkSpeed(0);
            player.SetRunSpeed(0);
            player.SetStrafeSpeed(0);
            player.SetGravityStrength(0);
        }
    }


    private void Update()
    {
        currentRotation = player.GetRotation().eulerAngles;
        float currentRotationRadians = currentRotation[1] * Mathf.PI / 180;
        float sinOfRotation = Mathf.Sin(currentRotationRadians);
        float cosinOfRotation = Mathf.Cos(currentRotationRadians);

        Vector3 playerMovementVector = player.GetVelocity();

        SetMovementParams(player.IsPlayerGrounded());

        if (!player.IsPlayerGrounded())
        {
            if (playerMoveHorizontal == 0 && playerMoveVertical == 0)
            {
                playerMovementVector[0] = playerMovementVector[0] * (float)0.95;
                playerMovementVector[2] = playerMovementVector[2] * (float)0.95;
                //timer = 0.0f;
            }
            else
            {
                //timer += Time.deltaTime;
                //timer = Mathf.Clamp(timer, 0.0f, 3.0f);
                //float movement = 0.1f;

                rotationText.text = Time.deltaTime.ToString();

                float movementlimit = 3.0f;
                float ymovement = playerMovementVector[1];

                playerMovementVector[1] = 0.0f;

                float xlimit = (movementlimit * cosinOfRotation * playerMoveHorizontal) + (movementlimit * sinOfRotation * playerMoveVertical);
                float ylimit = (movementlimit * sinOfRotation * -1 * playerMoveHorizontal) + (movementlimit * cosinOfRotation * playerMoveVertical);

                if (!(playerMovementVector[0] == xlimit))
                {
                    float difference = xlimit - playerMovementVector[0];
                    playerMovementVector[0] += (difference * (float)0.1);
                }
                if (!(playerMovementVector[2] == ylimit))
                {
                    float difference = ylimit - playerMovementVector[2];
                    playerMovementVector[2] += (difference * (float)0.1);
                }

                playerMovementVector[1] = ymovement;
            }

            if (spaceIsHeld)
            {
                playerMovementVector[1] += 0.1f;
            }
            else
            {
                playerMovementVector[1] -= 0.1f;
            }

            if (playerMovementVector[1] > 3)
            {
                playerMovementVector[1] = 3.0f;
            }
            player.SetVelocity(playerMovementVector);
        }

        /*
        rotationText.text = playerMovementVector[0].ToString("0.0000") + "\n" + playerMovementVector[1].ToString("0.0000") + "\n" + playerMovementVector[2].ToString("0.0000");
        if (playerMovementVector[0] == 0 && playerMovementVector[2] == 0)
        {
            rotationText.text += "\n" + "Stationary";
        }
        else
        {
            rotationText.text += "\n" + "Moving";
        }

        if (player.IsPlayerGrounded())
        {
            rotationText.text += "\n" + "Grounded";
        }
        else
        {
            rotationText.text += "\n" + "Airborne";
        }

        rotationText.text += "\n" + playerMoveHorizontal;
        rotationText.text += "\n" + playerMoveVertical;
        */
    }

}

/*
    private void MyJump()
    {
        Vector3 currentVelocity;

        currentVelocity = player.GetVelocity();
        currentVelocity[1] = jumpVelocity;

        player.SetVelocity(currentVelocity);

        //Debug.Log("Grounded JUMP!");
        //Debug.Log(currentVelocity[1]);
    }

    private void Jetpack()
    {
        Vector3 currentVelocity;

        currentVelocity = player.GetVelocity();
        currentVelocity[1] += 1.0f;

        if (currentVelocity[1] > 3.0f)
        {
            currentVelocity[1] = 3.0f;
        }

        player.SetVelocity(currentVelocity);
    }
*/

//player.SetVelocity(playerMovementVector);

/*
if (spaceIsHeld)
{
    Debug.Log("Space is being held");
    if (player.IsPlayerGrounded())
    {
        MyJump();
    }
}
*/

//previousRotation = currentRotation;

//x axis movement will be the horizontal component + vertical component, then normalized
//playerMovementVector[0] += movement * cosinOfRotation * playerMoveHorizontal;
//playerMovementVector[0] += movement * sinOfRotation * playerMoveVertical;

//playerMovementVector[2] += movement * sinOfRotation * -1 * playerMoveHorizontal;
//playerMovementVector[2] += movement * cosinOfRotation * playerMoveVertical;

//problem i am having is trying to use the vector normalization is causing the polarity of movement to not flip when changing movement direction
//so a possible fix to this is to not use the normalization, but to instead calculate the maximum values that should be set when moving
//this can likely be calculated by using the players rotation and their movement floats
//however, it should be noted that the client sim and VRChat return slightly different things when players are moving vertically and horizontally
//client sim returns 1 and 1, where as VRChat returns 0.707 and 0.707

/*
if (playerMovementVector[0] < xlimit)
{
    float difference = xlimit - playerMovementVector[0]; //xlimit 3, pMV 0, difference = 3
    playerMovementVector[0] += (difference * (float)0.1); //+ (3*0.5) = +1.5
}
else if (playerMovementVector[0] > xlimit)
{
    float difference = xlimit - playerMovementVector[0]; //xlimit 3, PMV 4.3, difference = -1.3
    playerMovementVector[0] += (difference * (float)0.1); // + (-1.3*0.5) = -0.65
}

if (playerMovementVector[2] < ylimit)
{
    float difference = ylimit - playerMovementVector[2]; //xlimit 3, pMV 0, difference = 3
    playerMovementVector[2] += (difference * (float)0.1); //+ (3*0.5) = +1.5
}
else if (playerMovementVector[2] > ylimit)
{
    float difference = ylimit - playerMovementVector[2]; //xlimit 3, PMV 4.3, difference = -1.3
    playerMovementVector[2] += (difference * (float)0.1); // + (-1.3*0.5) = -0.65
}
*/


/*
if (xlimit > 0.0f)
{
    if (playerMovementVector[0] > xlimit) //xlimit 3, pMV 4, difference = 1
    {
        float difference = playerMovementVector[0] - xlimit;
        playerMovementVector[0] = xlimit + (difference * (float)0.8);
    }
}
else if (xlimit < 0.0f)
{
    if (playerMovementVector[0] < xlimit) //xlimit -3, PMV -4 difference -1
    {
        float difference = playerMovementVector[0] - xlimit;
        playerMovementVector[0] = xlimit + (difference * (float)0.8);
    }
}
else
{
    playerMovementVector[0] = playerMovementVector[0] * (float)0.8;
}

if (ylimit > 0.0f)
{
    if (playerMovementVector[2] > ylimit) //xlimit 3, pMV 4, difference = 1
    {
        float difference = playerMovementVector[2] - ylimit;
        playerMovementVector[2] = ylimit + (difference * (float)0.8);
    }
}
else if (ylimit < 0.0f)
{
    if (playerMovementVector[2] < ylimit) //xlimit -3, PMV -4 difference -1
    {
        float difference = playerMovementVector[2] - ylimit;
        playerMovementVector[2] = ylimit + (difference * (float)0.8);
    }
}
else
{
    playerMovementVector[2] = playerMovementVector[2] * (float)0.8;
}
*/





/*
//playerMovementVector[1] -= 100.0f;

//if (player.IsPlayerGrounded())
//{
    if (playerMoveHorizontal == 0 && playerMoveVertical == 0) //i.e if the player is not moving
    {
        //ok so this basically doesnt work
        //reason being is because if the player is grounded and they have velocity and ARENT input movement keys, the game instantly reduces velocity to 0
        //this code had sort of worked until now because the player was fluctuating between being airborne and being grounded rapidly, many times a second
        //when you force the player to be grounded, they always stop instantly
        timer = 0.0f;
        playerMovementVector[0] = playerMovementVector[0] * (float)0.75;
        playerMovementVector[2] = playerMovementVector[2] * (float)0.75;
    }
    else //i.e. if the player is moving (or pressing movement keys)
    {
        float movement = 0.5f;
        //x axis movement will be the horizontal component + vertical component, then normalized
        playerMovementVector[0] += movement * cosinOfRotation * playerMoveHorizontal;
        playerMovementVector[0] += movement * sinOfRotation * playerMoveVertical;

        playerMovementVector[2] += movement * sinOfRotation * -1 * playerMoveHorizontal;
        playerMovementVector[2] += movement * cosinOfRotation * playerMoveVertical;

        timer += Time.deltaTime;

        playerMovementVector = playerMovementVector.normalized * Mathf.Clamp(timer * 10, 0, 3);
    }
    //playerMovementVector[1] -= 100.0f;
    //playerMovementVector[1] = -0.01f;
    player.SetVelocity(playerMovementVector);
//}
*/

/*
 *                 if (playerMoveHorizontal > 0)
                {
                    playerMovementVector[0] += movement * cosinOfRotation;
                    playerMovementVector[2] += movement * sinOfRotation * -1;
                }
                else if (playerMoveHorizontal < 0)
                {
                    playerMovementVector[0] -= movement * cosinOfRotation;
                    playerMovementVector[2] -= movement * sinOfRotation * -1;
                }

                if (playerMoveVertical > 0)
                {
                    playerMovementVector[0] += movement * sinOfRotation;
                    playerMovementVector[2] += movement * cosinOfRotation;
                }
                else if (playerMoveVertical < 0)
                {
                    playerMovementVector[0] -= movement * sinOfRotation;
                    playerMovementVector[2] -= movement * cosinOfRotation;
                }
*/

/*
         if (playerMoveHorizontal == 0 && playerMoveVertical == 0)
        {
            playerMovementVector[0] = playerMovementVector[0] * (float)0.8;
            playerMovementVector[2] = playerMovementVector[2] * (float)0.8;
            timer = 0.0f;
        }
        else
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, 3.0f);
        }

        if (!player.IsPlayerGrounded())
        {
            float movement = 0.1f;
            //x axis movement will be the horizontal component + vertical component, then normalized
            playerMovementVector[0] += movement * cosinOfRotation * playerMoveHorizontal;
            playerMovementVector[0] += movement * sinOfRotation * playerMoveVertical;

            playerMovementVector[2] += movement * sinOfRotation * -1 * playerMoveHorizontal;
            playerMovementVector[2] += movement * cosinOfRotation * playerMoveVertical;

            //playerMovementVector[0] = Mathf.Clamp(playerMovementVector[0], -3, 3);
            //playerMovementVector[2] = Mathf.Clamp(playerMovementVector[2], -3, 3);

            Vector2 player2DMovement = new Vector2(playerMovementVector[0], playerMovementVector[2]);
            player2DMovement = player2DMovement.normalized * Mathf.Clamp(timer * 10, 0, 3);

            playerMovementVector[0] = player2DMovement[0];
            playerMovementVector[2] = player2DMovement[1];


            if (spaceIsHeld)
            {
                playerMovementVector[1] += 0.1f;
            }
            else
            {
                playerMovementVector[1] -= 0.05f;
            }

            if (playerMovementVector[1] > 3)
            {
                playerMovementVector[1] = 3.0f;
            }
            player.SetVelocity(playerMovementVector);
        }
*/

/*
float movement = 0.5f;
//Vector2 player2DMovement = new Vector2(0.0f, 0.0f);
player2DMovement.x = playerMovementVector.x;
player2DMovement.y = playerMovementVector.z; //y component of Vector2 is z component of Vector3!!!! Dont forget!!!

//x axis movement will be the horizontal component + vertical component, then normalized
player2DMovement.x += movement * cosinOfRotation * playerMoveHorizontal;
player2DMovement.x += movement * sinOfRotation * playerMoveVertical;

player2DMovement.y += movement * sinOfRotation * -1 * playerMoveHorizontal;
player2DMovement.y += movement * cosinOfRotation * playerMoveVertical;

player2DMovement = player2DMovement.normalized * Mathf.Clamp(timer, 0, 3);

Debug.Log(player2DMovement);

playerMovementVector.x = player2DMovement.x;
playerMovementVector.z = player2DMovement.y; //y component of Vector2 is z component of Vector3!!!! Dont forget!!!

playerMovementVector.y -= 0.005f;
player.SetVelocity(playerMovementVector);
*/


/*
if (!player.IsPlayerGrounded())
{
    playerMovementVector[1] -= 0.1f;  
    //player.SetVelocity(playerMovementVector);
}
else
{
    playerMovementVector[1] = -0.01f;
}
*/



//rotationText.text = currentRotation[1].ToString() + "\n" + currentRotationRadians.ToString() + "\n" + sinOfRotation.ToString();
//rotationText.text = currentRotation[1].ToString();
//rotationText.text += "\n" + previousRotation[1].ToString();
/*
if (rotating == false)
{
    rotationText.text = "Still";

    if (currentRotation[1] > previousRotation[1])
    {
        rotationText.text = "Turning Right";
        rotating = true;
    }
    if (currentRotation[1] < previousRotation[1])
    {
        rotationText.text = "Turning Left";
        rotating = true;
    }
}
else if (rotating == true)
{
    if (currentRotation[1] == previousRotation[1])
    {
        rotationText.text = "Still";
        rotating = false;
    }
}
*/

/*
if (bhop)
{
    if (spaceIsHeld)
    {
        Debug.Log("Space is being held");
        if (player.IsPlayerGrounded())
        {
            MyJump();
        }
    }
}
*/


/*
if (spaceIsHeld)
{
    Jetpack();
}

*/

//Vector3 playerMovementVector = new Vector3(0.0f, 0.0f, 0.0f);

/* Current Horizontal implementation

        if (playerMoveHorizontal == -1)
        {
            float movement = -0.5f;
            playerMovementVector[0] += movement * cosinOfRotation;
            playerMovementVector[2] += movement * sinOfRotation * -1;

            timer += Time.deltaTime;

            playerMovementVector = playerMovementVector.normalized * Mathf.Clamp(timer * 10, 0, 3);
        }
        else if (playerMoveHorizontal == 1)
        {
            float movement = 0.5f;
            playerMovementVector[0] += movement * cosinOfRotation;
            playerMovementVector[2] += movement * sinOfRotation * -1;

            timer += Time.deltaTime;

            playerMovementVector = playerMovementVector.normalized * Mathf.Clamp(timer * 10, 0, 3);

        }
        else
        {
            timer = 0.0f;
            playerMovementVector[0] = playerMovementVector[0] * (float)0.75;
            playerMovementVector[2] = playerMovementVector[2] * (float)0.75;
        }
        */

/* Current Vertical Implementation

if (playerMoveVertical == -1)
{
    float movement = -0.5f;
    playerMovementVector[0] += movement * sinOfRotation;
    playerMovementVector[2] += movement * cosinOfRotation;

    timer += Time.deltaTime;

    playerMovementVector = playerMovementVector.normalized * Mathf.Clamp(timer * 10, 0, 3);
}
else if (playerMoveVertical == 1)
{
    float movement = 0.5f;
    playerMovementVector[0] += movement * sinOfRotation;
    playerMovementVector[2] += movement * cosinOfRotation;

    timer += Time.deltaTime;

    playerMovementVector = playerMovementVector.normalized * Mathf.Clamp(timer * 10, 0, 3);

}
else
{
    timer = 0.0f;
    playerMovementVector[0] = playerMovementVector[0] * (float)0.75;
    playerMovementVector[2] = playerMovementVector[2] * (float)0.75;
}
*/
