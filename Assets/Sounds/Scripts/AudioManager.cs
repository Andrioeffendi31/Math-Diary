using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip defaultTrack;

    private AudioSource track01, track02;
    private bool isPlayingTrack01;

    public static AudioManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        track01 = gameObject.AddComponent<AudioSource>();
        track02 = gameObject.AddComponent<AudioSource>();
        isPlayingTrack01 = true;

        SwapTrack(defaultTrack);
    }

    public void SwapTrack(AudioClip newClip)
    {
        StartCoroutine(FadeOut());
        StartCoroutine(FadeIn(newClip));
        isPlayingTrack01 = !isPlayingTrack01;
    }

    public void FadeInBGM(AudioClip newClip)
    {
        StartCoroutine(FadeIn(newClip));
        isPlayingTrack01 = !isPlayingTrack01;
    }

    public void FadeOutBGM()
    {
        StartCoroutine(FadeOut());
    }

    public void ReturnToDefault()
    {
        SwapTrack(defaultTrack);
    }

    private IEnumerator FadeOut()
    {
        float timeToFade = 1.8f;
        float timeElapsed = 0;

        if (isPlayingTrack01)
        {
            while (timeElapsed < timeToFade)
            {
                track01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            track01.Stop();
        }
        else
        {
            while (timeElapsed < timeToFade)
            {
                track02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            track02.Stop();
        }
    }

    private IEnumerator FadeIn(AudioClip newClip)
    {
        float timeToFade = 1.8f;
        float timeElapsed = 0;

        if (isPlayingTrack01)
        {
            yield return new WaitForSeconds(1.4f);
            track02.clip = newClip;
            track02.Play();
            track02.loop = true;

            while (timeElapsed < timeToFade)
            {
                track02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1.4f);
            track01.clip = newClip;
            track01.Play();
            track01.loop = true;

            while (timeElapsed < timeToFade)
            {
                track01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            track02.Stop();
        }
    }

    public IEnumerator VolumeDown()
    {
        Debug.Log("VolumeDown");
        while (track02.volume > 0.12)
        {
            Debug.Log(track02.volume);
            track02.volume -= 0.03f;
            yield return null;
        }
    }

    public IEnumerator VolumeUp()
    {
        while (track02.volume < 0.9f)
        {
            track02.volume += 0.02f;
            yield return null;
        }
    }

}
