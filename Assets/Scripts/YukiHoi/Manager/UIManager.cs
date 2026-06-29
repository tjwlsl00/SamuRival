using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    // 싱글톤 
    public static UIManager Instance;

    [Header("플레이어 연결")]
    [SerializeField] GameObject[] players;
    private Player red;
    private PlayerState redState;
    private Player blue;
    private PlayerState blueState;

    [Header("레디 패널 좌표 수치")]
    [SerializeField] float RRtargetPosX = 150f;
    [SerializeField] float RRhiddenPosX = -150f;
    [SerializeField] float BRtargetPosX = -150f;
    [SerializeField] float BRhiddenPosX = 150f;
    [SerializeField] float Animduration = 0.5f;

    [Header("UI")]
    // 레디 패널 
    [SerializeField] RectTransform RedReady;
    [SerializeField] RectTransform BlueReady;
    // 선공, 키보드 입력
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject DiscriptionPanel;
    [SerializeField] GameObject[] playerDiscriptionPanels;
    [SerializeField] GameObject FirstAttackPanel;
    [SerializeField] GameObject RedFirstPanel;
    [SerializeField] GameObject BlueFirstPanel;
    // 캐릭터 체력
    [SerializeField] GameObject RedHealthBar;
    [SerializeField] GameObject BlueHealthBar;
    // 카운터 게이지 
    [SerializeField] GameObject CounterPanel;
    [SerializeField] GameObject[] C_APanels;
    [SerializeField] GameObject[] RedC_ACount;
    [SerializeField] GameObject[] BlueC_ACount;
    // 각 아이템 패널, ui
    [SerializeField] GameObject ItemPanel;
    [SerializeField] GameObject[] RedActiveStateItemPanel;
    [SerializeField] GameObject[] BlueActiveStateItemPanel;
    [SerializeField] GameObject RedItemPanel;
    [SerializeField] GameObject BlueItemPanel;
    [SerializeField] Image[] RedItems;
    [SerializeField] Image[] BlueItems;
    // 한번 더 패널(공격)
    [SerializeField] GameObject R_OneMoreAttackPanel;
    [SerializeField] GameObject B_OneMoreAttackPanel;
    // 턴 체인지, 공격 찬스 패널
    [SerializeField] GameObject TurnChangePanel;
    [SerializeField] GameObject[] RedHintPanels;
    [SerializeField] GameObject[] BlueHintPanels;
    [SerializeField] GameObject AttackChancePanel;

    // 코루틴 
    private Coroutine BlueBlinkCoroutine;
    private Coroutine RedBlinkCoroutine;

    // bool 
    private bool isRedArrowPanelBlink = false;
    private bool isBlueArrowPanelBlink = false;
    private bool isRedUIShown = false;
    private bool isBlueUIShown = false;

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

        red = players[0].GetComponent<Player>();
        blue = players[1].GetComponent<Player>();
    }

    void Start()
    {
        // 게임 UI 비활성화 상태로 시작 
        UnvisibleUI();
    }

    #region 시작 UI 비활성화
    private void UnvisibleUI()
    {
        // 캐릭터 체력바 패널 비활성화 
        RedHealthBar.SetActive(false);
        BlueHealthBar.SetActive(false);
        // 카운터 패널 비활성화
        CounterPanel.SetActive(false);
        // 카운터 초기화 
        UpdateCounterAttack(PlayerType.Red, 0);
        UpdateCounterAttack(PlayerType.Blue, 0);
        // 메뉴 비활성화 
        MenuPanel.gameObject.SetActive(false);
        // 설명 패널 비활성화
        DiscriptionPanel.gameObject.SetActive(false);
        // 힌트 패널 비활성화 
        for (int i = 0; i < RedHintPanels.Length; i++)
        {
            RedHintPanels[i].SetActive(false);
        }
        for (int i = 0; i < BlueHintPanels.Length; i++)
        {
            BlueHintPanels[i].SetActive(false);
        }
        // 아이템 패널 비활성화
        ItemPanel.SetActive(false);
        UpdateStateItemPanel(0, false);
        UpdateStateItemPanel(1, false);
        // 시작할 때 레드/블루 패널 모두 비활성 상태(0.5f)로 초기화
        ScaleAnimItemPanel(RedItemPanel, false);
        ScaleAnimItemPanel(BlueItemPanel, false);
        // 공격 성공 패널 비활성화
        R_OneMoreAttackPanel.SetActive(false);
        B_OneMoreAttackPanel.SetActive(false);
        // 턴체인지, 공격 찬스 패널 비활성화
        TurnChangePanel.SetActive(false);
        AttackChancePanel.SetActive(false);
    }
    #endregion

    void Update()
    {
        if (GameManager.Instance.currentDirection != GameManager.GameDirection.GameEnd)
        {
            // 메뉴 토글 가능 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenuPanel();
            }

            // 코인(선공결정) 이후 아이템 패널 활성화
            if (GameManager.Instance.currentDirection != GameManager.GameDirection.Coin)
            {
                // 캐릭터 체력바 패널 활성화 
                RedHealthBar.SetActive(true);
                BlueHealthBar.SetActive(true);

                // 카운터 패널 활성화
                CounterPanel.SetActive(true);

                // 경우에 따른 카운터 패널 배치(레드: 공격일때 레드 카운터 패널 그외 블루 카운터 패널 배치)
                if (CounterPanel.activeSelf && red.currentPlayerDirection == Player.PlayerDirection.Attack)
                {
                    C_APanels[0].SetActive(true);
                    C_APanels[1].SetActive(false);
                }
                else
                {
                    C_APanels[0].SetActive(false);
                    C_APanels[1].SetActive(true);
                }

                // 설명 패널 활성화
                DiscriptionPanel.SetActive(true);
                // 아이템 패널 활성화
                ItemPanel.SetActive(true);

                // 플레이별 설명 패널 효과(깜빡임)
                if (!TurnManager.Instance.isRedReady && !ItemManager.Instance.isRedItemGoActive)
                {
                    if (!isRedArrowPanelBlink)
                    {
                        isRedArrowPanelBlink = true;
                        RedBlinkCoroutine = StartCoroutine(Blink(playerDiscriptionPanels[0], 0));
                    }
                }
                else
                {
                    isRedArrowPanelBlink = false;
                    if (RedBlinkCoroutine != null)
                    {
                        StopCoroutine(RedBlinkCoroutine);
                        RedBlinkCoroutine = null;
                    }
                    playerDiscriptionPanels[0].SetActive(false);
                }

                if (!TurnManager.Instance.isBlueReady && !ItemManager.Instance.isBlueItemGoActive)
                {
                    if (!isBlueArrowPanelBlink)
                    {
                        isBlueArrowPanelBlink = true;
                        BlueBlinkCoroutine = StartCoroutine(Blink(playerDiscriptionPanels[1], 1));
                    }
                }
                else
                {
                    isBlueArrowPanelBlink = false;
                    if (BlueBlinkCoroutine != null)
                    {
                        StopCoroutine(BlueBlinkCoroutine);
                        BlueBlinkCoroutine = null;
                    }
                    playerDiscriptionPanels[1].SetActive(false);
                }
            }
        }
        else
        {
            // 캐릭터 체력바 패널 비활성화 
            RedHealthBar.SetActive(false);
            BlueHealthBar.SetActive(false);
            // 게이지 패널 비활성화
            CounterPanel.SetActive(false);
            // 설명 패널 비활성화
            DiscriptionPanel.SetActive(false);
            // 아이템 패널 비활성화
            ItemPanel.SetActive(false);
        }
    }

    #region 선공 패널 UI 페이드 효과
    public void VisibleFirstAttackPanel(int Direction)
    {
        if (FirstAttackPanel == null || RedFirstPanel == null || BlueFirstPanel == null) return;

        if (Direction == 0)
        {
            RedFirstPanel.gameObject.SetActive(true);
            StartCoroutine(UnvisibleFirstAttackPanel(RedFirstPanel));
        }
        else
        {
            BlueFirstPanel.gameObject.SetActive(true);
            StartCoroutine(UnvisibleFirstAttackPanel(BlueFirstPanel));
        }
    }

    IEnumerator UnvisibleFirstAttackPanel(GameObject panel)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        FirstAttackPanel.gameObject.SetActive(false);
    }
    #endregion

    #region 방향키 선택 시각적 효과(블링크)
    IEnumerator Blink(GameObject panel, int type)
    {
        while (true)
        {
            panel.SetActive(false);
            yield return new WaitForSeconds(1f);
            panel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region 레디 패널 효과
    public void ShowReadyUI()
    {
        if (TurnManager.Instance.isRedReady && !isRedUIShown)
        {
            isRedUIShown = true;
            RedReady.anchoredPosition = new Vector3(RRhiddenPosX, RedReady.anchoredPosition.y);
            RedReady.DOAnchorPosX(RRtargetPosX, Animduration).SetEase(Ease.OutBack);
        }

        if (TurnManager.Instance.isBlueReady && !isBlueUIShown)
        {
            isBlueUIShown = true;
            BlueReady.anchoredPosition = new Vector3(BRhiddenPosX, BlueReady.anchoredPosition.y);
            BlueReady.DOAnchorPosX(BRtargetPosX, Animduration).SetEase(Ease.OutBack);
        }
    }

    public void HideReadyUI()
    {
        RedReady.DOAnchorPosX(RRhiddenPosX, Animduration).SetEase(Ease.InBack);
        BlueReady.DOAnchorPosX(BRhiddenPosX, Animduration).SetEase(Ease.InBack);

        isRedUIShown = false;
        isBlueUIShown = false;
    }
    #endregion

    #region 공격 성공 효과
    public void SuccessAttack(int SuccessPlayerNum)
    {
        GameObject targetPanel = (SuccessPlayerNum == 0) ? R_OneMoreAttackPanel : B_OneMoreAttackPanel;

        Player targetPlayer = (SuccessPlayerNum == 0) ? red : blue;

        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        targetPanel.SetActive(true);
        canvasGroup.alpha = 1f;
        targetPanel.transform.DOKill();
        targetPanel.transform.localScale = Vector3.one * 0.5f;

        PlayerAudio playerAudio = targetPlayer.GetComponent<PlayerAudio>();
        playerAudio?.PlaySuccessAttack();

        Sequence turnSequence = DOTween.Sequence();
        turnSequence
            // 더 빠르게 팝업 (0.3s -> 0.15s)
            .Append(targetPanel.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack))
            // 머무르는 시간 단축 (0.5s -> 0.2s)
            .AppendInterval(0.2f)
            // 사라지는 속도 조절
            .Append(targetPanel.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack))
            .Join(canvasGroup.DOFade(0f, 0.3f))
            .OnComplete(() =>
            {
                targetPanel.SetActive(false);
                targetPanel.transform.localScale = Vector3.one;
            });

    }
    #endregion

    #region 스노우 카운트 패널 업데이트(역공)
    public void UpdateCounterAttack(PlayerType playerType, int count)
    {
        // 코드 중복 방지
        GameObject[] targetArray = (playerType == PlayerType.Red) ? RedC_ACount : BlueC_ACount;

        for (int i = 0; i < targetArray.Length; i++)
        {
            bool shouldBeActive = i < count;
            GameObject obj = targetArray[i].gameObject;

            if (shouldBeActive && !obj.activeSelf)
            {
                obj.SetActive(true);
                obj.transform.localScale = Vector3.zero;

                obj.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

                // 눈덩이 추가 효과음
                UIEffect.Instance.PlaySnowball();
            }
            else if (!shouldBeActive && obj.activeSelf)
            {
                obj.SetActive(false);
                obj.transform.localScale = Vector3.one;
            }
        }
    }
    #endregion

    #region 아이템 패널 시각 효과
    // 아이템 패널 활성화 상태 업데이트
    public void UpdateStateItemPanel(int PlayerNum, bool active)
    {
        GameObject[] targetStatusPanels = (PlayerNum == 0) ? RedActiveStateItemPanel : BlueActiveStateItemPanel;

        if (targetStatusPanels != null && targetStatusPanels.Length >= 2)
        {
            int showIndex = active ? 1 : 0;
            int hideIndex = active ? 0 : 1;

            targetStatusPanels[hideIndex].SetActive(false);

            targetStatusPanels[showIndex].SetActive(true);
            targetStatusPanels[showIndex].transform.localScale = Vector3.zero; // 0에서 시작

            // 애니메이션
            targetStatusPanels[showIndex].transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        }
    }

    // 스케일 애니메이션 
    public void ScaleAnimItemPanel(GameObject panel, bool active)
    {
        // 원래 크기 
        Vector3 originalScale = Vector3.one;
        Transform panelTransform = panel.transform;

        if (active)
        {
            panelTransform.DOKill();
            panel.SetActive(true);

            panelTransform.localScale = Vector3.zero;
            panelTransform.DOScale(originalScale, 0.2f)
                          .SetEase(Ease.OutBack);
        }
        else
        {
            panelTransform.DOKill();
            panelTransform.DOScale(Vector3.zero, 0.2f)
                          .SetEase(Ease.InQuad)
                          .OnComplete(() =>
                          {
                              panel.SetActive(false);
                          });
        }
    }

    // 아이템 처리 효과(사용 직후 알파값 조절)
    public void UpdateRedItemImage(int num)
    {
        if (num < 0 || num >= RedItems.Length) return;

        // 현재 이미지의 원래 색상
        Color originalColor = RedItems[num].color;

        RedItems[num].color = new Color(
            originalColor.r * 0.5f,
            originalColor.g * 0.5f,
            originalColor.b * 0.5f,
            0.8f
        );
    }

    public void UpdateBlueItemImage(int num)
    {
        if (num < 0 || num >= BlueItems.Length) return;

        // 현재 이미지의 원래 색상
        Color originalColor = RedItems[num].color;

        BlueItems[num].color = new Color(
            originalColor.r * 0.5f,
            originalColor.g * 0.5f,
            originalColor.b * 0.5f,
            0.8f);
    }
    #endregion

    #region 힌트 패널 시각효과 
    public void VisibleHintPanel(int PlayerNum, int SelectIndex)
    {
        GameObject[] targetPanels = (PlayerNum == 0) ? BlueHintPanels : RedHintPanels;

        if (SelectIndex >= 0 && SelectIndex < targetPanels.Length)
        {
            GameObject activePanel = targetPanels[SelectIndex];

            // 크기 애니메이션
            activePanel.transform.DOKill();
            activePanel.SetActive(true);
            activePanel.transform.localScale = Vector3.zero;
            activePanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void UnvisibleHintPanel(int PlayerNum)
    {
        GameObject[] targetPanels = (PlayerNum == 0) ? BlueHintPanels : RedHintPanels;

        for (int i = 0; i < targetPanels.Length; i++)
        {
            if (targetPanels[i] != null)
            {
                targetPanels[i].SetActive(false);
            }
        }
    }
    #endregion

    #region 턴 체인지, 공격 찬스 패널 효과(페이드, 통통 효과)
    public void VisibleTurnChangePanel()
    {
        CanvasGroup canvasGroup = TurnChangePanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            TurnChangePanel.SetActive(true);
            canvasGroup.alpha = 1f;

            TurnChangePanel.transform.DOKill();
            TurnChangePanel.transform.localScale = Vector3.one * 0.8f;

            Sequence turnSequence = DOTween.Sequence();
            turnSequence.Append(TurnChangePanel.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack)).AppendInterval(0.5f).Append(TurnChangePanel.transform.DOScale(0.5f, 0.5f).SetEase(Ease.InBack)).Join(canvasGroup.DOFade(0f, 0.5f)).OnComplete(() =>
            {
                TurnChangePanel.SetActive(false);
                TurnChangePanel.transform.localScale = Vector3.one;
            });
        }
    }

    public void VisibleAttackChancePanel()
    {
        CanvasGroup canvasGroup = AttackChancePanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            AttackChancePanel.SetActive(true);
            canvasGroup.alpha = 1f;

            AttackChancePanel.transform.DOKill();
            AttackChancePanel.transform.localScale = Vector3.one * 0.8f;

            Sequence turnSequence = DOTween.Sequence();
            turnSequence.Append(AttackChancePanel.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack)).AppendInterval(0.5f).Append(AttackChancePanel.transform.DOScale(0.5f, 0.5f).SetEase(Ease.InBack)).Join(canvasGroup.DOFade(0f, 0.5f)).OnComplete(() =>
            {
                AttackChancePanel.SetActive(false);
                AttackChancePanel.transform.localScale = Vector3.one;
            });
        }
    }
    #endregion

    #region 메뉴 패널 토글
    void ToggleMenuPanel()
    {
        if (MenuPanel != null)
        {
            bool isActive = !MenuPanel.activeSelf;
            MenuPanel.SetActive(isActive);

            if (isActive)
            {
                MouseEvent.Instance.ShowCursor();
            }
            else
            {
                MouseEvent.Instance.HideCursor();
            }
        }
    }
    #endregion
}