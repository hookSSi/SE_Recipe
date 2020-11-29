using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeDetailedInfoLoader : MonoBehaviour
{
    public TMP_Text _recipeNameText;
    public TMP_Text _levelText;
    public GameObject _ingredientHolder;
    public GameObject _ingredientPref;
    public GameObject _cookingProcessHolder;
    public GameObject _cookingProcessPref;

    public List<GameObject> _ingredientObjList = new List<GameObject>();
    public List<GameObject> _cookingProcessObjList = new List<GameObject>();

    RecipeDetailedInfo _info;

    public void LoadRecipe(RecipeDetailedInfo info)
    {
        Clear();
        _info = info;

        // 레시피 정보
        _recipeNameText.text = info.RECIPE.RECIPE_NM_KO;
        _levelText.text = info.RECIPE.LEVEL_NM;

        // 재료 정보
        foreach(var ingredient in info.INGREDIENT)
        {
            GameObject obj = Instantiate(_ingredientPref);
            obj.GetComponent<Ingredient>().Init(ingredient);
            obj.transform.SetParent(_ingredientHolder.transform);
            _ingredientObjList.Add(obj);
        }

        // 요리 과정
        foreach(var process in info.PROCESS)
        {
            GameObject obj = Instantiate(_cookingProcessPref);
            obj.GetComponent<CookingProcess>().Init(process);
            obj.transform.SetParent(_cookingProcessHolder.transform);
            _cookingProcessObjList.Add(obj);
        }
    }

    public void Clear()
    {
        _recipeNameText.text = "";
        _levelText.text = "";
        ClearIngredientList();
        ClearCookingProcessList();
    }

    public void ClearIngredientList()
    {
        foreach(var obj in _ingredientObjList)
        {
            Destroy(obj);
        }
        _ingredientObjList.Clear();
    }

    public void ClearCookingProcessList()
    {
        foreach(var obj in _cookingProcessObjList)
        {
            Destroy(obj);
        }
        _cookingProcessObjList.Clear();
    }
}
