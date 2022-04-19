using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    public float typingSpeed;
    private int index;
    [SerializeField]
    private GameObject nextButton;
    [SerializeField]
    private GameObject nextIcon;
    [SerializeField]
    private Animator textDisplayAnim;
    [SerializeField]
    private Animator dialogueBoxAnim;
    [SerializeField]
    private GameObject DialogueBox;

    public bool dialogEnd;
    public static Dialogue instance;

    public void DisplayDialogue()
    {
        index = 0;
        dialogEnd = false;
        StartCoroutine(Type());
    }

    private void Awake()
    {
        instance = this.gameObject.GetComponent<Dialogue>();
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (textDisplay.text == sentences[index])
        {
            nextButton.SetActive(true);
            nextIcon.SetActive(true);
        }
    }

    public void NextSentence()
    {
        textDisplayAnim.SetTrigger("change");
        nextButton.SetActive(false);
        nextIcon.SetActive(false);

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            nextButton.SetActive(false);
            nextIcon.SetActive(false);
            StartCoroutine(closeDialogueBox());
        }
    }

    private IEnumerator closeDialogueBox()
    {
        dialogEnd = true;
        dialogueBoxAnim.SetBool("isClose", true);
        yield return new WaitForSeconds(1f);
        DialogueBox.SetActive(false);
        StopAllCoroutines();
    }
}
