
using BestHTTP.SecureProtocol.Org.BouncyCastle.Cms;
using UdonSharp;
using UnityEngine;
using UnityEngine.ProBuilder;
using VRC.SDK3.ClientSim;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class AutoRespawnBox : UdonSharpBehaviour
{
    //TODO:     Change this section so that only the GameObjects are declared as public variables
    //          We can use the GameObjects to populate the variables with references to the Components on the GameObject
    //          Use the GetComponent method from the GameObject class to do this!
    public GameObject       flyingCube;
    public Rigidbody        flyingCubeRigidbody;
    public Transform        flyingCubeTransform;
    public MeshRenderer     flyingCubeMeshRenderer;
    public Material[]       flyingCubeMaterials; //green, yellow, blue, red
    public VRCObjectSync    flyingCubeObjectSync;
    public GameObject       spawnBoxRadius;

    float                   flyingCubeVelocity;
    float                   timer = 0.0f;
    Vector3                 flyingCubeDefaultPos;
    [UdonSynced]bool        objectInteractStatus = false;

    Vector3                 spawnBoxRadiusTransform;
    Vector3                 spawnBoxRadiusCollider;

    void Start()
    {
        flyingCubeMeshRenderer = flyingCube.GetComponent<MeshRenderer>();
        spawnBoxRadiusTransform = spawnBoxRadius.GetComponent<Transform>().position;
        spawnBoxRadiusCollider = spawnBoxRadius.GetComponent<BoxCollider>().size;
        flyingCubeDefaultPos = new Vector3(5.0f, 1.0f, -1.0f);
        //flyingCubeRigidbody = flyingCube.GetComponent<Rigidbody>();
    }

    public override void OnPickup()
    {
        objectInteractStatus = true;
    }

    public override void OnDrop()
    {
        objectInteractStatus = false;
    }


    private bool CheckIfBoxIsInsideRadius(Vector3 transform, Vector3 size, Vector3 boxcoords)
    {
        for (int coord = 0; coord < 3; coord++)
        {
            if ((boxcoords[coord] > transform[coord] + (size[coord]/2)) || (boxcoords[coord] < transform[coord] - (size[coord]/2)))
            {
                return false;
            }
        }
        return true;
    }

    private void RespawnFlyingCube(int reason)
    {
        Networking.SetOwner(Networking.LocalPlayer, flyingCubeObjectSync.gameObject);
        flyingCubeObjectSync.Respawn();
        timer = 0.0f;

        //also whilst we are respawning, we could try changing the colour to green to see if it is seamless
        flyingCubeMeshRenderer.material = flyingCubeMaterials[0];

        //we can also print a message to the debug log depending on the 'reason' passed into the function
        //TODO: add this functionality :)

    }

    private void Update()
    {
        flyingCubeVelocity = flyingCubeRigidbody.velocity.magnitude;


        if (objectInteractStatus == true)
        {
            //if object is being interacted with, reset timer to 0
            timer = 0.0f;
            //also change colour to yellow :D
            flyingCubeMeshRenderer.material = flyingCubeMaterials[1];
        }
        else
        {
            //if object is moving, assume it is airborne (this is a somewhat faulty assumption)
            if (flyingCubeVelocity > 0.1f)
            {
                //increase timer by deltatime each frame
                timer += Time.deltaTime;
                //also change colour to RED :O
                flyingCubeMeshRenderer.material = flyingCubeMaterials[3];
                if (timer > 5.0f)
                {
                    //if time spent airborne > 5s, respawn object and reset timer
                    RespawnFlyingCube(0);
                }
            }
            else if (CheckIfBoxIsInsideRadius(spawnBoxRadiusTransform, spawnBoxRadiusCollider, flyingCubeTransform.position) == false)
            {
                //if object is not inside the spawn radius box, increase timer
                timer += Time.deltaTime;
                //also change colour to Blue :3
                flyingCubeMeshRenderer.material = flyingCubeMaterials[2];
                if (timer > 15.0f)
                {
                    //if time spent outside spawn radius box > 15s, respawn box and reset timer
                    RespawnFlyingCube(1);
                }
                
            }
            else
            {
                //if neither condition is met, the box is stationary and inside the spawn radius box, so keep timer at 0.
                timer = 0.0f;
                //also change the colour to green :D
                flyingCubeMeshRenderer.material = flyingCubeMaterials[0];
            }
        }

        //Debug.Log(flyingCubeVelocity);
        //Debug.Log(timer);

    }



}
