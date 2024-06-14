
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SpawnedInCube : UdonSharpBehaviour
{
    public GameObject cube;

    private Vector3 cubeOrigin;
    private GameObject spawnedCube;
    //private Vector3 moveCubeAway = new Vector3(0.0f, -10.0f, 0.0f);

    void Start()
    {
        cubeOrigin = cube.transform.position;
        //cube.transform.position = moveCubeAway;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        player.SetVelocity(new Vector3(0.0f, 10.0f, 0.0f));
    }

}

/*
public void Spawn()
{
    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SpawnNetworked");
}

public void SpawnNetworked()
{
    spawnedCube = Object.Instantiate(cube);
    spawnedCube.transform.position = cubeOrigin;
    Networking.SetOwner(Networking.LocalPlayer, spawnedCube);
    //SendCustomEventDelayedSeconds("MoveNewCube", 0.01f);
}

public void Despawn()
{
    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DespawnNetworked");
}

public void DespawnNetworked()
{
    Object.Destroy(spawnedCube);
}

public void MoveNewCube()
{
    spawnedCube.transform.position = cubeOrigin;
}
*/