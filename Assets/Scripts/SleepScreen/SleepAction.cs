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
    [SerializeField]
    private AudioClip BattleScreenTrack;

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
        if (StageManager.selectedStage == "unit1_section1" || StageManager.selectedStage == "unit1_section2")
        {
            AudioManager.instance.FadeOutBGM();
            GameManager.instance.ChangeScene(12);
        }
        else if (StageManager.selectedStage == "unit2_section1")
        {
            AudioManager.instance.SwapTrack(BattleScreenTrack);
            GameManager.instance.ChangeScene(17);
        }
        else if (StageManager.selectedStage == "unit2_section2")
        {
            AudioManager.instance.SwapTrack(BattleScreenTrack);
            GameManager.instance.ChangeScene(18);
        }
        else if (StageManager.selectedStage == "unit3_section1")
        {
            AudioManager.instance.SwapTrack(BattleScreenTrack);
            GameManager.instance.ChangeScene(19);
        }
        else if (StageManager.selectedStage == "unit3_section2")
        {
            AudioManager.instance.SwapTrack(BattleScreenTrack);
            GameManager.instance.ChangeScene(20);
        }
        StopAllCoroutines();
    }
}
