
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleCubeOnAndOff : UdonSharpBehaviour
{
    public Toggle cubeToggle;
    public SpawnedInCube SpawnedInCube;

    private bool previousToggleVal;

    VRCPlayerApi player;

    void Start()
    {
        previousToggleVal = cubeToggle.isOn;
        player = Networking.LocalPlayer;
    }

    private void Update()
    {
        if (player == Networking.LocalPlayer)
        {
            if (cubeToggle != previousToggleVal)
            {
                if (cubeToggle.isOn)
                {
                    //spawn cube
                    //SpawnedInCube.Spawn();

                }
                else
                {
                    //despawn cube
                    //SpawnedInCube.Despawn();
                }
            }
        }
        /*
        if (cubeToggle != previousToggleVal)
        {
            if (cubeToggle.isOn)
            {
                //spawn cube
                SpawnedInCube.Spawn();

            }
            else
            {
                //despawn cube
                SpawnedInCube.Despawn();
            }
        }
        */

        previousToggleVal = cubeToggle.isOn;
    }
}
