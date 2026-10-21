using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SnowEscape_DecoUIManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] GameObject warningPanel;

    void Start()
    {
        StartCoroutine(PlayWarningPanel(warningPanel));
    }

    IEnumerator PlayWarningPanel(GameObject panel)
    {
        // 1차 연출 대기 
        yield return new WaitForSeconds(4f);

        if (panel != null)
        {
            RectTransform rect = panel.GetComponent<RectTransform>();

            if (rect != null)
            {
                //크기를 0으로 만들고 활성화
                rect.localScale = Vector3.zero;
                panel.SetActive(true);

                // 등장 애니메이션
                rect.DOScale(1f, 0.6f).SetEase(Ease.OutBack);

                // 잠시 대기
                yield return new WaitForSeconds(0.6f);

                // 퇴장 애니메이션
                rect.DOScale(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    // 애니메이션이 완전히 끝난 후 비활성화
                    panel.SetActive(false);
                });

            }
        }
    }
}
