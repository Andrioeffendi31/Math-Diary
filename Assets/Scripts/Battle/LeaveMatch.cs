using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveMatch : MonoBehaviour
{
    [SerializeField]
    private AudioClip HomeScreenTrack;
    [SerializeField]
    private List<Button> buttons;
    private float ButtonReactivateDelay = 1.4f;

    public void LeaveCurMatch()
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToHome();
        GameManager.instance.ChangeScene(2);
    }

    public void CancelLeavingMatch()
    {
        StartCoroutine(ClosePopup());
    }

    private IEnumerator ClosePopup()
    {
        this.GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(0.4f);
        this.gameObject.SetActive(false);
    }

    private void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(HomeScreenTrack);
    }

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }
}
