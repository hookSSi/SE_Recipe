using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogOut : MonoBehaviour
{
    public void DoLogOut()
    {
        UserManager.Instance.LogOut();

        if(!UserManager.Instance.GetLoginState())
        {
            SceneManager.LoadScene("StartMenu");
        }
    }
}
