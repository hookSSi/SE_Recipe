using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RecipeHistory : Recipe
{
    public void RemoveHistory()
    {
        string userID = UserManager.Instance.GetId();
        string recipeID = this._info.RECIPE_ID;

        StartCoroutine(_historyManager.RemoveRecipe(userID, recipeID));
        Destroy(this.gameObject);
    }

    override protected IEnumerator LoadRecipeDetailInfo(string recipeID)
    {
        WWWForm form = new WWWForm();
        form.AddField("recipeID", recipeID);

        UnityWebRequest www = UnityWebRequest.Post(_loadRecipeDetailInfoUrl, form);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonStr = www.downloadHandler.text;

            if(jsonStr != "결과가 없습니다.")
            {
                RecipeDetailedInfo detailedInfo = JsonUtility.FromJson<RecipeDetailedInfo>(jsonStr);
                _loader.gameObject.SetActive(true);
                _loader.LoadRecipe(detailedInfo);
            }
        }
    }
}
