using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginUITrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject popUpVersion;

    [SerializeField]
    private GameObject forgotPassPopup;
    [SerializeField]
    private GameObject closeForgotPassBtn;
    [SerializeField]
    private GameObject forgotPassPopupSec;
    [SerializeField]
    private GameObject closeForgotPassBtnSec;

    [SerializeField]
    private GameObject verifyPopup;

    [SerializeField]
    private GameObject loginForm;

    [SerializeField]
    private GameObject registerForm;

    [SerializeField]
    private GameObject registerSec;

    [SerializeField]
    private GameObject loginSec;

    [SerializeField]
    private GameObject gameLogo;

    [SerializeField]
    private TMP_Text verifyEmailText;

    public void OpenPopUpVersion()
    {
        popUpVersion.SetActive(true);
        popUpVersion.GetComponent<Animator>().SetBool("isOpen", true);
    }
    public void ClosePopupVersion()
    {
        StartCoroutine(ClosingPopupVersion());
    }

    private IEnumerator ClosingPopupVersion()
    {
        popUpVersion.GetComponent<Animator>().SetBool("isOpen", false);
        yield return new WaitForSeconds(0.4f);
        popUpVersion.SetActive(false);
    }

    public void OpenForgotPassPopup()
    {
        forgotPassPopup.SetActive(true);
        closeForgotPassBtn.SetActive(true);
        forgotPassPopup.GetComponent<Animator>().SetBool("isOpen", true);
        CloseLoginForm();
    }

    public void CloseForgotPassPopup()
    {
        closeForgotPassBtn.SetActive(false);
        forgotPassPopup.GetComponent<Animator>().SetBool("isOpen", false);
    }

    public void OpenForgotPassPopupSec()
    {
        forgotPassPopupSec.SetActive(true);
        closeForgotPassBtnSec.SetActive(true);
        forgotPassPopupSec.GetComponent<Animator>().SetBool("isOpen", true);
        CloseRegisterForm();
    }

    public void CloseForgotPassPopupSec()
    {
        closeForgotPassBtnSec.SetActive(false);
        forgotPassPopupSec.GetComponent<Animator>().SetBool("isOpen", false);
    }

    public void CloseVerifyPopup()
    {
        verifyPopup.GetComponent<Animator>().SetBool("isOpen", false);
    }

    public void OpenLoginForm()
    {
        loginForm.SetActive(true);
        loginForm.GetComponent<Animator>().SetBool("isOpen", true);
        gameLogo.GetComponent<Animator>().SetBool("isGoingUp", true);
    }
    public void CloseLoginForm()
    {
        registerSec.GetComponent<Animator>().SetBool("isOpen", false);
        loginForm.GetComponent<Animator>().SetBool("isOpen", false);
        loginForm.GetComponent<Animator>().SetBool("isRegister", false);
        gameLogo.GetComponent<Animator>().SetBool("isGoingUp", false);
    }

    public void OpenRegisterForm()
    {
        registerForm.SetActive(true);
        registerForm.GetComponent<Animator>().SetBool("isOpen", true);
        gameLogo.GetComponent<Animator>().SetBool("isGoingUp", true);
    }
    public void CloseRegisterForm()
    {
        loginSec.GetComponent<Animator>().SetBool("isOpen", false);
        registerForm.GetComponent<Animator>().SetBool("isOpen", false);
        registerForm.GetComponent<Animator>().SetBool("isLogin", false);
        gameLogo.GetComponent<Animator>().SetBool("isGoingUp", false);
    }

    public void OpenRegisterSec()
    {
        registerSec.SetActive(true);
        loginForm.GetComponent<Animator>().SetBool("isRegister", true);
        registerSec.GetComponent<Animator>().SetBool("isOpen", true);
    }
    public void CloseRegisterSec()
    {
        registerSec.GetComponent<Animator>().SetBool("isOpen", false);
        loginForm.GetComponent<Animator>().SetBool("isRegister", false);
    }

    public void OpenLoginSec()
    {
        loginSec.SetActive(true);
        registerForm.GetComponent<Animator>().SetBool("isLogin", true);
        loginSec.GetComponent<Animator>().SetBool("isOpen", true);
    }
    public void CloseLoginSec()
    {
        loginSec.GetComponent<Animator>().SetBool("isOpen", false);
        registerForm.GetComponent<Animator>().SetBool("isLogin", false);
    }

    public void AwaitVerification(bool _emailSent, string _email, string _output)
    {
        if (_emailSent)
        {
            verifyPopup.SetActive(true);
            verifyPopup.GetComponent<Animator>().SetBool("isOpen", true);
            verifyEmailText.text = "Verification email sent to\n" + _email;
        }
        else
        {
            verifyPopup.SetActive(true);
            verifyPopup.GetComponent<Animator>().SetBool("isOpen", true);
            verifyEmailText.text = "Email Not Sent: " + _output + " \nPlease try again later";
        }
    }

    public void EventDeActive()
    {
        this.gameObject.SetActive(false);
    }
    public void EventActive()
    {
        this.gameObject.SetActive(true);
    }
}
