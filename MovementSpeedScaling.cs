
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;

public class MovementSpeedScaling : UdonSharpBehaviour
{
    /*
    public Slider scalingFactorSlider;
    public Toggle scalingFactorToggle;
    Vector3 playerCoords;
    bool defaultObtained;
    float absoluteDistance;
    float defaultWalkSpeed = 0;
    float defaultStrafeSpeed = 0;
    float defaultRunSpeed = 0;
    float scalingFactor = 0;

    void Start()
    {
        
    }

    void Update()
    {
        VRCPlayerApi player = Networking.LocalPlayer;

        if (defaultObtained == false)
        {
            defaultWalkSpeed = player.GetWalkSpeed();
            defaultStrafeSpeed = player.GetStrafeSpeed();
            defaultRunSpeed = player.GetRunSpeed();
            defaultObtained = true;
        }

        scalingFactor = scalingFactorSlider.value;
        playerCoords = player.GetPosition();
        absoluteDistance = Mathf.Sqrt((playerCoords[0] * playerCoords[0]) + (playerCoords[2] * playerCoords[2]));

        if (scalingFactorToggle.isOn)
        {
            if (scalingFactor == 0)
            {
                player.SetWalkSpeed(0);
                player.SetStrafeSpeed(0);
                player.SetRunSpeed(0);
            }
            else
            {
                player.SetWalkSpeed((float)((defaultWalkSpeed * scalingFactor) / (absoluteDistance + (defaultWalkSpeed * scalingFactor))));
                player.SetStrafeSpeed((float)((defaultStrafeSpeed * scalingFactor) / (absoluteDistance + (defaultStrafeSpeed * scalingFactor))));
                player.SetRunSpeed((float)((defaultRunSpeed * scalingFactor) / (absoluteDistance + (defaultRunSpeed * scalingFactor))));
            }
        }
        else
        {
            player.SetWalkSpeed(defaultWalkSpeed);
            player.SetStrafeSpeed(defaultStrafeSpeed);
            player.SetRunSpeed(defaultRunSpeed);
        }

    }
    */
}
