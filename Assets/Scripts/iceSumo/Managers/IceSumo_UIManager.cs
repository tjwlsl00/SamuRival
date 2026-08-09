using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;

public class IceSumo_UIManager : MonoBehaviour
{
    // 싱글톤
    public static IceSumo_UIManager Instance;

    [Header("플레이어 참조")]
    [SerializeField] GameObject[] players;

    [Header("UI 참조")]
    [SerializeField] GameObject pushPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject roundPanel;
    [SerializeField] GameObject dashDiectionPanel;
    [SerializeField] RectTransform[] chargePanels;
    [SerializeField] Image[] roundInfos;
    [SerializeField] GameObject becarefulPanel;
    [SerializeField] GameObject reduceMapGazePanel;
    [SerializeField] Slider gaugeSlider;
    [SerializeField] GameObject drawPanel;
    [SerializeField] GameObject fadePanel;
    [SerializeField] GameObject finalPanel;
    // 플레이어 
    [SerializeField] Image[] playerGaugeImages;

    [Header("차징 패널 좌표 참조")]
    private Vector2[] originalPositions;

    [Header("변수 참조")]
    // 게이지 가득 차는 시간(40초)
    [SerializeField] float duration = 50f;

    [Header("스크립트 참조")]
    private IceSumo_RoundManager iceSumo_RoundManager;
    private IceSumo_GameManager iceSumo_GameManager;
    private IceSumo_SceneManager iceSumo_SceneManager;
    private IceSumo_MapManager iceSumo_MapManager;
    private IceSumo_Player red;
    private IceSumo_Player blue;

    // bool 
    private bool isVisible = false;

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

