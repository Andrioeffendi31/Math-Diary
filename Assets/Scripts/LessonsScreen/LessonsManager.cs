using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private List<Button> navbarButtons;
    [SerializeField]
    private List<Button> lessonButtons;
    [SerializeField]
    private Button backToLessonButton;

    private float ButtonReactivateDelay = 1.4f;

    public void homeButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToHome();
        GameManager.instance.ChangeScene(2);
    }

    public void OpenFractionalPage()
    {
        foreach (Button btn in lessonButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        StartCoroutine(AudioManager.instance.VolumeDown());
        GameManager.instance.ChangeScene(4);
    }

    public void OpenTwoDimenPage()
    {
        foreach (Button btn in lessonButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        StartCoroutine(AudioManager.instance.VolumeDown());
        GameManager.instance.ChangeScene(5);
    }

    public void OpenThreeDimenPage()
    {
        foreach (Button btn in lessonButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        StartCoroutine(AudioManager.instance.VolumeDown());
        GameManager.instance.ChangeScene(6);
    }

    public void switchLessonsPage(int page)
    {
        GameManager.instance.ChangeScene(page);
    }

    public void BackToLessons()
    {
        backToLessonButton.interactable = false;
        StartCoroutine(EnableButtonAfterDelay(backToLessonButton, ButtonReactivateDelay));
        StartCoroutine(AudioManager.instance.VolumeUp());
        GameManager.instance.ChangeScene(3);
    }

    private void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(HomeScreenTrack);
    }

    public void rankButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToRankScreen();
        GameManager.instance.ChangeScene(7);
    }

    private void SwapTrackToRankScreen()
    {
        AudioManager.instance.SwapTrack(RankScreenTrack);
    }

    public void profileButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToProfileScreen();
        GameManager.instance.ChangeScene(8);
    }

    private void SwapTrackToProfileScreen()
    {
        AudioManager.instance.SwapTrack(ProfileScreenTrack);
    }

    public void shopButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToShopScreen();
        GameManager.instance.ChangeScene(9);
    }

    private void SwapTrackToShopScreen()
    {
        AudioManager.instance.SwapTrack(ShopScreenTrack);
    }

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }
}
