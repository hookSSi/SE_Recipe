using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TMPro;

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
            UserManager.Instance.LogOut();
            sceneLoaded = true;
        }

        [UnityTest]
        public IEnumerator 회원가입창_열기_테스트()
        {
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager.OpenRegisterForm();
            Assert.IsTrue(manager._registerForm.activeSelf);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 회원가입창_닫기_테스트()
        {
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager.CloseRegisterForm();
            Assert.IsFalse(manager._registerForm.activeSelf);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 회원가입_정상_입력_테스트()
        {
            string userID = "test";
            string userPW = "ab4202";
            string userEmail = "test";

            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();
            manager._registerIDInput.text = userID;
            manager._registerPWInput.text = userPW;
            manager._registerEmailInput.text = userEmail;

            manager.TestRegister();

            yield return new WaitWhile(() => manager._requestProcessing == false);

            Assert.IsTrue(manager._data.state);

            yield return null;
        }

        [UnityTest]
        public IEnumerator 서버통신불량_회원가입_테스트()
        {
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();

            string temp = manager._testRegisterUrl;
            manager._testRegisterUrl = "통신불량URL";
            manager._registerIDInput.text = "test";
            manager._registerPWInput.text = "ab4202";
            manager._registerEmailInput.text = "test";

            manager.TestRegister();

            yield return new WaitWhile(() => manager._requestProcessing == false);
            Assert.IsFalse(manager._data.state);
            StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);

            manager._testRegisterUrl = temp;

            yield return null;
        }

        [UnityTest]
        public IEnumerator 회원가입_아이디_비정상_입력_테스트()
        {
            string userPW = "ab4202";
            string userEmail = "test";
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                // 아이디가 4자 이하인 경우
                // 회원가입 실패 메시지
                {"t", false},
                {"tt", false},
                {"ttt", false},

                // 아이디가 30자 초과인 경우
                // 초과하지 못하도록 막음
                {"0123456789012345678901234567800123456789", true},

                // 아이디에 특수문자가 포함된 경우
                // 특수문자가 자동으로 지워짐
                {"test@", true},
                {"te@st", true},
                {"!##test", true},

                // 아이디에 공백이 포함된 경우
                // 공백이 자동으로 지워짐
                {"te st", true},
                {" te sttes", true},
                {" te st tte", true},

                // 영어 이외의 언어를 입력한 경우
                // 영어 이외의 언어는 자동으로 지워짐
                {"한글날이오는구난test", true},
                {"한글날이ㄱ오는구난", false},

                // 서버에 이미 존재하는 아이디
                {"hook", false},
                
                // 대문자를 입력한 경우
                {"TEST", true}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                TestRegister(manager, pair.Key, userPW, userEmail);
                yield return new WaitWhile(() => manager._requestProcessing == false);

                if(pair.Value == false)
                {
                    Assert.IsFalse(manager._data.state);
                    if(pair.Key != "hook")
                    {
                        StringAssert.AreEqualIgnoringCase("회원가입 실패", manager._errorMessage._text);
                    }
                    else
                    {
                        StringAssert.AreEqualIgnoringCase("중복ID오류", manager._errorMessage._text);
                    }
                }
                else
                {
                    Assert.IsTrue(manager._data.state);
                }
            }

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator 회원가입_비밀번호_비정상_입력_테스트()
        {
            string userID = "test";
            string userEmail = "test";
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                // 비밀번호가 4자 이하인 경우
                // 회원가입 실패 메시지
                {"t", false},
                {"tt", false},
                {"ttt", false},

                // 비밀번호가 30자 초과인 경우
                // 초과하지 못하도록 막음
                {"0123456789012345678901234567890123456789", true},

                // 비밀번호에 특수문자가 포함된 경우
                {"test@", true},
                {"te@st", true},
                {"!##test", true},

                // 비밀번호에 공백이 포함된 경우
                // 공백이 자동으로 지워짐
                {"te st", true},
                {" te sttes", true},
                {" te st tte", true},

                // 영어 이외의 언어를 입력한 경우
                // 영어 이외의 언어는 자동으로 지워짐
                {"한글날이오는구난test", true},
                {"한글날이ㄱ오는구난", false},
                
                // 대문자를 입력한 경우
                {"TEST", true}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                TestRegister(manager, userID, pair.Key, userEmail);
                yield return new WaitWhile(() => manager._requestProcessing == false);

                if(pair.Value == false)
                {
                    Assert.IsFalse(manager._data.state);
                    StringAssert.AreEqualIgnoringCase("회원가입 실패", manager._errorMessage._text);
                }
                else
                {
                    Assert.IsTrue(manager._data.state);
                }
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 회원가입_이메일_비정상_입력_테스트()
        {
            string userID = "test";
            string userPW = "ab4202";
            StartMenuManager manager = GameObject.FindObjectOfType<StartMenuManager>();

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                // 이메일 4자 이하인 경우
                // 회원가입 실패 메시지
                {"t", false},
                {"tt", false},
                {"ttt", false},

                // 이메일 30자 초과인 경우
                // 초과하지 못하도록 막음
                {"0123456789012345678901234567890123456789", true},

                // 이메일에 특수문자가 포함된 경우
                // 특수문자가 자동으로 지워짐
                {"test@", true},
                {"te@st", true},
                {"!##test", true},

                // 이메일에 공백이 포함된 경우
                // 공백이 자동으로 지워짐
                {"te st", true},
                {" te sttes", true},
                {" te st tte", true},

                // 영어 이외의 언어를 입력한 경우
                // 영어 이외의 언어 자동으로 지워짐
                {"한글날이오는구난test", true},
                {"한글날이ㄱ오는구난", false},
                
                // 대문자를 입력한 경우
                {"TEST", true}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                TestRegister(manager, userID, userPW, pair.Key);
                yield return new WaitWhile(() => manager._requestProcessing == false);

                if(pair.Value == false)
                {
                    Assert.IsFalse(manager._data.state);
                    StringAssert.AreEqualIgnoringCase("회원가입 실패", manager._errorMessage._text);
                }
                else
                {
                    Assert.IsTrue(manager._data.state);
                }
            }

            yield return null;
        }

        public void TestRegister(StartMenuManager manager, string userID, string userPW, string userEmail)
        {
            manager._registerIDInput.text = userID;
            manager._registerPWInput.text = userPW;
            manager._registerEmailInput.text = userEmail;

            manager.TestRegister();
        }
    }
}
