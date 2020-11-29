using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
            UserManager.Instance.LogOut();
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
            
            string userID = "hook";
            string userPW = "ab4202";

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            
            manager._loginIDInput.text = userID;
            manager._loginPWInput.text = userPW;
            
            manager.Login(true);

            yield return new WaitWhile(() => manager._requestProcessing == false);

            string result = UserManager.Instance.GetID();

            StringAssert.AreEqualIgnoringCase("hook", result);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 비정상적인_아이디_입력_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            
            string userPW = "ab4202";

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                // 서버에 존재하지 않는 아이디
                // 로그인 실패
                {"notexist", false},

                // 4자 미만의 아이디
                // 로그인 실패
                {"", false},
                {"1", false},
                {"12", false},
                {"123", false},

                // 30자 초과로 입력한 아이디
                // 자동으로 초과된 문자 지움
                {"0123456789012345678901234567890123456789", true},
                
                // 특수문자를 포함한 아이디
                // 자동으로 특수문자 지움
                {"@hook", true},
                {"ho#ok", true},
                {"hook!", true},

                // 영어 이외의 언어를 입력한 경우
                // 영어 이외의 언어 지움
                {"한글날hook", true},
                {"한글날", false}
            };

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();            
            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                TestLogin(manager, pair.Key, userPW);
                yield return new WaitWhile(() => manager._requestProcessing == false);
                bool result = UserManager.Instance.GetLoginState();

                if(pair.Value == false)
                {
                    Assert.IsFalse(result);
                    StringAssert.AreEqualIgnoringCase("로그인 실패", manager._errorMessage._text);
                }
                else
                {
                    Assert.IsTrue(result);
                }
                UserManager.Instance.LogOut();
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 서버통신불량_로그인_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);

            string userID = "hook";
            string userPW = "ab4202";

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            
            string temp = manager._loginUrl;
            manager._loginUrl = "통신불량URL";
            manager._loginIDInput.text = userID;
            manager._loginPWInput.text = userPW;
            
            manager.Login(true);

            yield return new WaitWhile(() => manager._requestProcessing == false);

            string result = UserManager.Instance.GetID();
            StringAssert.AreNotEqualIgnoringCase("hook", result);
            StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);
            manager._loginUrl = temp;
        }

        [UnityTest]
        public IEnumerator 비정상적인_비밀번호_입력_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            
            string userID = "test";
            string userEmail = "test";

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                // 매칭되지않는 비밀번호
                // 로그인 실패
                {"notexist", false},

                // 4자 미만의 비밀번호
                // 로그인 실패
                {"", false},
                {"1", false},
                {"12", false},
                {"123", false},

                // 30자 초과로 입력한 비밀번호
                // 자동으로 초과된 문자 지움
                {"0123456789012345678901234567890123456789", true},
                
                // 특수문자를 포함한 비밀번호
                {"ab4202@", true},
                {"ho#ok", true},
                {"hook!", true},

                // 영어 이외의 언어를 입력한 경우
                // 영어 이외의 언어 지움
                {"한글날hook", true},
                {"한글날", false}
            };

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();            
            foreach(var pair in dic)
            {
                if(pair.Key == "notexist")
                {
                    Register(manager, userID, pair.Key + "test", userEmail); // 임시로 회원가입
                }
                else
                {
                    Register(manager, userID, pair.Key, userEmail); // 임시로 회원가입
                }
                yield return new WaitWhile(() => manager._requestProcessing == false);

                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                TestLogin(manager, userID, pair.Key);
                yield return new WaitWhile(() => manager._requestProcessing == false);
                bool result = UserManager.Instance.GetLoginState();

                if(pair.Value == false)
                {
                    Assert.IsFalse(result);
                    StringAssert.AreEqualIgnoringCase("로그인 실패", manager._errorMessage._text);
                }
                else
                {
                    Assert.IsTrue(result);
                }
                UserManager.Instance.LogOut();
                yield return UnRegister(userID);
            }

            yield return null;
        }

        public void TestLogin(StartMenuManager manager, string userID, string userPW)
        {
            manager._loginIDInput.text = userID;
            manager._loginPWInput.text = userPW;

            manager.Login(true);
        }

        public void Register(StartMenuManager manager, string userID, string userPW, string userEmail)
        {
            manager._registerIDInput.text = userID;
            manager._registerPWInput.text = userPW;
            manager._registerEmailInput.text = userEmail;

            manager.Register();
        }

        protected IEnumerator UnRegister(string userID)
        {
            string url = "hook.iptime.org:1080/unregister.php";
            WWWForm form = new WWWForm();
            form.AddField("loginID", userID);

            UnityWebRequest www = UnityWebRequest.Post(url, form);

            www.timeout = 10; // 타임아웃 10초
            yield return www.SendWebRequest(); 
        }
    }
}
