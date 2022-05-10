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
using Google;

public class ProfileManager : MonoBehaviour
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
    private GameObject achievementPanel;
    [SerializeField]
    private GameObject usedAvatar;
    [SerializeField]
    private GameObject Background;
    [SerializeField]
    private Button KumaThePlatoCommonBtn;
    [SerializeField]
    private Button KumaThePlatoEliteBtn;
    [SerializeField]
    private Button KumaTheWarriorCommonBtn;
    [SerializeField]
    private Button KumaTheWarriorEliteBtn;
    [SerializeField]
    private GameObject avatarCollection;
    [SerializeField]
    private TMP_Text highestSocreText;
    [SerializeField]
    private TMP_Text averageScoreText;
    [SerializeField]
    private TMP_Text averageAccuracyText;
    [SerializeField]
    private List<Button> navbarButtons;
    [Space(5)]

    [Header("Achievement Icon")]
    [SerializeField]
    private Image achievementLV10;
    [SerializeField]
    private Image achievementLV20;
    [SerializeField]
    private Image achievementLV40;
    [SerializeField]
    private Image achievementLV60;
    [SerializeField]
    private Image achievementLV80;
    [SerializeField]
    private Image achievementLV100;
    [SerializeField]
    private Image achievementLP100;
    [SerializeField]
    private Image achievementLP500;
    [SerializeField]
    private Image achievementLP1000;
    [SerializeField]
    private Image achievementLP1500;
    [SerializeField]
    private Image achievementLP2000;
    [SerializeField]
    private Image achievementLP3000;
    [SerializeField]
    private Image achievementTop50;
    [SerializeField]
    private Image achievementTop10;
    [SerializeField]
    private Image achievementTop3;
    [SerializeField]
    private Image achievementAvatar2;
    [SerializeField]
    private Image achievementAvatar3;
    [SerializeField]
    private Image achievementAvatar4;
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
    private AudioClip ShopScreenTrack;

    private ProfileManager instance;
    private string photoURL;
    private string username;
    private string totalLP;
    private string totalGold;
    private string totalSilver;
    private string currentLV;
    private int currentXPValue;
    private int XPToNextLV;
    private int userRank;
    private string usedAvatarName;


    private int highestScore;
    private float averageScore;
    private float averageAccuracy;

    private float ButtonReactivateDelay = 1.4f;

    private void Awake()
    {
        instance = this.gameObject.GetComponent<ProfileManager>();
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
                totalLP = GetTotalLPTask.Result.Value.ToString();
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
                                    XPToNextLV = Int32.Parse(GetXPToNextLvTask.Result.Value.ToString());
                                    Debug.Log("XP To Next Level: " + XPToNextLV);

                                    System.Uri photoURI = user.PhotoUrl;
                                    StartCoroutine(LoadProfilePic(photoURI.ToString()));
                                    StartCoroutine(LoadExpBar(currentXPValue, XPToNextLV));
                                    StartCoroutine(GetUserRank());
                                    StartCoroutine(GetUsedAvatar());
                                    StartCoroutine(GetMatchesInfo());

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

    private IEnumerator GetMatchesInfo()
    {
        var GetMatchesTask = DBreference.Child("matches").Child(user.UserId).OrderByChild("overallScore").GetValueAsync();

        yield return new WaitUntil(() => GetMatchesTask.IsCompleted);

        if (GetMatchesTask.Exception != null)
        {
            Debug.LogError(GetMatchesTask.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = GetMatchesTask.Result;

            List<Tuple<int, int>> matcheslist = new List<Tuple<int, int>>();
            foreach (var childSnapshot in snapshot.Children)
            {
                var overallScore = int.Parse(childSnapshot.Child("overallScore").Value.ToString());
                var accuracy = int.Parse(childSnapshot.Child("Accuracy").Value.ToString());

                matcheslist.Add(new Tuple<int, int>(overallScore, accuracy));
            }
            matcheslist.Reverse();

            if (matcheslist.Count > 0)
            {
                foreach (var item in matcheslist)
                {
                    Debug.Log("overallScore : " + item.Item1);
                    Debug.Log("accuracy : " + item.Item2);
                }
                highestScore = matcheslist[0].Item1;

                foreach (var item in matcheslist)
                {
                    averageScore += item.Item1;
                }
                averageScore /= matcheslist.Count;

                foreach (var item in matcheslist)
                {
                    averageAccuracy += item.Item2;
                }
                averageAccuracy /= matcheslist.Count;

                highestSocreText.text = highestScore.ToString();
                averageScoreText.text = Math.Round(averageScore, 1).ToString();
                averageAccuracyText.text = Math.Round(averageAccuracy, 1).ToString() + "%";
            }
            else
            {
                highestSocreText.text = "0";
                averageScoreText.text = "0.0";
                averageAccuracyText.text = "0%";
            }
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

    public void OpenAchievementList()
    {
        StartCoroutine(LoadAchievement());
    }

    private IEnumerator LoadAchievement()
    {
        var GetAchievementTask = DBreference.Child("achievements").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => GetAchievementTask.IsCompleted);

        if (GetAchievementTask.Exception != null)
        {
            Debug.LogError(GetAchievementTask.Exception);
        }
        else
        {
            DataSnapshot snapshot = GetAchievementTask.Result;
            List<string> achievementList = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                var achievementName = childSnapshot.Value.ToString();
                achievementList.Add(achievementName);
                Debug.Log("Achievement Name: " + achievementName);
            }
            achievementList.Reverse();

            if (achievementList.Count != 0)
            {
                if (achievementList.Contains("Lv10"))
                {
                    achievementLV10.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Lv20"))
                {
                    achievementLV20.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Lv40"))
                {
                    achievementLV40.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Lv60"))
                {
                    achievementLV60.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Lv80"))
                {
                    achievementLV80.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Lv100"))
                {
                    achievementLV100.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("LP1000"))
                {
                    achievementLP100.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("LP5000"))
                {
                    achievementLP500.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("LP10000"))
                {
                    achievementLP1000.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("LP20000"))
                {
                    achievementLP1500.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("LP40000"))
                {
                    achievementLP2000.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("LP100000"))
                {
                    achievementLP3000.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Top50"))
                {
                    achievementTop50.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Top10"))
                {
                    achievementTop10.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("Top3"))
                {
                    achievementTop3.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("avatar2"))
                {
                    achievementAvatar2.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("avatar3"))
                {
                    achievementAvatar3.color = new Color(255, 255, 255, 255);
                }
                if (achievementList.Contains("avatar4"))
                {
                    achievementAvatar4.color = new Color(255, 255, 255, 255);
                }
            }

            achievementPanel.SetActive(true);
            achievementPanel.GetComponent<Animator>().SetBool("isOpen", true);
            yield return new WaitForSeconds(1f);
            Background.SetActive(false);
            usedAvatar.SetActive(false);
        }
    }

    public void CloseAchievementList()
    {
        StartCoroutine(CloseAchievement());
    }

    private IEnumerator CloseAchievement()
    {
        Background.SetActive(true);
        usedAvatar.SetActive(true);
        yield return new WaitForSeconds(1f);
        achievementPanel.GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(1f);
        achievementPanel.SetActive(false);
    }

    public void OpenAvatarList()
    {
        StartCoroutine(LoadAvatarCollection());
    }

    private IEnumerator LoadAvatarCollection()
    {
        var GetAvatarTask = DBreference.Child("avatars").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => GetAvatarTask.IsCompleted);

        if (GetAvatarTask.Exception != null)
        {
            Debug.LogError(GetAvatarTask.Exception);
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
                if (avatarList.Contains("KumaThePlatoCommon"))
                {
                    KumaThePlatoCommonBtn.interactable = true;
                }
                if (avatarList.Contains("KumaThePlatoElite"))
                {
                    KumaThePlatoEliteBtn.interactable = true;
                }
                if (avatarList.Contains("KumaTheWarriorCommon"))
                {
                    KumaTheWarriorCommonBtn.interactable = true;
                }
                if (avatarList.Contains("KumaTheWarriorElite"))
                {
                    KumaTheWarriorEliteBtn.interactable = true;
                }
            }
            avatarCollection.SetActive(true);
            avatarCollection.GetComponent<Animator>().SetBool("isOpen", true);
        }
    }

    public void CloseAvatarList()
    {
        StartCoroutine(CloseAvatarCollection());
    }

    private IEnumerator CloseAvatarCollection()
    {
        avatarCollection.GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(0.6f);
        avatarCollection.SetActive(false);
    }

    public void ChangeUsedAvatar(string avatarName)
    {
        StartCoroutine(ChangeAvatar(avatarName));
    }

    private IEnumerator ChangeAvatar(string avatarName)
    {
        var UpdateUsedAvatarTask = DBreference.Child("users").Child(user.UserId).Child("used_avatar").SetValueAsync(avatarName);
        yield return new WaitUntil(() => UpdateUsedAvatarTask.IsCompleted);

        if (UpdateUsedAvatarTask.Exception != null)
        {
            Debug.LogError(UpdateUsedAvatarTask.Exception);
        }
        else
        {
            storageRef = storage.GetReferenceFromUrl("gs://mathdiary-d169a.appspot.com/avatar/" + avatarName + ".jpg");
            var UpdatePhotoTask = storageRef.GetDownloadUrlAsync();
            yield return new WaitUntil(() => UpdatePhotoTask.IsCompleted);

            if (UpdatePhotoTask.Exception != null)
            {
                Debug.Log("Error: " + UpdatePhotoTask.Exception);
            }
            else
            {
                Debug.Log("Download URL: " + UpdatePhotoTask.Result);
                photoURL = UpdatePhotoTask.Result.ToString();

                UserProfile profile = new UserProfile
                {
                    PhotoUrl = new System.Uri(photoURL)
                };

                var updateProfileTask = user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    Debug.LogError(updateProfileTask.Exception);
                }
                else
                {
                    Debug.Log("Successfully updated user profile");
                    GameManager.instance.ChangeScene(8);
                }
            }
        }
    }

    public void homeButtonClicked()
    {
        foreach (Button btn in navbarButtons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToHome();
        GameManager.instance.ChangeScene(2);
    }

    private void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(HomeScreenTrack);
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

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        GoogleSignIn.DefaultInstance.SignOut();
        AudioManager.instance.SwapTrack(LoginScreenTrack);
        GameManager.instance.ChangeScene(0);
    }
}