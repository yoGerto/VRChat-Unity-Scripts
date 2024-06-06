
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
    private readonly float  jumpVelocity = 3.0f;
    private VRCPlayerApi    player;
    private bool            spaceIsHeld;

    float                   playerMoveHorizontal = 0.0f;
    float                   playerMoveVertical = 0.0f;
    float[]                 defaultMovementSettings = new float[4];

    public TextMeshProUGUI  rotationText;
    Vector3 currentRotation;

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

        float ymovement = 6.0f;

        Vector3 playerMovementVector = player.GetVelocity();

        SetMovementParams(player.IsPlayerGrounded());

        if (!player.IsPlayerGrounded())
        {
            if (playerMoveHorizontal == 0 && playerMoveVertical == 0)
            {
                playerMovementVector[0] = playerMovementVector[0] * (float)0.95;
                playerMovementVector[2] = playerMovementVector[2] * (float)0.95;
            }
            else
            {

                rotationText.text = Time.deltaTime.ToString();

                float movementlimit = 3.0f;

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
            }

            if (spaceIsHeld)
            {
                playerMovementVector[1] += ymovement * Time.deltaTime;
            }
            else
            {
                playerMovementVector[1] -= ymovement * Time.deltaTime;
            }

            if (playerMovementVector[1] > 3)
            {
                playerMovementVector[1] = 3.0f;
            }
            player.SetVelocity(playerMovementVector);
        }
    }

}
