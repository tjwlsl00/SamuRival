using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;

public class Score_UIManager : MonoBehaviour
{
    // 싱글톤
    public static Score_UIManager Instance { get; private set; }

    [Header("UI 참조")]
    [SerializeField] TextMeshProUGUI redScore;
    [SerializeField] TextMeshProUGUI blueScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdatePlayerScore();
    }

    void UpdatePlayerScore()
    {
        if (redScore != null && blueScore != null)
        {
            redScore.text = Global_ScoreManger.Instance.redGetScore.ToString();
            blueScore.text = Global_ScoreManger.Instance.blueGetScore.ToString();
        }
    }

    public IEnumerator ScoreUIAnimRoutine(int textIndex)
    {
        TextMeshProUGUI targetTextPanel = (textIndex == 0) ? redScore : blueScore;
        if (targetTextPanel != null)
        {
            float originalFontSize = targetTextPanel.fontSize;
            float targetFontSize = originalFontSize * 1.4f;

            // fontSize 0 상태에서 원래 사이즈로 애니메이션 
            DOTween.To(() => targetTextPanel.fontSize, x => targetTextPanel.fontSize = x, targetFontSize, 0.3f)
                   .SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.4f);

            DOTween.To(() => targetTextPanel.fontSize, x => targetTextPanel.fontSize = x, originalFontSize, 0.2f)
                   .SetEase(Ease.InQuad);

            yield return new WaitForSeconds(0.2f);
        }
    }
}