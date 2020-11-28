using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class ResponseData
{
    [SerializeField] bool state;
    [SerializeField] string desc;

    public ResponseData(bool state, string desc)
    {
        this.state = state;
        this.desc = desc;
    }

    public bool stated()
    {
        Debug.Log(state);
        return state;
    }
}

public class StartMenuManager : MonoBehaviour
{
    public string _loginUrl = "http://hook.iptime.org:1080/login.php";
    public string _registerUrl = "http://hook.iptime.org:1080/register.php";

    /// 회원가입 폼
    public GameObject _registerForm;

    /// 로그인 UI
    public TMP_InputField _loginIDInput;
    public TMP_InputField _loginPWInput;

    /// 회원가입 UI
    public TMP_InputField _registerIDInput;
    public TMP_InputField _registerPWInput;
    public TMP_InputField _registerEmailInput;

    public ErrorMessage _errorMessage;

    void Start()
    {
        // 모든 ID 입력은 소문자로만
        // 특수문자, 공백 처리, 영어외 언어 처리
        if(_loginIDInput != null)
        {
            _loginIDInput.onValidateInput += delegate(string text, int charIndex, char addedChar) {
                return InputHandleHelper.changeLowerCase (addedChar);
            };
            _loginIDInput.onValueChanged.AddListener(delegate { _loginIDInput.text = InputHandleHelper.CleanIDInput(_loginIDInput.text); });
        }

        if(_registerIDInput != null)
        {
            _registerIDInput.onValidateInput += delegate(string text, int charIndex, char addedChar) {
                return InputHandleHelper.changeLowerCase (addedChar);
            };
            _registerIDInput.onValueChanged.AddListener(delegate { _registerIDInput.text = InputHandleHelper.CleanIDInput(_registerIDInput.text); });
        }

        // 모든 Email 입력은 소문자로만
        // 특수문자, 공백 처리, 영어외 언어 처리
        if(_registerEmailInput != null)
        {
            _registerEmailInput.onValidateInput += delegate(string text, int charIndex, char addedChar) {
                return InputHandleHelper.changeLowerCase (addedChar);
            };
            _registerEmailInput.onValueChanged.AddListener(delegate { _registerEmailInput.text = InputHandleHelper.CleanEmailInput(_registerEmailInput.text); });
        }

        // 패스워드의 공백 처리, 영어외 언어 처리
        if(_loginPWInput != null)
        {
            _loginPWInput.onValueChanged.AddListener(delegate { _loginPWInput.text = InputHandleHelper.CleanPasswordInput(_loginPWInput.text); });
        }
        if(_registerPWInput != null)
        {
            _registerPWInput.onValueChanged.AddListener(delegate { _registerPWInput.text = InputHandleHelper.CleanPasswordInput(_registerPWInput.text); });
        }
    }

    /// 회원가입
    IEnumerator CoRegister(string username, string password, string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("registerID", username);
        form.AddField("registerPW", password);
        form.AddField("registerEmail", email);

        UnityWebRequest www = UnityWebRequest.Post(_registerUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);
            Debug.Log(jsonStr);
        }
    }

    public void OpenRegisterForm()
    {
        _registerForm.SetActive(true);
    }

    public void CloseRegisterForm()
    {
        _registerForm.SetActive(false);
    }

    public void Register()
    {
        // 4자 이상 입력했는지 확인
        if(_registerIDInput.text.Length < 4 || _registerPWInput.text.Length < 4 || _registerEmailInput.text.Length < 4)
        {
            return;
        }

        var registerID = _registerIDInput.text;
        var registerPW = _registerPWInput.text;
        var registerEmail = _registerEmailInput.text + "@naver.com";

        StartCoroutine(CoRegister(registerID, registerPW, registerEmail));
        CloseRegisterForm();
    }

    /// 로그인
    IEnumerator CoLogin(string loginID, string loginPW)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginID", loginID);
        form.AddField("loginPW", loginPW);

        UnityWebRequest www = UnityWebRequest.Post(_loginUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            Debug.Log(jsonStr);
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);
            
            if(data.stated() == true)
            {
                UserManager.Instance.Login(loginID);
            }
        }
    }

    public void Login()
    {
        var loginID = _loginIDInput.text;
        var loginPW = _loginPWInput.text;

        Login(loginID, loginPW);

        if(UserManager.Instance.GetLoginState())
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Login(string loginID, string loginPW)
    {
        StartCoroutine(CoLogin(loginID, loginPW));
    }

    public void ExitProgram()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit() // 어플리케이션 종료
        #endif
    }
}
