using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;

public class IntroManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("Prefabs")]
    [SerializeField]
    private GameObject PlatoCommonPrefab;
    [SerializeField]
    private GameObject PlatoElitePrefab;
    [SerializeField]
    private GameObject WarriorCommonPrefab;
    [SerializeField]
    private GameObject WarriorElitePrefab;
    [SerializeField]
    private GameObject Avatar;
    [SerializeField]
    private Transform avatarParent;
    [Space(5)]

    [SerializeField]
    private Animator avatar;
    [SerializeField]
    private Animator CameraController;
    [SerializeField]
    private AudioSource SFX;
    [SerializeField]
    private AudioClip zombieSFX, Unit1Track;
    [SerializeField]
    private GameObject DialogueBox;
    [SerializeField]
    private TMP_Text dialogueName;
    [SerializeField]
    private GameObject SpawnPoint;
    [SerializeField]
    private GameObject EnemyPoint;
    [SerializeField]
    private Animator EnemyIntro;

    private int cutSceneState = 1;
    private string usedAvatarName;


    void Start()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;
        StartCoroutine(GetUsedAvatar());
        StartCoroutine(CutScene1());
    }

    public void NextCutScene()
    {
        if (cutSceneState == 2 && Dialogue.instance.dialogEnd)
        {
            dialogueName.text = "Kamu : ";
            Dialogue.instance.sentences = new string[4];
            Dialogue.instance.sentences[0] = "Ap.. ap.. APAAA !?";
            Dialogue.instance.sentences[1] = "Apa yang terjadi padaku ?";
            Dialogue.instance.sentences[2] = "Kenapa Aku memegang sebuah pedang ?";
            Dialogue.instance.sentences[3] = "Tunggu, dimana ini ?";
            StartCoroutine(CutScene2());
        }
        if (cutSceneState == 3 && Dialogue.instance.dialogEnd)
        {
            dialogueName.text = "Kamu : ";
            Dialogue.instance.typingSpeed = 0.02f;
            Dialogue.instance.sentences = new string[6];
            Dialogue.instance.sentences[0] = "Hmmm....";
            Dialogue.instance.sentences[1] = "Apa ini mimpi ??";
            Dialogue.instance.sentences[2] = "Ohhh iyaa.. Aku ingat !!";
            Dialogue.instance.sentences[3] = "Sebelumnya Aku mengerjakan PR matematikaku hingga larut malam.";
            Dialogue.instance.sentences[4] = "Dan sepertinya Aku tertidur saat mengerjakannya.";
            Dialogue.instance.sentences[5] = "Hmmm, selanjutnya apa yang harus ku lakukan ?";
            StartCoroutine(CutScene3());
        }
        if (cutSceneState == 4 && Dialogue.instance.dialogEnd)
        {
            dialogueName.text = "??? : ";
            SFX.pitch = 1.3f;
            SFX.volume = 0.6f;
            SFX.clip = zombieSFX;
            Dialogue.instance.sentences = new string[1];
            Dialogue.instance.sentences[0] = "Arrgggghh......";
            StartCoroutine(CutScene4());
        }

        if (cutSceneState == 5 && Dialogue.instance.dialogEnd)
        {
            dialogueName.text = "Kamu : ";
            Dialogue.instance.sentences = new string[2];
            Dialogue.instance.sentences[0] = "Huh ??, Si.. SIAPA itu !?";
            Dialogue.instance.sentences[1] = "Sepertinya dia bukan manusia ...";
            StartCoroutine(CutScene5());
        }

        if (cutSceneState == 6 && Dialogue.instance.dialogEnd)
        {
            dialogueName.text = "??? : ";
            Dialogue.instance.sentences = new string[1];
            Dialogue.instance.sentences[0] = "Arrgggghh......";
            StartCoroutine(CutScene6());
        }

        if (cutSceneState == 7 && Dialogue.instance.dialogEnd)
        {
            SwapTrackToUnit1();
            GameManager.instance.ChangeScene(13);
        }
    }

    public IEnumerator CutScene1()
    {
        SFX.Play();
        yield return new WaitForSeconds(2f);
        SpawnPoint.SetActive(true);
        yield return new WaitForSeconds(3f);
        Dialogue.instance.typingSpeed = 0.02f;
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
        cutSceneState = 2;
    }

    public IEnumerator CutScene2()
    {
        yield return new WaitForSeconds(1.4f);
        CameraController.SetBool("Right", true);
        yield return new WaitForSeconds(1.8f);
        CameraController.SetBool("Right", false);
        yield return new WaitForSeconds(1f);
        avatar.SetTrigger("viewAround");
        CameraController.SetBool("Left", true);
        yield return new WaitForSeconds(1.8f);
        CameraController.SetBool("Left", false);
        yield return new WaitForSeconds(1.3f);
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
        cutSceneState = 3;
    }

    public IEnumerator CutScene3()
    {
        yield return new WaitForSeconds(1.4f);
        CameraController.SetBool("Right", true);
        yield return new WaitForSeconds(1.8f);
        CameraController.SetBool("Right", false);
        yield return new WaitForSeconds(1f);
        avatar.SetTrigger("viewAround");
        CameraController.SetBool("Left", true);
        yield return new WaitForSeconds(1.8f);
        CameraController.SetBool("Left", false);
        yield return new WaitForSeconds(1.3f);
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
        cutSceneState = 4;
    }

    public IEnumerator CutScene4()
    {
        yield return new WaitForSeconds(1.4f);
        CameraController.SetBool("Right", true);
        yield return new WaitForSeconds(1f);
        EnemyPoint.SetActive(true);
        yield return new WaitForSeconds(2.3f);
        EnemyIntro.SetBool("isReady", true);
        yield return new WaitForSeconds(1.8f);
        SFX.Play();
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
        cutSceneState = 5;
    }

    public IEnumerator CutScene5()
    {
        CameraController.SetBool("Right", false);
        yield return new WaitForSeconds(1.4f);
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
        cutSceneState = 6;
    }

    public IEnumerator CutScene6()
    {
        CameraController.SetBool("Right", true);
        yield return new WaitForSeconds(1.4f);
        SFX.Play();
        DialogueBox.SetActive(true);
        Dialogue.instance.DisplayDialogue();
        cutSceneState = 7;
    }

    private IEnumerator GetUsedAvatar()
    {
        var GetAvatarTask = DBreference.Child("users").Child(user.UserId).Child("used_avatar").GetValueAsync();

        yield return new WaitUntil(() => GetAvatarTask.IsCompleted);

        if (GetAvatarTask.Exception != null)
        {
            Debug.LogError(GetAvatarTask.Exception.Message);
        }
        else
        {
            usedAvatarName = GetAvatarTask.Result.Value.ToString();
            Debug.Log("Avatar: " + usedAvatarName);
            if (usedAvatarName == "KumaThePlatoCommon")
            {
                Avatar = Instantiate(PlatoCommonPrefab, avatarParent);
                Avatar.name = PlatoCommonPrefab.name;
            }
            else if (usedAvatarName == "KumaThePlatoElite")
            {
                Avatar = Instantiate(PlatoElitePrefab, avatarParent);
                Avatar.name = PlatoElitePrefab.name;
            }
            else if (usedAvatarName == "KumaTheWarriorCommon")
            {
                Avatar = Instantiate(WarriorCommonPrefab, avatarParent);
                Avatar.name = WarriorCommonPrefab.name;
            }
            else if (usedAvatarName == "KumaTheWarriorElite")
            {
                Avatar = Instantiate(WarriorElitePrefab, avatarParent);
                Avatar.name = WarriorElitePrefab.name;
            }
        }
    }

    private void SwapTrackToUnit1()
    {
        AudioManager.instance.SwapTrack(Unit1Track);
    }
}
