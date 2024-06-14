
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

    private GameObject cubeFromPool;
    private VRCPlayerApi player;
    private bool hasCube = false;
    private int cubeIter = -1;

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

                    // Set Ownership of GameObject to the player that interacting player
                    // This should be sycned over the network so can be done here instead
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

        // These need to be done here as it can't be done inside of the else
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
