using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;
using TMPro;

public class NicknameScreen : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("Nickname Screen UI")]
    [SerializeField]
    private TMP_Text nicknameOutput;
    [SerializeField]
    private TMP_InputField nicknameInput;
    [Space(5f)]

    [Header("Audio")]
    [SerializeField]
    private AudioClip homeTrack;

    private NicknameScreen instance;
    private string photoURL;



    private void Awake()
    {
        instance = this.gameObject.GetComponent<NicknameScreen>();
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

    public void SetNickname()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://mathdiary-d169a.appspot.com/avatar/KumaThePlatoCommon.png");
        StartCoroutine(SetNickname(nicknameInput.text));
    }

    private IEnumerator SetNickname(string displayName)
    {
        string output = "Unknown Error, Please Try Again";

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
                DisplayName = displayName,

                //TODO: Give Profile Default Photo
                PhotoUrl = new System.Uri(photoURL)
            };

            if (displayName == null || displayName.Length == 0)
            {
                output = "Please enter a nickname.";
                nicknameOutput.text = output;
            }
            else if (displayName.Length > 12)
            {
                output = "Nickname is too long.";
                nicknameOutput.text = output;
            }
            else if (displayName.Length < 3)
            {
                output = "Nickname is too short.";
                nicknameOutput.text = output;
            }
            else
            {
                var UpdateEmailTask = DBreference.Child("users").Child(user.UserId).Child("email").SetValueAsync(user.Email);

                yield return new WaitUntil(predicate: () => UpdateEmailTask.IsCompleted);

                if (UpdateEmailTask.Exception != null)
                {
                    FirebaseException firebaseException = (FirebaseException)UpdateEmailTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;

                    switch (error)
                    {
                        case AuthError.Cancelled:
                            output = "Update User Cancelled.";
                            break;
                        case AuthError.SessionExpired:
                            output = "Session Expired.";
                            break;
                    }
                    nicknameOutput.text = output;
                }
                else
                {
                    var UpdateUsernameTask = DBreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(displayName);

                    yield return new WaitUntil(predicate: () => UpdateUsernameTask.IsCompleted);

                    if (UpdateUsernameTask.Exception != null)
                    {
                        output = "Username already taken.";
                        nicknameOutput.text = output;
                    }
                    else
                    {
                        var UpdateUsernameList = DBreference.Child("usernames").Child(displayName).SetValueAsync(user.UserId);

                        yield return new WaitUntil(predicate: () => UpdateUsernameList.IsCompleted);

                        if (UpdateUsernameList.Exception != null)
                        {
                            FirebaseException firebaseException = (FirebaseException)UpdateUsernameList.Exception.GetBaseException();
                            AuthError error = (AuthError)firebaseException.ErrorCode;

                            switch (error)
                            {
                                case AuthError.Cancelled:
                                    output = "Update User Cancelled.";
                                    break;
                                case AuthError.SessionExpired:
                                    output = "Session Expired.";
                                    break;
                            }
                            nicknameOutput.text = output;
                        }
                        else
                        {
                            var updateProfileTask = user.UpdateUserProfileAsync(profile);

                            yield return new WaitUntil(predicate: () => updateProfileTask.IsCompleted);

                            if (updateProfileTask.Exception != null)
                            {
                                FirebaseException firebaseException = (FirebaseException)updateProfileTask.Exception.GetBaseException();
                                AuthError error = (AuthError)firebaseException.ErrorCode;

                                switch (error)
                                {
                                    case AuthError.Cancelled:
                                        output = "Update User Cancelled.";
                                        break;
                                    case AuthError.SessionExpired:
                                        output = "Session Expired.";
                                        break;
                                }
                                nicknameOutput.text = output;
                            }
                            else
                            {
                                nicknameOutput.text = "Nickname Set!";

                                var InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("total_lp").SetValueAsync(0);
                                InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("level").SetValueAsync(1);
                                InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("xp").SetValueAsync(0);
                                InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("xp_to_next_level").SetValueAsync(100);
                                InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("used_avatar").SetValueAsync("KumaThePlatoCommon");
                                InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("total_gold").SetValueAsync(0);
                                InitializeUserData = DBreference.Child("users").Child(user.UserId).Child("total_silver").SetValueAsync(0);
                                InitializeUserData = DBreference.Child("avatars").Child(user.UserId).Push().SetValueAsync("KumaThePlatoCommon");

                                yield return new WaitUntil(predicate: () => InitializeUserData.IsCompleted);

                                if (InitializeUserData.Exception != null)
                                {
                                    FirebaseException firebaseException = (FirebaseException)InitializeUserData.Exception.GetBaseException();
                                    AuthError error = (AuthError)firebaseException.ErrorCode;

                                    switch (error)
                                    {
                                        case AuthError.Cancelled:
                                            output = "Update User Cancelled.";
                                            break;
                                        case AuthError.SessionExpired:
                                            output = "Session Expired.";
                                            break;
                                    }
                                    nicknameOutput.text = output;
                                }
                                else
                                {
                                    yield return new WaitForSeconds(1f);
                                    Debug.Log(user.PhotoUrl);
                                    SwapTrackToHome();
                                    GameManager.instance.ChangeScene(2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(homeTrack);
    }

    private IEnumerator UpdateProfilePictureLogic(string _profilPictureURL)
    {
        if (user != null)
        {
            UserProfile profile = new UserProfile();

            try
            {
                UserProfile _profile = new UserProfile
                {
                    PhotoUrl = new System.Uri(_profilPictureURL)
                };

                profile = _profile;
            }
            catch
            {
                yield break;
            }

            var updateProfilePictureTask = user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => updateProfilePictureTask.IsCompleted);

            if (updateProfilePictureTask.Exception != null)
            {
                Debug.LogError("Updating Profile Picture was Unsuccessful: " + updateProfilePictureTask.Exception.Message);
            }
            else
            {
                Debug.Log("Profile Picture Updated!");
            }
        }
    }
}
