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
    [SerializeField]
    private List<GameObject> TwoDimensionShapes;
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
    [SerializeField]
    private List<Sprite> nets;
    [SerializeField]
    private List<Sprite> ThreeDimensionShapes;
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
    private int startSeconds;

    public static string generatedQuestion;
    public static string generatedAnswer;
    private static int den3, num3;

    public static bool isWin;
    public static int totalCorrectAns;
    public static int totalWrongAns;
    RCG.FisherYatesRandomizer fr = new RCG.FisherYatesRandomizer();
    TwoDimentionalFigure.CalculateAreaParimeter calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter();
    ThreeDimentionalFigure.CalculateSurfaceVolume calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume();
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
        if (StageManager.selectedStage == "unit2_section1" || StageManager.selectedStage == "unit3_section1")
        {
            startSeconds = 70;
        }
        else
        {
            startSeconds = 100;
        }

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
                totalWrongAns++;
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
        StartCoroutine(EnemyAttack());
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
            HPAmount.text = totalHP.ToString() + "/100";
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
        else if (StageManager.selectedStage == "unit2_section1")
        {
            List<string> questionList = new List<string>();
            List<string> choices = new List<string>();

            string[] question = {
                "Berapakah jumlah simetri lipat dari persegi",
                "Berapakah jumlah simetri lipat dari persegi panjang",
                "Berapakah jumlah simetri lipat dari belah ketupat",
                "Berapakah jumlah simetri lipat dari jajar genjang",
                "Berapakah jumlah simetri lipat dari segitiga sama kaki",
                "Berapakah jumlah simetri lipat dari segitiga sama sisi",
                "Berapakah jumlah simetri lipat dari segitiga sembarang",
                "Berapakah jumlah simetri lipat dari segitiga siku-siku",
                "Berapakah jumlah simetri lipat dari trapesium sama kaki",
                "Berapakah jumlah simetri lipat dari trapesium siku-siku",
                "Berapakah jumlah simetri lipat dari trapesium sembarang",
                "Berapakah jumlah simetri lipat dari layang-layang",
                "Berapakah jumlah simetri lipat dari lingkaran",
                "Berapakah jumlah simetri putar dari persegi",
                "Berapakah jumlah simetri putar dari persegi panjang",
                "Berapakah jumlah simetri putar dari belah ketupat",
                "Berapakah jumlah simetri putar dari jajar genjang",
                "Berapakah jumlah simetri putar dari segitiga sama kaki",
                "Berapakah jumlah simetri putar dari segitiga sama sisi",
                "Berapakah jumlah simetri putar dari segitiga sembarang",
                "Berapakah jumlah simetri putar dari segitiga siku-siku",
                "Berapakah jumlah simetri putar dari trapesium sama kaki",
                "Berapakah jumlah simetri putar dari trapesium siku-siku",
                "Berapakah jumlah simetri putar dari trapesium sembarang",
                "Berapakah jumlah simetri putar dari layang-layang",
                "Berapakah jumlah simetri putar dari lingkaran"};

            questionList.AddRange(question);
            int randomQuestion = Random.Range(0, questionList.Count);
            if (randomQuestion == 0 || randomQuestion == 13)
            {
                choices.Clear();
                choices.Add("1");
                choices.Add("2");
                choices.Add("3");
                choices.Add("4");
                fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "4");
                fr.ShuffleChoices();
                generatedQuestion = fr.Question;
                generatedAnswer = fr.Answer;
            }
            else if (randomQuestion == 1 || randomQuestion == 2 || randomQuestion == 14 || randomQuestion == 15 || randomQuestion == 16)
            {
                choices.Clear();
                choices.Add("0");
                choices.Add("2");
                choices.Add("3");
                choices.Add("Tak Hingga");
                fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "2");
                fr.ShuffleChoices();
                generatedQuestion = fr.Question;
                generatedAnswer = fr.Answer;
            }
            else if (randomQuestion == 3 || randomQuestion == 6 || randomQuestion == 6 || randomQuestion == 9 || randomQuestion == 10 ||
                     randomQuestion == 17 || randomQuestion == 19 || randomQuestion == 20 || randomQuestion == 21 || randomQuestion == 22 ||
                     randomQuestion == 23 || randomQuestion == 24)
            {
                choices.Clear();
                choices.Add("0");
                choices.Add("2");
                choices.Add("3");
                choices.Add("Tak Hingga");
                fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "0");
                fr.ShuffleChoices();
                generatedQuestion = fr.Question;
                generatedAnswer = fr.Answer;
            }
            else if (randomQuestion == 4 || randomQuestion == 7 || randomQuestion == 8 || randomQuestion == 11)
            {
                choices.Clear();
                choices.Add("0");
                choices.Add("1");
                choices.Add("2");
                choices.Add("4");
                fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "1");
                fr.ShuffleChoices();
                generatedQuestion = fr.Question;
                generatedAnswer = fr.Answer;
            }
            else if (randomQuestion == 12 || randomQuestion == 25)
            {
                choices.Clear();
                choices.Add("0");
                choices.Add("1");
                choices.Add("2");
                choices.Add("Tak Hingga");
                fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "Tak Hingga");
                fr.ShuffleChoices();
                generatedQuestion = fr.Question;
                generatedAnswer = fr.Answer;
            }
            else if (randomQuestion == 5 || randomQuestion == 18)
            {
                choices.Clear();
                choices.Add("0");
                choices.Add("1");
                choices.Add("2");
                choices.Add("3");
                fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "3");
                fr.ShuffleChoices();
                generatedQuestion = fr.Question;
                generatedAnswer = fr.Answer;
            }
            questionText.text = generatedQuestion;
            GenerateAnswer();
            Debug.Log("Answer: " + generatedAnswer);
        }
        else if (StageManager.selectedStage == "unit2_section2")
        {
            int _num1 = Random.Range(1, 50);
            int _num2 = Random.Range(1, 50);
            int _num3 = Random.Range(1, 50);

            string[] questionTypes = { "luas", "keliling" };
            string[] shapeNames = { "persegi", "persegi panjang", "belah ketupat", "jajar genjang", "segitiga", "trapesium", "layang-layang", "lingkaran" };
            List<string> choices = new List<string>();

            int questionType = Random.Range(0, questionTypes.Length);
            int shapeName = Random.Range(0, shapeNames.Length);
            generatedQuestion = "Berapakah " + questionTypes[questionType] + " dari " + shapeNames[shapeName] + " disamping ?";

            if (questionType == 0)
            {
                if (shapeName == 0)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1);
                    generatedAnswer = calculateAreaParimeter.SquareArea().ToString();
                }
                else if (shapeName == 1)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.RectangleArea().ToString();
                }
                else if (shapeName == 2)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.RhombusArea().ToString();
                }
                else if (shapeName == 3)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.ParallelogramArea().ToString();
                }
                else if (shapeName == 4)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.TriangleArea().ToString();
                }
                else if (shapeName == 5)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = _num3.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2, _num3);
                    generatedAnswer = calculateAreaParimeter.TrapezoidArea().ToString();
                }
                else if (shapeName == 6)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.KiteArea().ToString();
                }
                else if (shapeName == 7)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1);
                    generatedAnswer = calculateAreaParimeter.CircleArea().ToString();
                }
            }
            else if (questionType == 1)
            {
                if (shapeName == 0)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1);
                    generatedAnswer = calculateAreaParimeter.SquarePerimeter().ToString();
                }
                else if (shapeName == 1)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.RectanglePerimeter().ToString();
                }
                else if (shapeName == 2)
                {
                    Instantiate(TwoDimensionShapes[shapeName + 6], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1);
                    generatedAnswer = calculateAreaParimeter.RhombusPerimeter().ToString();
                }
                else if (shapeName == 3)
                {
                    Instantiate(TwoDimensionShapes[shapeName + 6], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.ParallelogramPerimeter().ToString();
                }
                else if (shapeName == 4)
                {
                    Instantiate(TwoDimensionShapes[shapeName + 6], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = _num3.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2, _num3);
                    generatedAnswer = calculateAreaParimeter.TrianglePerimeter().ToString();
                }
                else if (shapeName == 5)
                {
                    Instantiate(TwoDimensionShapes[shapeName + 6], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = _num3.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2, _num3);
                    generatedAnswer = calculateAreaParimeter.TrapezoidPerimeter().ToString();
                }
                else if (shapeName == 6)
                {
                    Instantiate(TwoDimensionShapes[shapeName + 6], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    QuestionPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _num2.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1, _num2);
                    generatedAnswer = calculateAreaParimeter.KitePerimeter().ToString();
                }
                else if (shapeName == 7)
                {
                    Instantiate(TwoDimensionShapes[shapeName], QuestionPanel.transform);
                    QuestionPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num1.ToString() + "cm";
                    calculateAreaParimeter = new TwoDimentionalFigure.CalculateAreaParimeter(_num1);
                    generatedAnswer = calculateAreaParimeter.CirclePerimeter().ToString();
                }
            }

            choices.Clear();
            choices.Add(generatedAnswer + " cm");
            choices.Add((float.Parse(generatedAnswer) + (float)Random.Range(1, 10) * 2f).ToString() + " cm");
            choices.Add((float.Parse(generatedAnswer) - Random.Range(1, 10) * 2f).ToString() + " cm");
            choices.Add((float.Parse(generatedAnswer) + (float)Random.Range(1, 10)).ToString() + " cm");
            fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, generatedAnswer + " cm");

            generatedAnswer = fr.Answer;
            fr.ShuffleChoices();
            questionText.text = fr.Question;
            GenerateAnswer();
            Debug.Log("Answer: " + generatedAnswer);
        }

        else if (StageManager.selectedStage == "unit3_section1")
        {
            List<string> questionList = new List<string>();
            List<string> choices = new List<string>();
            int randomQuestion = 0;

            string[] question = {
                "Berapakah jumlah rusuk dari sebuah kubus ?",
                "Berapakah jumlah rusuk dari sebuah balok ?",
                "Berapakah jumlah rusuk dari sebuah prisma segitiga ?",
                "Berapakah jumlah rusuk dari sebuah prisma segi empat ?",
                "Berapakah jumlah rusuk dari sebuah limas segitiga ?",
                "Berapakah jumlah rusuk dari sebuah limas segi empat ?",
                "Berapakah jumlah rusuk dari sebuah tabung ?",
                "Berapakah jumlah rusuk dari sebuah kerucut ?",
                "Berapakah jumlah rusuk dari sebuah bola ?",
                "Berapakah jumlah sudut dari sebuah kubus ?",
                "Berapakah jumlah sudut dari sebuah balok ?",
                "Berapakah jumlah sudut dari sebuah prisma segitiga ?",
                "Berapakah jumlah sudut dari sebuah prisma segi empat ?",
                "Berapakah jumlah sudut dari sebuah limas segitiga ?",
                "Berapakah jumlah sudut dari sebuah limas segi empat ?",
                "Berapakah jumlah sudut dari sebuah tabung ?",
                "Berapakah jumlah sudut dari sebuah kerucut ?",
                "Berapakah jumlah sudut dari sebuah bola ?",};

            questionList.AddRange(question);
            var questionType = Random.Range(0, 10);

            if (questionType > 6)
            {
                randomQuestion = Random.Range(0, nets.Count - 1);

                if (randomQuestion == 0)
                {
                    choices.Clear();
                    choices.Add("Kubus");
                    choices.Add("Balok");
                    choices.Add("Prisma");
                    choices.Add("Limas");
                    fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, "Kubus");
                    fr.ShuffleChoices();
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 1)
                {
                    choices.Clear();
                    choices.Add("Kubus");
                    choices.Add("Balok");
                    choices.Add("Bola");
                    choices.Add("Limas");
                    fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, "Balok");
                    fr.ShuffleChoices();
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 2)
                {
                    choices.Clear();
                    choices.Add("Tabung");
                    choices.Add("Balok");
                    choices.Add("Prisma");
                    choices.Add("Limas");
                    fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, "Prisma");
                    fr.ShuffleChoices();
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 3)
                {
                    choices.Clear();
                    choices.Add("Tabung");
                    choices.Add("Kerucut");
                    choices.Add("Prisma");
                    choices.Add("Limas");
                    fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, "Limas");
                    fr.ShuffleChoices();
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 4)
                {
                    choices.Clear();
                    choices.Add("Tabung");
                    choices.Add("Kerucut");
                    choices.Add("Prisma");
                    choices.Add("Bola");
                    fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, "Tabung");
                    fr.ShuffleChoices();
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 5)
                {
                    choices.Clear();
                    choices.Add("Kerucut");
                    choices.Add("Bola");
                    choices.Add("Tabung");
                    choices.Add("Limas");
                    fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, "Kerucut");
                    fr.ShuffleChoices();
                    generatedAnswer = fr.Answer;
                }
                generatedQuestion = "Gambar disamping merupakan jaring-jaring ?";
                questionText.transform.GetChild(0).gameObject.SetActive(true);
                questionText.transform.GetChild(0).GetComponent<Image>().sprite = nets[randomQuestion];
                questionText.text = generatedQuestion;
                questionText.alignment = TextAlignmentOptions.Left;
                questionText.fontSize = 32;
                questionText.GetComponent<RectTransform>().offsetMin = new Vector2(400, questionText.GetComponent<RectTransform>().offsetMin.y);
                GenerateAnswer();
            }
            else
            {
                questionText.transform.GetChild(0).gameObject.SetActive(false);
                randomQuestion = Random.Range(0, questionList.Count);
                if (randomQuestion == 0 || randomQuestion == 3)
                {
                    choices.Clear();
                    choices.Add("10");
                    choices.Add("12");
                    choices.Add("14");
                    choices.Add("16");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "12");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 2)
                {
                    choices.Clear();
                    choices.Add("8");
                    choices.Add("9");
                    choices.Add("10");
                    choices.Add("12");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "9");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 4 || randomQuestion == 11)
                {
                    choices.Clear();
                    choices.Add("4");
                    choices.Add("6");
                    choices.Add("7");
                    choices.Add("8");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "6");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 5 || randomQuestion == 9 || randomQuestion == 10 || randomQuestion == 12)
                {
                    choices.Clear();
                    choices.Add("6");
                    choices.Add("8");
                    choices.Add("10");
                    choices.Add("12");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "8");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 6)
                {
                    choices.Clear();
                    choices.Add("2");
                    choices.Add("1");
                    choices.Add("3");
                    choices.Add("4");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "2");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 7 || randomQuestion == 16)
                {
                    choices.Clear();
                    choices.Add("1");
                    choices.Add("2");
                    choices.Add("3");
                    choices.Add("0");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "1");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 8 || randomQuestion == 15 || randomQuestion == 17)
                {
                    choices.Clear();
                    choices.Add("0");
                    choices.Add("1");
                    choices.Add("2");
                    choices.Add("3");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "0");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 13)
                {
                    choices.Clear();
                    choices.Add("4");
                    choices.Add("5");
                    choices.Add("6");
                    choices.Add("8");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "4");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                else if (randomQuestion == 14)
                {
                    choices.Clear();
                    choices.Add("4");
                    choices.Add("5");
                    choices.Add("6");
                    choices.Add("8");
                    fr = new RCG.FisherYatesRandomizer(questionList[randomQuestion], choices, "5");
                    fr.ShuffleChoices();
                    generatedQuestion = fr.Question;
                    generatedAnswer = fr.Answer;
                }
                questionText.text = generatedQuestion;
                questionText.alignment = TextAlignmentOptions.Center;
                questionText.fontSize = 36;
                questionText.GetComponent<RectTransform>().offsetMin = new Vector2(74.5f, questionText.GetComponent<RectTransform>().offsetMin.y);
                GenerateAnswer();
            }
            Debug.Log("Answer: " + generatedAnswer);
        }
        else if (StageManager.selectedStage == "unit3_section2")
        {
            List<string> choices = new List<string>();
            int randomQuestion = Random.Range(0, ThreeDimensionShapes.Count);
            int num1 = 0, num2 = 0, num3 = 0;
            questionText.transform.GetChild(0).GetComponent<Image>().sprite = ThreeDimensionShapes[randomQuestion];

            if (randomQuestion == 0)
            {
                int questionType = Random.Range(0, 3);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah luas permukaan dari kubus disamping yang memiliki panjang rusuk sebesar " + num1 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1);
                    generatedAnswer = calculateSurfaceVolume.CubeSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah volume dari kubus disamping yang memiliki panjang rusuk sebesar " + num1 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1);
                    generatedAnswer = calculateSurfaceVolume.CubeVolume().ToString();
                }
                else if (questionType == 2)
                {
                    num1 = Random.Range(1, 30);
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1);
                    float volume = calculateSurfaceVolume.CubeVolume();
                    generatedQuestion = "Berapakah luas permukaan dari kubus disamping yang memiliki volume sebesar " + volume + " cm ?";
                    generatedAnswer = calculateSurfaceVolume.CubeSurface().ToString();
                }
            }
            else if (randomQuestion == 1)
            {
                int questionType = Random.Range(0, 3);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    num3 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah luas permukaan dari balok disamping yang memiliki panjang " + num1 + " cm, lebar " + num2 + " cm, dan tinggi " + num3 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2, num3);
                    generatedAnswer = calculateSurfaceVolume.BoxSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    num3 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah volume dari balok disamping yang memiliki panjang " + num1 + " cm, lebar " + num2 + " cm, dan tinggi " + num3 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2, num3);
                    generatedAnswer = calculateSurfaceVolume.BoxVolume().ToString();
                }
                else if (questionType == 2)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    num3 = Random.Range(1, 30);
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2, num3);
                    float volume = calculateSurfaceVolume.BoxVolume();
                    generatedQuestion = "Berapakah luas permukaan dari balok disamping yang memiliki volume sebesar " + volume + " cm, jika lebar balok " + num2 + " cm dan tingginya " + num3 + " cm ?";
                    generatedAnswer = calculateSurfaceVolume.BoxSurface().ToString();
                }
            }
            else if (randomQuestion == 2)
            {
                int questionType = Random.Range(0, 3);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    num3 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah luas permukaan dari prisma disamping yang memiliki luas dan keliling alas sebesar " + num1 + " cm dan " + num2 + " cm, dengan tinggi prisma " + num3 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2, num3);
                    generatedAnswer = calculateSurfaceVolume.PrismSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah volume dari prisma disamping yang memiliki luas alas sebesar " + num1 + " cm, dan tinggi " + num2 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.PrismVolume().ToString();
                }
                else if (questionType == 2)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    float volume = calculateSurfaceVolume.PrismVolume();
                    generatedQuestion = "Diketahui volume prisma segitiga disamping adalah " + volume + " cm. Jika luas alasnya " + num1 + " cm. Berapakah tinggi prisma tersebut ?";
                    generatedAnswer = num2.ToString();
                }
            }
            else if (randomQuestion == 3)
            {
                int questionType = Random.Range(0, 2);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah luas permukaan dari limas disamping yang memiliki luas alas sebesar " + num1 + " cm dan luas sisi miring sebesar " + num2 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.PyramidSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah volume dari limas disamping dengan luas alas sebesar " + num1 + " cm, dan tinggi " + num2 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.PyramidVolume().ToString();
                }
            }
            else if (randomQuestion == 4)
            {
                int questionType = Random.Range(0, 2);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah luas permukaan dari tabung disamping yang memiliki tinggi " + num2 + " cm dan jari-jari alas sebesar " + num1 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.TubeSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Berapakah volume dari tabung disamping dengan tinggi " + num2 + " cm dan jari-jari alas sebesar " + num1 + " cm ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.TubeSurface().ToString();
                }
            }
            else if (randomQuestion == 5)
            {
                int questionType = Random.Range(0, 2);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Diketahui kerucut disamping memiliki sisi alas dengan jari-jari " + num1 + " cm dan tinggi " + num2 + " cm. Berapakah luas permukaan kerucut tersebut ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.ConeSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Diketahui kerucut disamping memiliki sisi alas dengan jari-jari " + num1 + " cm dan tinggi " + num2 + " cm. Berapakah volume kerucut tersebut ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1, num2);
                    generatedAnswer = calculateSurfaceVolume.ConeVolume().ToString();
                }
            }
            else if (randomQuestion == 6)
            {
                int questionType = Random.Range(0, 2);
                if (questionType == 0)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Sebuah bola memiliki jari-jari " + num1 + " cm. Berapakah luas permukaan bola tersebut ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1);
                    generatedAnswer = calculateSurfaceVolume.SphereSurface().ToString();
                }
                else if (questionType == 1)
                {
                    num1 = Random.Range(1, 30);
                    num2 = Random.Range(1, 30);
                    generatedQuestion = "Sebuah bola memiliki jari-jari " + num1 + " cm. Berapakah volume bola tersebut ?";
                    calculateSurfaceVolume = new ThreeDimentionalFigure.CalculateSurfaceVolume(num1);
                    generatedAnswer = calculateSurfaceVolume.SphereVolume().ToString();
                }
            }

            choices.Clear();
            choices.Add(generatedAnswer + " cm");
            choices.Add((float.Parse(generatedAnswer) + (float)Random.Range(1, 10) * 2f).ToString() + " cm");
            choices.Add((float.Parse(generatedAnswer) - Random.Range(1, 10) * 2f).ToString() + " cm");
            choices.Add((float.Parse(generatedAnswer) + (float)Random.Range(1, 10)).ToString() + " cm");
            fr = new RCG.FisherYatesRandomizer(generatedQuestion, choices, generatedAnswer + " cm");

            generatedAnswer = fr.Answer;
            fr.ShuffleChoices();
            questionText.text = fr.Question;
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
        else
        {
            for (int i = 0; i < answerBtnList.Count; i++)
            {
                answerBtnList[i].GetComponentInChildren<TMP_Text>().text = fr.Choices[i];
                answerBtnList[i].GetComponent<Image>().color = new Color32(255, 255, 255, 225);
            }
        }
    }

    public void CheckAnswer(int buttonIndex)
    {
        timerActive = false;
        if (StageManager.selectedStage == "unit2_section2")
        {
            Destroy(QuestionPanel.transform.GetChild(2).gameObject);
        }

        foreach (Button btn in answerBtnList)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }

        if (answerBtnList[buttonIndex].GetComponentInChildren<TMP_Text>().text != generatedAnswer)
        {
            answerBtnList[buttonIndex].GetComponent<Image>().color = new Color32(255, 127, 127, 225);
            StartCoroutine(EnemyAttack());
            totalWrongAns++;
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
            if (enemyHP <= 0)
            {
                enemyHP = 0;
                EnemyCamera.Follow = enemyParent.GetChild(1).GetChild(0);
                EnemyCamera.LookAt = enemyParent.GetChild(1).GetChild(0);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    yield return new WaitForSeconds(3f);
                    StartCoroutine(EnemyIsDead());
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(EnemyIsDead());
                }
            }
            else
            {
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack");
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    yield return new WaitForSeconds(0.6f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
                    yield return new WaitForSeconds(2.4f);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
                else
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack");
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    yield return new WaitForSeconds(0.4f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
                    yield return new WaitForSeconds(0.6f);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
            }
        }
        else if (isUltimate)
        {
            enemyHP -= 80;
            if (enemyHP <= 0)
            {
                enemyHP = 0;
                EnemyCamera.Follow = enemyParent.GetChild(1).GetChild(0);
                EnemyCamera.LookAt = enemyParent.GetChild(1).GetChild(0);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isUltimate");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    yield return new WaitForSeconds(3.5f);
                    StartCoroutine(EnemyIsDead());
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                    StartCoroutine(EnemyIsDead());
                }
            }
            else
            {
                if (usedAvatarName == "KumaTheWarriorCommon" || usedAvatarName == "KumaTheWarriorElite")
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isUltimate");
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    yield return new WaitForSeconds(1.2f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
                    yield return new WaitForSeconds(2.3f);
                    avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
                    QuestionPanel.SetActive(false);
                    TimerPanel.SetActive(false);
                    ShowActionPanel();
                }
                else
                {
                    avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isUltimate");
                    QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                    TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                    yield return new WaitForSeconds(0.6f);
                    enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
                    enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
                    yield return new WaitForSeconds(1.4f);
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
        avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
        cameraController.SetBool("Hero", false);
        cameraController.SetBool("Enemy", true);
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = enemyHP;
        enemyParent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enemyHP + "/" + enemyTotalHP;
        yield return new WaitForSeconds(1.5f);
        if (state < 4)
        {
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isRun", true);
            QuestionPanel.SetActive(false);
            TimerPanel.SetActive(false);
            yield return new WaitForSeconds(2f);

            cameraController.SetBool("Enemy", false);
            cameraController.SetBool("Hero", true);
            Destroy(enemyParent.GetChild(1).gameObject);
            backgroundAnim.SetTrigger("state" + (state += 1).ToString());

            if (state == 4)
            {
                AudioManager.instance.SwapTrack(battleBGM[0]);
            }

            StartCoroutine(SpawnEnemy(state));
            yield return new WaitForSeconds(4f);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("isRun", false);
            cameraController.SetBool("Hero", false);
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
        avatarParent.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);

        if (enemyParent.GetChild(1).tag == "1HandedNorm")
        {
            cameraController.SetBool("Hero", false);
            cameraController.SetBool("Enemy", true);
            yield return new WaitForSeconds(1f);

            enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack");
            QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
            TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
            yield return new WaitForSeconds(0.6f);

            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
            yield return new WaitForSeconds(1f);

            StartCoroutine(ReduceHP(enemyDemage));
        }
        else if (enemyParent.GetChild(1).tag == "MageNorm")
        {
            cameraController.SetBool("Hero", false);
            cameraController.SetBool("Enemy", true);
            yield return new WaitForSeconds(1f);

            enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack");
            QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
            TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
            yield return new WaitForSeconds(1f);
            cameraController.SetBool("Enemy", false);
            cameraController.SetBool("Hero", true);
            yield return new WaitForSeconds(0.4f);
            avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

            yield return new WaitForSeconds(1.6f);

            StartCoroutine(ReduceHP(enemyDemage));
        }
        else if (enemyParent.GetChild(1).tag == "WolfmanBoss")
        {
            var attackSkill = Random.Range(1, 5);
            if (attackSkill > 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);

                enemyDemage = 30;
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(1f);

                StartCoroutine(ReduceHP(enemyDemage));
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

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(1f);

                cameraController.SetBool("Enemy", false);
                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(1f);

                StartCoroutine(ReduceHP(enemyDemage));
                QuestionPanel.SetActive(false);
                TimerPanel.SetActive(false);
                ShowActionPanel();
            }
        }
        else if (enemyParent.GetChild(1).tag == "SkeletonBoss")
        {
            var attackSkill = Random.Range(1, 5);
            if (attackSkill > 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);

                enemyDemage = 30;
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2.6f);

                StartCoroutine(ReduceHP(enemyDemage));
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

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.4f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2.4f);
                cameraController.SetBool("Enemy", false);
                cameraController.SetBool("Hero", true);
                yield return new WaitForSeconds(0.8f);
                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2f);
                cameraController.SetBool("Hero", false);


                StartCoroutine(ReduceHP(enemyDemage));
                QuestionPanel.SetActive(false);
                TimerPanel.SetActive(false);
                ShowActionPanel();
            }
        }
        if (enemyParent.GetChild(1).tag == "Demon1")
        {
            var attackSkill = Random.Range(1, 3);
            if (attackSkill == 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(1f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
            else
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(1f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
        }
        if (enemyParent.GetChild(1).tag == "Demon2")
        {
            var attackSkill = Random.Range(1, 3);
            if (attackSkill == 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.4f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(0.6f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
            else
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(1.4f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
        }
        if (enemyParent.GetChild(1).tag == "Demon3")
        {
            var attackSkill = Random.Range(1, 3);
            if (attackSkill == 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(1f);
                cameraController.SetBool("Enemy", false);
                cameraController.SetBool("Hero", true);
                yield return new WaitForSeconds(0.4f);
                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

                yield return new WaitForSeconds(1.6f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
            else
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(1f);
                cameraController.SetBool("Enemy", false);
                cameraController.SetBool("Hero", true);
                yield return new WaitForSeconds(0.4f);
                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

                yield return new WaitForSeconds(2f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
        }
        if (enemyParent.GetChild(1).tag == "Demon4")
        {
            var attackSkill = Random.Range(1, 3);
            if (attackSkill == 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2.4f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
            else
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(1.2f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2.3f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
        }
        if (enemyParent.GetChild(1).tag == "DemonBoss")
        {
            var attackSkill = Random.Range(1, 3);
            if (attackSkill == 1)
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack1");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(0.6f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2.4f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
            else
            {
                cameraController.SetBool("Hero", false);
                cameraController.SetBool("Enemy", true);
                yield return new WaitForSeconds(1f);

                enemyParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isAttack2");
                QuestionPanel.GetComponent<Animator>().SetBool("isClose", true);
                TimerPanel.GetComponent<Animator>().SetBool("isClose", true);
                yield return new WaitForSeconds(1.2f);

                avatarParent.GetChild(1).GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
                yield return new WaitForSeconds(2.3f);

                StartCoroutine(ReduceHP(enemyDemage));
            }
        }
    }
}
