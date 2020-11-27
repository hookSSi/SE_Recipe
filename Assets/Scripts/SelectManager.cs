using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SelectManager : MonoBehaviour
{
    public string recipeID = string.Empty;
    public Button selecButton;
    public string selectURL;
    public TMP_Text selectText;
    public GameObject manager;
    RecipeInfo info;

    void Start()
    {
        
    }
    
    public void SelectRecipe()
    {
        StartCoroutine(LoadRecipeDetail(recipeID));
    }

    IEnumerator LoadRecipeDetail(string recipe_ID)
    {
        Debug.Log("전" + recipe_ID);
        WWWForm form = new WWWForm();

        form.AddField("recipeID", int.Parse(recipe_ID));

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(selectURL);

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
        }
    }

    public void setID(string ID)
    {
        recipeID = ID;
        Debug.Log(ID + "!!!");
    }
}