        iceSumo_RoundManager = GetComponent<IceSumo_RoundManager>();
        iceSumo_GameManager = GetComponent<IceSumo_GameManager>();
        iceSumo_SceneManager = GetComponent<IceSumo_SceneManager>();
        iceSumo_MapManager = GetComponent<IceSumo_MapManager>();
        red = players[0].GetComponent<IceSumo_Player>();
        blue = players[1].GetComponent<IceSumo_Player>();
    }

    void Start()
    {
        InitUISetting();

        // 차징 패널 좌표 백업
        BackupChargePanelPos();

        // 맵 게이지 초기화
        gaugeSlider.value = 0f;
    }

    #region 초기 세팅
    void InitUISetting()
    {
        // 라운드 UI 업데이트 
        UpdateRoundUI();

        // 메뉴
        menuPanel.SetActive(false);

        // 라운드
        roundPanel.SetActive(false);

        // 대쉬 게이지 패널
        dashDiectionPanel.SetActive(false);

        // 게이지 패널
        reduceMapGazePanel.SetActive(false);

        // 발조심 패널
        becarefulPanel.SetActive(false);

        // 무승부 패널
        drawPanel.SetActive(false);

        // 페이드 패널 
        fadePanel.SetActive(false);

        // 파이널 패널
        finalPanel.SetActive(false);

        StartCoroutine(EffectGameStartPanel(pushPanel));
    }

    // 상대 밀어라!
    IEnumerator EffectGameStartPanel(GameObject gameObject)
    {
        if (gameObject == null) yield break;
        RectTransform rect = gameObject.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;
        gameObject.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        // 잠시 대기
        yield return new WaitForSeconds(1.2f);

        // 퇴장 애니메이션
        rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 애니메이션이 완전히 끝난 후 비활성화
            gameObject.SetActive(false);
        });
    }
    #endregion

    #region 차징 패널 UI 위치 백업
    private void BackupChargePanelPos()
    {
        originalPositions = new Vector2[chargePanels.Length];
        for (int i = 0; i < chargePanels.Length; i++)
        {
            if (chargePanels[i] != null)
            {
                originalPositions[i] = chargePanels[i].anchoredPosition;
            }
        }
    }
    #endregion

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

        if (iceSumo_GameManager.gameDirection == IceSumo_GameManager.GameDirection.Play)
        {
            // 라운드 패널
            roundPanel.SetActive(true);

            // 대쉬 게이지 패널
            dashDiectionPanel.SetActive(true);

            // 맵 게이지 패널 
            reduceMapGazePanel.SetActive(true);

            // 게이지 실시간 업데이트 
            UpdateMapReduceGaze();
            UpdatePlayerDashGauge();
        }
    }

    #region 대쉬 게이지 업데이트
    private void UpdatePlayerDashGauge()
    {
        if (red != null)
        {
            playerGaugeImages[0].fillAmount = red.currentChargeTime / red.maxChargeTime;
        }

        if (blue != null)
        {
            playerGaugeImages[1].fillAmount = blue.currentChargeTime / blue.maxChargeTime;
        }
    }
    #endregion

    #region 차징 시 패널 흔들림
    public void ShackChargePanel(int playerNum)
    {
        if (chargePanels[playerNum] == null) return;

        // 기존 연출이 있다면 종료 후 시작
        chargePanels[playerNum].DOKill(true);

        // 백업 위치로 강제 초기화 
        chargePanels[playerNum].anchoredPosition = originalPositions[playerNum];

        // 무한 진동 
        chargePanels[playerNum].DOShakeAnchorPos(0.5f, 5f, 10).SetLoops(-1, LoopType.Restart);
    }

    public void StopShackChargePanel(int playerNum)
    {
        if (chargePanels[playerNum] == null) return;

        // 정지하고 원래 위치로 강제 복귀
        chargePanels[playerNum].DOKill(true);

        // 백업 위치로 강제 초기화
        chargePanels[playerNum].anchoredPosition = originalPositions[playerNum];
    }
    #endregion

    #region 라운드 상태 업데이트
    public void UpdateRoundUI()
    {
        if (roundInfos == null) return;

        for (int i = 0; i < roundInfos.Length; i++)
        {
            if (i >= IceSumo_RoundManager.roundHistroy.Length) break;

            int winner = IceSumo_RoundManager.roundHistroy[i];

            if (winner == 1)
            {
                roundInfos[i].color = Color.red;
            }
            else if (winner == 2)
            {
                roundInfos[i].color = Color.blue;
            }
            else
            {
                roundInfos[i].color = Color.white;
            }
        }
    }

    // 무승부
    public void VisibleDrawPanel()
    {
        drawPanel.SetActive(true);
    }
    #endregion

    #region 라운드 종료 페이드 인
    public IEnumerator FadeInAfterRound(Action onComplete)
    {
        // 1초 대기
        yield return new WaitForSeconds(1f);

        if (fadePanel != null)
        {
            CanvasGroup canvasGroup = fadePanel.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                fadePanel.SetActive(true);

                yield return canvasGroup.DOFade(1f, 2f).WaitForCompletion();
            }

            onComplete?.Invoke();
        }
    }
    #endregion

    #region 발 조심 코루틴
    public IEnumerator EffectBecarefulPanel()
    {
        RectTransform rect = becarefulPanel.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;
        becarefulPanel.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        // 잠시 대기
        yield return new WaitForSeconds(1.2f);

        // 퇴장 애니메이션
        rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 애니메이션이 완전히 끝난 후 비활성화
            becarefulPanel.SetActive(false);
        });
    }
    #endregion

    #region 맵 축소 게이지 업데이트
    private void UpdateMapReduceGaze()
    {
        if (gaugeSlider.value < gaugeSlider.maxValue)
        {
            gaugeSlider.value += (gaugeSlider.maxValue / duration) * Time.deltaTime;
        }
    }
    #endregion

    #region 최종 UI 세팅(게임 종료)
    public void FinalUISetting()
    {
        if (isVisible) return;

        isVisible = true;

        // 라운드 패널 
        roundPanel.SetActive(false);

        // 맵 게이지 패널
        reduceMapGazePanel.SetActive(false);

        // 대쉬 게이지 패널
        dashDiectionPanel.SetActive(false);

        // 파이널
        RectTransform rect = finalPanel.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;
        finalPanel.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }
    #endregion
}
