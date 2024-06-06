
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleHQMirror : UdonSharpBehaviour
{
    public GameObject HQMirror;
    public GameObject LQMirror;
    public ToggleLQMirror LQMirrorScript;
    public bool mirrorState;

    public override void Interact()
    {
        LQMirrorScript.SyncMirrorState();
        //LQMirrorScript.OtherScriptEvent();

        if (mirrorState == false)
        {
            HQMirror.SetActive(true);
            mirrorState = true;
        }
        else
        {
            HQMirror.SetActive(false);
            mirrorState = false;
        }
    }

    public void SyncMirrorState()
    {
        HQMirror.SetActive(false);
        mirrorState = false;
    }
}
