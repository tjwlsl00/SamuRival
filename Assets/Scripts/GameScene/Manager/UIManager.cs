using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    // 외부
    [SerializeField] GameObject[] players;
    private Player red;
    private Player blue;

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
    [SerializeField] GameObject FirstAttackPanel;
    [SerializeField] GameObject RedFirstPanel;
    [SerializeField] GameObject BlueFirstPanel;

    // 게이지 바(카운터, 체력)
    [SerializeField] GameObject GazePanels;
    [SerializeField] GameObject[] RedC_ACount;
    [SerializeField] GameObject[] BlueC_ACount;

    // 각 아이템 패널, ui
    [SerializeField] GameObject ItemPanel;
    [SerializeField] Image[] RedItems;
    [SerializeField] Image[] BlueItems;

    // 승리 패널 
    [SerializeField] GameObject VictroyPanelParent;
    [SerializeField] GameObject[] VictroyPanels;

    // bool 
    private bool isMenuPanelVisibled = false;
    private bool isAvoidPanelVisibled = false;
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
        // 승리 패널 비활성화
        VictroyPanelParent.SetActive(false);
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
            }
        }
        else
        {
            StartCoroutine(VisiblePanelAfterGameEnd());
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

    #region 스노우 카운트 패널 업데이트(역공)
    public void UpdateCounterAttack(PlayerType playerType, int count)
    {
        // 코드 중복 방지
        GameObject[] targetArray = (playerType == PlayerType.Red) ? RedC_ACount : BlueC_ACount;

        for (int i = 0; i < targetArray.Length; i++)
        {
            bool shouldBeActive = i < count;
            GameObject obj = targetArray[i].gameObject;

            if(shouldBeActive && !obj.activeSelf)
            {
                obj.SetActive(true);
                obj.transform.localScale = Vector3.zero;

                obj.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            }
            else if(!shouldBeActive && obj.activeSelf)
            {
                obj.SetActive(false);
                obj.transform.localScale = Vector3.one;
            }
        }
    }
    #endregion

    #region 아이템 패널 시각 효과
    // 패널 전체 활성화/비활성화 알파
    public void UpdateItemPanelUI(GameObject panel, bool value)
    {
        // 전달받은 패널 이미지 컴포넌트 참조
        Image image = panel.GetComponent<Image>();
        if (image == null) return;

        Color tempColor = image.color;

        if (value)
        {
            tempColor.a = 1f;
        }
        else
        {
            tempColor.a = 0.5f;
        }

        image.color = tempColor;
    }

    // 아이템 처리 효과
    public void UpdateRedItemImage(int num)
    {
        if (num == 1)
        {
            Color tempColor = RedItems[0].color;
            tempColor.a = 0.5f;
            RedItems[0].color = tempColor;
        }
        else if (num == 2)
        {
            Color tempColor = RedItems[1].color;
            tempColor.a = 0.5f;
            RedItems[1].color = tempColor;
        }
        else
        {
            Color tempColor = RedItems[2].color;
            tempColor.a = 0.5f;
            RedItems[2].color = tempColor;
        }
    }

    public void UpdateBlueItemImage(int num)
    {
        if (num == 1)
        {
            Color tempColor = BlueItems[0].color;
            tempColor.a = 0.5f;
            BlueItems[0].color = tempColor;
        }
        else if (num == 2)
        {
            Color tempColor = BlueItems[1].color;
            tempColor.a = 0.5f;
            BlueItems[1].color = tempColor;
        }
        else
        {
            Color tempColor = BlueItems[2].color;
            tempColor.a = 0.5f;
            BlueItems[2].color = tempColor;
        }
    }
    #endregion

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

    // 게임 종료 후 메뉴 패널 활성화
    IEnumerator VisiblePanelAfterGameEnd()
    {
        if (isMenuPanelVisibled) yield break;
        yield return new WaitForSeconds(2.5f);
        isMenuPanelVisibled = true;
        MenuPanel.gameObject.SetActive(true);
        MouseEvent.Instance.ShowCursor();
    }

    // 게임 종료 후 승리 패널 활성화
    public async void VisibleVictroyPanel(int num)
    {
        await Task.Delay(2500);

        VictroyPanelParent.SetActive(true);

        if (num == 0)
        {
            VictroyPanels[0].SetActive(true);
            VictroyPanels[1].SetActive(false);
        }
        else
        {
            VictroyPanels[0].SetActive(false);
            VictroyPanels[1].SetActive(true);
        }
    }

}
