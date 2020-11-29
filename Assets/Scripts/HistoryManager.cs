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
    public string _saveRecipeUrl = "hook.iptime.org:1080/saveRecipe.php";
    public string _loadRecipeSavedUrl = "hook.iptime.org:1080/loadRecipeSaved.php";
    public string _removeRecipeUrl = "hook.iptime.org:1080/removeRecipe.php";

    public Transform _contentHolder;
    public List<GameObject> _recipeObjList = new List<GameObject>();
    public GameObject _recipePref;

    public ErrorMessage _errorMessage;

    public RecipeDetailedInfoLoader _recipeDetailLoader;
    public bool _requestProcessing = false; // 리퀘스트 보내고 언제 끝나는지 확인용
    public ResponseData _data;

    void Start()
    {
        if(_contentHolder != null)
        {
            LoadRecipeSaved();
        }
    }

    public void SaveRecipe(string recipeID)
    {
        string userID = UserManager.Instance.GetID();
        Debug.LogFormat("{0}에 {1} 저장 시도", userID, recipeID);
        StartCoroutine(SaveRecipe(userID, recipeID));
    }

    protected IEnumerator SaveRecipe(string userID, string recipeID)
    {
        _requestProcessing = false;
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("recipeID", recipeID);

        UnityWebRequest www = UnityWebRequest.Post(_saveRecipeUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning(www.error);
            if(_errorMessage)
            {
                _errorMessage.PrintError("서버와 통신불량");
            }
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            Debug.Log(jsonStr);
            _data = JsonUtility.FromJson<ResponseData>(jsonStr);
        }
        _requestProcessing = true;
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
        _requestProcessing = false;
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
                _errorMessage.PrintError("서버와 통신불량");
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
                        yield return new WaitWhile(() => obj.GetComponent<RecipeHistory>()._isActive == false);
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
        _requestProcessing = true;
    }

    public void RemoveRecipe(string userID, string recipeID)
    {
        Debug.LogFormat("{0}에 {1} 삭제 시도", userID, recipeID);
        StartCoroutine(CoRemoveRecipe(userID, recipeID));
    }

    public void RemoveRecipe(Recipe recipe)
    {
        Debug.LogFormat("{0}에 {1} 삭제 시도", UserManager.Instance.GetID(), recipe._info.RECIPE_ID);
        StartCoroutine(CoRemoveRecipe(recipe));
    }

    public IEnumerator CoRemoveRecipe(string userID, string recipeID)
    {
        _requestProcessing = false;
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("recipeID", recipeID);

        UnityWebRequest www = UnityWebRequest.Post(_removeRecipeUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            
            if(_errorMessage != null)
            {
                _errorMessage.PrintError("서버와 통신불량");
            }
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            Debug.Log(jsonStr);
            _data = JsonUtility.FromJson<ResponseData>(jsonStr);
        }
        _requestProcessing = true;
    }

    public IEnumerator CoRemoveRecipe(Recipe recipe)
    {
        _requestProcessing = false;
        WWWForm form = new WWWForm();
        form.AddField("userID", UserManager.Instance.GetID());
        form.AddField("recipeID", recipe._info.RECIPE_ID);

        UnityWebRequest www = UnityWebRequest.Post(_removeRecipeUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            
            if(_errorMessage != null)
            {
                _errorMessage.PrintError("서버와 통신불량");
            }
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            Debug.Log(jsonStr);
            _data = JsonUtility.FromJson<ResponseData>(jsonStr);
            Destroy(recipe.gameObject);
        }
        _requestProcessing = true;
    }
}
