using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeHistory : Recipe
{
    public void RemoveHistory()
    {
        string userID = UserManager.Instance.GetId();
        string recipeID = this._info.RECIPE_ID;

        StartCoroutine(HistoryManager.Instance.RemoveRecipe(userID, recipeID));
        Destroy(this.gameObject);
    }
}
