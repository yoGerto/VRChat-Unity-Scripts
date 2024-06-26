
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.Core.ApiAvatar;

public class ToggleCubePool : UdonSharpBehaviour
{
    public TextMeshProUGUI debug;
    public GameObject[] cubeArray;
    [UdonSynced] public bool[] cubeArrayStatus;

    //private GameObject cubeFromPool;
    private VRCPlayerApi player;
    private bool hasCube = false;
    private int cubeLocalInt = -1;

    void Start()
    {
        // Create var to store local player (for ease of use)
        player = Networking.LocalPlayer;

        // Set correct length of cubeArrayStatus
        cubeArrayStatus = new bool[cubeArray.Length];

        // Populate cubeArrayStatus with 'false' value
        for (int i = 0; i < cubeArrayStatus.Length; i++)
        {
            cubeArrayStatus[i] = false;
        }
        
    }

    public override void Interact()
    {
        Debug.Log("Player has interacted with the toggle");

        // Set local player as owner of toggle, as this is required to use RequestSerialization
        Networking.SetOwner(player, this.gameObject);

        // Rather than looking at the Network Ownership of the GameObject, perhaps we can use if it is active or not
        for (int i = 0; i < cubeArray.Length; i++)
        {
            if (!hasCube) // If Player doesn't have a cube and wants one
            {
                if (!cubeArray[i].activeInHierarchy)
                {
                    cubeArrayStatus[i] = true;

                    cubeLocalInt = i;

                    // Set Ownership of GameObject to the interacting player
                    // This should be sycned over the network so its OK to do it here
                    Networking.SetOwner(player, cubeArray[i]);

                    // Serialise to sync cubeArrayStatus to all players
                    RequestSerialization();
                    OnDeserialization();

                    hasCube = true;

                    // Return here as we only want to assign one game object to player
                    return;
                }
            }
            else // If Player has a cube and wishes to return it
            {
                if (Networking.GetOwner(cubeArray[i]) == player) // Iterate through cubeArray and see if the Player owns the corresponding game object
                {
                    cubeArrayStatus[i] = false;

                    // No return here as it's more beneficial to continue checking each game object
                    // Need to test what happens when a player who owns a game object leaves the world
                    // Does ownership of their game object get passed to the Master or to a random player?
                }
            }
        }

        // These need to be done here as it can't be done inside of the else statement
        // If the execution makes it to this point, it must be because either:
        // The player was returning their cube(s) and the else condition finished iterating
        // The for loop could not find an available cube to assign to the player so they should be treated as if they don't have a cube

        hasCube = false;

        // Serialise to sync cubeArrayStatus to all players
        RequestSerialization();
        OnDeserialization();

    }

    public override void OnDeserialization()
    {
        for (int i = 0; i < cubeArray.Length; i++)
        {
            // use the cubeArrayStatus bool to set the active state of each cube in cubeArray
            cubeArray[i].SetActive(cubeArrayStatus[i]);
        }
    }


    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        
        Debug.Log("A player has left the instance");

        SendCustomEventDelayedSeconds("PreCubeCleanUp", 0.1f);

        SendCustomEventDelayedSeconds("CubeCleanUp", 1.0f);

    }

    public void PreCubeCleanUp()
    {
        Debug.Log("Running PreCubeCleanUp...");

        if (Networking.IsMaster)
        {
            Debug.Log("This player is the Instance Master");
            // Need to transfer ownership of any cubes that are not loaned by the Instance Master to the player who owns the Pool Toggle
            for (int i = 0; i < cubeArray.Length; i++)
            {
                Debug.Log("i = " + i);
                // If Instance Master owns the cube
                if (Networking.GetOwner(cubeArray[i]) == player)
                {
                    Debug.Log("cubeArray[" + i + "] is owned by current player");
                    // If the cube is not one that we've loaned
                    if (i != cubeLocalInt)
                    {
                        Debug.Log("cubeArray[" + i + "] is not the cube we loaned");
                        // If player who owns the Pool Toggle is not also the Instance Master
                        // - Attempting to transfer ownership of an object that you already own to yourself raises a network warning
                        if (Networking.GetOwner(this.gameObject) != player)
                        {
                            Debug.Log("The Instance Master does NOT own the pool toggle");
                            // Transfer ownership of the cube to the Pool Toggle owner
                            Networking.SetOwner(Networking.GetOwner(this.gameObject), cubeArray[i]);
                            Debug.Log("Ownership of cubeArray[" + i + "] has been transfered to the pool toggle owner");
                        }
                        else
                        {
                            Debug.Log("The Instance Master already owns the pool toggle, so we do not need to transfer ownership");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("This player is NOT the Instance Master!");
            Debug.Log("Skipping...");
        }

        Debug.Log("Ownership of the errand cube(s) should now be with the Pool Toggle owner");
    }

    public void CubeCleanUp()
    {
        Debug.Log("Running CubeCleanUp...");

        Debug.Log("cubeLocalInt = " + cubeLocalInt);

        // Iterate through cubeArray
        for (int i = 0; i < cubeArray.Length; i++)
        {
            Debug.Log("i = " + i);
            // Check ownership of each object in array
            if (Networking.GetOwner(cubeArray[i]) == player)
            {
                Debug.Log("cubeArray[" + i + "] is owned by current player");
                // We are only interested in cubes that we have not loaned from the pool
                if (i != cubeLocalInt)
                {
                    Debug.Log("cubeArray[" + i + "] is not the cube we loaned");
                    // If it is a cube we have not loaned then set corresponding cubeArrayStatus to false
                    // When this is serialized, all players should be up to date on which cube(s) are available to loan
                    if (cubeArray[i].activeInHierarchy)
                    {
                        Debug.Log("cubeArray[" + i + "] is active in the scene");
                        Debug.Log("Setting cubeArray[" + i + "] status to false ");
                        cubeArrayStatus[i] = false;
                    }
                    else
                    {
                        Debug.Log("cubeArray[" + i + "] is already inactive, so no further changes needed");
                    }
                }
                else
                {
                    Debug.Log("cubeArray[" + i + "] IS the cube we loaned");
                }
            }
            else
            {
                Debug.Log("cubeArray[" + i + "] is NOT owned by current player");
            }
        }

        // Serialise to sync cubeArrayStatus to all players
        RequestSerialization();
        OnDeserialization();
    }


    public void Update()
    {

    }
}
