using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Google;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseManager instance;

    [Header("Login UI Script")]
    [SerializeField]
    private LoginUITrigger loginUITrigger;
    [Space(5f)]

    [Header("Firebase")]
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBreference;
    [Space(5f)]

    [Header("Google Sign In")]
    [SerializeField]
    private string webClientId = "<your client id here>";
    private GoogleSignInConfiguration configuration;

    [Header("Login References")]
    [SerializeField]
    private TMP_InputField loginEmail;
    [SerializeField]
    private TMP_InputField loginPassword;
    [SerializeField]
    private TMP_Text loginOutputText;
    [Space(5f)]

    [Header("Login Sec References")]
    [SerializeField]
    private TMP_InputField loginEmailSec;
    [SerializeField]
    private TMP_InputField loginPasswordSec;
    [SerializeField]
    private TMP_Text loginOutputTextSec;
    [Space(5f)]

    [Header("Register References")]
    [SerializeField]
    private TMP_InputField registerEmail;
    [SerializeField]
    private TMP_InputField registerPassword;
    [SerializeField]
    private TMP_InputField registerConfirmPassword;
    [SerializeField]
    private TMP_Text registerOutputText;
    [Space(5f)]

    [Header("RegisterSec References")]
    [SerializeField]
    private TMP_InputField registerEmailSec;
    [SerializeField]
    private TMP_InputField registerPasswordSec;
    [SerializeField]
    private TMP_InputField registerConfirmPasswordSec;
    [SerializeField]
    private TMP_Text registerOutputTextSec;
    [Space(5f)]

    [Header("Forgot Pass References")]
    [SerializeField]
    private TMP_InputField resetEmail;
    [SerializeField]
    private TMP_Text forgotPassOutputText;
    [SerializeField]
    private TMP_InputField resetEmailSec;
    [SerializeField]
    private TMP_Text forgotPassOutputTextSec;

    [Header("Audio")]
    [SerializeField]
    private AudioClip homeTrack;
    [Space(5f)]

    [Header("Login Button")]
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button loginButtonSec;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        instance = this.gameObject.GetComponent<FirebaseManager>();
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

    public void Login()
    {
        StartCoroutine(CheckAndFixDepenencies());
    }

    public void LoginSec()
    {
        StartCoroutine(CheckAndFixDepenenciesSec());
    }

    private IEnumerator CheckAndFixDepenencies()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);

        var dependancyResult = checkAndFixDependanciesTask.Result;

        if (dependancyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependancyResult}");
        }
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;

        StartCoroutine(CheckAutoLogin());

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();
        if (user != null)
        {
            var reloadUserTask = user.ReloadAsync();

            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);

            AutoLogin();
        }
        else
        {
            loginUITrigger.OpenLoginForm();
        }
    }

    private IEnumerator CheckAndFixDepenenciesSec()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);

        var dependancyResult = checkAndFixDependanciesTask.Result;

        if (dependancyResult == DependencyStatus.Available)
        {
            InitializeFirebaseSec();
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependancyResult}");
        }
    }

    private void InitializeFirebaseSec()
    {
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.GetInstance("https://mathdiary-d169a-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;

        StartCoroutine(CheckAutoLoginSec());

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckAutoLoginSec()
    {
        yield return new WaitForEndOfFrame();
        if (user != null)
        {
            var reloadUserTask = user.ReloadAsync();

            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);

            loginUITrigger.CloseRegisterForm();
            AutoLogin();
        }
        else
        {
            loginUITrigger.OpenLoginSec();
        }
    }

    private void AutoLogin()
    {
        if (user != null)
        {
            if (user.IsEmailVerified)
            {
                if (user.DisplayName == null || user.DisplayName.Length == 0)
                {
                    GameManager.instance.ChangeScene(1);
                }
                else
                {
                    Debug.Log(user.UserId);
                    loginButton.interactable = false;
                    loginButtonSec.interactable = false;
                    SwapTrackToHome();
                    GameManager.instance.ChangeScene(2);
                }
            }
            else
            {
                StartCoroutine(SendVerificationEmail());
            }
        }
        else
        {
            loginUITrigger.OpenLoginForm();
        }
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed Out");
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Signed In: {user.DisplayName}");
            }
        }
    }

    public void LoginButton()
    {
        StartCoroutine(LoginLogic(loginEmail.text, loginPassword.text));
    }

    public void LoginButtonSec()
    {
        StartCoroutine(LoginLogicSec(loginEmailSec.text, loginPasswordSec.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(RegisterLogic(registerEmail.text, registerPassword.text, registerConfirmPassword.text));
    }

    public void RegisterButtonSec()
    {
        StartCoroutine(RegisterLogicSec(registerEmailSec.text, registerPasswordSec.text, registerConfirmPasswordSec.text));
    }

    public void ForgotPassButton()
    {
        StartCoroutine(ForgotPasswordLogic(resetEmail.text, forgotPassOutputText));
    }

    public void ForgotPassButtonSec()
    {
        StartCoroutine(ForgotPasswordLogic(resetEmailSec.text, forgotPassOutputTextSec));
    }

    private IEnumerator LoginLogic(string _email, string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

        var loginTask = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown Error, Please Try Again";

            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "Email is required.";
                    break;
                case AuthError.MissingPassword:
                    output = "Password is required.";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email.";
                    break;
                case AuthError.WrongPassword:
                    output = "Incorrect Password.";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does Not Exist.";
                    break;
            }
            loginOutputText.text = output;
        }
        else
        {
            if (user.IsEmailVerified)
            {
                yield return new WaitForSeconds(1f);
                if (user.DisplayName == null || user.DisplayName.Length == 0)
                {
                    GameManager.instance.ChangeScene(1);
                }
                else
                {
                    SwapTrackToHome();
                    GameManager.instance.ChangeScene(2);
                }
            }
            else
            {
                loginUITrigger.CloseLoginForm();
                StartCoroutine(SendVerificationEmail());
            }
        }
    }

    private IEnumerator LoginLogicSec(string _email, string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        var loginTask = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown Error, Please Try Again";

            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "Email is required.";
                    break;
                case AuthError.MissingPassword:
                    output = "Password is required.";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email.";
                    break;
                case AuthError.WrongPassword:
                    output = "Incorrect Password.";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does Not Exist.";
                    break;
            }
            loginOutputTextSec.text = output;
        }
        else
        {
            if (user.IsEmailVerified)
            {
                yield return new WaitForSeconds(1f);
                if (user.DisplayName == null || user.DisplayName.Length == 0)
                {
                    GameManager.instance.ChangeScene(1);
                }
                else
                {
                    SwapTrackToHome();
                    GameManager.instance.ChangeScene(2);
                }
            }
            else
            {
                loginUITrigger.CloseRegisterForm();
                StartCoroutine(SendVerificationEmail());
            }
        }
    }

    private IEnumerator RegisterLogic(string _email, string _password, string _confirmPassword)
    {

        if (_password != _confirmPassword)
        {
            registerOutputText.text = "Passwords do not match.";
        }
        else
        {
            auth = FirebaseAuth.DefaultInstance;
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown Error, Please Try Again";

                switch (error)
                {
                    case AuthError.InvalidEmail:
                        output = "Invalid Email.";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "Email Already In Use.";
                        break;
                    case AuthError.WeakPassword:
                        output = "Weak Password.";
                        break;
                    case AuthError.MissingEmail:
                        output = "Email is required.";
                        break;
                    case AuthError.MissingPassword:
                        output = "Password is required.";
                        break;
                }
                registerOutputText.text = output;
            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = "",

                    //TODO: Give Profile Default Photo
                };

                user = auth.CurrentUser;
                var defaultUserTask = user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);

                if (defaultUserTask.Exception != null)
                {
                    user.DeleteAsync();
                    FirebaseException firebaseException = (FirebaseException)defaultUserTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;
                    string output = "Unknown Error, Please Try Again";

                    switch (error)
                    {
                        case AuthError.Cancelled:
                            output = "Update User Cancelled.";
                            break;
                        case AuthError.SessionExpired:
                            output = "Session Expired.";
                            break;
                    }
                    registerOutputText.text = output;
                }
                else
                {
                    Debug.Log($"Firebase User Created Successfully: {user.DisplayName} ({user.UserId})");
                    loginUITrigger.CloseRegisterForm();

                    StartCoroutine(SendVerificationEmailForRegister());
                }
            }
        }
    }

    private IEnumerator RegisterLogicSec(string _email, string _password, string _confirmPassword)
    {

        if (_password != _confirmPassword)
        {
            registerOutputTextSec.text = "Passwords do not match.";
        }
        else
        {
            auth = FirebaseAuth.DefaultInstance;
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown Error, Please Try Again";

                switch (error)
                {
                    case AuthError.InvalidEmail:
                        output = "Invalid Email.";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "Email Already In Use.";
                        break;
                    case AuthError.WeakPassword:
                        output = "Weak Password.";
                        break;
                    case AuthError.MissingEmail:
                        output = "Email is required.";
                        break;
                    case AuthError.MissingPassword:
                        output = "Password is required.";
                        break;
                }
                registerOutputTextSec.text = output;
            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = "",

                    //TODO: Give Profile Default Photo
                };

                user = auth.CurrentUser;
                var defaultUserTask = user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);

                if (defaultUserTask.Exception != null)
                {
                    user.DeleteAsync();
                    FirebaseException firebaseException = (FirebaseException)defaultUserTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;
                    string output = "Unknown Error, Please Try Again";

                    switch (error)
                    {
                        case AuthError.Cancelled:
                            output = "Update User Cancelled.";
                            break;
                        case AuthError.SessionExpired:
                            output = "Session Expired.";
                            break;
                    }
                    registerOutputTextSec.text = output;
                }
                else
                {
                    Debug.Log($"Firebase User Created Successfully: {user.DisplayName} ({user.UserId})");
                    loginUITrigger.CloseLoginForm();

                    StartCoroutine(SendVerificationEmailForRegister());
                }
            }
        }
    }

    private IEnumerator SendVerificationEmail()
    {
        var emailTask = user.SendEmailVerificationAsync();

        yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

        if (emailTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = "Unknown Error, Please Try Again";

            switch (error)
            {
                case AuthError.Cancelled:
                    output = "Send Email Cancelled.";
                    break;
                case AuthError.InvalidRecipientEmail:
                    output = "Invalid Email.";
                    break;
                case AuthError.SessionExpired:
                    output = "Session Expired.";
                    break;
                case AuthError.TooManyRequests:
                    output = "Too Many Requests.";
                    break;
            }
            loginUITrigger.AwaitVerification(false, user.Email, output);
        }
        else
        {
            loginUITrigger.AwaitVerification(true, user.Email, "Email Sent");
            Debug.Log("Email Verification Sent Successfully");
        }
    }

    private IEnumerator SendVerificationEmailForRegister()
    {
        string email = user.Email;
        var emailTask = user.SendEmailVerificationAsync();

        yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

        if (emailTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = "Unknown Error, Please Try Again";

            switch (error)
            {
                case AuthError.Cancelled:
                    output = "Send Email Cancelled.";
                    break;
                case AuthError.InvalidRecipientEmail:
                    output = "Invalid Email.";
                    break;
                case AuthError.SessionExpired:
                    output = "Session Expired.";
                    break;
                case AuthError.TooManyRequests:
                    output = "Too Many Requests.";
                    break;
            }
            loginUITrigger.AwaitVerification(false, email, output);
        }
        else
        {
            loginUITrigger.AwaitVerification(true, email, "Email Sent");
            Debug.Log("Email Verification Sent Successfully");
            yield return new WaitForSeconds(0.8f);
            auth.SignOut();
        }
    }

    private IEnumerator ForgotPasswordLogic(string _email, TMP_Text _outputText)
    {
        auth = FirebaseAuth.DefaultInstance;
        var forgotPasswordTask = auth.SendPasswordResetEmailAsync(_email);

        yield return new WaitUntil(predicate: () => forgotPasswordTask.IsCompleted);

        if (forgotPasswordTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)forgotPasswordTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown Error, Please Try Again";

            switch (error)
            {
                case AuthError.InvalidRecipientEmail:
                    output = "Invalid Email.";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email.";
                    break;
                case AuthError.MissingEmail:
                    output = "Email is required.";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does Not Exist.";
                    break;
            }
            _outputText.text = output;
        }
        else
        {
            loginUITrigger.CloseForgotPassPopup();
            loginUITrigger.CloseForgotPassPopupSec();
        }
    }

    public void SignInWithGoogle() { OnSignIn(); }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Sign in canceled");
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            Debug.Log("Email = " + task.Result.Email);
            Debug.Log("Google ID Token = " + task.Result.IdToken);
            Debug.Log("Email = " + task.Result.Email);
            StartCoroutine(SignInWithGoogleOnFirebase(task.Result.IdToken));
        }
    }

    private IEnumerator SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        var googleSignInTask = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => googleSignInTask.IsCompleted);

        if (googleSignInTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)googleSignInTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown Error, Please Try Again";

            switch (error)
            {
                case AuthError.Cancelled:
                    output = "Sign In Cancelled.";
                    break;
                case AuthError.InvalidCredential:
                    output = "Invalid Credential.";
                    break;
                case AuthError.MissingEmail:
                    output = "Email is required.";
                    break;
                case AuthError.MissingPassword:
                    output = "Password is required.";
                    break;
                case AuthError.NetworkRequestFailed:
                    output = "Network Request Failed.";
                    break;
                case AuthError.OperationNotAllowed:
                    output = "Operation Not Allowed.";
                    break;
                case AuthError.WeakPassword:
                    output = "Weak Password.";
                    break;
            }
            loginUITrigger.AwaitVerification(false, "", output);
        }
        else
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = "",

                //TODO: Give Profile Default Photo
            };

            user = auth.CurrentUser;
            var defaultUserTask = user.UpdateUserProfileAsync(profile);

            yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);

            if (defaultUserTask.Exception != null)
            {
                user.DeleteAsync();
                FirebaseException firebaseException = (FirebaseException)defaultUserTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown Error, Please Try Again";

                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Update User Cancelled.";
                        break;
                    case AuthError.SessionExpired:
                        output = "Session Expired.";
                        break;
                }
                registerOutputTextSec.text = output;
            }
            else
            {
                if (user.IsEmailVerified)
                {
                    yield return new WaitForSeconds(1f);
                    if (user.DisplayName == null || user.DisplayName.Length == 0)
                    {
                        GameManager.instance.ChangeScene(1);
                    }
                    else
                    {
                        GameManager.instance.ChangeScene(2);
                    }
                }
                else
                {

                    StartCoroutine(SendVerificationEmail());
                }
            }
        }
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void SwapTrackToHome()
    {
        AudioManager.instance.SwapTrack(homeTrack);
    }
}
