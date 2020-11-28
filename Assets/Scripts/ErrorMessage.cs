using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorMessage : MonoBehaviour
{
    public TMP_Text _text;
    public float _duration = 2; // 지속시간

    private float _lapsed = 0;

    void Awake()
    {
        if(_text == null)
        {
            _text = this.GetComponent<TMP_Text>();
        }

        if(_text != null)
        {
            _text.text = "";
        }
    }

    public void PrintError(string errorMessage)
    {
        StartCoroutine(CoPrintError(errorMessage));
    }

    IEnumerator CoPrintError(string errorMessage)
    {
        _text.text = errorMessage;
        _lapsed = 0;

        while(_lapsed < _duration)
        {
            _lapsed += Time.deltaTime;
            yield return null;
        }

        _text.text = "";
    }
}
