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

public class StageManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text LPAmount;
    [SerializeField]
    private TMP_Text GoldAmount;
    [SerializeField]
    private TMP_Text SilverAmount;
    [SerializeField]
    private Image Unit2;
    [SerializeField]
    private Image Unit3;
    [SerializeField]
    private TMP_Text Unit2Desc;
    [SerializeField]
    private TMP_Text Unit3Desc;
    [SerializeField]
    private Button Unit2Section1;
    [SerializeField]
    private Button Unit2Section2;
    [SerializeField]
    private Button Unit3Section1;
    [SerializeField]
    private Button Unit3Section2;
    [SerializeField]
    private TMP_Text Unit2Section1Desc;
    [SerializeField]
    private TMP_Text Unit2Section2Desc;
    [SerializeField]
    private TMP_Text Unit3Section1Desc;
    [SerializeField]
    private TMP_Text Unit3Section2Desc;
    [Space(5)]

    [Header("Audio")]
    [SerializeField]
    private AudioClip HomeScreenTrack;
    [SerializeField]
    private AudioClip SleepScreenTrack;

    private StageManager instance;
    private string totalLP;
    private string totalGold;
    private string totalSilver;
    public static string selectedStage;


    private void Awake()
    {
        instance = this.gameObject.GetComponent<StageManager>();
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
        selectedStage = "";
        StartCoroutine(GetUserData());
        StartCoroutine(GetUserStageData());
    }

    public IEnumerator GetUserData()
    {
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

                    LPAmount.text = totalLP + " LP";
                    GoldAmount.text = totalGold;
                    SilverAmount.text = totalSilver;
                }
            }
        }
    }

    public IEnumerator GetUserStageData()
    {
        var GetStageDataTask = DBreference.Child("stages").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(() => GetStageDataTask.IsCompleted);

        if (GetStageDataTask.Exception != null)
        {
            Debug.LogError(GetStageDataTask.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = GetStageDataTask.Result;
            List<string> completedStageList = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                var stageName = childSnapshot.Value.ToString();
                completedStageList.Add(stageName);
                Debug.Log("Achievement Name: " + stageName);
            }
            completedStageList.Reverse();

            if (completedStageList.Count > 0)
            {
                if (completedStageList.Contains("unit1_section1") && completedStageList.Contains("unit1_section2"))
                {
                    Unit2.color = new Color(255, 255, 255, 255);
                    Unit2Desc.color = new Color(0, 0, 0, 255);
                    Unit2Section1.interactable = true;
                    Unit2Section2.interactable = true;
                    Unit2Section1Desc.color = new Color(0, 0, 0, 255);
                    Unit2Section2Desc.color = new Color(0, 0, 0, 255);
                }
                if (completedStageList.Contains("unit2_section1") && completedStageList.Contains("unit2_section2"))
                {
                    Unit3.color = new Color(255, 255, 255, 255);
                    Unit3Desc.color = new Color(0, 0, 0, 255);
                    Unit3Section1.interactable = true;
                    Unit3Section2.interactable = true;
                    Unit3Section1Desc.color = new Color(0, 0, 0, 255);
                    Unit3Section2Desc.color = new Color(0, 0, 0, 255);
                }
            }
            else
            {
                Debug.Log("No Completed Stage");
            }
        }
    }

    public void closeButtonClicked()
    {
        SwapTrackToHome();
        GameManager.instance.ChangeScene(2);
    }

    private void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(HomeScreenTrack);
    }

    public void Unit1_Section1_Clicked()
    {
        selectedStage = "unit1_section1";
        SwapTrackToSleep();
        GameManager.instance.ChangeScene(11);
    }

    public void Unit1_Section2_Clicked()
    {
        selectedStage = "unit1_section2";
        SwapTrackToSleep();
        GameManager.instance.ChangeScene(11);
    }

    private void SwapTrackToSleep()
    {
        AudioManager.instance.SwapTrack(SleepScreenTrack);
    }
}
