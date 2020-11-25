using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class RecipeSelect : MonoBehaviour
{
    public string _searchUrl;
    public TMP_InputField KeywordInput;
    public TMP_InputField OptionInput;

    public TMP_Text Result;
    public GameObject Printing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator print(string keyword, string option)
    {
        WWWForm form = new WWWForm();
        form.AddField("search_keyword", keyword);
        form.AddField("search_option", option);

        UnityWebRequest www = UnityWebRequest.Post(_searchUrl, form);

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
                Result.text += data;
                Printing.SetActive(true);
            }
            Debug.Log(jsonStr);
        }
    }

    public void print()
    {
        var keyword = KeywordInput.text;
        var option = OptionInput.text;

        StartCoroutine(print(keyword, option));
    }
}
