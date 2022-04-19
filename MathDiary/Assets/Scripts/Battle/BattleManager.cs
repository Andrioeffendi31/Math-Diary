using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;
using Cinemachine;
using System;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("Camera")]
    [SerializeField]
    private CinemachineVirtualCamera HeroCamera;
    [SerializeField]
    private CinemachineVirtualCamera EnemyCamera;
    [SerializeField]
    private Animator cameraController;
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
    private List<GameObject> EnemyPrefabs;
    [Space(5)]

    [Header("Spawnner")]
    [SerializeField]
    private GameObject Avatar;
    [SerializeField]
    private Transform avatarParent;
    [SerializeField]
    private GameObject Enemy;
    [SerializeField]
    private Transform enemyParent;
    [Space(5)]

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text usernameText;
    [SerializeField]
    private TMP_Text totalGoldText;
    [SerializeField]
    private TMP_Text totalSilverText;
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private TMP_Text currentLVText;
    [SerializeField]
    private Slider XPSlider;
    [SerializeField]
    private Slider HPSlider;
    [SerializeField]
    private Slider TimerSlider;
    [SerializeField]
    private TMP_Text HPAmount;
    [SerializeField]
    private Button WakeUpBtn;
    [SerializeField]
    private GameObject profileStatPanel;
    [SerializeField]
    private GameObject actionChoicePanel;
    [SerializeField]
    private GameObject currencyPanel;
    [SerializeField]
    private GameObject QuestionPanel;
    [SerializeField]
    private GameObject TimerPanel;
    [SerializeField]
    private GameObject MatchEndPanel;
    [SerializeField]
    private TMP_Text questionText;
    [SerializeField]
    private List<Button> answerBtnList;
    [SerializeField]
    private List<Button> actionChoice;
    [Space(5)]

    [Header("Baackground")]
    [SerializeField]
    private Animator backgroundAnim;

    [Header("Audio")]
    [SerializeField]
    private AudioClip[] battleBGM;

    private string usedAvatarName;
    private string username;
    private string totalGold;
    private string totalSilver;
    private string currentLV;
    private int currentXPValue;
    private int XPToNextLV;
    private int state = 0;
    public static BattleManager instance;
    public static int totalHP;
    public static int enemyHP;
    public static int enemyTotalHP;
    private int enemyDemage;
    private int totalTurn;
    private bool isUltimate = false;
    private bool timerActive = false;
    private float currentTime;
    private int startSeconds = 100;

    public static string generatedQuestion;
    public static string generatedAnswer;
    private static int den3, num3;

    public static bool isWin;
    public static int totalCorrectAns;
    public static int totalWrongAns;

    private float ButtonReactivateDelay = 1.4f;


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
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;
        totalHP = 100;
        isWin = false;
        totalCorrectAns = 0;
        totalWrongAns = 0;

        StartCoroutine(GetUsedAvatar());
        StartCoroutine(GetUserData());
        StartCoroutine(SpawnEnemy(state));
    }

    private void Update()
    {
        if (timerActive == true)
        {
            currentTime = currentTime - Time.deltaTime;
            if (currentTime <= 0)
            {
                timerActive = false;
                StartCoroutine(EnemyAttack());
            }
        }
        TimerSlider.value = currentTime;
    }

    private IEnumerator SpawnEnemy(int i)
    {
        if (i <= 1)
        {
            enemyDemage = 20;
            enemyHP = 150;
        }
        else if (i >= 2 && i <= 3)
        {
            enemyDemage = 30;
            enemyHP = 250;
        }
        else
        {
            enemyHP = 500;
        }

        enemyTotalHP = enemyHP;
        yield return new WaitForSeconds(4f);
        Enemy = Instantiate(EnemyPrefabs[i], enemyParent);
        Enemy.name = EnemyPrefabs[i].name;
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = enemyTotalHP;
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
        StopCoroutine(SpawnEnemy(i));
    }

    private IEnumerator GetUserData()
    {
        var GetUsernameTask = DBreference.Child("users").Child(user.UserId).Child("username").GetValueAsync();

        yield return new WaitUntil(() => GetUsernameTask.IsCompleted);

        if (GetUsernameTask.Exception != null)
        {
            Debug.LogError(GetUsernameTask.Exception.Message);
        }
        else
        {
            username = GetUsernameTask.Result.Value.ToString();
            Debug.Log("Username: " + username);

            var GetTotalGoldTask = DBreference.Child("users").Child(user.UserId).Child("total_gold").GetValueAsync();

            yield return new WaitUntil(() => GetTotalGoldTask.IsCompleted);

            if (GetTotalGoldTask.Exception != null)
            {
                Debug.LogError(GetTotalGoldTask.Exception.Message);
            }
            else
            {
                totalGold = GetTotalGoldTask.Result.Value.ToString();
                Debug.Log("Total Gold: " + totalGold);

                var GetTotalSilverTask = DBreference.Child("users").Child(user.UserId).Child("total_silver").GetValueAsync();

                yield return new WaitUntil(() => GetTotalSilverTask.IsCompleted);

                if (GetTotalSilverTask.Exception != null)
                {
                    Debug.LogError(GetTotalSilverTask.Exception.Message);
                }
                else
                {
                    totalSilver = GetTotalSilverTask.Result.Value.ToString();
                    Debug.Log("Total Silver: " + totalSilver);

                    var GetCurrentLvTask = DBreference.Child("users").Child(user.UserId).Child("level").GetValueAsync();

                    yield return new WaitUntil(() => GetCurrentLvTask.IsCompleted);

                    if (GetCurrentLvTask.Exception != null)
                    {
                        Debug.LogError(GetCurrentLvTask.Exception.Message);
                    }
                    else
                    {
                        currentLV = GetCurrentLvTask.Result.Value.ToString();
                        Debug.Log("Current Level: " + currentLV);

                        var GetCurrentXPValueTask = DBreference.Child("users").Child(user.UserId).Child("xp").GetValueAsync();

                        yield return new WaitUntil(() => GetCurrentXPValueTask.IsCompleted);

                        if (GetCurrentXPValueTask.Exception != null)
                        {
                            Debug.LogError(GetCurrentXPValueTask.Exception.Message);
                        }
                        else
                        {
                            currentXPValue = int.Parse(GetCurrentXPValueTask.Result.Value.ToString());
                            Debug.Log("Current XP Value: " + currentXPValue);

                            var GetXPToNextLvTask = DBreference.Child("users").Child(user.UserId).Child("xp_to_next_level").GetValueAsync();

                            yield return new WaitUntil(() => GetXPToNextLvTask.IsCompleted);

                            if (GetXPToNextLvTask.Exception != null)
                            {
                                Debug.LogError(GetXPToNextLvTask.Exception.Message);
                            }
                            else
                            {
                                XPToNextLV = int.Parse(GetXPToNextLvTask.Result.Value.ToString());
                                Debug.Log("XP To Next Level: " + XPToNextLV);

                                System.Uri photoURI = user.PhotoUrl;
                                StartCoroutine(LoadProfilePic(photoURI.ToString()));
                                StartCoroutine(LoadExpBar(currentXPValue, XPToNextLV));
                                StartCoroutine(LoadHPBar(totalHP));

                                usernameText.text = username;
                                totalGoldText.text = totalGold;
                                totalSilverText.text = totalSilver;
                                currentLVText.text = currentLV;

                                yield return new WaitForSeconds(1f);
                                profileStatPanel.SetActive(true);
                                currencyPanel.SetActive(true);
                                yield return new WaitForSeconds(1.6f);
                                actionChoicePanel.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    public void NormalAttack()
    {
        foreach (Button btn in actionChoice)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        currentTime = startSeconds;
        timerActive = true;
        TimerSlider.maxValue = startSeconds;

        isUltimate = false;
        totalTurn++;
        Debug.Log(totalTurn);
        HeroCamera.Follow = avatarParent.GetChild(1).GetChild(0);
        HeroCamera.LookAt = avatarParent.GetChild(1).GetChild(0);
        cameraController.SetBool("Hero", true);
        StartCoroutine(HideActionPanel());
        QuestionPanel.SetActive(true);
        TimerPanel.SetActive(true);
        actionChoice[1].transform.GetChild(0).gameObject.SetActive(false);
        GenerateQuestion();
    }

    public void UltimateAttack()
    {
        foreach (Button btn in actionChoice)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        currentTime = startSeconds;
        timerActive = true;
        TimerSlider.maxValue = startSeconds;

        isUltimate = true;
        totalTurn = 0;
        Debug.Log(totalTurn);
        HeroCamera.Follow = avatarParent.GetChild(1).GetChild(0);
        HeroCamera.LookAt = avatarParent.GetChild(1).GetChild(0);
        cameraController.SetBool("Hero", true);
        StartCoroutine(HideActionPanel());
        QuestionPanel.SetActive(true);
        TimerPanel.SetActive(true);
        actionChoice[1].transform.GetChild(0).gameObject.SetActive(false);
        GenerateQuestion();
    }

    public void SkipTurn()
    {
        foreach (Button btn in actionChoice)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        isUltimate = false;
        totalTurn += 2;
        Debug.Log(totalTurn);
        if (state != 4)
        {
            StartCoroutine(EnemyAttack());
        }
        else
        {
            StartCoroutine(BossAttack());
        }
        StartCoroutine(HideActionPanel());
        actionChoice[1].transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }

    private IEnumerator HideActionPanel()
    {
        actionChoicePanel.GetComponent<Animator>().SetBool("isClose", true);
        WakeUpBtn.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        actionChoicePanel.SetActive(false);
    }

    private void ShowActionPanel()
    {
        actionChoicePanel.SetActive(true);
        cameraController.SetBool("Hero", false);
        cameraController.SetBool("Enemy", false);
        WakeUpBtn.gameObject.SetActive(true);
        Debug.Log("Turn : " + totalTurn);
        if (totalTurn >= 6)
        {
            actionChoice[1].interactable = true;
            actionChoice[1].transform.GetChild(0).gameObject.SetActive(true);
            avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            actionChoice[1].interactable = false;
            actionChoice[1].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private IEnumerator LoadProfilePic(string _photoUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_photoUrl);
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.LogError(request.error);
            if (request.error.Contains("404"))
            {
                Debug.Log("404");
                StartCoroutine(LoadProfilePic(user.PhotoUrl.ToString()));
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Unsupported Image Type");
            }
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

    public IEnumerator LoadExpBar(float _experienceToAdd, float _XPToNextLV)
    {
        XPSlider.maxValue = 100;
        _experienceToAdd = (_experienceToAdd / _XPToNextLV) * 100;
        Debug.Log("Experience to Add: " + _experienceToAdd);
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < _experienceToAdd; i++)
        {
            XPSlider.value++;
            yield return new WaitForSeconds(.001f);
        }
    }

    public IEnumerator LoadHPBar(int _totalHP)
    {
        HPSlider.maxValue = 100;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < _totalHP; i++)
        {
            HPSlider.value++;
            HPAmount.text = HPSlider.value.ToString() + "/100";
            yield return new WaitForSeconds(.001f);
        }
    }

    public IEnumerator ReduceHP(int _reduceAmount)
    {
        for (int i = 0; i < _reduceAmount; i++)
        {
            totalHP--;
            HPSlider.value = totalHP;
            HPAmount.text = totalHP.ToString() + "/100";
            yield return new WaitForSeconds(.001f);
        }
        if (totalHP <= 0)
        {
            totalHP = 0;
            cameraController.SetBool("Hero", true);
            cameraController.SetBool("Enemy", false);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(1.5f);
            MatchEndPanel.SetActive(true);
            isWin = false;
            yield return new WaitForSeconds(2.4f);
            AudioManager.instance.SwapTrack(battleBGM[1]);
            GameManager.instance.ChangeScene(14);
        }
        else
        {
            QuestionPanel.SetActive(false);
            TimerPanel.SetActive(false);
            ShowActionPanel();
        }
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

    private void GenerateQuestion()
    {
        if (StageManager.selectedStage == "unit1_section1")
        {
            List<string> questionList = new List<string>();
            questionList.Add("Berapakah hasil penjumlahan dari\n");
            questionList.Add("Berapakah hasil pengurangan dari\n");
            questionList.Add("Lengkapilah penjumlahan pada bilangan pecahan berikut :\n");
            questionList.Add("Lengkapilah pengurangan pada bilangan pecahan berikut :\n");

            int randomQuestion = Random.Range(0, questionList.Count);
            int num1, den1, num2, den2;
            Fractional.Fraction result;

            num1 = Random.Range(1, 20);
            den1 = Random.Range(1, 20);
            num2 = Random.Range(1, 20);
            den2 = Random.Range(1, 20);

            if (randomQuestion == 0)
            {
                var Denominator = Random.Range(0, 2);
                if (Denominator == 0)
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                    generatedQuestion = questionList[randomQuestion] + num1 + "/" + den1 + " + " + num2 + "/" + den2 + " ?";
                    result = fraction1 + fraction2;
                }
                else
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den1);

                    generatedQuestion = questionList[randomQuestion] + num1 + "/" + den1 + " + " + num2 + "/" + den1 + " ?";
                    result = fraction1 + fraction2;
                }
                generatedAnswer = result.Numerator + "/" + result.Denominator;
            }
            else if (randomQuestion == 1)
            {
                var Denominator = Random.Range(0, 2);
                if (Denominator == 0)
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                    generatedQuestion = questionList[randomQuestion] + num1 + "/" + den1 + " - " + num2 + "/" + den2 + " ?";
                    result = fraction1 - fraction2;
                }
                else
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den1);

                    generatedQuestion = questionList[randomQuestion] + num1 + "/" + den1 + " - " + num2 + "/" + den1 + " ?";
                    result = fraction1 - fraction2;
                }
                generatedAnswer = result.Numerator + "/" + result.Denominator;
            }
            else if (randomQuestion == 2)
            {
                var Denominator = Random.Range(0, 2);
                if (Denominator == 0)
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                    result = fraction1 + fraction2;
                    generatedQuestion = questionList[randomQuestion] + "....." + " + " + num2 + "/" + den2 + " = " + result.Numerator + "/" + result.Denominator;
                }
                else
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den1);

                    result = fraction1 + fraction2;
                    generatedQuestion = questionList[randomQuestion] + "....." + " + " + num2 + "/" + den1 + " = " + result.Numerator + "/" + result.Denominator;
                }
                generatedAnswer = num1 + "/" + den1;
            }
            else if (randomQuestion == 3)
            {
                var Denominator = Random.Range(0, 2);
                if (Denominator == 0)
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                    result = fraction1 - fraction2;
                    generatedQuestion = questionList[randomQuestion] + "....." + " - " + num2 + "/" + den2 + " = " + result.Numerator + "/" + result.Denominator;
                }
                else
                {
                    Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                    Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den1);

                    result = fraction1 - fraction2;
                    generatedQuestion = questionList[randomQuestion] + "....." + " - " + num2 + "/" + den1 + " = " + result.Numerator + "/" + result.Denominator;
                }
                generatedAnswer = num1 + "/" + den1;
            }

            questionText.text = generatedQuestion;
            GenerateAnswer();
            Debug.Log("Answer: " + generatedAnswer);
        }
        else if (StageManager.selectedStage == "unit1_section2")
        {
            List<string> questionList = new List<string>();
            questionList.Add("Berapakah hasil perkalian dari\n");
            questionList.Add("Berapakah hasil pembagian dari\n");
            questionList.Add("Lengkapilah perkalian pada bilangan pecahan berikut :\n");
            questionList.Add("Lengkapilah pembagian pada bilangan pecahan berikut :\n");

            int randomQuestion = Random.Range(0, questionList.Count);
            int num1, den1, num2, den2;
            Fractional.Fraction result;

            num1 = Random.Range(1, 20);
            den1 = Random.Range(1, 20);
            num2 = Random.Range(1, 20);
            den2 = Random.Range(1, 20);

            if (randomQuestion == 0)
            {
                Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                generatedQuestion = questionList[randomQuestion] + num1 + "/" + den1 + " x " + num2 + "/" + den2 + " ?";
                result = fraction1 * fraction2;
                generatedAnswer = result.Numerator + "/" + result.Denominator;
            }
            else if (randomQuestion == 1)
            {
                Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                generatedQuestion = questionList[randomQuestion] + num1 + "/" + den1 + " : " + num2 + "/" + den2 + " ?";
                result = fraction1 / fraction2;
                generatedAnswer = result.Numerator + "/" + result.Denominator;
            }
            else if (randomQuestion == 2)
            {
                Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                result = fraction1 * fraction2;
                generatedQuestion = questionList[randomQuestion] + "....." + " x " + num2 + "/" + den2 + " = " + result.Numerator + "/" + result.Denominator;
                generatedAnswer = num1 + "/" + den1;
            }
            else if (randomQuestion == 3)
            {
                Fractional.Fraction fraction1 = new Fractional.Fraction(num1, den1);
                Fractional.Fraction fraction2 = new Fractional.Fraction(num2, den2);

                result = fraction1 / fraction2;
                generatedQuestion = questionList[randomQuestion] + "....." + " : " + num2 + "/" + den2 + " = " + result.Numerator + "/" + result.Denominator;
                generatedAnswer = num1 + "/" + den1;
            }

            questionText.text = generatedQuestion;
            GenerateAnswer();
            Debug.Log("Answer: " + generatedAnswer);
        }
    }

    private void GenerateAnswer()
    {
        int correctAnswer = Random.Range(0, 4);

        if (StageManager.selectedStage == "unit1_section1" || StageManager.selectedStage == "unit1_section2")
        {
            int num, den;

            for (int i = 0; i < answerBtnList.Count; i++)
            {
                if (i == correctAnswer)
                {
                    answerBtnList[i].GetComponentInChildren<TMP_Text>().text = generatedAnswer;
                    answerBtnList[i].GetComponent<Image>().color = new Color32(255, 255, 255, 225);

                }
                else
                {
                    String[] strlist = generatedAnswer.Split('/');
                    num = int.Parse(strlist[0]);
                    den = int.Parse(strlist[1]);

                    answerBtnList[i].GetComponentInChildren<TMP_Text>().text = ((num + Random.Range(1, 9)) * Random.Range(1, 2)) + "/" + ((den + Random.Range(1, 9)) * Random.Range(1, 2));
                    answerBtnList[i].GetComponent<Image>().color = new Color32(255, 255, 255, 225);
                }
            }
        }
    }

    public void CheckAnswer(int buttonIndex)
    {
        timerActive = false;
        foreach (Button btn in answerBtnList)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }

        if (answerBtnList[buttonIndex].GetComponentInChildren<TMP_Text>().text != generatedAnswer)
        {
            answerBtnList[buttonIndex].GetComponent<Image>().color = new Color32(255, 127, 127, 225);
            if (state != 4)
            {
                StartCoroutine(EnemyAttack());
                totalWrongAns++;
            }
            else
            {
                StartCoroutine(BossAttack());
                totalWrongAns++;
            }
        }
        else
        {
            StartCoroutine(HeroAttack());
            totalCorrectAns++;
        }

        if (answerBtnList[0].GetComponentInChildren<TMP_Text>().text == generatedAnswer)
        {
            answerBtnList[0].GetComponent<Image>().color = Color.green;
        }
        else if (answerBtnList[1].GetComponentInChildren<TMP_Text>().text == generatedAnswer)
        {
            answerBtnList[1].GetComponent<Image>().color = Color.green;
        }
        else if (answerBtnList[2].GetComponentInChildren<TMP_Text>().text == generatedAnswer)
        {
            answerBtnList[2].GetComponent<Image>().color = Color.green;
        }
        else if (answerBtnList[3].GetComponentInChildren<TMP_Text>().text == generatedAnswer)
        {
            answerBtnList[3].GetComponent<Image>().color = Color.green;
        }
    }

    private IEnumerator HeroAttack()
    {
        if (!isUltimate)
        {
            enemyHP -= 40;
            if (enemyHP < 0)
            {
                enemyHP = 0;
                EnemyCamera.Follow = enemyParent.GetChild(1).GetChild(0);
                EnemyCamera.LookAt = enemyParent.GetChild(1).GetChild(0);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", true);
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    AudioBattleManager.instance.playHeavySwordSFX1();
                    yield return new WaitForSeconds(2f);
                    StartCoroutine(EnemyIsDead());
                }
                else
                {
                    AudioBattleManager.instance.playSwordSFX1();
                    yield return new WaitForSeconds(1.2f);
                    StartCoroutine(EnemyIsDead());
                }
            }
            else
            {
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", true);
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    AudioBattleManager.instance.playHeavySwordSFX1();
                    yield return new WaitForSeconds(0.6f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
                    yield return new WaitForSeconds(1.8f);
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", false);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
                else
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", true);
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    AudioBattleManager.instance.playSwordSFX1();
                    yield return new WaitForSeconds(0.4f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
                    yield return new WaitForSeconds(0.5f);
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", false);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
            }
        }
        else if (isUltimate)
        {
            enemyHP -= 80;
            if (enemyHP < 0)
            {
                enemyHP = 0;
                EnemyCamera.Follow = enemyParent.GetChild(1).GetChild(0);
                EnemyCamera.LookAt = enemyParent.GetChild(1).GetChild(0);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isUltimate", true);
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    if (usedAvatarName == "KumaTheWarriorCommon")
                    {
                        AudioBattleManager.instance.playHeavySwordSFX2();
                    }
                    else
                    {
                        AudioBattleManager.instance.playHeavySwordSFX3();
                    }
                    yield return new WaitForSeconds(2.6f);
                    StartCoroutine(EnemyIsDead());
                }
                else
                {
                    AudioBattleManager.instance.playSwordSFX2();
                    yield return new WaitForSeconds(1.2f);
                    StartCoroutine(EnemyIsDead());
                }
            }
            else
            {
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isUltimate", true);
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    if (usedAvatarName == "KumaTheWarriorCommon")
                    {
                        AudioBattleManager.instance.playHeavySwordSFX2();
                    }
                    else
                    {
                        AudioBattleManager.instance.playHeavySwordSFX3();
                    }
                    yield return new WaitForSeconds(1.2f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP.ToString() + "/100";
                    yield return new WaitForSeconds(1.8f);
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isUltimate", false);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
                    avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
                else
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isUltimate", true);
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    AudioBattleManager.instance.playSwordSFX2();
                    yield return new WaitForSeconds(0.6f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP.ToString() + "/100";
                    yield return new WaitForSeconds(1.5f);
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isUltimate", false);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
                    avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
            }
        }
    }

    private IEnumerator EnemyIsDead()
    {
        enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isDead");
        avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", false);
        avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isUltimate", false);
        avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
        cameraController.SetBool("Hero", false);
        cameraController.SetBool("Enemy", true);
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP.ToString() + "/100";
        yield return new WaitForSeconds(1.5f);
        if (state < 4)
        {
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isRun", true);
            QuestionPanel.SetActive(false);
            TimerPanel.SetActive(false);
            yield return new WaitForSeconds(2f);

            cameraController.SetBool("Enemy", false);
            Destroy(enemyParent.GetChild(1).gameObject);
            backgroundAnim.SetTrigger("state" + (state += 1).ToString());

            if (state == 4)
            {
                AudioManager.instance.SwapTrack(battleBGM[0]);
            }

            StartCoroutine(SpawnEnemy(state));
            yield return new WaitForSeconds(4f);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isRun", false);
            ShowActionPanel();
            startSeconds -= 15;
        }
        else
        {
            MatchEndPanel.SetActive(true);
            isWin = true;
            yield return new WaitForSeconds(5f);
            AudioManager.instance.SwapTrack(battleBGM[1]);
            GameManager.instance.ChangeScene(14);
        }
    }

    private IEnumerator EnemyAttack()
    {
        EnemyCamera.Follow = enemyParent.GetChild(1).GetChild(0);
        EnemyCamera.LookAt = enemyParent.GetChild(1).GetChild(0);
        cameraController.SetBool("Hero", false);
        cameraController.SetBool("Enemy", true);
        yield return new WaitForSeconds(1f);

        enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", true);
        QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
        TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
        AudioBattleManager.instance.playSwordSFX1();
        yield return new WaitForSeconds(0.6f);

        avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
        yield return new WaitForSeconds(1f);

        StartCoroutine(ReduceHP(enemyDemage));
        enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack", false);
        avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
    }

    private IEnumerator BossAttack()
    {
        EnemyCamera.Follow = enemyParent.GetChild(1).GetChild(0);
        EnemyCamera.LookAt = enemyParent.GetChild(1).GetChild(0);
        var attackSkill = Random.Range(1, 5);
        if (attackSkill > 1)
        {
            cameraController.SetBool("Hero", false);
            cameraController.SetBool("Enemy", true);

            enemyDemage = 30;
            yield return new WaitForSeconds(1f);

            enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack1", true);
            QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
            TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
            yield return new WaitForSeconds(0.6f);

            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
            yield return new WaitForSeconds(1f);

            StartCoroutine(ReduceHP(enemyDemage));
            enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack1", false);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
            QuestionPanel.SetActive(false);
            TimerPanel.SetActive(false);
            ShowActionPanel();
        }
        else
        {
            cameraController.SetBool("Hero", false);
            cameraController.SetBool("Enemy", true);

            enemyDemage = 40;
            yield return new WaitForSeconds(1f);

            enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack2", true);
            QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
            TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
            yield return new WaitForSeconds(1f);

            cameraController.SetBool("Enemy", false);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", true);
            yield return new WaitForSeconds(1f);

            StartCoroutine(ReduceHP(enemyDemage));
            enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isAttack2", false);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isHit", false);
            QuestionPanel.SetActive(false);
            TimerPanel.SetActive(false);
            ShowActionPanel();
        }
    }
}
