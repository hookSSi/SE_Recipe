using UnityEngine;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    private static UserManager instance;

    [SerializeField] private string _userId;
    [SerializeField] private bool _loginState;
    
    private UserManager()
    {
        _userId = "";
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
                instance = new UserManager();
            }
            return instance;
        }
    }

    public void Login(string userId)
    {
        if(!_loginState)
        {
            _userId = userId;
            _loginState = true;
        }
    }

    public void LogOut()
    {
        if(_loginState)
        {
            _userId = "";
            _loginState = false;
            SceneManager.LoadScene("Login");
        }
    }

    public string GetId() { return _userId; }
    public bool GetLoginState() { return _loginState; }
}
