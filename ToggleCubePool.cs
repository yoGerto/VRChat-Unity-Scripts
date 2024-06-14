
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
    //private bool tracking = false;

    void Start()
    {
        // Create var to store local player (for ease of use)
        player = Networking.LocalPlayer;

        // Set correct length of cubeArrayStatus
        cubeArrayStatus = new bool[cubeArray.Length];

        // Populate cubeArrayStatus with false value
        
        for (int i = 0; i < cubeArrayStatus.Length; i++)
        {
            cubeArrayStatus[i] = false;
        }
        
    }

    public override void Interact()
    {
        if (Networking.GetOwner(this.gameObject) != player)
        {
            Networking.SetOwner(player, this.gameObject);
        }

        if (!hasCube)
        {
            cubeArrayStatus[0] = true;
            cubeFromPool = cubeArray[0];
            Networking.SetOwner(player, cubeFromPool);
            RequestSerialization();
            OnDeserialization();
        }
        else
        {
            cubeFromPool.SetActive(false);
        }
        /*
        // Rather than looking at the Network Ownership of the GameObject, perhaps we can use if it is active or not
        for (int i = 0; i < cubeArray.Length; i++)
        {
            if (!cubeArray[i].activeInHierarchy)
            {
                cubeArrayStatus[i] = true;

                // Store cubeArray index in local variable for later use?
                cubeFromPool = cubeArray[i];

                // Set Ownership of GameObject to the player that interacting player
                // This should be sycned over the network so can be done here instead
                Networking.SetOwner(player, cubeFromPool);

                // Serialise to sync cubeArrayStatus
                RequestSerialization();
                OnDeserialization();

                // Exit the for loop
                // In theory we should return some Int value so we know the status of the request
                // As it is theoretically possible to run out of cube objects available
                // Or for some error to occur
                return;
            }
        }
        */
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
        debug.text = Networking.IsMaster.ToString();
        debug.text += "\n";
        for (int i = 0;i < cubeArray.Length; i++)
        {
            debug.text += cubeArrayStatus[i].ToString() + " ";
        }
        debug.text += "\n";

        //for (int i = 0; i < cubeArray.Length; i++)
        //{
        //   debug.text += cubeArrayStatusLocal[i].ToString() + " ";
        //}
        /*
        if (tracking)
        {
            VRCPlayerApi.TrackingData headTrackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            Vector3 headPos = headTrackingData.position;
            headPos.y = headPos.y + 2.0f;
            cubeFromPool.transform.position = headPos;
        }
        */
    }
}
