using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class HistoryData
{
    [SerializeField] string userID;
    [SerializeField] int recipeID;

    public HistoryData(string userID, int recipeID)
    {
        this.userID = userID;
        this.recipeID = recipeID;
    }
}

public class HistoryManager : MonoBehaviour
{
    public string _saveRecipeUrl;
    public string _loadRecipeSavedUrl;
    public string _removeRecipeUrl;

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
            Debug.Log(www.error);
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);
            Debug.Log(jsonStr);
        }
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
        }
        else
        {
            string jsonStr = www.downloadHandler.text;
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);
            Debug.Log(jsonStr);
        }
    }

    protected IEnumerator RemoveRecipe(string userID, string recipeID)
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
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonStr);
            Debug.Log(jsonStr);
        }
    }
}
