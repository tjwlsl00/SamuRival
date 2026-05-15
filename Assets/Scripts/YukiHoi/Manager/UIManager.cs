using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System.Reflection;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    // 외부
    [SerializeField] GameObject[] players;
    private Player red;
    private PlayerState redState;
    private Player blue;
    private PlayerState blueState;

    // UI
    [SerializeField] RectTransform RedReady;
    [SerializeField] RectTransform BlueReady;
    // 숨김, 표지 상태 RectTransform PosX 좌표 
    [SerializeField] float RRtargetPosX = 150f;
    [SerializeField] float RRhiddenPosX = -150f;
    [SerializeField] float BRtargetPosX = -150f;
    [SerializeField] float BRhiddenPosX = 150f;
    [SerializeField] float Animduration = 0.5f;
    // 레디, 비레디 ui위치 값 
    [SerializeField] float UnReadyOffset = 1f;
    [SerializeField] float ReadyOffset = 0.5f;

    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject DiscriptionPanel;
    [SerializeField] GameObject[] playerDiscriptionPanels;
    [SerializeField] GameObject FirstAttackPanel;
    [SerializeField] GameObject RedFirstPanel;
    [SerializeField] GameObject BlueFirstPanel;

    // 게이지 바(카운터, 체력)
    [SerializeField] GameObject GazePanels;
    [SerializeField] GameObject[] RedC_ACount;
    [SerializeField] GameObject[] BlueC_ACount;

    // 각 아이템 패널, ui
    [SerializeField] GameObject ItemPanel;
    [SerializeField] GameObject RedItemPanel;
    [SerializeField] GameObject BlueItemPanel;
    [SerializeField] Image[] RedItems;
    [SerializeField] Image[] BlueItems;

    // 한번 더 패널(공격)
    [SerializeField] GameObject R_OneMoreAttackPanel;
    [SerializeField] GameObject B_OneMoreAttackPanel;

    // 턴 체인지, 공격 찬스 패널
    [SerializeField] GameObject TurnChangePanel;
    [SerializeField] GameObject AttackChancePanel;

    // 코루틴 
    private Coroutine BlueBlinkCoroutine;
    private Coroutine RedBlinkCoroutine;

    // bool 
    private bool isMenuPanelVisibled = false;
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
        // 게이즈 패널 비활성화
        GazePanels.SetActive(false);
        // 카운터 초기화 
        UpdateCounterAttack(PlayerType.Red, 0);
        UpdateCounterAttack(PlayerType.Blue, 0);
        // 메뉴 비활성화 
        MenuPanel.gameObject.SetActive(false);
        // 설명 패널 비활성화
        DiscriptionPanel.gameObject.SetActive(false);
        // 아이템 패널 비활성화
        ItemPanel.SetActive(false);
        // 시작할 때 레드/블루 패널 모두 비활성 상태(0.5f)로 초기화
        UpdateItemPanelUI(RedItemPanel, false, 0);
        UpdateItemPanelUI(BlueItemPanel, false, 1);
        // 공격 성공 패널 비활성화
        R_OneMoreAttackPanel.SetActive(false);
        B_OneMoreAttackPanel.SetActive(false);
        // 턴체인지, 공격 찬스 패널 비활성화
        TurnChangePanel.SetActive(false);
        AttackChancePanel.SetActive(false);
    }

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
                // 게이지 패널 활성화
                GazePanels.SetActive(true);
                // 설명 패널 활성화
                DiscriptionPanel.SetActive(true);
                // 아이템 패널 활성화
                ItemPanel.SetActive(true);

                // 플레이별 설명 패널 효과(깜빡임)
                if (!TurnManager.Instance.isRedReady)
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

                if (!TurnManager.Instance.isBlueReady)
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
            // 게이지 패널 활성화
            GazePanels.SetActive(false);
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

    #region 스탯 패널 흔들림 효과(데미지)
    public void ShakeMyUI(RectTransform myUIPanel)
    {
        if (myUIPanel != null)
        {
            myUIPanel.DOKill(true);
            myUIPanel.DOShakePosition(0.5f, 20f);
        }
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
    // 패널 전체 활성화/비활성화 알파
    public void UpdateItemPanelUI(GameObject panel, bool value, int teamIndex)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup != null) canvasGroup.alpha = value ? 1f : 0.5f;

        if (value)
        {
            ApplyUsedItemAlpha(teamIndex);
        }
    }

    // 이미 사용된 아이템 확인 후 알파값 조절 
    private void ApplyUsedItemAlpha(int teamIndex)
    {
        if (teamIndex == 0)
        {
            if (ItemManager.Instance.RedUseItem1) SetAlpha(RedItems[0], 0.5f);
            if (ItemManager.Instance.RedUseItem2) SetAlpha(RedItems[1], 0.5f);
            if (ItemManager.Instance.RedUseItem3) SetAlpha(RedItems[2], 0.5f);
        }
        else
        {
            if (ItemManager.Instance.BlueUseItem1) SetAlpha(BlueItems[0], 0.5f);
            if (ItemManager.Instance.BlueUseItem2) SetAlpha(BlueItems[1], 0.5f);
            if (ItemManager.Instance.BlueUseItem3) SetAlpha(BlueItems[2], 0.5f);
        }
    }

    private void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    // 아이템 처리 효과(사용 직후 알파값 조절)
    public void UpdateRedItemImage(int num)
    {
        if (num < 0 || num >= RedItems.Length) return;

        Color tempColor = RedItems[num].color;
        tempColor.a = 0.5f;
        RedItems[num].color = tempColor;
    }

    public void UpdateBlueItemImage(int num)
    {
        if (num < 0 || num >= BlueItems.Length) return;

        Color tempColor = BlueItems[num].color;
        tempColor.a = 0.5f;
        BlueItems[num].color = tempColor;
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

    #region 메뉴 패널 시각 효과
    // 메뉴 패널 토글(메뉴 상태에 따라 마우스 커서 활성화/비활성화)
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
