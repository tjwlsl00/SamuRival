using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class Score_UIManager : MonoBehaviour
{
    // 싱글톤
    public static Score_UIManager Instance { get; private set; }

    [Header("UI 참조")]
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject[] victoryPanels;
    [SerializeField] TextMeshProUGUI redScore;
    [SerializeField] TextMeshProUGUI blueScore;
    [SerializeField] Image[] twinkles;
    [SerializeField] RectTransform throphy;
    [SerializeField] RectTransform redThrophyPos;
    [SerializeField] RectTransform blueThrophyPos;

    // 스크립트 참조
    private Score_SoundManager score_SoundManager;

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

        // 스크립트 참조 
        score_SoundManager = GetComponent<Score_SoundManager>();
    }

    void Start()
    {
        if (Global_ScoreManger.Instance.isRedWin)
        {
            StartCoroutine(ChangeScorePanelToVictroyPanel(0));
            MoveThrophyToWinner(0);

            // 메뉴 패널 활성화
            StartCoroutine(ActiveMenuBtn());

            // 사운드 효과
            score_SoundManager.PlayCongraClip();
        }
        else if (Global_ScoreManger.Instance.isBlueWin)
        {
            StartCoroutine(ChangeScorePanelToVictroyPanel(1));
            MoveThrophyToWinner(1);

            // 메뉴 패널 활성화
            StartCoroutine(ActiveMenuBtn());

            // 사운드 효과
            score_SoundManager.PlayCongraClip();
        }
        else
        {
            UpdatePlayerScore();

            // 사운드 효과
            score_SoundManager.PlayScoreClip();
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         StartCoroutine(ChangeScorePanelToVictroyPanel(0));
    //         MoveThrophyToWinner(0);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         StartCoroutine(ChangeScorePanelToVictroyPanel(1));
    //         MoveThrophyToWinner(1);
    //     }
    // }

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

    #region 점수 현황 패널 -> 최종 승리 패널 교체 
    public IEnumerator ChangeScorePanelToVictroyPanel(int playerIndex)
    {
        // 점수 현황 패널 비활성화
        if (scorePanel != null)
        {
            RectTransform rect = scorePanel.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.DOScale(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    scorePanel.SetActive(false);
                });
            }
        }

        yield return new WaitForSeconds(1f);

        GameObject targetVictoryPanel = (playerIndex == 0) ? victoryPanels[0] : victoryPanels[1];

        if (targetVictoryPanel != null)
        {
            RectTransform rect = targetVictoryPanel.GetComponent<RectTransform>();

            if (rect != null)
            {
                //크기를 0으로 만들고 활성화
                rect.localScale = Vector3.zero;
                targetVictoryPanel.SetActive(true);

                // 등장 애니메이션
                rect.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            }
        }
    }
    #endregion

    #region 트로피 이동
    private void MoveThrophyToWinner(int playerIndex)
    {
        // --------
        // 텍스트 패널 비활성화 
        // --------
        TextMeshProUGUI targetTextPanel = (playerIndex == 0) ? redScore : blueScore;

        if (targetTextPanel != null)
        {
            targetTextPanel.gameObject.SetActive(false);
        }

        Sequence sequence = DOTween.Sequence();
        // --------
        // 트로피 위치 
        // --------
        RectTransform rect = throphy.GetComponent<RectTransform>();
        RectTransform targetThrophyPos = (playerIndex == 0) ? redThrophyPos : blueThrophyPos;

        if (rect != null && targetThrophyPos != null)
        {
            sequence.Append(rect.DOAnchorPos(targetThrophyPos.anchoredPosition, 0.5f).SetEase(Ease.OutQuad));
        }

        // --------
        // 트윙크 이미지 
        // --------
        var twinkleImage = twinkles[playerIndex];
        Color color = twinkleImage.color;
        color.a = 0f;
        twinkleImage.color = color;
        twinkleImage.gameObject.SetActive(true);

        // 트로피 이동이 끝난 직후 트윙클 페이드 인
        sequence.Append(twinkleImage.DOFade(1f, 0.5f).SetEase(Ease.OutQuad));
    }
    #endregion

    #region 메뉴 버튼 활성화
    IEnumerator ActiveMenuBtn()
    {
        yield return new WaitForSeconds(3f);

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);

            // 마우스 커서 활성화 
            MouseEvent.Instance.ShowCursor();
        }
    }
    #endregion
}