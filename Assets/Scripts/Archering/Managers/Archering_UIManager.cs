using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class Archering_UIManager : MonoBehaviour
{
    [Header("안내 UI")]
    [SerializeField] GameObject menuPanel;
    public GameObject turnDecidePanel;
    [SerializeField] Transform decideResultPanel;
    [SerializeField] GameObject redGuidePanel;
    [SerializeField] GameObject blueGuidePanel;
    [SerializeField] GameObject redKeyGuidePanel;
    [SerializeField] GameObject blueKeyGuidePanel;
    [SerializeField] GameObject redCCTV;
    [SerializeField] GameObject blueCCTV;
    [SerializeField] GameObject turnStatusPanel;
    [SerializeField] GameObject checkScorePanel;
    [SerializeField] Image[] redTurns;
    [SerializeField] Image[] blueTurns;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI redScore;
    [SerializeField] TextMeshProUGUI blueScore;
    [SerializeField] GameObject redWin;
    [SerializeField] GameObject redLose;
    [SerializeField] GameObject blueWin;
    [SerializeField] GameObject blueLose;
    [Header("라스트 스퍼트 참조")]
    [SerializeField] GameObject drawPanel;
    [SerializeField] GameObject lastSpurtPanel;
    [SerializeField] Transform lastSpurtResultPanel;
    [SerializeField] GameObject[] LastSpurtWinnerPanels;


    [Header("스프라이트 참조")]
    [SerializeField] private Sprite redPlay;
    [SerializeField] private Sprite redNotPlay;
    [SerializeField] private Sprite bluePlay;
    [SerializeField] private Sprite blueNotPlay;

    // 외부
    private Archering_GameManager archering_GameManager;
    private Archering_TurnManager archering_TurnManager;
    private Archering_ScoreManager archering_ScoreManager;
    private Archering_SoundManager archering_SoundManager;

    void Awake()
    {
        archering_GameManager = GetComponent<Archering_GameManager>();
        archering_TurnManager = GetComponent<Archering_TurnManager>();
        archering_ScoreManager = GetComponent<Archering_ScoreManager>();
        archering_SoundManager = GetComponent<Archering_SoundManager>();

        InitialUISetting();
    }

    void Update()
    {
        // 메뉴 패널 토글 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPanel.activeSelf)
            {
                menuPanel.SetActive(false);
            }
            else
            {
                menuPanel.SetActive(true);
            }
        }
    }

    #region 초기 UI 세팅
    private void InitialUISetting()
    {
        menuPanel.SetActive(false);
        turnStatusPanel.SetActive(false);
        redGuidePanel.SetActive(false);
        blueGuidePanel.SetActive(false);
        redKeyGuidePanel.SetActive(false);
        blueKeyGuidePanel.SetActive(false);
        redCCTV.SetActive(false);
        blueCCTV.SetActive(false);
        checkScorePanel.SetActive(false);
        resultPanel.SetActive(false);
        drawPanel.SetActive(false);
        lastSpurtPanel.SetActive(false);
        for (int i = 0; i < decideResultPanel.childCount; i++)
        {
            decideResultPanel.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < lastSpurtResultPanel.childCount; i++)
        {
            lastSpurtResultPanel.GetChild(i).gameObject.SetActive(false);
        }

        // 턴 결정 패널 애니메이션 
        StartCoroutine(EffectTurnDecidePanel());
    }

    IEnumerator EffectTurnDecidePanel()
    {
        if (turnDecidePanel == null) yield break;

        RectTransform rect = turnDecidePanel.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;
        turnDecidePanel.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        // 잠시 대기
        yield return new WaitForSeconds(1.2f);

        // 퇴장 애니메이션
        rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 애니메이션이 완전히 끝난 후 비활성화
            turnDecidePanel.SetActive(false);
        });
    }
    #endregion

    #region 디사이드 결과값 안대 패널
    public IEnumerator EffectDecideResultPanel(int turnIndex)
    {
        GameObject targetResultPanel = (turnIndex == 0) ? decideResultPanel.GetChild(0).gameObject : decideResultPanel.GetChild(1).gameObject;

        if (targetResultPanel != null)
        {
            RectTransform rect = targetResultPanel.GetComponent<RectTransform>();

            //크기를 0으로 만들고 활성화
            rect.localScale = Vector3.zero;
            targetResultPanel.SetActive(true);

            // 등장 애니메이션
            rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

            // 잠시 대기
            yield return new WaitForSeconds(1.2f);

            // 퇴장 애니메이션
            rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                // 애니메이션이 완전히 끝난 후 비활성화
                targetResultPanel.SetActive(false);
            });
        }
        else
        {
            yield break;
        }
    }
    #endregion

    #region 조작 가이드 패널 
    public void ActiveGuidePanel()
    {
        int targetVisualMode = 0;

        if (archering_GameManager.gameDirection == Archering_GameManager.GameDirection.TurnDecide)
        {
            if (!archering_GameManager.isD_RThrowed && !archering_GameManager.isD_BThrowed)
            {
                targetVisualMode = 1; // 레드 켬
            }
            else if (archering_GameManager.isD_RThrowed && !archering_GameManager.isD_BThrowed)
            {
                targetVisualMode = 2; // 블루 켬
            }
            else if (archering_GameManager.isD_RThrowed && archering_GameManager.isD_BThrowed)
            {
                targetVisualMode = 0; // 모두 끔
            }
        }
        else if (archering_GameManager.gameDirection == Archering_GameManager.GameDirection.LastSpurt)
        {
            if (!archering_GameManager.isL_RThrowed && !archering_GameManager.isL_BThrowed)
            {
                targetVisualMode = 1; // 레드 켬
            }
            else if (archering_GameManager.isL_RThrowed && !archering_GameManager.isL_BThrowed)
            {
                targetVisualMode = 2; // 블루 켬
            }
            else if (archering_GameManager.isL_RThrowed && archering_GameManager.isL_BThrowed)
            {
                targetVisualMode = 0; // 모두 끔
            }
        }
        else
        {
            targetVisualMode = (archering_TurnManager.gameTurn == Archering_TurnManager.GameTurn.RedTurn) ? 1 : 2;
        }

        // 판별 결과 바탕으로 UI 일괄 적용
        redGuidePanel.SetActive(targetVisualMode == 1);
        redKeyGuidePanel.SetActive(targetVisualMode == 1);

        blueGuidePanel.SetActive(targetVisualMode == 2);
        blueKeyGuidePanel.SetActive(targetVisualMode == 2);
    }
    #endregion

    #region CCTV 패널
    public void VisibleCCTVPanel(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            // 레드 등장 애니메이션 
            RectTransform redRect = redCCTV.GetComponent<RectTransform>();
            if (redRect != null)
            {
                redRect.localScale = Vector3.zero;
                redCCTV.SetActive(true);
                redRect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
            }

            // 블루 퇴장 애니메이션 
            RectTransform blueRect = blueCCTV.GetComponent<RectTransform>();
            if (blueRect != null)
            {
                blueRect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    blueCCTV.SetActive(false);
                });
            }
        }
        else
        {
            // 블루 등장 애니메이션
            RectTransform blueRect = blueCCTV.GetComponent<RectTransform>();
            if (blueRect != null)
            {
                blueRect.localScale = Vector3.zero;
                blueCCTV.SetActive(true);
                blueRect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
            }

            // 레드 퇴장 애니메이션
            RectTransform redRect = redCCTV.GetComponent<RectTransform>();
            if (redRect != null)
            {
                redRect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    redCCTV.SetActive(false);
                });
            }
        }
    }
    #endregion

    #region 턴 스테이터스 패널
    public void VisibleTurnStatusPanel()
    {
        RectTransform rect = turnStatusPanel.GetComponent<RectTransform>();

        if (rect != null)
        {
            //크기를 0으로 만들고 활성화
            rect.localScale = Vector3.zero;
            turnStatusPanel.SetActive(true);

            // 등장 애니메이션
            rect.DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.OutBack);
        }

        // 상태 받아오기
        UpdateTurnStatus(0, true);
        UpdateTurnStatus(1, true);
    }

    public void UpdateTurnStatus(int playerNum, bool isInit)
    {
        Image[] targetImages = (playerNum == 0) ? redTurns : blueTurns;
        int targetCurrentTurn = (playerNum == 0) ? archering_TurnManager.currentRedTurn : archering_TurnManager.currentBlueTurn;
        Sprite targetPlaySprite = (playerNum == 0) ? redPlay : bluePlay;
        Sprite targetNotPlaySprite = (playerNum == 0) ? redNotPlay : blueNotPlay;

        if (targetImages != null)
        {
            for (int i = 0; i < targetImages.Length; i++)
            {
                if (targetImages[i] == null) continue;

                targetImages[i].transform.DOKill();

                if (i < targetCurrentTurn)
                {
                    if (targetImages[i].sprite != targetPlaySprite)
                    {
                        targetImages[i].sprite = targetPlaySprite;

                        if (!isInit)
                        {
                            targetImages[i].transform.localScale = Vector3.one;
                            targetImages[i].transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 5, 0.5f);
                        }
                        else
                        {
                            targetImages[i].transform.localScale = Vector3.one;
                        }
                    }
                }
                else
                {
                    if (targetImages[i].sprite != targetNotPlaySprite)
                    {
                        targetImages[i].sprite = targetNotPlaySprite;

                        if (!isInit)
                        {
                            targetImages[i].transform.localScale = Vector3.one;
                            targetImages[i].transform.DOShakePosition(0.2f, 8f, 15);
                        }
                        else
                        {
                            targetImages[i].transform.localScale = Vector3.one;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region UI 세팅
    public void FinalUISetting(bool isGameEnd)
    {
        redCCTV.SetActive(false);
        redKeyGuidePanel.SetActive(false);
        blueCCTV.SetActive(false);
        blueKeyGuidePanel.SetActive(false);
        turnStatusPanel.SetActive(false);

        if (isGameEnd)
        {
            StartCoroutine(VisibleCheckScorePanel(checkScorePanel));
        }
        else
        {
            StartCoroutine(SetUpLastSpurtUI());
        }
    }

    // -------------
    // 정상적으로 게임이 종료 
    // -------------
    IEnumerator VisibleCheckScorePanel(GameObject panel)
    {
        yield return null;

        if (panel != null)
        {
            RectTransform rect = panel.GetComponent<RectTransform>();

            //크기를 0으로 만들고 활성화
            rect.localScale = Vector3.zero;
            panel.SetActive(true);

            // 등장 애니메이션
            rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(2f);

            // 퇴장 애니메이션
            rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    panel.SetActive(false);
                });

            yield return new WaitForSeconds(2f);

            // 결과 패널 호출 
            VisibleResultPanel();
        }
    }

    private void VisibleResultPanel()
    {
        resultPanel.SetActive(true);
        redScore.text = archering_ScoreManager.redFinalScore.ToString();
        blueScore.text = archering_ScoreManager.blueFinalScore.ToString();

        // -------
        // 레드 승
        // ------- 
        if (archering_ScoreManager.redFinalScore > archering_ScoreManager.blueFinalScore)
        {
            // 비활성화
            redLose.SetActive(false);
            blueWin.SetActive(false);

            // 활성화
            RectTransform red = redWin.GetComponent<RectTransform>();

            if (red != null)
            {
                red.localScale = Vector3.zero;
                redWin.SetActive(true);

                // 등장 애니메이션
                red.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

                // 사운드 효과
                archering_SoundManager.PlayWhoIsWinner(0);
            }

            RectTransform blue = blueLose.GetComponent<RectTransform>();

            if (blue != null)
            {
                blue.localScale = Vector3.zero;
                blueLose.SetActive(true);

                // 등장 애니메이션
                blue.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
            }

            // 점수 추가
            Global_ScoreManger.Instance.redGetScore++;
        }
        // -------
        // 블루 승
        // -------
        else if (archering_ScoreManager.redFinalScore < archering_ScoreManager.blueFinalScore)
        {
            // 비활성화
            redWin.SetActive(false);
            blueLose.SetActive(false);

            // 활성화
            RectTransform red = redLose.GetComponent<RectTransform>();

            if (red != null)
            {
                red.localScale = Vector3.zero;
                redLose.SetActive(true);

                // 등장 애니메이션
                red.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
            }

            RectTransform blue = blueWin.GetComponent<RectTransform>();

            if (blue != null)
            {
                blue.localScale = Vector3.zero;
                blueWin.SetActive(true);

                // 등장 애니메이션
                blue.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

                // 사운드 효과
                archering_SoundManager.PlayWhoIsWinner(1);
            }

            // 점수 추가 
            Global_ScoreManger.Instance.blueGetScore++;
        }
    }
    // -------------
    // 무승부로 게임이 종료 
    // -------------
    public IEnumerator SetUpLastSpurtUI()
    {
        if (drawPanel == null) yield break;

        yield return new WaitForSeconds(2f);

        // -------------
        // 무승부 결과 안내 UI 애니메이션 
        // -------------
        RectTransform rect = drawPanel.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;

        drawPanel.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        // 잠시 대기
        yield return new WaitForSeconds(2.5f);

        // 퇴장 애니메이션
        rect.DOScale(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 애니메이션이 완전히 끝난 후 비활성화
            drawPanel.SetActive(false);

            // 라스트 스퍼트 게임 UI 활성화 
            lastSpurtPanel.SetActive(true);
        });
    }
    // -------------
    // 라스프 스터프 이후 게임 종료
    // -------------
    public IEnumerator VisibleLastSpurtResultPanel(int winnderIndex)
    {
        GameObject targetObject = (winnderIndex == 0) ? LastSpurtWinnerPanels[0] : LastSpurtWinnerPanels[1];

        if (targetObject == null) yield break;

        // RectTransform 참조 
        RectTransform rectTransform = targetObject.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            //크기를 0으로 만들고 활성화
            rectTransform.localScale = Vector3.zero;
            targetObject.SetActive(true);

            // 등장 애니메이션
            rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }

        // 효과음 재생
        archering_SoundManager.PlayWhoIsWinner(winnderIndex);
    }
    #endregion
}
