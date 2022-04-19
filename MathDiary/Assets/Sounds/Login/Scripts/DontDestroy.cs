using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField]
    private GameObject BGM;

    private void Awake()
    {
        DontDestroyOnLoad(BGM);
    }
}
