using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepAction : MonoBehaviour
{
    [SerializeField]
    private Animator Transition2;
    [SerializeField]
    private Animator Transition3;
    [SerializeField]
    private GameObject DialogueBox;
    [SerializeField]
    private Animator dialogueBoxAnim;

    void Start()
    {
        StartCoroutine(DisplayDialogue());
    }

    public void NextScene()
    {
        if (Dialogue.instance.dialogEnd == true)
        {
            StartCoroutine(ChangeScene());
        }
    }

    public IEnumerator DisplayDialogue()
    {
        yield return new WaitForSeconds(2.6f);
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1f);
        Transition2.SetBool("isSleep", true);
        Transition3.SetBool("isSleep", true);
        yield return new WaitForSeconds(2f);
        AudioManager.instance.FadeOutBGM();
        GameManager.instance.ChangeScene(12);
        StopAllCoroutines();
    }
}
