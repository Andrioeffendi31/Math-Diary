using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text usernameText;
    [SerializeField]
    private TMP_Text currentLVText;
    [SerializeField]
    private Slider LVSlider;
    [SerializeField]
    private Button ContinueBtn;
    [SerializeField]
    private Image GoldPopup;
    [SerializeField]
    private Image achievementImage;
    [SerializeField]
    private List<Sprite> achievementSprite;
    [SerializeField]
    private GameObject AchievementPopup;
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
    [SerializeField]
    private GameObject LevelUpVFX;
    [Space(5)]

    private LevelSystem instance;
    private string username;
    private int currentLV;
    private int currentXPValue;
    private int XPToNextLV;
    private int totalGold;
    private int totalLP;
    private string usedAvatarName;

    public static string RecentMatchKey;

    private void Start()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;
        RecentMatchKey = "";
        StartCoroutine(GetUserData());
    }

    public IEnumerator GetUserData()
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

            var GetCurrentLvTask = DBreference.Child("users").Child(user.UserId).Child("level").GetValueAsync();

            yield return new WaitUntil(() => GetCurrentLvTask.IsCompleted);

            if (GetCurrentLvTask.Exception != null)
            {
                Debug.LogError(GetCurrentLvTask.Exception.Message);
            }
            else
            {
                currentLV = int.Parse(GetCurrentLvTask.Result.Value.ToString());
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

                        var GetTotalGoldTask = DBreference.Child("users").Child(user.UserId).Child("total_gold").GetValueAsync();

                        yield return new WaitUntil(() => GetTotalGoldTask.IsCompleted);

                        if (GetTotalGoldTask.Exception != null)
                        {
                            Debug.LogError(GetTotalGoldTask.Exception.Message);
                        }
                        else
                        {
                            totalGold = int.Parse(GetTotalGoldTask.Result.Value.ToString());
                            Debug.Log("Total Gold: " + totalGold);

                            var GetTotalLPTask = DBreference.Child("users").Child(user.UserId).Child("total_lp").GetValueAsync();

                            yield return new WaitUntil(() => GetTotalLPTask.IsCompleted);

                            if (GetTotalLPTask.Exception != null)
                            {
                                Debug.LogError(GetTotalLPTask.Exception.Message);
                            }
                            else
                            {
                                totalLP = int.Parse(GetTotalLPTask.Result.Value.ToString());
                                Debug.Log("Total LP: " + totalLP);

                                StartCoroutine(GetUsedAvatar());
                                setupXPBar(currentXPValue, XPToNextLV);
                                StartCoroutine(PushRecentMatch());

                                usernameText.text = username;
                                currentLVText.text = "Lv. " + currentLV;
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator PushRecentMatch()
    {
        float Accuracy = ((float)BattleManager.totalCorrectAns / ((float)BattleManager.totalCorrectAns + (float)BattleManager.totalWrongAns)) * 100;
        RecentMatchKey = DBreference.Child("matches").Child(user.UserId).Push().Key;
        yield return new WaitUntil(() => RecentMatchKey != null);

        if (RecentMatchKey != null)
        {
            var pushRecentMatchTask = DBreference.Child("matches").Child(user.UserId).Child(RecentMatchKey).Child("total_HP").SetValueAsync(BattleManager.totalHP);
            var overallScore = 0;

            if (StageManager.selectedStage == "unit1_section1")
            {
                overallScore = (BattleManager.totalCorrectAns * 5) + (BattleManager.totalHP * 2);
            }
            else if (StageManager.selectedStage == "unit1_section2")
            {
                overallScore = (BattleManager.totalCorrectAns * 5) + (BattleManager.totalHP * 2);
            }
            else if (StageManager.selectedStage == "unit2_section1")
            {
                overallScore = (BattleManager.totalCorrectAns * 5) + (BattleManager.totalHP * 2);
            }
            else if (StageManager.selectedStage == "unit2_section2")
            {
                overallScore = (BattleManager.totalCorrectAns * 10) + (BattleManager.totalHP * 2);
            }
            else if (StageManager.selectedStage == "unit3_section1")
            {
                overallScore = (BattleManager.totalCorrectAns * 5) + (BattleManager.totalHP * 2);
            }
            else if (StageManager.selectedStage == "unit3_section2")
            {
                overallScore = (BattleManager.totalCorrectAns * 15) + (BattleManager.totalHP * 2);
            }

            pushRecentMatchTask = DBreference.Child("matches").Child(user.UserId).Child(RecentMatchKey).Child("totalCorrectAns").SetValueAsync(BattleManager.totalCorrectAns);
            pushRecentMatchTask = DBreference.Child("matches").Child(user.UserId).Child(RecentMatchKey).Child("totalWrongAns").SetValueAsync(BattleManager.totalWrongAns);
            pushRecentMatchTask = DBreference.Child("matches").Child(user.UserId).Child(RecentMatchKey).Child("Accuracy").SetValueAsync((int)Accuracy);
            pushRecentMatchTask = DBreference.Child("matches").Child(user.UserId).Child(RecentMatchKey).Child("overallScore").SetValueAsync(overallScore);
            pushRecentMatchTask = DBreference.Child("users").Child(user.UserId).Child("total_lp").SetValueAsync(totalLP + overallScore);

            yield return new WaitUntil(() => pushRecentMatchTask.IsCompleted);

            if (pushRecentMatchTask.Exception != null)
            {
                Debug.LogError(pushRecentMatchTask.Exception.Message);
            }
            else
            {
                Debug.Log("Recent Match Data Pushed");
            }
        }
    }

    private void setupXPBar(int _currentXPValue, int _XPToNextLV)
    {
        LVSlider.maxValue = _XPToNextLV;
        LVSlider.value = _currentXPValue;

        if (BattleManager.isWin == true)
        {
            GoldPopup.gameObject.SetActive(true);
            if (StageManager.selectedStage == "unit1_section1")
            {
                StartCoroutine(AddGold(200));
                StartCoroutine(AddExp(150));
            }
            else if (StageManager.selectedStage == "unit1_section2")
            {
                StartCoroutine(AddGold(200));
                StartCoroutine(AddExp(150));
            }
            else if (StageManager.selectedStage == "unit2_section1")
            {
                StartCoroutine(AddGold(200));
                StartCoroutine(AddExp(150));
            }
            else if (StageManager.selectedStage == "unit2_section2")
            {
                StartCoroutine(AddGold(400));
                StartCoroutine(AddExp(250));
            }
            else if (StageManager.selectedStage == "unit3_section1")
            {
                StartCoroutine(AddGold(200));
                StartCoroutine(AddExp(150));
            }
            else if (StageManager.selectedStage == "unit3_section2")
            {
                StartCoroutine(AddGold(500));
                StartCoroutine(AddExp(400));
            }
            StartCoroutine(PushClearMatch());
        }
        else
        {
            StartCoroutine(AddExp(20));
        }

    }

    private IEnumerator PushClearMatch()
    {
        var GetClearedStageTask = DBreference.Child("stages").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => GetClearedStageTask.IsCompleted);
        if (GetClearedStageTask.Exception != null)
        {
            Debug.LogError(GetClearedStageTask.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = GetClearedStageTask.Result;
            List<string> stageList = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                var stageName = childSnapshot.Value.ToString();
                stageList.Add(stageName);
            }
            stageList.Reverse();
            if (!stageList.Contains(StageManager.selectedStage))
            {
                var PushRecentClearMatchTask = DBreference.Child("stages").Child(user.UserId).Push().SetValueAsync(StageManager.selectedStage);
                yield return new WaitUntil(() => PushRecentClearMatchTask.IsCompleted);
                if (PushRecentClearMatchTask.Exception != null)
                {
                    Debug.LogError(PushRecentClearMatchTask.Exception.Message);
                }
            }
        }
    }

    private IEnumerator AddGold(int amount)
    {
        totalGold += amount;
        var SetTotalGoldTask = DBreference.Child("users").Child(user.UserId).Child("total_gold").SetValueAsync(totalGold);

        yield return new WaitUntil(() => SetTotalGoldTask.IsCompleted);

        if (SetTotalGoldTask.Exception != null)
        {
            Debug.LogError(SetTotalGoldTask.Exception.Message);
        }
    }

    private IEnumerator AddExp(int amount)
    {
        var totalEXPtoAdd = currentXPValue + amount;
        if (totalEXPtoAdd >= XPToNextLV)
        {
            var addExpTask = DBreference.Child("users").Child(user.UserId).Child("xp").SetValueAsync(totalEXPtoAdd - XPToNextLV);
            if (addExpTask.Exception != null)
            {
                Debug.LogError(addExpTask.Exception.Message);
            }
        }
        else
        {
            var addExpTask = DBreference.Child("users").Child(user.UserId).Child("xp").SetValueAsync(totalEXPtoAdd);
            if (addExpTask.Exception != null)
            {
                Debug.LogError(addExpTask.Exception.Message);
            }
        }

        yield return new WaitForSeconds(3f);
        for (int i = currentXPValue; i < totalEXPtoAdd; i++)
        {
            LVSlider.value = i;

            if (i >= XPToNextLV)
            {
                currentLV++;
                var addLevelTask = DBreference.Child("users").Child(user.UserId).Child("level").SetValueAsync(currentLV);
                addLevelTask = DBreference.Child("users").Child(user.UserId).Child("xp_to_next_level").SetValueAsync((int)(XPToNextLV * 1.6f));
                if (addLevelTask.Exception != null)
                {
                    Debug.LogError(addLevelTask.Exception.Message);
                }
                else
                {
                    currentLVText.text = "Lv. " + currentLV;
                    i = 0;
                    LVSlider.maxValue = (int)(XPToNextLV * 1.6f);
                    totalEXPtoAdd -= XPToNextLV;
                    LevelUpVFX.SetActive(true);

                    if (currentLV == 10)
                    {
                        StartCoroutine(AchievementCheck(currentLV));
                    }
                    else if (currentLV == 20)
                    {
                        StartCoroutine(AchievementCheck(currentLV));
                    }
                    else if (currentLV == 40)
                    {
                        StartCoroutine(AchievementCheck(currentLV));
                    }
                    else if (currentLV == 60)
                    {
                        StartCoroutine(AchievementCheck(currentLV));
                    }
                    else if (currentLV == 80)
                    {
                        StartCoroutine(AchievementCheck(currentLV));
                    }
                    else if (currentLV == 100)
                    {
                        StartCoroutine(AchievementCheck(currentLV));
                    }
                }
            }
            yield return new WaitForSeconds(.0001f);
        }
        ContinueBtn.interactable = true;
        LevelUpVFX.SetActive(false);
    }

    private IEnumerator AchievementCheck(int _level)
    {
        var GetAchievementsTask = DBreference.Child("achievements").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => GetAchievementsTask.IsCompleted);

        if (GetAchievementsTask.Exception != null)
        {
            Debug.LogError(GetAchievementsTask.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = GetAchievementsTask.Result;
            List<string> achievementList = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                var achievementName = childSnapshot.Value.ToString();
                achievementList.Add(achievementName);
                Debug.Log("Achievement Name: " + achievementName);
            }
            achievementList.Reverse();
            if (!achievementList.Contains("Lv" + _level))
            {
                var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Lv" + _level);
                yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                if (UpdateAchievementsTask.Exception != null)
                {
                    Debug.LogError(UpdateAchievementsTask.Exception.Message);
                }
                else
                {
                    Debug.Log("The achievement to reach level " + _level + " has been added");
                    if (_level == 10)
                    {
                        achievementImage.sprite = achievementSprite[0];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached Lv 10", 400));
                    }
                    else if (_level == 20)
                    {
                        achievementImage.sprite = achievementSprite[1];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached Lv 20", 800));
                    }
                    else if (_level == 40)
                    {
                        achievementImage.sprite = achievementSprite[2];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached Lv 40", 1200));
                    }
                    else if (_level == 60)
                    {
                        achievementImage.sprite = achievementSprite[3];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached Lv 60", 1500));
                    }
                    else if (_level == 80)
                    {
                        achievementImage.sprite = achievementSprite[4];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached Lv 80", 3000));
                    }
                    else if (_level == 100)
                    {
                        achievementImage.sprite = achievementSprite[5];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached Lv 100", 5000));
                    }
                }
            }
        }
    }

    private IEnumerator pushAchievementMail(string achievementName, int rewardAmount)
    {
        var MailKey = DBreference.Child("mails").Child(user.UserId).Push().Key;
        var MailTask = DBreference.Child("mails").Child(user.UserId).Child(MailKey).Child("Title").SetValueAsync(achievementName);
        MailTask = DBreference.Child("mails").Child(user.UserId).Child(MailKey).Child("Desc").SetValueAsync("Congratulations!, you have " + achievementName + ". Keep improving your math skills! This is a reward for your achievements.");
        MailTask = DBreference.Child("mails").Child(user.UserId).Child(MailKey).Child("Date").SetValueAsync(DateTime.Now.ToString());
        MailTask = DBreference.Child("mails").Child(user.UserId).Child(MailKey).Child("Reward").SetValueAsync(rewardAmount);
        MailTask = DBreference.Child("mails").Child(user.UserId).Child(MailKey).Child("Read").SetValueAsync(0);

        yield return new WaitUntil(() => MailTask.IsCompleted);
        if (MailTask.Exception != null)
        {
            Debug.LogError(MailTask.Exception.Message);
        }
    }

    private void openAchievementPopup()
    {
        AchievementPopup.SetActive(true);
        AchievementPopup.GetComponent<Animator>().SetBool("isOpen", true);
    }

    public void CloseAchievement()
    {
        StartCoroutine(CloseAchievementPopUp());
    }

    private IEnumerator CloseAchievementPopUp()
    {
        AchievementPopup.GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(0.6f);
        AchievementPopup.SetActive(false);
    }

    public IEnumerator AddExperience(int amount)
    {
        for (int i = currentXPValue; i < currentXPValue + amount; i++)
        {
            LVSlider.value++;
            yield return new WaitForSeconds(.001f);
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
            }
            else if (usedAvatarName == "KumaThePlatoElite")
            {
                Avatar = Instantiate(PlatoElitePrefab, avatarParent);
            }
            else if (usedAvatarName == "KumaTheWarriorCommon")
            {
                Avatar = Instantiate(WarriorCommonPrefab, avatarParent);
            }
            else if (usedAvatarName == "KumaTheWarriorElite")
            {
                Avatar = Instantiate(WarriorElitePrefab, avatarParent);
            }
        }
    }

    public void ChangeScene()
    {
        GameManager.instance.ChangeScene(16);
        StopAllCoroutines();
    }
}
