﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    private static UserManager instance;

    [SerializeField] private string _userID;
    [SerializeField] private bool _loginState;
    
    private UserManager()
    {
        _userID = "";
        _loginState = false;
    }

    void Awake()
    {
        if(null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }   
    }

    public static UserManager Instance
    {
        get
        {
            if(null == instance)
            {
                instance = new GameObject().AddComponent<UserManager>();
            }
            return instance;
        }
    }

    public void Login(string userId, bool isTest)
    {
        if(!_loginState)
        {
            _userID = userId;
            _loginState = true;

            if(!isTest)
                SceneManager.LoadScene("MainMenu");
        }
    }

    public void LogOut()
    {
        if(_loginState)
        {
            _userID = "";
            _loginState = false;
        }
    }

    public string GetID() { return _userID; }
    public bool GetLoginState() { return _loginState; }
}
