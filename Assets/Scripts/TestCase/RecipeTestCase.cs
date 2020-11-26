using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeTestCase : Recipe
{
    public string _recipeID = "1";

    void InitTest()
    {
        Init(_recipeID);
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            InitTest();
        }
    }
}
