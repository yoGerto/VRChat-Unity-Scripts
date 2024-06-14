
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleCubePool : UdonSharpBehaviour
{
    public VRCObjectPool cubePool;

    private GameObject cubeFromPool;
    private VRCPlayerApi player;
    private bool tracking = false;

    [UdonSynced] int[] philip;

    void Start()
    {
        player = Networking.LocalPlayer;
    }

    public override void Interact()
    {
        if (!tracking)
        {
            Networking.SetOwner(Networking.LocalPlayer, cubePool.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, cubeFromPool = cubePool.TryToSpawn());
            tracking = true;
        }
        else
        {
            cubePool.Return(cubeFromPool);
            tracking = false;
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        Networking.SetOwner(Networking.LocalPlayer, cubePool.gameObject);
        cubePool.Return(cubeFromPool);
    }

    public void Update()
    {
        if (tracking)
        {
            VRCPlayerApi.TrackingData headTrackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            Vector3 headPos = headTrackingData.position;
            headPos.y = headPos.y + 2.0f;
            cubeFromPool.transform.position = headPos;
        }

    }
}
