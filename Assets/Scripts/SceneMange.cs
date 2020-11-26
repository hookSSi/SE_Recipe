using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMange : MonoBehaviour
{
    public string sceneName;

    public void SeceneChange()
    {
        SceneManager.LoadScene(sceneName);
    }
}
