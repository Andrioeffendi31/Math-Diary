using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwap : MonoBehaviour
{
    [SerializeField]
    private AudioClip homeTrack;

    public void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(homeTrack);
    }
}
