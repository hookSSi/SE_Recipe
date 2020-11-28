using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class ResponseData
{
    [SerializeField] public bool state;
    [SerializeField] public string desc;

    public ResponseData(bool state, string desc)
    {
        this.state = state;
        this.desc = desc;
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

    public bool _requestProcessing = false; // 리퀘스트 보내고 언제 끝나는지 확인용
    public ResponseData _data;

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
    IEnumerator CoRegister(string userID, string userPW, string userEmail, string registerUrl)
    {
        _requestProcessing = false;
        // 4자 이상 입력했는지 확인
        if(userID.Length < 4 || userPW.Length < 4 || userEmail.Length < 4)
        {
            _data.state = false;
            _requestProcessing = true;
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("registerID", userID);
            form.AddField("registerPW", userPW);
            form.AddField("registerEmail", userEmail + "@naver.com");

            UnityWebRequest www = UnityWebRequest.Post(registerUrl, form);

            www.timeout = 10; // 타임아웃 10초
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                _requestProcessing = true;
            }
            else
            {
                string jsonStr = www.downloadHandler.text;
                Debug.Log(jsonStr);
                _data = JsonUtility.FromJson<ResponseData>(jsonStr);
                _requestProcessing = true;
            }
        }

        if(_data.state)
            CloseRegisterForm();
        else
            _errorMessage.PrintError("회원가입 실패");
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
        var registerID = _registerIDInput.text;
        var registerPW = _registerPWInput.text;
        var registerEmail = _registerEmailInput.text;

        StartCoroutine(CoRegister(registerID, registerPW, registerEmail, _registerUrl));
    }

    /// 회원가입 테스트 함수
    public void TestRegister()
    {
        string temp = _registerUrl;
        string testUrl = "http://hook.iptime.org:1080/registerTest.php";
        _registerUrl = testUrl;
        Register();
        _registerUrl = temp;
    }

    /// 로그인
    IEnumerator CoLogin(string loginID, string loginPW, bool isTest)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginID", loginID);
        form.AddField("loginPW", loginPW);

        UnityWebRequest www = UnityWebRequest.Post(_loginUrl, form);

        _requestProcessing = false;
        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            _requestProcessing = true;
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            Debug.Log(jsonStr);
            _data = JsonUtility.FromJson<ResponseData>(jsonStr);
            
            if(_data.state == true)
            {
                UserManager.Instance.Login(loginID, isTest);
            }
            else
            {
                _errorMessage._text = "로그인 실패";
            }
            _requestProcessing = true;
        }
    }

    public void Login()
    {
        Login(false);
    }

    public void Login(bool isTest)
    {
        var loginID = _loginIDInput.text;
        var loginPW = _loginPWInput.text;

        StartCoroutine(CoLogin(loginID, loginPW, isTest));
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
