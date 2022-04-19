using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip HomeScreenTrack;
    [SerializeField]
    private AudioClip RankScreenTrack;
    [SerializeField]
    private AudioClip ProfileScreenTrack;
    [SerializeField]
    private AudioClip ShopScreenTrack;

    public void homeButtonClicked()
    {
        SwapTrackToHome();
        GameManager.instance.ChangeScene(2);
    }

    public void OpenFractionalPage()
    {
        StartCoroutine(AudioManager.instance.VolumeDown());
        GameManager.instance.ChangeScene(4);
    }

    public void OpenTwoDimenPage()
    {
        StartCoroutine(AudioManager.instance.VolumeDown());
        GameManager.instance.ChangeScene(5);
    }

    public void OpenThreeDimenPage()
    {
        StartCoroutine(AudioManager.instance.VolumeDown());
        GameManager.instance.ChangeScene(6);
    }

    public void switchLessonsPage(int page)
    {
        GameManager.instance.ChangeScene(page);
    }

    public void BackToLessons()
    {
        StartCoroutine(AudioManager.instance.VolumeUp());
        GameManager.instance.ChangeScene(3);
    }

    private void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(HomeScreenTrack);
    }

    public void rankButtonClicked()
    {
        SwapTrackToRankScreen();
        GameManager.instance.ChangeScene(7);
    }

    private void SwapTrackToRankScreen()
    {
        AudioManager.instance.SwapTrack(RankScreenTrack);
    }

    public void profileButtonClicked()
    {
        SwapTrackToProfileScreen();
        GameManager.instance.ChangeScene(8);
    }

    private void SwapTrackToProfileScreen()
    {
        AudioManager.instance.SwapTrack(ProfileScreenTrack);
    }

    public void shopButtonClicked()
    {
        SwapTrackToShopScreen();
        GameManager.instance.ChangeScene(9);
    }

    private void SwapTrackToShopScreen()
    {
        AudioManager.instance.SwapTrack(ShopScreenTrack);
    }
}
