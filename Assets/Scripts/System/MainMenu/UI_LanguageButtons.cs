using UnityEngine;
using UnityEngine.UI;

public class UI_LanguageButtons : MonoBehaviour
{
    void Start()
    {
        GameObject toUsObj = GameObject.Find("ToUs");
        GameObject toJpObj = GameObject.Find("ToJp");
        GameObject toKoObj = GameObject.Find("ToKo");

        if (toUsObj != null && toUsObj.TryGetComponent<Button>(out var toUsBtn))
        {
            toUsBtn.onClick.AddListener(() => Global_LanguageManager.Instance.OnClickChangeLanguage("en-US"));
        }

        if (toJpObj != null && toJpObj.TryGetComponent<Button>(out var toJpBtn))
        {
            toJpBtn.onClick.AddListener(() => Global_LanguageManager.Instance.OnClickChangeLanguage("ja-JP"));
        }

        if (toKoObj != null && toKoObj.TryGetComponent<Button>(out var toKoBtn))
        {
            toKoBtn.onClick.AddListener(() => Global_LanguageManager.Instance.OnClickChangeLanguage("ko-KR"));
        }
    }
}