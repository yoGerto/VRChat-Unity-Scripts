
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Spawner : UdonSharpBehaviour
{
    public SpawnedInCube SpawnedInCube;

    void Start()
    {
        
    }

    public override void Interact()
    {
        //SpawnedInCube.Spawn();
    }
}
