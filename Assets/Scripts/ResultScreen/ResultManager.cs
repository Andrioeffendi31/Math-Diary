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

public class ResultManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text overallScoreText;
    [SerializeField]
    private TMP_Text totalCorrectText;
    [SerializeField]
    private TMP_Text totalWrongText;
    [SerializeField]
    private TMP_Text totalAccuracyText;
    [SerializeField]
    private Image fillOverallScore;
    [SerializeField]
    private TMP_Text radialValueText;
    [SerializeField]
    private Slider accuracySlider;
    [SerializeField]
    private List<Button> Buttons;
    [Space(5)]

    [Header("Audio")]
    [SerializeField]
    private AudioClip HomeScreenTrack;
    [SerializeField]
    private AudioClip BattleScreenTrack;

    private int overallScore;
    private int totalCorrect;
    private int totalWrong;
    private float totalAccuracy;
    private int totalHP;

    private float ButtonReactivateDelay = 1.4f;

    private void Start()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        storage = FirebaseStorage.DefaultInstance;
        accuracySlider.value = 0;
        fillOverallScore.fillAmount = 0;
        StartCoroutine(GetUserData());
    }

    public IEnumerator GetUserData()
    {
        var GetOverallScoreTask = DBreference.Child("matches").Child(user.UserId).Child(LevelSystem.RecentMatchKey).Child("overallScore").GetValueAsync();
        yield return new WaitUntil(() => GetOverallScoreTask.IsCompleted);

        if (GetOverallScoreTask.Exception != null)
        {
            Debug.LogError(GetOverallScoreTask.Exception.Message + "haha");
        }
        else
        {
            overallScore = Convert.ToInt32(GetOverallScoreTask.Result.Value);
            var GetTotalCorrectTask = DBreference.Child("matches").Child(user.UserId).Child(LevelSystem.RecentMatchKey).Child("totalCorrectAns").GetValueAsync();
            yield return new WaitUntil(() => GetTotalCorrectTask.IsCompleted);

            if (GetOverallScoreTask.Exception != null)
            {
                Debug.LogError(GetOverallScoreTask.Exception.Message);
            }
            else
            {
                totalCorrect = Convert.ToInt32(GetTotalCorrectTask.Result.Value);
                var GetTotalWrongTask = DBreference.Child("matches").Child(user.UserId).Child(LevelSystem.RecentMatchKey).Child("totalWrongAns").GetValueAsync();
                yield return new WaitUntil(() => GetTotalWrongTask.IsCompleted);

                if (GetOverallScoreTask.Exception != null)
                {
                    Debug.LogError(GetOverallScoreTask.Exception.Message);
                }
                else
                {
                    totalWrong = Convert.ToInt32(GetTotalWrongTask.Result.Value);
                    var GetTotalAccuracyTask = DBreference.Child("matches").Child(user.UserId).Child(LevelSystem.RecentMatchKey).Child("Accuracy").GetValueAsync();
                    yield return new WaitUntil(() => GetTotalAccuracyTask.IsCompleted);

                    if (GetOverallScoreTask.Exception != null)
                    {
                        Debug.LogError(GetOverallScoreTask.Exception.Message);
                    }
                    else
                    {
                        totalAccuracy = Convert.ToInt32(GetTotalAccuracyTask.Result.Value);
                        var GetTotalHPTask = DBreference.Child("matches").Child(user.UserId).Child(LevelSystem.RecentMatchKey).Child("total_HP").GetValueAsync();
                        yield return new WaitUntil(() => GetTotalHPTask.IsCompleted);

                        if (GetOverallScoreTask.Exception != null)
                        {
                            Debug.LogError(GetOverallScoreTask.Exception.Message);
                        }
                        else
                        {
                            var maxOverallScore = 0;
                            if (StageManager.selectedStage == "unit1_section1")
                            {
                                maxOverallScore = ((totalCorrect + totalWrong) * 5) + (100 * 2);
                            }
                            else if (StageManager.selectedStage == "unit1_section2")
                            {
                                maxOverallScore = ((totalCorrect + totalWrong) * 5) + (100 * 2);
                            }
                            else if (StageManager.selectedStage == "unit2_section1")
                            {
                                maxOverallScore = ((totalCorrect + totalWrong) * 5) + (100 * 2);
                            }
                            else if (StageManager.selectedStage == "unit2_section2")
                            {
                                maxOverallScore = ((totalCorrect + totalWrong) * 10) + (100 * 2);
                            }
                            else if (StageManager.selectedStage == "unit3_section1")
                            {
                                maxOverallScore = ((totalCorrect + totalWrong) * 5) + (100 * 2);
                            }
                            else if (StageManager.selectedStage == "unit3_section2")
                            {
                                maxOverallScore = ((totalCorrect + totalWrong) * 15) + (100 * 2);
                            }
                            totalHP = Convert.ToInt32(GetTotalHPTask.Result.Value);
                            overallScoreText.text = overallScore.ToString() + " LP";
                            totalCorrectText.text = totalCorrect.ToString();
                            totalWrongText.text = totalWrong.ToString();
                            StartCoroutine(LoadOverallScore(overallScore, maxOverallScore));
                            StartCoroutine(LoadTotalAccuracy(totalAccuracy, 100));
                        }

                    }
                }
            }
        }
    }

    private IEnumerator LoadOverallScore(float _overallScore, float _maxValue)
    {
        _maxValue = _overallScore / _maxValue;
        yield return new WaitForSeconds(1f);

        while (fillOverallScore.fillAmount < _maxValue)
        {
            fillOverallScore.fillAmount += 0.01f;
            radialValueText.text = ((int)(fillOverallScore.fillAmount * 100)).ToString() + "%";
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator LoadTotalAccuracy(float _totalAccuracy, float _maxValue)
    {
        accuracySlider.maxValue = 100;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < _totalAccuracy; i++)
        {
            accuracySlider.value++;
            totalAccuracyText.text = accuracySlider.value.ToString() + "%";
            yield return new WaitForSeconds(.001f);
        }
    }

    public void homeButtonClicked()
    {
        foreach (Button btn in Buttons)
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

    public void Unit1_Section1_Clicked()
    {
        foreach (Button btn in Buttons)
        {
            btn.interactable = false;
            StartCoroutine(EnableButtonAfterDelay(btn, ButtonReactivateDelay));
        }
        SwapTrackToBattle();
        if (StageManager.selectedStage == "unit1_section1" || StageManager.selectedStage == "unit1_section2")
        {
            GameManager.instance.ChangeScene(13);
        }
        else if (StageManager.selectedStage == "unit2_section1")
        {
            GameManager.instance.ChangeScene(17);
        }
        else if (StageManager.selectedStage == "unit2_section2")
        {
            GameManager.instance.ChangeScene(18);
        }
        else if (StageManager.selectedStage == "unit3_section1")
        {
            GameManager.instance.ChangeScene(19);
        }
        else if (StageManager.selectedStage == "unit3_section2")
        {
            GameManager.instance.ChangeScene(20);
        }
    }

    private void SwapTrackToBattle()
    {
        AudioManager.instance.SwapTrack(BattleScreenTrack);
    }

    IEnumerator EnableButtonAfterDelay(Button button, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }
}
