using System;
using System.Linq;
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

public class RankManager : MonoBehaviour
{
    private RankManager instance;

    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("UI")]
    [SerializeField]
    private TMP_Text currentUserRankText;
    [SerializeField]
    private TMP_Text currentUsernameText;
    [SerializeField]
    private TMP_Text currentUserLPText;
    [SerializeField]
    private TMP_Text currentUserLevelText;
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private List<Button> navbarButtons;
    [Space(5)]

    [Header("Audio")]
    [SerializeField]
    private AudioClip HomeScreenTrack;
    [SerializeField]
    private AudioClip LessonsScreenTrack;
    [SerializeField]
    private AudioClip ProfileScreenTrack;
    [SerializeField]
    private AudioClip ShopScreenTrack;

    [Header("Rank Objects")]
    [SerializeField]
    private GameObject scoreElementFirst;
    [SerializeField]
    private GameObject scoreElementSec;
    [SerializeField]
    private GameObject scoreElementThird;
    [SerializeField]
    private GameObject scoreElementCommon;
    [SerializeField]
    private Transform scoreboardContent;

    private int XPToNextLV;
    private int userRank;
    private string currentLV;
    private string currentUsername;
    private string currentTotalLP;
    private float ButtonReactivateDelay = 1.4f;

    private void Awake()
    {
        instance = this.gameObject.GetComponent<RankManager>();
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
        StartCoroutine(GetUserRank());
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

            List<Tuple<string, string, string, string, string>> rankList = new List<Tuple<string, string, string, string, string>>();
            foreach (var childSnapshot in snapshot.Children)
            {
                var username = childSnapshot.Child("username").Value.ToString();
                var total_lp = childSnapshot.Child("total_lp").Value.ToString();
                var level = childSnapshot.Child("level").Value.ToString();
                var used_avatar = childSnapshot.Child("used_avatar").Value.ToString();

                rankList.Add(new Tuple<string, string, string, string, string>(username, total_lp, level, used_avatar, childSnapshot.Key));
            }
            rankList.Reverse();
            foreach (var item in rankList)
            {
                Debug.Log("username : " + item.Item1);
                Debug.Log("LP : " + item.Item2);
                Debug.Log("level : " + item.Item3);
                Debug.Log("used_avatar : " + item.Item4);
                Debug.Log("userID : " + item.Item5);
            }
            userRank = rankList.FindIndex(x => x.Item5 == user.UserId) + 1;
            currentUsername = rankList[userRank - 1].Item1;
            currentLV = rankList[userRank - 1].Item3;
            currentTotalLP = rankList[userRank - 1].Item2;
            currentUsernameText.text = currentUsername;
            currentUserLevelText.text = "Level " + currentLV;
            currentUserRankText.text = "#" + userRank;
            currentUserLPText.text = currentTotalLP + " LP";

            storageRef = storage.GetReferenceFromUrl("gs://mathdiary-d169a.appspot.com/avatar/" + rankList[userRank - 1].Item4 + ".jpg");
            var GetPhotoURLTask = storageRef.GetDownloadUrlAsync();

            yield return new WaitUntil(() => GetPhotoURLTask.IsCompleted);

            if (GetPhotoURLTask.Exception != null)
            {
                Debug.Log("Error: " + GetPhotoURLTask.Exception);
            }
            else
            {
                Debug.Log("Download URL: " + GetPhotoURLTask.Result);
                System.Uri photoURI = GetPhotoURLTask.Result;

                StartCoroutine(LoadProfilePic(photoURI.ToString()));

                var topFifty = rankList.Take(50).ToList();

                for (int i = 0; i < topFifty.Count(); i++)
                {
                    if (i == 0)
                    {
                        GameObject newScoreElement = Instantiate(scoreElementFirst, scoreboardContent);
                        newScoreElement.GetComponent<UserRank>().NewScoreboardElement(topFifty[i].Item1, topFifty[i].Item2, topFifty[i].Item3, topFifty[i].Item4, (rankList.FindIndex(x => x.Item5 == topFifty[i].Item5) + 1).ToString());
                    }
                    else if (i == 1)
                    {
                        GameObject newScoreElement = Instantiate(scoreElementSec, scoreboardContent);
                        newScoreElement.GetComponent<UserRank>().NewScoreboardElement(topFifty[i].Item1, topFifty[i].Item2, topFifty[i].Item3, topFifty[i].Item4, (rankList.FindIndex(x => x.Item5 == topFifty[i].Item5) + 1).ToString());
                    }
                    else if (i == 2)
                    {
                        GameObject newScoreElement = Instantiate(scoreElementThird, scoreboardContent);
                        newScoreElement.GetComponent<UserRank>().NewScoreboardElement(topFifty[i].Item1, topFifty[i].Item2, topFifty[i].Item3, topFifty[i].Item4, (rankList.FindIndex(x => x.Item5 == topFifty[i].Item5) + 1).ToString());
                    }
                    else
                    {
                        GameObject newScoreElement = Instantiate(scoreElementCommon, scoreboardContent);
                        newScoreElement.GetComponent<UserRank>().NewScoreboardElement(topFifty[i].Item1, topFifty[i].Item2, topFifty[i].Item3, topFifty[i].Item4, (rankList.FindIndex(x => x.Item5 == topFifty[i].Item5) + 1).ToString());
                    }
                }

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

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }
}
