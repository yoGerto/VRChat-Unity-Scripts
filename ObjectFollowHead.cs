
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class ObjectFollowHead : UdonSharpBehaviour
{
    //my thinking is
    //object sync
    //get gameobject
    //then, in update
    //get the players head position?
    //move the cube to a position above that
    //profit??

    public GameObject cubeFollower;
    public Collider cubeFollowerCollider;

    private VRCPlayerApi player;

    void Start()
    {
        //cubeFollower = this.GetComponent<GameObject>();
        player = Networking.LocalPlayer;
        //player = Networking.InstanceOwner;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        //I originally was going to try and determine if the player is falling onto the box collider
        //but in theory by setting a minimum falling speed, this should eliminate cases where you could accidentally trigger it by entering it sideways


        player.SetVelocity(new Vector3(0.0f, 10.0f, 0.0f));
    }

    private void Update()
    {
        VRCPlayerApi.TrackingData headTrackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = headTrackingData.position;
        headPos.y = headPos.y + 2.0f;
        cubeFollower.transform.position = headPos;
    }
}
