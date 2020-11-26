using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingProcess : MonoBehaviour
{
    public TMP_Text _numText;
    public RawImage _image;
    public TMP_Text _desc;

    public void Init(CookingProcessInfo info)
    {
        StartCoroutine(LoadImageTexture(info.STRE_STEP_IMAGE_URL));

        _numText.text = info.COOKING_NO;
        _desc.text = info.COOKING_DC;
    }

        /// URL 이미지 로드
    protected IEnumerator LoadImageTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        www.timeout = 10; // 타임아웃 10초
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            _image.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }
}
