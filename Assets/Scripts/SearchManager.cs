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
    public TMP_InputField _keywordInput;
    public Dropdown _optionInput;

    public Transform _resultRecipeHolder;
    public GameObject _recipePref;
    public List<GameObject> _recipeObjList;
    public RecipeDetailedInfoLoader _recipeDetailLoader;
    public HistoryManager _historyManager;

    void Start()
    {
        foreach(var obj in _recipeObjList)
        {
            Destroy(obj);
        }
        _recipeObjList.Clear();
    }

    // a List of Dropdown options (building name)
    IEnumerator Search(string search_Keyword, string search_Option)
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
            if(jsonStr != "결과가 없습니다.")
            {
                RecipeInfo[] dataList = JsonHelper.FromJson<RecipeInfo>(jsonStr);

                if(_recipePref != null)
                {
                    foreach(var data in dataList)
                    {
                        GameObject obj = Instantiate(_recipePref);
                        obj.GetComponent<Recipe>().Init(data.RECIPE_ID.ToString());
                        obj.GetComponent<Recipe>().Link(_recipeDetailLoader, _historyManager);
                        obj.transform.SetParent(_resultRecipeHolder);
                        _recipeObjList.Add(obj);
                    }
                }
            }
        }
    }

    public void Search()
    {
        foreach(var obj in _recipeObjList)
        {
            Destroy(obj);
        }
        _recipeObjList.Clear();

        var keyword = _keywordInput.text;
        var option = _optionInput.value;
        if(option == 0)
        {
            StartCoroutine(Search(keyword, "이름"));
        }
        else if(option == 1)
        {
            StartCoroutine(Search(keyword, "난이도"));
        }
        else if(option == 2)
        {
            StartCoroutine(Search(keyword, "재료"));
        }
    }
}