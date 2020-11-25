using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistoryManagerTestCase : HistoryManager
{
    public TMP_InputField _userID;
    public TMP_InputField _recipeID;

    private void Start() 
    {
        _userID.text = "Test";
        _recipeID.text = "1";
    }

    public void SaveRecipeTest()
    {
        StartCoroutine(SaveRecipe(_userID.text, _recipeID.text));
    }

    public void RemoveRecipeTest()
    {
        StartCoroutine(RemoveRecipe(_userID.text, _recipeID.text));
    }

    public void LoadRecipeSavedTest()
    {
        StartCoroutine(LoadRecipeSaved(_userID.text));
    }
}
