using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class HistoryTester
    {
        public bool sceneLoaded;
 
        [SetUp]
        public void OneTimeSetup()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("RecipeHistory", LoadSceneMode.Single);
        }
 
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneLoaded = true;
            UserManager.Instance.LogOut();
            UserManager.Instance.Login("hook2", true); // 이미 회원가입된 계정 사용
        }

        [UnityTest]
        public IEnumerator 열람기록_저장_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            HistoryManager manager = GameObject.FindObjectOfType<HistoryManager>();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            manager.SaveRecipe("1");
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsTrue(manager._data.state);

            manager.SaveRecipe("1"); // 중복 레시피
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsFalse(manager._data.state);

            manager.SaveRecipe("7777"); // 없는 레시피
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsFalse(manager._data.state);

            manager.RemoveRecipe(UserManager.Instance.GetID(), "1"); // 저장된 레시피
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsTrue(manager._data.state);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 서버통신불량_열람기록_저장_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            HistoryManager manager = GameObject.FindObjectOfType<HistoryManager>();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            string temp = manager._saveRecipeUrl;
            manager._saveRecipeUrl = "통신불량URL";

            manager.SaveRecipe("1");
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsFalse(manager._data.state);
            StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);

            manager._saveRecipeUrl = temp;

            yield return null;
        }

        [UnityTest]
        public IEnumerator 열람기록_삭제_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            HistoryManager manager = GameObject.FindObjectOfType<HistoryManager>();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            manager.SaveRecipe("1");
            yield return new WaitWhile(() => manager._requestProcessing == false);

            manager.LoadRecipeSaved();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            // 생성된 레시피 오브젝트
            RecipeHistory history = GameObject.FindObjectOfType<RecipeHistory>();

            // 레시피의 버튼을 눌러 삭제
            history.RemoveHistory();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            Assert.IsTrue(history == null && manager._data.state);

            // 열람 기록에 없는 레시피 삭제
            manager.RemoveRecipe(UserManager.Instance.GetID(), "7777");
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsFalse(manager._data.state);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 서버통신불량_열람기록_삭제_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            HistoryManager manager = GameObject.FindObjectOfType<HistoryManager>();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            manager.SaveRecipe("1");
            yield return new WaitWhile(() => manager._requestProcessing == false);

            manager.LoadRecipeSaved();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            // 생성된 레시피 오브젝트
            RecipeHistory history = GameObject.FindObjectOfType<RecipeHistory>();

            string temp = manager._removeRecipeUrl;
            manager._removeRecipeUrl = "통신불량URL";

            // 레시피의 버튼을 눌러 삭제
            history.RemoveHistory();
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsFalse(history == null && manager._data.state);
            StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);

            // 다시 서버와 연결후
            // 레시피의 버튼을 눌러 삭제
            manager._removeRecipeUrl = temp;
            history.RemoveHistory();
            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsTrue(history == null && manager._data.state);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 열람기록_조회_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            HistoryManager manager = GameObject.FindObjectOfType<HistoryManager>();
            yield return new WaitWhile(() => manager._requestProcessing == false);
            
            // 레시피 저장
            for(int i = 1; i <= 4; i++)
            {
                manager.SaveRecipe(i.ToString());
                yield return new WaitWhile(() => manager._requestProcessing == false);
            }

            manager.LoadRecipeSaved();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            // 생성된 레시피 오브젝트들
            RecipeHistory[] historyList = GameObject.FindObjectsOfType<RecipeHistory>();
            Assert.IsTrue(historyList.Length == 4);
            Assert.IsTrue(historyList.Length == manager._recipeObjList.Count);

            // 삭제
            for(int i = 1; i <= 4; i++)
            {
                manager.RemoveRecipe(UserManager.Instance.GetID(), i.ToString());
                yield return new WaitWhile(() => manager._requestProcessing == false);
            }

            manager.LoadRecipeSaved();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            historyList = GameObject.FindObjectsOfType<RecipeHistory>();
            Assert.IsTrue(historyList.Length == 0);
            Assert.IsTrue(historyList.Length == manager._recipeObjList.Count);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 서버통신불량_열람기록_조회_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            HistoryManager manager = GameObject.FindObjectOfType<HistoryManager>();
            yield return new WaitWhile(() => manager._requestProcessing == false);
            
            // 레시피 저장
            for(int i = 1; i <= 4; i++)
            {
                manager.SaveRecipe(i.ToString());
                yield return new WaitWhile(() => manager._requestProcessing == false);
            }

            string temp = manager._loadRecipeSavedUrl;
            manager._loadRecipeSavedUrl = "통신불량URL";
            manager.LoadRecipeSaved();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            // 생성된 레시피 오브젝트들
            RecipeHistory[] historyList = GameObject.FindObjectsOfType<RecipeHistory>();
            Assert.IsTrue(historyList.Length == 0);
            Assert.IsTrue(historyList.Length == manager._recipeObjList.Count);
            StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);
            manager._loadRecipeSavedUrl = temp;

            // 삭제
            for(int i = 1; i <= 4; i++)
            {
                manager.RemoveRecipe(UserManager.Instance.GetID(), i.ToString());
                yield return new WaitWhile(() => manager._requestProcessing == false);
            }

            manager.LoadRecipeSaved();
            yield return new WaitWhile(() => manager._requestProcessing == false);

            historyList = GameObject.FindObjectsOfType<RecipeHistory>();
            Assert.IsTrue(historyList.Length == 0);
            Assert.IsTrue(historyList.Length == manager._recipeObjList.Count);
            
            yield return null;
        }
    }
}
