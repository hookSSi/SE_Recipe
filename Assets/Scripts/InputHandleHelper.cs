using System;
using System.Text.RegularExpressions;

public static class InputHandleHelper
{
    public static char changeLowerCase(char _cha)
	{
		char tmpChar = _cha;

		string tmpString = tmpChar.ToString();

		tmpString = tmpString.ToLower ();

		tmpChar = System.Convert.ToChar (tmpString);

		return tmpChar;
	}

    /// 특수문자 체크
    public static bool CheckingSpecialText(string txt)
    {
        string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
        System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
        return rex.IsMatch(txt);
    }

    /// 공백 체크
    public static bool CheckingEmptyText(string txt)
    {
        return (txt.Trim().Length) == 0;
    }

    public static string CleanIDInput(string str)
    {
        try 
        {
           string temp = Regex.Replace(str, @"[^0-9a-zA-Z]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

           if(temp.Length > 30)
           {
               temp = temp.Substring(0, 30);
           }

           return temp;
        }
        catch (RegexMatchTimeoutException) 
        {
           return String.Empty;
        }
    }

    public static string CleanEmailInput(string str)
    {
        try 
        {
            string temp = Regex.Replace(str, @"[^0-9a-zA-Z]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

            if(temp.Length > 30)
            {
                temp = temp.Substring(0, 30);
            }

            return temp;
        }
        catch (RegexMatchTimeoutException) 
        {
           return String.Empty;
        }
    }

    public static string CleanPasswordInput(string str)
    {
        try 
        {
            string temp = Regex.Replace(str, @"[^a-zA-Z0-9~`!@#$%^&*()_\-+={}[\]|\\;:'""<>,.?/]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

            if(temp.Length > 30)
            {
                temp = temp.Substring(0, 30);
            }

            return temp;
        }
        catch (RegexMatchTimeoutException) 
        {
           return String.Empty;
        }
    }
    public static string CleanSearchInput(string str)
    {
        try 
        {
            string temp = Regex.Replace(str, @"[^ㄱ-ㅎㅏ-ㅣ가-힣a-zA-Z0-9 ]+", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

            if(temp.Length > 50)
            {
                temp = temp.Substring(0, 50);
            }

            return temp;
        }
        catch (RegexMatchTimeoutException) 
        {
           return String.Empty;
        }
    }
}
