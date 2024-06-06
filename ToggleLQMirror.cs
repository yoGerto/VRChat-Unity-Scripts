using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleLQMirror : UdonSharpBehaviour
{
    public GameObject HQMirror;
    public GameObject LQMirror;
    public ToggleHQMirror HQMirrorScript;
    public bool mirrorState;

    public override void Interact()
    {
        HQMirrorScript.SyncMirrorState();

        if (mirrorState == false)
        {
            LQMirror.SetActive(true);
            mirrorState = true;
        }
        else
        {
            LQMirror.SetActive(false);
            mirrorState = false;
        }
    }
    public void SyncMirrorState()
    {
        LQMirror.SetActive(false);
        mirrorState = false;
    }

    public void OtherScriptEvent()
    {
        Debug.Log("MyNameJeff");
    }
}
