
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class RespawnTheBloodyBall : UdonSharpBehaviour
{
    public VRCObjectSync theBloodyBall;

    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, theBloodyBall.gameObject);
        theBloodyBall.Respawn();
    }

}
