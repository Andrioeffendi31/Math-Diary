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

public class HomeManager : MonoBehaviour
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
    private TMP_Text totalLPText;
    [SerializeField]
    private TMP_Text totalGoldText;
    [SerializeField]
    private TMP_Text totalSilverText;
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private TMP_Text currentLVText;
    [SerializeField]
    private TMP_Text userRankText;
    [SerializeField]
    private Slider LVSlider;
    [SerializeField]
    private Image achievementImage;
    [SerializeField]
    private List<Sprite> achievementSprite;
    [SerializeField]
    private GameObject AchievementPopup;
    [SerializeField]
    private List<Button> navbarButtons;
    [Space(5)]

    [Header("Audio")]
    [SerializeField]
    private AudioClip LessonsScreenTrack;
    [SerializeField]
    private AudioClip RankScreenTrack;
    [SerializeField]
    private AudioClip LoginScreenTrack;
    [SerializeField]
    private AudioClip ProfileScreenTrack;
    [SerializeField]
    private AudioClip ShopScreenTrack;
    [SerializeField]
    private AudioClip StageSelectionTrack;

    public static HomeManager instance;
    private string photoURL;
    private string username;
    private int totalLP;
    private string totalGold;
    private string totalSilver;
    private string currentLV;
    private int currentXPValue;
    private int XPToNextLV;
    private int userRank;
    private float ButtonReactivateDelay = 1.4f;

    private void Awake()
    {
        instance = this.gameObject.GetComponent<HomeManager>();
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
        storageRef = storage.GetReferenceFromUrl("gs://mathdiary-d169a.appspot.com/avatar/Kuma.png");
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
                                    StartCoroutine(GetUserRank());

                                    usernameText.text = username;
                                    totalLPText.text = totalLP + " LP";
                                    totalGoldText.text = totalGold;
                                    totalSilverText.text = totalSilver;
                                    currentLVText.text = "Lv. " + currentLV;
                                }
                            }

                        }
                    }
                }
            }
        }
    }

    private IEnumerator AchievementCheck(int _totalLP, int _rank)
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

            yield return new WaitForSeconds(1.2f);
            if (_totalLP >= 1000 && _totalLP < 5000)
            {
                if (!achievementList.Contains("LP1000"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("LP1000");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement to get 1000 LP has been added");
                        achievementImage.sprite = achievementSprite[0];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Earned 1000 LP", 600));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }
            else if (_totalLP >= 5000 && _totalLP < 10000)
            {
                if (!achievementList.Contains("LP5000"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("LP5000");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement to get 5000 LP has been added");
                        achievementImage.sprite = achievementSprite[1];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Earned 5000 LP", 800));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }
            else if (_totalLP >= 10000 && _totalLP < 20000)
            {
                if (!achievementList.Contains("LP10000"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("LP10000");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement to get 10000 LP has been added");
                        achievementImage.sprite = achievementSprite[2];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Earned 10000 LP", 1000));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }
            else if (_totalLP >= 20000 && _totalLP < 40000)
            {
                if (!achievementList.Contains("LP20000"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("LP20000");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement to get 20000 LP has been added");
                        achievementImage.sprite = achievementSprite[3];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Earned 20000 LP", 1500));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }
            else if (_totalLP >= 40000 && _totalLP < 80000)
            {
                if (!achievementList.Contains("LP40000"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("LP40000");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement to get 40000 LP has been added");
                        achievementImage.sprite = achievementSprite[4];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Earned 40000 LP", 2000));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }
            else if (_totalLP >= 100000)
            {
                if (!achievementList.Contains("LP100000"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("LP100000");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement to get 100000 LP has been added");
                        achievementImage.sprite = achievementSprite[5];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Earned 100000 LP", 5000));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }

            if (_rank <= 50 && _rank > 10)
            {
                if (!achievementList.Contains("Top50"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top50");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        Debug.Log("The achievement for reaching top 50 has been added");
                        achievementImage.sprite = achievementSprite[6];
                        openAchievementPopup();
                        StartCoroutine(pushAchievementMail("Reached the top 50", 800));
                        StartCoroutine(MailboxManager.instance.GetMail());
                    }
                }
            }
            else if (_rank <= 10 && _rank > 3)
            {
                if (!achievementList.Contains("Top50"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top50");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        StartCoroutine(pushAchievementMail("Reached the top 50", 800));
                        StartCoroutine(MailboxManager.instance.GetMail());
                        Debug.Log("The achievement for reaching top 50 has been added");
                        achievementImage.sprite = achievementSprite[6];

                        if (!achievementList.Contains("Top10"))
                        {
                            UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top10");
                            yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                            if (UpdateAchievementsTask.Exception != null)
                            {
                                Debug.LogError(UpdateAchievementsTask.Exception.Message);
                            }
                            else
                            {
                                StartCoroutine(pushAchievementMail("Reached the top 10", 1500));
                                StartCoroutine(MailboxManager.instance.GetMail());
                                Debug.Log("The achievement for reaching top 10 has been added");
                                achievementImage.sprite = achievementSprite[7];
                            }
                        }
                        openAchievementPopup();
                    }
                }
                else
                {
                    if (!achievementList.Contains("Top10"))
                    {
                        var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top10");
                        yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                        if (UpdateAchievementsTask.Exception != null)
                        {
                            Debug.LogError(UpdateAchievementsTask.Exception.Message);
                        }
                        else
                        {
                            StartCoroutine(pushAchievementMail("Reached the top 10", 1500));
                            StartCoroutine(MailboxManager.instance.GetMail());
                            Debug.Log("The achievement for reaching top 10 has been added");
                            achievementImage.sprite = achievementSprite[7];
                            openAchievementPopup();
                        }
                    }
                }
            }
            else if (_rank <= 3)
            {
                if (!achievementList.Contains("Top50"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top50");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        StartCoroutine(pushAchievementMail("Reached the top 50", 800));
                        StartCoroutine(MailboxManager.instance.GetMail());
                        Debug.Log("The achievement for reaching top 50 has been added");
                        achievementImage.sprite = achievementSprite[6];
                        openAchievementPopup();
                    }
                }

                if (!achievementList.Contains("Top10"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top10");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        StartCoroutine(pushAchievementMail("Reached the top 10", 1500));
                        StartCoroutine(MailboxManager.instance.GetMail());
                        Debug.Log("The achievement for reaching top 10 has been added");
                        achievementImage.sprite = achievementSprite[7];
                        openAchievementPopup();
                    }
                }

                if (!achievementList.Contains("Top3"))
                {
                    var UpdateAchievementsTask = DBreference.Child("achievements").Child(user.UserId).Push().SetValueAsync("Top3");
                    yield return new WaitUntil(() => UpdateAchievementsTask.IsCompleted);

                    if (UpdateAchievementsTask.Exception != null)
                    {
                        Debug.LogError(UpdateAchievementsTask.Exception.Message);
                    }
                    else
                    {
                        StartCoroutine(pushAchievementMail("Reached the top 3", 3000));
                        StartCoroutine(MailboxManager.instance.GetMail());
                        Debug.Log("The achievement for reaching top 3 has been added");
                        achievementImage.sprite = achievementSprite[8];
                        openAchievementPopup();
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
        LVSlider.maxValue = 100;
        _experienceToAdd = (_experienceToAdd / _XPToNextLV) * 100;
        Debug.Log("Experience to Add: " + _experienceToAdd);
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < _experienceToAdd; i++)
        {
            LVSlider.value++;
            yield return new WaitForSeconds(.001f);
        }
    }

    public IEnumerator GetUserRank()
    {
        var GetRankTask = DBreference.Child("users").OrderByChild("total_lp").GetValueAsync();

        yield return new WaitUntil(() => GetRankTask.IsCompleted);

        if (GetRankTask.Exception != null)
        {
            Debug.LogError(GetRankTask.Exception);
        }
        else
        {
            DataSnapshot snapshot = GetRankTask.Result;

            List<Tuple<string, string, string>> rankList = new List<Tuple<string, string, string>>();
            foreach (var childSnapshot in snapshot.Children)
            {
                var username = childSnapshot.Child("username").Value.ToString();
                var total_lp = childSnapshot.Child("total_lp").Value.ToString();

                rankList.Add(new Tuple<string, string, string>(username, total_lp, childSnapshot.Key));
            }
            rankList.Reverse();
            foreach (var item in rankList)
            {
                Debug.Log("username : " + item.Item1);
                Debug.Log("LP : " + item.Item2);
                Debug.Log("userID : " + item.Item3);
            }
            userRank = rankList.FindIndex(x => x.Item3 == user.UserId) + 1;
            userRankText.text = "#" + userRank;
            Debug.Log("User Rank: " + userRank);
            StartCoroutine(AchievementCheck(totalLP, userRank));
        }
    }

    public void lessonsButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToLessonsScreen();
        GameManager.instance.ChangeScene(3);
    }

    private void SwapTrackToLessonsScreen()
    {
        AudioManager.instance.SwapTrack(LessonsScreenTrack);
    }

    public void rankButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToRankScreen();
        GameManager.instance.ChangeScene(7);
    }

    private void SwapTrackToRankScreen()
    {
        AudioManager.instance.SwapTrack(RankScreenTrack);
    }

    public void profileButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToProfileScreen();
        GameManager.instance.ChangeScene(8);
    }

    private void SwapTrackToProfileScreen()
    {
        AudioManager.instance.SwapTrack(ProfileScreenTrack);
    }

    public void shopButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToShopScreen();
        GameManager.instance.ChangeScene(9);
    }

    private void SwapTrackToShopScreen()
    {
        AudioManager.instance.SwapTrack(ShopScreenTrack);
    }

    public void PlaySoloBtnClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToStageSelection();
        GameManager.instance.ChangeScene(10);
    }

    private void SwapTrackToStageSelection()
    {
        AudioManager.instance.SwapTrack(StageSelectionTrack);
    }

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }
}