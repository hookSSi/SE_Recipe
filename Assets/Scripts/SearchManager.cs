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
    List<RecipeInfo> list = new List<RecipeInfo>();
    public GameObject prefabButton;
    public RectTransform ParentPanel;
    public GameObject prefabtext;


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
            Debug.Log(jsonStr);
            string[] strar = jsonStr.Split(new string[] { "}" }, StringSplitOptions.None);

            Debug.Log(strar.Length);

            for(int i = 0; i< strar.Length - 1; i++)
            {
                string[] aar =  strar[i].Split(new string[] { "\"" }, StringSplitOptions.None);

                list.Add(new RecipeInfo(aar[3],aar[7],aar[11],aar[15],aar[19],aar[23]));

                list[i].RECIPE_ID = aar[3];
                list[i].RECIPE_NM_KO = aar[7];
                list[i].SUMRY = aar[11];
                list[i].TY_NM = aar[15];
                list[i].LEVEL_NM = aar[19];
                list[i].IMG_URL = aar[23];

                Result.text += list[i].RECIPE_NM_KO + "\n";
                Printing.SetActive(true);
            
            }
                //prefabtext
            for(int i = 0; i < strar.Length - 1; i++)
            {
                /*GameObject instText = (GameObject)Instantiate(prefabtext);
                instText.transform.SetParent(ParentPanel, false);
                instText.transform.localScale = new Vector3(1, 1 , 1);
            
                instText.transform.position = new Vector3(0, -50 * i, 1);*/

                GameObject goButton = (GameObject)Instantiate(prefabButton);
                goButton.transform.SetParent(ParentPanel, false);
                goButton.transform.localScale = new Vector3(1, 1 , 1);
                goButton.transform.position = new Vector3 (Result.transform.position.x + 150, Result.transform.position.y - i * 50, 1);
                

                Button tempButton = goButton.GetComponent<Button>();
                int tempInt = i;
           
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