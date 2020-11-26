using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void DoLogOut()
    {
        UserManager.Instance.LogOut();
    }
}
