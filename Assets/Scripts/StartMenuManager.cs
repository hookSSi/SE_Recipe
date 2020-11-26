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
    public string _loginUrl;
    public string _registerUrl;

    /// 회원가입 폼
    public GameObject _registerForm;

    /// 로그인 UI
    public TMP_InputField _loginIDInput;
    public TMP_InputField _loginPWInput;

    /// 회원가입 UI
    public TMP_InputField _registerIDInput;
    public TMP_InputField _registerPWInput;
    public TMP_InputField _registerEmailInput;

    /// 회원가입
    IEnumerator Register(string username, string password, string email)
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
        var registerID = _registerIDInput.text;
        var registerPW = _registerPWInput.text;
        var registerEmail = _registerEmailInput.text + "@naver.com";

        StartCoroutine(Register(registerID, registerPW, registerEmail));
        CloseRegisterForm();
    }

    /// 로그인
    IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginID", username);
        form.AddField("loginPW", password);

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
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);
            
            if(data.stated() == true)
            {
                SceneManager.LoadScene("MainMenu");
            }
            Debug.Log(jsonStr);
        }
    }

    public void Login()
    {
        var loginID = _loginIDInput.text;
        var loginPW = _loginPWInput.text;

        StartCoroutine(Login(loginID, loginPW));
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
