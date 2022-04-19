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

public class UserRank : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("UI")]
    [SerializeField]
    private TMP_Text usernameText;
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private TMP_Text totalLPText;
    [SerializeField]
    private TMP_Text rankText;
    [SerializeField]
    private Image photoProfile;

    public void NewScoreboardElement(string _username, string _totalLP, string _level, string _used_avatar, string _rank)
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;

        usernameText.text = _username;
        levelText.text = "Level " + _level;
        totalLPText.text = _totalLP + " LP";
        rankText.text = "#" + _rank;
        StartCoroutine(LoadProfilePic(_used_avatar));
    }

    private IEnumerator LoadProfilePic(string _used_avatar)
    {
        storageRef = storage.GetReferenceFromUrl("gs://mathdiary-d169a.appspot.com/avatar/" + _used_avatar + ".jpg");
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

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoURI);
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
                photoProfile.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }

}
