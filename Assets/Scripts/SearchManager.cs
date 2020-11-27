using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SearchManager : MonoBehaviour
{
    public string _searchUrl;
    public string _loadUrl;
    public TMP_InputField KeywordInput;
    public Dropdown OptionInput;
    public TMP_Text Result;
    public GameObject Printing;
    public TMP_Text lookupText;

    public TMP_Text recipeIDInput;
    string[] ar;
    public GameObject prefabButton;
    public RectTransform ParentPanel;
    public TMP_Text prefabtext;
    public GameObject preRecipe;

    public GameObject content;
    RecipeInfo[] list;

    // a List of Dropdown options (building name)
    IEnumerator print(string search_Keyword, string search_Option)
    {
        WWWForm form = new WWWForm();
        form.AddField("searchKeyword", search_Keyword);
        form.AddField("searchOption", search_Option);

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

            list = JsonHelper.FromJson<RecipeInfo>(jsonStr);

            for(int i = 0; i < list.Length - 1; i++)
            {
                GameObject recipeObject = (GameObject)Instantiate(preRecipe);
                recipeObject.GetComponent<SelectManager>().setID(list[i].RECIPE_ID);
                recipeObject.transform.position = new Vector3(1200, 250 - i * 40, 1);
                recipeObject.transform.parent = content.transform;
                //recipeObject.GetComponentInChildren<TMP_Text>().text = list[i].RECIPE_NM_KO;
                recipeObject.transform.GetChild(0).GetComponentInChildren<Text>().text = list[i].RECIPE_NM_KO;

            }
        }
    }

    public void print()
    {
        var keyword = KeywordInput.text;
        var option = OptionInput.value;
        if(option == 0)
        {
            StartCoroutine(print(keyword, "이름"));
        }
        else if(option == 1)
        {
            StartCoroutine(print(keyword, "난이도"));
        }
        else if(option == 2)
        {
            StartCoroutine(print(keyword, "재료"));
        }

    }
    IEnumerator lookupInfo(int recipeID)
    {
        WWWForm form = new WWWForm();
        form.AddField("recipeID", recipeID);

        UnityWebRequest www = UnityWebRequest.Post(_loadUrl, form);

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
    public void lookupInfo()
    {
        int recipeID = 27;
        StartCoroutine(lookupInfo(recipeID));
    }

    public void HandInputData(int val)
    {

    }
}