using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorMessage : MonoBehaviour
{
    public TMP_Text _textUI;
    public string _text;
    public float _duration = 2; // 지속시간

    private float _lapsed = 0;

    void Awake()
    {
        if(_textUI == null)
        {
            _textUI = this.GetComponent<TMP_Text>();
        }

        if(_textUI != null)
        {
            _textUI.text = "";
        }
    }

    public void PrintError(string errorMessage)
    {
        _text = errorMessage;
        StartCoroutine(CoPrintError(_text));
    }

    IEnumerator CoPrintError(string errorMessage)
    {
        _textUI.text = _text.ToString();
        _lapsed = 0;

        while(_lapsed < _duration)
        {
            _lapsed += Time.deltaTime;
            yield return null;
        }

        _textUI.text = "";
    }
}
