using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class RegisterTester
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

        [UnityTest]
        public IEnumerator 회원가입창_열기_테스트()
        {
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager.OpenRegisterForm();

            Assert.That(manager._registerForm.activeSelf);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 회원가입창_닫기_테스트()
        {
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager.CloseRegisterForm();

            Assert.That(!manager._registerForm.activeSelf);

            yield return null;
        }
    }
}
