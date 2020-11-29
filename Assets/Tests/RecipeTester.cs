using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class RecipeTester
    {
        public bool sceneLoaded;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("SearchRecipe", LoadSceneMode.Single);
        }
 
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneLoaded = true;
            UserManager.Instance.LogOut();
            UserManager.Instance.Login("hook", true); // 이미 회원가입된 계정 사용
        }

        [UnityTest]
        public IEnumerator 서버통신불량_레시피_자세한_정보_불러오기()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            yield return Search(manager, "계란", SearchOption.RECIPE_NAME);

            Recipe[] recipeList = GameObject.FindObjectsOfType<Recipe>();

            foreach(var recipe in recipeList)
            {
                string temp = recipe._loadRecipeDetailInfoUrl;
                recipe._loadRecipeDetailInfoUrl = "통신불량URL";

                recipe.LoadRecipeDetailInfo();
                yield return new WaitWhile(() => recipe._requestProcessing == false);
                StringAssert.AreNotEqualIgnoringCase(recipe._info.RECIPE_NM_KO, recipe._loader._recipeNameText.text);
                StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);
                recipe._loadRecipeDetailInfoUrl = temp;
            }

            foreach(var recipe in recipeList)
            {
                recipe.LoadRecipeDetailInfo();
                yield return new WaitWhile(() => recipe._requestProcessing == false);
                StringAssert.AreEqualIgnoringCase(recipe._info.RECIPE_NM_KO, recipe._loader._recipeNameText.text);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 레시피_자세한_정보_불러오기()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            yield return Search(manager, "계란", SearchOption.RECIPE_NAME);

            Recipe[] recipeList = GameObject.FindObjectsOfType<Recipe>();

            foreach(var recipe in recipeList)
            {
                recipe.LoadRecipeDetailInfo();
                yield return new WaitWhile(() => recipe._requestProcessing == false);
                StringAssert.AreEqualIgnoringCase(recipe._info.RECIPE_NM_KO, recipe._loader._recipeNameText.text);
            }

            yield return null;
        }

        protected IEnumerator Search(SearchManager manager, string keyword, SearchOption option)
        {
            manager._keywordInput.text = keyword;
            manager._optionInput.value = (int)option;
            manager.Search();
            yield return new WaitWhile(() => manager._requestProcessing == false);
        }
    }
}
