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

public class ShopManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    [Space(5)]

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text totalGoldText;
    [SerializeField]
    private TMP_Text totalSilverText;
    [SerializeField]
    private TMP_Text currencyPopupText;
    [SerializeField]
    private GameObject goldCurrencyBtn;
    [SerializeField]
    private GameObject silverCurrencyBtn;
    [SerializeField]
    private GameObject WarningPopup;
    [SerializeField]
    private GameObject AchievementPopup;
    [SerializeField]
    private List<Sprite> achievementSprite;
    [SerializeField]
    private Image achievementImage;
    [SerializeField]
    private TMP_Text rewardTextLuck;
    [SerializeField]
    private TMP_Text rewardTextUnluck;
    [SerializeField]
    private GameObject rewardPopupLuck;
    [SerializeField]
    private GameObject rewardPopupUnluck;
    [SerializeField]
    private Transform rewardSpawn1;
    [SerializeField]
    private Transform rewardSpawn2;
    [SerializeField]
    private List<GameObject> rewardList = new List<GameObject>();
    [Space(5)]

    [Header("Audio")]
    [SerializeField]
    private AudioClip HomeScreenTrack;
    [SerializeField]
    private AudioClip LessonsScreenTrack;
    [SerializeField]
    private AudioClip RankScreenTrack;
    [SerializeField]
    private AudioClip LoginScreenTrack;
    [SerializeField]
    private AudioClip ProfileScreenTrack;

    private ShopManager instance;
    private string totalGold;
    private string totalSilver;
    private bool UsingGold;

    private void Awake()
    {
        instance = this.gameObject.GetComponent<ShopManager>();
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

    private void Start()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;
        StartCoroutine(GetUserData());
        UsingGold = true;
        List<Sprite> achievementIachievementSprite = new List<Sprite>();
    }

    public IEnumerator GetUserData()
    {
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

                totalGoldText.text = totalGold;
                totalSilverText.text = totalSilver;
            }
        }
    }

    public void PullBtnClicked()
    {
        StartCoroutine(PullAction());
    }

    private IEnumerator PullAction()
    {
        if (UsingGold)
        {
            var GetTotalGoldTask = DBreference.Child("users").Child(user.UserId).Child("total_gold").GetValueAsync();
            yield return new WaitUntil(() => GetTotalGoldTask.IsCompleted);

            if (GetTotalGoldTask.Exception != null)
            {
                Debug.LogError(GetTotalGoldTask.Exception.Message);
            }
            else
            {
                var goldAmount = int.Parse(GetTotalGoldTask.Result.Value.ToString());

                if (goldAmount >= 1200)
                {
                    totalGoldText.text = (goldAmount - 1200).ToString();
                    goldAmount = (goldAmount - 1200);
                    var UpdateTotalGold = DBreference.Child("users").Child(user.UserId).Child("total_gold").SetValueAsync(goldAmount);
                    yield return new WaitUntil(() => UpdateTotalGold.IsCompleted);

                    if (UpdateTotalGold.Exception != null)
                    {
                        Debug.LogError(UpdateTotalGold.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("Total Gold Updated");

                        var number = UnityEngine.Random.Range(0, 5);
                        Debug.Log("Seed: " + number);

                        if (number >= 2)
                        {
                            var number2 = UnityEngine.Random.Range(0, 4);
                            Debug.Log("Common: " + number2);

                            if (number2 <= 1)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaThePlatoCommon", rewardList[0].gameObject));
                            }
                            else if (number2 >= 2)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaTheWarriorCommon", rewardList[2].gameObject));
                            }
                        }
                        else
                        {
                            var number3 = UnityEngine.Random.Range(0, 4);
                            Debug.Log("Elite: " + number3);

                            if (number3 <= 1)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaThePlatoElite", rewardList[1].gameObject));
                            }
                            else if (number3 >= 2)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaTheWarriorElite", rewardList[3].gameObject));
                            }
                        }
                    }

                }
                else
                {
                    WarningPopup.SetActive(true);
                    currencyPopupText.text = "Insufficient Gold, try to use a different currency.\n\nYou can do Gold farming by completing many matches.";
                    WarningPopup.GetComponent<Animator>().SetBool("isOpen", true);
                }
            }
        }
        else
        {
            var GetTotalSilverTask = DBreference.Child("users").Child(user.UserId).Child("total_silver").GetValueAsync();
            yield return new WaitUntil(() => GetTotalSilverTask.IsCompleted);

            if (GetTotalSilverTask.Exception != null)
            {
                Debug.LogError(GetTotalSilverTask.Exception.Message);
            }
            else
            {
                var silverAmount = int.Parse(GetTotalSilverTask.Result.Value.ToString());

                if (silverAmount >= 600)
                {
                    totalSilverText.text = (silverAmount - 600).ToString();
                    silverAmount = (silverAmount - 600);
                    var UpdateTotalSilver = DBreference.Child("users").Child(user.UserId).Child("total_silver").SetValueAsync(silverAmount);
                    yield return new WaitUntil(() => UpdateTotalSilver.IsCompleted);

                    if (UpdateTotalSilver.Exception != null)
                    {
                        Debug.LogError(UpdateTotalSilver.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("Total Silver Updated");

                        var number = UnityEngine.Random.Range(0, 5);
                        Debug.Log("Seed: " + number);

                        if (number >= 2)
                        {
                            var number2 = UnityEngine.Random.Range(0, 4);
                            Debug.Log("Common: " + number2);

                            if (number2 <= 1)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaThePlatoCommon", rewardList[0].gameObject));
                            }
                            else if (number2 >= 2)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaTheWarriorCommon", rewardList[2].gameObject));
                            }
                        }
                        else
                        {
                            var number3 = UnityEngine.Random.Range(0, 4);
                            Debug.Log("Elite: " + number3);

                            if (number3 <= 1)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaThePlatoElite", rewardList[1].gameObject));
                            }
                            else if (number3 >= 2)
                            {
                                StartCoroutine(OwnedAvatarCheck("KumaTheWarriorElite", rewardList[3].gameObject));
                            }
                        }
                    }
                }
                else
                {
                    WarningPopup.SetActive(true);
                    currencyPopupText.text = "Insufficient Silver, try to use a different currency.\n\nYou can do Silver farming by doing pull.";
                    WarningPopup.GetComponent<Animator>().SetBool("isOpen", true);
                }
            }
        }
        {

        }
    }

    private IEnumerator OwnedAvatarCheck(string _avatarName, GameObject _avatar)
    {
        var GetAvatarTask = DBreference.Child("avatars").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => GetAvatarTask.IsCompleted);

        if (GetAvatarTask.Exception != null)
        {
            Debug.LogError(GetAvatarTask.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = GetAvatarTask.Result;
            List<string> avatarList = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                var avatarName = childSnapshot.Value.ToString();
                avatarList.Add(avatarName);
                Debug.Log("Avatar Name: " + avatarName);
            }
            avatarList.Reverse();


            if (avatarList.Count != 0)
            {
                if (avatarList.Contains(_avatarName))
                {
                    var GetTotalSilverTask = DBreference.Child("users").Child(user.UserId).Child("total_silver").GetValueAsync();
                    yield return new WaitUntil(() => GetTotalSilverTask.IsCompleted);

                    if (GetTotalSilverTask.Exception != null)
                    {
                        Debug.LogError(GetTotalSilverTask.Exception.Message);
                    }
                    else
                    {
                        totalSilver = (int.Parse(GetTotalSilverTask.Result.Value.ToString()) + 200).ToString();

                        var UpdateTotalSilverTask = DBreference.Child("users").Child(user.UserId).Child("total_silver").SetValueAsync(totalSilver);
                        yield return new WaitUntil(() => UpdateTotalSilverTask.IsCompleted);

                        if (UpdateTotalSilverTask.Exception != null)
                        {
                            Debug.LogError(UpdateTotalSilverTask.Exception.Message);
                        }
                        else
                        {
                            totalSilverText.text = totalSilver;
                            var rewardName = "";

                            switch (_avatarName)
                            {
                                case "KumaThePlatoCommon":
                                    rewardName = "Kuma The Plato [Common]";
                                    break;

                                case "KumaThePlatoElite":
                                    rewardName = "Kuma The Plato [Elite]";
                                    break;

                                case "KumaTheWarriorCommon":
                                    rewardName = "Kuma The Warrior [Common]";
                                    break;

                                case "KumaTheWarriorElite":
                                    rewardName = "Kuma The Warrior [Elite]";
                                    break;
                            }

                            Debug.Log("Total Silver Updated");
                            rewardPopupUnluck.SetActive(true);
                            rewardPopupUnluck.GetComponent<Animator>().SetBool("isClose", false);
                            rewardTextUnluck.text = rewardName;
                            Instantiate(_avatar, rewardSpawn2);
                        }
                    }
                }
                else
                {
                    var UpdateAvatarTask = DBreference.Child("avatars").Child(user.UserId).Push().SetValueAsync(_avatarName);
                    yield return new WaitUntil(() => UpdateAvatarTask.IsCompleted);

                    if (UpdateAvatarTask.Exception != null)
                    {
                        Debug.LogError(UpdateAvatarTask.Exception.Message);
                    }
                    else
                    {
                        var rewardName = "";

                        switch (_avatarName)
                        {
                            case "KumaThePlatoCommon":
                                rewardName = "Kuma The Plato [Common]";
                                break;

                            case "KumaThePlatoElite":
                                rewardName = "Kuma The Plato [Elite]";
                                break;

                            case "KumaTheWarriorCommon":
                                rewardName = "Kuma The Warrior [Common]";
                                break;

                            case "KumaTheWarriorElite":
                                rewardName = "Kuma The Warrior [Elite]";
                                break;
                        }

                        rewardPopupLuck.SetActive(true);
                        rewardPopupLuck.GetComponent<Animator>().SetBool("isClose", false);
                        rewardTextLuck.text = rewardName;
                        Instantiate(_avatar, rewardSpawn1);
                    }
                }
            }

        }
    }

    public void ClosePullWindowLuck()
    {
        StartCoroutine(ClosePullLuck());
    }

    public void ClosePullWindowUnluck()
    {
        StartCoroutine(ClosePullUnluck());
    }

    private IEnumerator ClosePullLuck()
    {
        rewardPopupLuck.GetComponent<Animator>().SetBool("isClose", true);
        yield return new WaitForSeconds(0.6f);
        rewardPopupLuck.SetActive(false);

        var GetAvatarTask = DBreference.Child("avatars").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => GetAvatarTask.IsCompleted);

        if (GetAvatarTask.Exception != null)
        {
            Debug.LogError(GetAvatarTask.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = GetAvatarTask.Result;
            List<string> avatarList = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                var avatarName = childSnapshot.Value.ToString();
                avatarList.Add(avatarName);
                Debug.Log("Avatar Name: " + avatarName);
            }
            avatarList.Reverse();

            if (avatarList.Count == 2)
            {
                StartCoroutine(AchievementCheck("2"));
            }
            else if (avatarList.Count == 3)
            {
                StartCoroutine(AchievementCheck("3"));
            }
            else if (avatarList.Count == 4)
            {
                StartCoroutine(AchievementCheck("4"));
            }
        }
    }

    private IEnumerator AchievementCheck(string _totalAvatar)
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
            if (!achievementList.Contains("avatar" + _totalAvatar))
            {
                var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("avatar" + _totalAvatar);
                yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                if (UpdateAchievementsTask.Exception != null)
                {
                    Debug.LogError(UpdateAchievementsTask.Exception.Message);
                }
                else
                {
                    Debug.Log("Achievement for " + _totalAvatar + " avatars has been added");
                    if (_totalAvatar == "2")
                    {
                        achievementImage.sprite = achievementSprite[0];
                    }
                    else if (_totalAvatar == "3")
                    {
                        achievementImage.sprite = achievementSprite[1];
                    }
                    else if (_totalAvatar == "4")
                    {
                        achievementImage.sprite = achievementSprite[2];
                    }
                    AchievementPopup.SetActive(true);
                    AchievementPopup.GetComponent<Animator>().SetBool("isOpen", true);
                }
            }
        }
    }

    private IEnumerator ClosePullUnluck()
    {
        rewardPopupUnluck.GetComponent<Animator>().SetBool("isClose", true);
        yield return new WaitForSeconds(0.6f);
        rewardPopupUnluck.SetActive(false);
    }

    public void CloseWarning()
    {
        StartCoroutine(CloseWarningPopUp());
    }

    private IEnumerator CloseWarningPopUp()
    {
        WarningPopup.GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(0.6f);
        WarningPopup.SetActive(false);
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

    public void SwapCurrency()
    {
        if (UsingGold)
        {
            UsingGold = false;
            goldCurrencyBtn.SetActive(false);
            silverCurrencyBtn.SetActive(true);
        }
        else
        {
            UsingGold = true;
            goldCurrencyBtn.SetActive(true);
            silverCurrencyBtn.SetActive(false);
        }
    }

    public void homeButtonClicked()
    {
        SwapTrackToHome();
        GameManager.instance.ChangeScene(2);
    }

    private void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(HomeScreenTrack);
    }
    public void lessonsButtonClicked()
    {
        SwapTrackToLessonsScreen();
        GameManager.instance.ChangeScene(3);
    }

    private void SwapTrackToLessonsScreen()
    {
        AudioManager.instance.SwapTrack(LessonsScreenTrack);
    }

    public void rankButtonClicked()
    {
        SwapTrackToRankScreen();
        GameManager.instance.ChangeScene(7);
    }

    private void SwapTrackToRankScreen()
    {
        AudioManager.instance.SwapTrack(RankScreenTrack);
    }

    public void profileButtonClicked()
    {
        SwapTrackToProfileScreen();
        GameManager.instance.ChangeScene(8);
    }

    private void SwapTrackToProfileScreen()
    {
        AudioManager.instance.SwapTrack(ProfileScreenTrack);
    }
}
