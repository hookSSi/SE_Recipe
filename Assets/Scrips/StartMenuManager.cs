using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StartMenuManager : MonoBehaviour
{
    public string _loginUrl;
    public string _registerUrl;

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
            Debug.Log(www.downloadHandler.text);
        }
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
            Debug.Log(www.downloadHandler.text);
        }
    }
}
