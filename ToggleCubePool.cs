
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleCubePool : UdonSharpBehaviour
{
    public VRCObjectPool cubePool;

    void Start()
    {
        
    }

    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, cubePool.gameObject);
        //cubePool.TryToSpawn();
        Networking.SetOwner(Networking.LocalPlayer, cubePool.TryToSpawn());
    }
}
