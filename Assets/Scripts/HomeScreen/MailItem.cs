using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailItem : MonoBehaviour
{

    [Header("UI")]
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text descText;
    [SerializeField]
    private TMP_Text dateText;

    public void NewMailItem(string _title, string _desc, string _date)
    {
        titleText.text = _title;
        descText.text = _desc;
        dateText.text = _date;
    }
}
