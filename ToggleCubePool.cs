
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

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
        /*
        if (player != Networking.Master)
        {
            // This guard prevents any non-master players from running this code
            return;
        }
        */

        if (Networking.GetOwner(this.gameObject) != player)
        {
            // In practice it seems that only the owner of the cubePool should be the one to run this cleanup code
            // So this guard exists as an explicit rejection of the non-owner players
            // This is because if multiple people try to set the UdonSync variable it could cause some hijinks 
            return;
        }

        // This is my attempt at writing some cleanup code
        // My working assumption is that the Instance Master recieves ownership of the Game Objects of the leaving player
        // However, I will need to test how this code works when the Master leaves the game instance
        // Because there could potentially be a race condition

        /*
        // Iterate through cubeArray
        for (int i = 0; i < cubeArray.Length; i++)
        {
            // Check ownership of each object in array
            if (Networking.GetOwner(cubeArray[i]) == player)
            {
                // We are only interested in cubes that we have not loaned from the pool
                if (i != cubeLocalInt)
                {
                    // If it is a cube we have not loaned, then set to Inactive and update cubeArrayStatus accordingly
                    // This marks the cube as available to be 'loaned' again and every player, after Serialization, will also have the cube as inactive
                    cubeArray[i].SetActive(false);
                    cubeArrayStatus[i] = false;
                }
            }
        }
        */

        // Could the problem be that I don't own the cube pool toggle?

        // Iterate through cubeArray
        for (int i = 0; i < cubeArray.Length; i++)
        {
            // Check ownership of each object in array
            if (Networking.GetOwner(cubeArray[i]) == player)
            {
                // We are only interested in cubes that we have not loaned from the pool
                if (i != cubeLocalInt)
                {
                    // If it is a cube we have not loaned then set corresponding cubeArrayStatus to false
                    // When this is serialized, all players should be up to date on which cube(s) are available to loan
                    cubeArrayStatus[i] = false;
                }
            }
        }

        // Serialise to sync cubeArrayStatus to all players
        RequestSerialization();
        OnDeserialization();
    }


    public void Update()
    {
        /*
        debug.text = Networking.IsMaster.ToString();
        debug.text += "\n";
        for (int i = 0;i < cubeArray.Length; i++)
        {
            debug.text += cubeArrayStatus[i].ToString() + " ";
        }
        debug.text += "\n";
        */
    }
}
