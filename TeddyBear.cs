
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.Udon.Common;
using System.Threading;

public class TeddyBear : UdonSharpBehaviour
{
    public Animator teddyAnimator;

    public AudioSource teddyAudio;
    public AudioClip[] teddyClips;

    private float timer = 0.0f;
    private float cooldownTime = 0.5f;
    private bool timerLatch = false;

    public override void OnPickupUseDown()
    {
        if (timerLatch)
        {
            float currentTime = Time.time;
            if (currentTime - timer > cooldownTime)
            {
                TeddyBearAudioHandler();
                timer = currentTime;
            }
        }
        else
        {
            TeddyBearAudioHandler();
            timer = Time.time;
            timerLatch = true;
        }
    }

    public void TeddyBearAudioHandler()
    {
        int audioRNG;
        audioRNG = Random.Range(0, 200);
        if (audioRNG < 100)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayTeddyBearAudio0");
        }
        else if (audioRNG >= 100 && audioRNG < 199)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayTeddyBearAudio1");
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayTeddyBearAudio2");
        }
    }

    public void PlayTeddyBearAudio0()
    {
        teddyAudio.PlayOneShot(teddyClips[0]);
        teddyAnimator.SetInteger("BearState", 1);
        SendCustomEventDelayedSeconds("ResetAnimatorParam", 0.05f);
    }

    public void PlayTeddyBearAudio1()
    {
        teddyAudio.PlayOneShot(teddyClips[1]);
        teddyAnimator.SetInteger("BearState", 2);
        SendCustomEventDelayedSeconds("ResetAnimatorParam", 0.05f);
    }

    public void PlayTeddyBearAudio2()
    {
        teddyAudio.PlayOneShot(teddyClips[2]);
    }

    public void ResetAnimatorParam()
    {

        teddyAnimator.SetInteger("BearState", 0);
    }

}


/*

    public int AudioRNG
    {
        set
        {
            audioRNG = value;
            PlayTeddyBearAudio();
        }
        get 
        { 
            return AudioRNG; 
        }

public void PlayTeddyBearAudio()
    {
        if (audioRNG < 100)
        {
            teddyAudio.clip = teddySqueak1;
        }
        else if (audioRNG >= 100 && audioRNG < 200)
        {
            teddyAudio.clip = teddySqueak2;
        }
        else
        {
            teddyAudio.clip = tomAndJerryScream;
        }

        teddyBearRngDebug.text = audioRNG.ToString();

        teddyAudio.PlayOneShot(teddySqueak1);
    }

//SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayTeddyBearAudio");

public void PlayTeddyBearAudio()
{
    if (audioRNG < 100)
    {
        teddyAudio.clip = teddySqueak1;
    }
    else if (audioRNG >= 100 && audioRNG < 200)
    {
        teddyAudio.clip = teddySqueak2;
    }
    else
    {
        teddyAudio.clip = tomAndJerryScream;
    }

    teddyBearRngDebug.text = audioRNG.ToString();

    teddyAudio.Play();
} 

public void ResetAnimatorParam()
{

    teddyAnimator.SetInteger("BearState", 0);
}

int randomAnim = Random.Range(1, 3);
teddyAnimator.SetInteger("BearState", randomAnim);
SendCustomEventDelayedSeconds("ResetAnimatorParam", 0.05f);
*/
