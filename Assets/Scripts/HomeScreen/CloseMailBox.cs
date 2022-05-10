using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMailBox : MonoBehaviour
{
    public void CloseMailbox()
    {
        StartCoroutine(ClosingMailbox());
    }

    private IEnumerator ClosingMailbox()
    {
        this.transform.GetChild(0).GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }

    public void CloseMailItem()
    {
        StartCoroutine(ClosingMailItem());
    }

    private IEnumerator ClosingMailItem()
    {
        this.transform.GetChild(1).GetComponent<Animator>().SetBool("isOpen", false);
        this.transform.GetChild(0).GetComponent<Animator>().SetBool("isOpen", true);
        yield return new WaitForSeconds(0.4f);
        this.transform.GetChild(1).gameObject.SetActive(false);
    }
}
