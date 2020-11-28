using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class LoginTester
    {
        public bool sceneLoaded;
 
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
        }
 
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneLoaded = true;
        }

        [TearDown]
        public void TearDown()
        {
            UserManager.Instance.LogOut();
        }

        [UnityTest]
        public IEnumerator 정상적인_로그인_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            
            string userID = "test";
            string userPW = "ab4202";

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager.Login(userID, userPW);

            yield return new WaitForSeconds(10);

            string result = UserManager.Instance.GetID();

            StringAssert.AreEqualIgnoringCase("test", result);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 아이디가_존재하지_않음_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            
            string userID = "존재하지않는아이디";
            string userPW = "ab4202";

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager.Login(userID, userPW);

            yield return new WaitForSeconds(10);

            bool result = UserManager.Instance.GetLoginState();

            Assert.That(!result);

            yield return null;
        }
    }
}
