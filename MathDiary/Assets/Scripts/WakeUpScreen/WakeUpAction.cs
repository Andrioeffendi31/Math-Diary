using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUpAction : MonoBehaviour
{
    [SerializeField]
    private GameObject DialogueBox;
    [SerializeField]
    private AudioClip LessonCompleteTrack;
    [SerializeField]
    private Animator BlindAnim;

    void Start()
    {
        StartCoroutine(DisplayDialogue());
    }

    public void NextScene()
    {
        if (Dialogue.instance.dialogEnd == true)
        {
            BlindAnim.SetTrigger("isWakeUp");
            StartCoroutine(ChangeScene());
        }
    }

    public IEnumerator DisplayDialogue()
    {
        yield return new WaitForSeconds(4f);
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1.2f);
        AudioManager.instance.SwapTrack(LessonCompleteTrack);
        GameManager.instance.ChangeScene(15);
        StopAllCoroutines();
    }
}
