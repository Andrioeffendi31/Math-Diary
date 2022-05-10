using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;
using TMPro;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(() => OnClick(param));
    }
}
public class MailboxManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBreference;
    public FirebaseUser user;
    public FirebaseStorage storage;
    public StorageReference storageRef;
    [Space(5)]

    [Header("UI Elements")]
    [SerializeField]
    private GameObject mailCount;
    [SerializeField]
    private Transform canvas;
    [SerializeField]
    private List<Sprite> btnSprite;
    [Space(5)]

    [Header("Prefabs")]
    [SerializeField]
    private GameObject mailBoxPrefab;
    [SerializeField]
    private GameObject mailItem;
    private GameObject mailBox;

    public static MailboxManager instance;
    private List<Tuple<string, string, string, string, int, int>> mailList = new List<Tuple<string, string, string, string, int, int>>();

    private void Awake()
    {
        instance = this.gameObject.GetComponent<MailboxManager>();
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
        StartCoroutine(GetMail());
    }

    public IEnumerator GetMail()
    {
        mailList.Clear();
        int readMail = 0;

        var GetMailTask = DBreference.Child("mails").Child(user.UserId).OrderByChild("Date").GetValueAsync();
        yield return new WaitUntil(() => GetMailTask.IsCompleted);
        if (GetMailTask.Exception != null)
        {
            Debug.LogError(GetMailTask.Exception);
        }
        else
        {
            DataSnapshot snapshot = GetMailTask.Result;

            foreach (var childSnapshot in snapshot.Children)
            {
                var title = childSnapshot.Child("Title").Value.ToString();
                var desc = childSnapshot.Child("Desc").Value.ToString();
                var date = childSnapshot.Child("Date").Value.ToString();
                var reward = int.Parse(childSnapshot.Child("Reward").Value.ToString());
                var readStatus = int.Parse(childSnapshot.Child("Read").Value.ToString());
                Debug.Log("Title: " + title);
                Debug.Log("Desc: " + desc);
                Debug.Log("Date: " + date);
                Debug.Log("Reward: " + reward);
                Debug.Log("Read status: " + readStatus);

                if (readStatus == 0)
                {
                    readMail++;
                }
                mailList.Add(new Tuple<string, string, string, string, int, int>(title, desc, date, childSnapshot.Key, reward, readStatus));
            }
            mailList.Reverse();
            if (readMail > 0)
            {
                mailCount.SetActive(true);
                mailCount.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = readMail.ToString();
            }
            else
            {
                mailCount.SetActive(false);
            }
        }
    }

    public void OpenMailbox()
    {
        mailBox = Instantiate(mailBoxPrefab, canvas);

        for (int i = 0; i < mailList.Count; i++)
        {
            var shortDesc = mailList[i].Item2.Length <= 80 ? mailList[i].Item2 : mailList[i].Item2.Substring(0, 80) + "...";
            GameObject newScoreElement = Instantiate(mailItem, mailBox.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
            newScoreElement.GetComponent<MailItem>().NewMailItem(mailList[i].Item1, shortDesc, mailList[i].Item3);

            newScoreElement.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
    }

    void ItemClicked(int index)
    {
        mailBox.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        mailBox.transform.GetChild(0).GetComponent<Animator>().SetBool("isOpen", false);
        mailBox.transform.GetChild(1).gameObject.SetActive(true);

        mailBox.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = mailList[index].Item1;
        mailBox.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = mailList[index].Item2;

        if (mailList[index].Item6 == 0)
        {
            if (mailList[index].Item5 > 0)
            {
                mailBox.transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + mailList[index].Item5.ToString();
                mailBox.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                mailBox.transform.GetChild(1).GetChild(3).GetComponent<Image>().sprite = btnSprite[0];
                mailBox.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = "Claim";
                mailBox.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(() => StartCoroutine(ClaimReward(index)));
            }
            else
            {
                mailBox.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
                mailBox.transform.GetChild(1).GetChild(3).GetComponent<Image>().sprite = btnSprite[1];
                mailBox.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = "Close";
                mailBox.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(() => StartCoroutine(ReadMail(index)));
            }
        }
        else
        {
            mailBox.transform.GetChild(1).GetChild(3).GetComponent<Image>().sprite = btnSprite[1];
            mailBox.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = "Close";
            if (mailList[index].Item5 > 0)
            {
                mailBox.transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + mailList[index].Item5.ToString();
                mailBox.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                mailBox.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ReadMail(int _index)
    {
        var readMailTask = DBreference.Child("mails").Child(user.UserId).Child(mailList[_index].Item4).Child("Read").SetValueAsync(1);
        yield return new WaitUntil(() => readMailTask.IsCompleted);
        if (readMailTask.Exception != null)
        {
            Debug.LogError(readMailTask.Exception);
        }
        else
        {
            StartCoroutine(GetMail());
        }
    }

    private IEnumerator ClaimReward(int _index)
    {
        var GetTotalGoldTask = DBreference.Child("users").Child(user.UserId).Child("total_gold").GetValueAsync();
        yield return new WaitUntil(() => GetTotalGoldTask.IsCompleted);
        if (GetTotalGoldTask.Exception != null)
        {
            Debug.LogError(GetTotalGoldTask.Exception);
        }
        else
        {
            var claimRewardTask = DBreference.Child("mails").Child(user.UserId).Child(mailList[_index].Item4).Child("Read").SetValueAsync(1);
            claimRewardTask = DBreference.Child("users").Child(user.UserId).Child("total_gold").SetValueAsync(int.Parse(GetTotalGoldTask.Result.Value.ToString()) + mailList[_index].Item5);
            yield return new WaitUntil(() => claimRewardTask.IsCompleted);

            if (claimRewardTask.Exception != null)
            {
                Debug.LogError(claimRewardTask.Exception);
            }
            else
            {
                StartCoroutine(GetMail());
                StartCoroutine(HomeManager.instance.GetUserData());
            }
        }
    }
}
