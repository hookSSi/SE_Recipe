using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class SearchTester
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
        public IEnumerator 서버통신불량_검색_테스트()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, SearchOption> dic = new Dictionary<string, SearchOption>()
            {
                {"계란", SearchOption.RECIPE_NAME},
                {"보통", SearchOption.LEVEL},
                {"오이", SearchOption.INGREDIENT}
            };

            string temp = manager._searchUrl;
            manager._searchUrl = "통신불량URL";
            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, pair.Value);
                Assert.That(manager._recipeObjList.Count == 0);
                StringAssert.AreEqualIgnoringCase("서버와 통신불량", manager._errorMessage._text);
            }
            manager._searchUrl = temp;

            yield return null;
        }

        [UnityTest]
        public IEnumerator 정상_검색어_입력_테스트_옵션_레시피명()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                {"계란", true},
                {"된장", true},
                {"오이", true}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, SearchOption.RECIPE_NAME);
                Assert.That(manager._recipeObjList.Count > 0 && pair.Value);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 정상_검색어_입력_테스트_옵션_난이도()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                {"초보환영", true},
                {"보통", true},
                {"어려움", true}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, SearchOption.LEVEL);
                Assert.That(manager._recipeObjList.Count > 0 && pair.Value);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 정상_검색어_입력_테스트_옵션_재료()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, bool> dic = new Dictionary<string, bool>()
            {
                {"계란", true},
                {"감자", true},
                {"오이", true}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, SearchOption.INGREDIENT);
                Assert.That(manager._recipeObjList.Count > 0 && pair.Value);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 비정상_검색어_입력_테스트_옵션_레시피명()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                // 특수 문자가 포함된 경우
                // 특수문자를 지워줌
                {"@계란", "성공"},
                {"된장@", "성공"},
                {"오@이", "성공"},
                // 공백이거나 빈 검색어인 경우
                {" ", "검색어 오류"},
                {"", "검색어 오류"},
                // 결과가 없을 경우
                {"결과가 존재하지 않는 검색엉", "결과가 없습니다."},
                // 50자이상의 검색어
                {"01234567890123456789012345678901234567890123456789012345678901234567890123456789", "결과가 없습니다."},
                // 영어 또는 숫자로된 검색어
                {"fsdagasg", "결과가 없습니다."},
                {"341424", "결과가 없습니다."}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, SearchOption.RECIPE_NAME);

                if(pair.Value != "성공")
                {
                    Assert.That(manager._recipeObjList.Count == 0);
                    StringAssert.AreEqualIgnoringCase(manager._errorMessage._text, pair.Value);
                }
                else
                {
                    Assert.That(manager._recipeObjList.Count > 0);
                }
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 비정상_검색어_입력_테스트_옵션_난이도명()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                // 특수 문자가 포함된 경우
                // 특수문자를 지워줌
                {"@초보환영", "성공"},
                {"보통@", "성공"},
                {"어@려움", "성공"},
                // 공백이거나 빈 검색어인 경우
                {" ", "검색어 오류"},
                {"", "검색어 오류"},
                // 50자이상의 검색어
                {"01234567890123456789012345678901234567890123456789012345678901234567890123456789", "검색어 오류"},
                // 영어 또는 숫자로된 검색어
                {"fsdagasg", "검색어 오류"},
                {"341424", "검색어 오류"}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, SearchOption.LEVEL);

                if(pair.Value != "성공")
                {
                    Assert.That(manager._recipeObjList.Count == 0);
                    StringAssert.AreEqualIgnoringCase(manager._errorMessage._text, pair.Value);
                }
                else
                {
                    Assert.That(manager._recipeObjList.Count > 0);
                }
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator 비정상_검색어_입력_테스트_옵션_재료명()
        {
            yield return new WaitWhile(() => sceneLoaded == false);
            SearchManager manager = GameObject.FindObjectOfType<SearchManager>();

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                // 특수 문자가 포함된 경우
                // 특수문자를 지워줌
                {"@계란", "성공"},
                {"감자@", "성공"},
                {"오@이", "성공"},
                // 공백이거나 빈 검색어인 경우
                {" ", "검색어 오류"},
                {"", "검색어 오류"},
                // 결과가 없을 경우
                {"결과가 존재하지 않는 검색엉", "결과가 없습니다."},
                // 50자이상의 검색어
                {"01234567890123456789012345678901234567890123456789012345678901234567890123456789", "결과가 없습니다."},
                // 영어 또는 숫자로된 검색어
                {"fsdagasg", "결과가 없습니다."},
                {"341424", "결과가 없습니다."}
            };

            foreach(var pair in dic)
            {
                Debug.LogFormat("{0}:{1}", pair.Key, pair.Value);
                yield return Search(manager, pair.Key, SearchOption.RECIPE_NAME);

                if(pair.Value != "성공")
                {
                    Assert.That(manager._recipeObjList.Count == 0);
                    StringAssert.AreEqualIgnoringCase(manager._errorMessage._text, pair.Value);
                }
                else
                {
                    Assert.That(manager._recipeObjList.Count > 0);
                }
            }

            yield return null;
        }

        public IEnumerator Search(SearchManager manager, string keyword, SearchOption option)
        {
            manager._keywordInput.text = keyword;
            manager._optionInput.value = (int)option;
            manager.Search();
            yield return new WaitWhile(() => manager._requestProcessing == false);
        }
    }
}
