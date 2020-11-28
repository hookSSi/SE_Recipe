using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class HistoryData
{
    public string USER_ID;
    public int RECIPE_ID;
}

public class HistoryManager : MonoBehaviour
{
    private static HistoryManager instance;

    public string _saveRecipeUrl = "hook.iptime.org:1080/saveRecipe.php";
    public string _loadRecipeSavedUrl = "hook.iptime.org:1080/loadRecipeSaved.php";
    public string _removeRecipeUrl = "hook.iptime.org:1080/removeRecipe.php";

    public Transform _contentHolder;
    public List<GameObject> _recipeObjList = new List<GameObject>();
    public GameObject _recipePref;

    public ErrorMessage _errorMessage;

    public RecipeDetailedInfoLoader _recipeDetailLoader;

    void Start()
    {
        if(_contentHolder != null)
        {
            LoadRecipeSaved();
        }
    }

    public void SaveRecipe(string recipeID)
    {
        StartCoroutine(SaveRecipe(UserManager.Instance.GetID(), recipeID));
    }

    protected IEnumerator SaveRecipe(string userID, string recipeID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("recipeID", recipeID);

        UnityWebRequest www = UnityWebRequest.Post(_saveRecipeUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning(www.error);
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);

            if(data.state)
            {
                Debug.Log("저장 성공");
            }
            else
            {
                Debug.LogWarning("저장 실패");
            }
        }
    }

    public void LoadRecipeSaved()
    {
        foreach(var obj in _recipeObjList)
        {
            Destroy(obj);
        }
        _recipeObjList.Clear();

        string userID = UserManager.Instance.GetID();
        StartCoroutine(LoadRecipeSaved(userID));
    }

    protected IEnumerator LoadRecipeSaved(string userID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);

        UnityWebRequest www = UnityWebRequest.Post(_loadRecipeSavedUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            if(_errorMessage != null)
            {
                _errorMessage.PrintError("서버와 통신 불량");
            }
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            Debug.Log(jsonStr);

            if(jsonStr != "결과가 없습니다.")
            {
                HistoryData[] dataList = JsonHelper.FromJson<HistoryData>(jsonStr);

                if(_recipePref != null)
                {
                    foreach(var data in dataList)
                    {
                        GameObject obj = Instantiate(_recipePref);
                        obj.GetComponent<RecipeHistory>().Init(data.RECIPE_ID.ToString());
                        obj.GetComponent<RecipeHistory>().Link(_recipeDetailLoader, this);
                        obj.transform.SetParent(_contentHolder);
                        _recipeObjList.Add(obj);
                    }
                }
            }
            else
            {
                if(_errorMessage != null)
                {
                    _errorMessage.PrintError("결과가 없습니다.");
                }
            }
        }
    }

    public IEnumerator RemoveRecipe(string userID, string recipeID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("recipeID", recipeID);

        UnityWebRequest www = UnityWebRequest.Post(_removeRecipeUrl, form);

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
        }
    }
}
