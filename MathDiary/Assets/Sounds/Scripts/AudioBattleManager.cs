using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBattleManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip swordSFX_1, swordSFX_2, swordSFX_3, swordSFX_4, swordSFX_5;
    private AudioSource Sword_SFX_1, Sword_SFX_2;
    public static AudioBattleManager instance;

    private void Awake()
    {
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
        Sword_SFX_1 = gameObject.AddComponent<AudioSource>();
        Sword_SFX_2 = gameObject.AddComponent<AudioSource>();
    }

    public void playSwordSFX1()
    {
        Sword_SFX_1.clip = swordSFX_1;
        Sword_SFX_1.Play();
    }

    public void playSwordSFX2()
    {
        Sword_SFX_2.clip = swordSFX_2;
        Sword_SFX_2.Play();
    }

    public void playHeavySwordSFX1()
    {
        Sword_SFX_1.clip = swordSFX_3;
        Sword_SFX_1.Play();
    }

    public void playHeavySwordSFX2()
    {
        Sword_SFX_2.clip = swordSFX_4;
        Sword_SFX_2.Play();
    }

    public void playHeavySwordSFX3()
    {
        Sword_SFX_2.clip = swordSFX_5;
        Sword_SFX_2.Play();
    }
}
