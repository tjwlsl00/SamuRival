using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 
    public static ItemManager Instance;

    [Header("오브젝트 참조")]
    [SerializeField] GameObject[] players;
    [SerializeField] GameObject[] PlayerItemPanels;

    // bool
    public bool isRedItemGoActive = false;
    public bool isBlueItemGoActive = false;
    bool RedSelected = false;
    bool BlueSelected = false;
    public bool RedUseItem1 = false;
    public bool RedUseItem2 = false;
    public bool RedUseItem3 = false;
    public bool RedUseItem4 = false;
    public bool BlueUseItem1 = false;
    public bool BlueUseItem2 = false;
    public bool BlueUseItem3 = false;
    public bool BlueUseItem4 = false;

    // 스크립트 참조 
    private Player red;
    private Player blue;
    private PlayerState redState;
    private SnowballSocket redSnowballSocket;
    private SnowManHead redSMHead;
    private PlayerAudio redAudio;
    private PlayerState blueState;
    private SnowballSocket blueSnowballSocket;
    private SnowManHead blueSMHead;
    private PlayerAudio blueAudio;

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
        redState = players[0].GetComponent<PlayerState>();
        redSnowballSocket = players[0].GetComponent<SnowballSocket>();
        redSMHead = players[0].GetComponent<SnowManHead>();
        redAudio = players[0].GetComponent<PlayerAudio>();
        blueState = players[1].GetComponent<PlayerState>();
        blueSnowballSocket = players[1].
        GetComponent<SnowballSocket>();
        blueSMHead = players[1].GetComponent<SnowManHead>();
        blueAudio = players[1].GetComponent<PlayerAudio>();
    }

    void Update()
    {
        if (GameManager.Instance.currentDirection == GameManager.GameDirection.Ready)
        {
            ToggleItemSocket();
            CheckActive();
        }
    }

    #region 아이템 소켓 토글
    void ToggleItemSocket()
    {
        if (!TurnManager.Instance.isRedReady)
        {
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                isRedItemGoActive = !isRedItemGoActive;
                Debug.Log(isRedItemGoActive ? "레드 아이템 창 활성화" : "레드 아이템 창 비활성화");

                // 아이템 패널 활성화 
                UIManager.Instance.UpdateStateItemPanel(0, isRedItemGoActive);
                UIManager.Instance.ScaleAnimItemPanel(PlayerItemPanels[0], isRedItemGoActive);

                // 공/방 아이템 활/비활성화
                Transform attack = PlayerItemPanels[0].transform.Find("Attack");
                Transform defense = PlayerItemPanels[0].transform.Find("Defense");

                if (attack != null && defense != null)
                {
                    if (red.currentPlayerDirection == Player.PlayerDirection.Attack)
                    {
                        CanvasGroup attackCanvasGroup = attack.GetComponent<CanvasGroup>();

                        CanvasGroup defenseCanvasGroup = defense.GetComponent<CanvasGroup>();

                        if (attackCanvasGroup != null && defenseCanvasGroup != null)
                        {
                            attackCanvasGroup.alpha = 1f;
                            defenseCanvasGroup.alpha = 0.5f;
                        }
                    }
                    else
                    {
                        CanvasGroup attackCanvasGroup = attack.GetComponent<CanvasGroup>();

                        CanvasGroup defenseCanvasGroup = defense.GetComponent<CanvasGroup>();

                        if (attackCanvasGroup != null && defenseCanvasGroup != null)
                        {
                            attackCanvasGroup.alpha = 0.5f;
                            defenseCanvasGroup.alpha = 1f;
                        }
                    }
                }

                // 활성화 효과음
                UIEffect.Instance.PlayItemActive();
            }
        }

        if (!TurnManager.Instance.isBlueReady)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isBlueItemGoActive = !isBlueItemGoActive;
                Debug.Log(isBlueItemGoActive ? "블루 아이템 창 활성화" : "블루 아이템 창 비활성화");

                // 아이템 패널 활성화
                UIManager.Instance.UpdateStateItemPanel(1, isBlueItemGoActive);
                UIManager.Instance.ScaleAnimItemPanel(PlayerItemPanels[1], isBlueItemGoActive);

                // 공/방 아이템 활/비활성화
                Transform attack = PlayerItemPanels[1].transform.Find("Attack");
                Transform defense = PlayerItemPanels[1].transform.Find("Defense");

                if (attack != null && defense != null)
                {
                    if (blue.currentPlayerDirection == Player.PlayerDirection.Attack)
                    {
                        CanvasGroup attackCanvasGroup = attack.GetComponent<CanvasGroup>();

                        CanvasGroup defenseCanvasGroup = defense.GetComponent<CanvasGroup>();

                        if (attackCanvasGroup != null && defenseCanvasGroup != null)
                        {
                            attackCanvasGroup.alpha = 1f;
                            defenseCanvasGroup.alpha = 0.5f;
                        }
                    }
                    else
                    {
                        CanvasGroup attackCanvasGroup = attack.GetComponent<CanvasGroup>();

                        CanvasGroup defenseCanvasGroup = defense.GetComponent<CanvasGroup>();

                        if (attackCanvasGroup != null && defenseCanvasGroup != null)
                        {
                            attackCanvasGroup.alpha = 0.5f;
                            defenseCanvasGroup.alpha = 1f;
                        }
                    }
                }

                // 활성화 효과음
                UIEffect.Instance.PlayItemActive();
            }
        }
    }

    // 아이템 선택, 방향 동시 선택 방지(1프레임 대기)
    private System.Collections.IEnumerator ClosePanelAfterFrame(int playerIndex)
    {
        yield return null;

        if (playerIndex == 0)
        {
            isRedItemGoActive = false;
            UIManager.Instance.ScaleAnimItemPanel(PlayerItemPanels[0], isRedItemGoActive);
        }
        else
        {
            isBlueItemGoActive = false;
            UIManager.Instance.ScaleAnimItemPanel(PlayerItemPanels[1], isBlueItemGoActive);
        }
    }
    #endregion

    #region 활성화 체크 / 아이템 사용 
    void CheckActive()
    {
        if (isRedItemGoActive)
        {
            CanUseItem(0);
        }
        else if (isBlueItemGoActive)
        {
            CanUseItem(1);
        }
    }

    void CanUseItem(int playerNum)
    {
        if (playerNum == 0)
        {
            Player red = players[0].GetComponent<Player>();

            if (red != null)
            {
                if (!RedSelected)
                {
                    if (red.currentPlayerDirection == Player.PlayerDirection.Attack)
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow) && !RedUseItem1)
                        {
                            // 아이템 복수 선택 방지 
                            RedSelected = true;

                            // 아이템 선택, 효과 
                            RedUseItem1 = true;
                            Debug.Log("레드 아이템 1");
                            redState.Item1Effect = true;

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateRedItemImage(0);

                            // 파티클(소켓)
                            redSnowballSocket.ToggleSocketParticle();

                            // 아이템 창 비활성화
                            CloseItemPanel(0);

                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !RedUseItem2 && TurnManager.Instance.isBlueReady)
                        {
                            // 아이템 복수 선택 방지 
                            RedSelected = true;

                            // 아이템 선택, 효과 
                            RedUseItem2 = true;
                            Debug.Log("레드 아이템 2");
                            redState.Item2Effect = true;

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateRedItemImage(1);

                            // 아이템 창 비활성화
                            CloseItemPanel(0);

                            // 힌트 패널 활성화
                            UIManager.Instance.VisibleHintPanel(0, TurnManager.Instance.BlueSelectedOption);
                        }
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow) && !RedUseItem3)
                        {
                            // 아이템 복수 선택 방지 
                            RedSelected = true;

                            // 아이템 선택, 효과 
                            RedUseItem3 = true;
                            Debug.Log("레드 아이템 2");
                            redState.Item3Effect = true;

                            // 스노우맨 머리 활성화
                            redSMHead.ToggleSnowHead();

                            // 장착 효과음
                            redAudio.PlayWearSnowHead();

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateRedItemImage(2);

                            // 아이템 창 비활성화
                            CloseItemPanel(0);

                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow) && !RedUseItem4)
                        {
                            // 아이템 복수 선택 방지 
                            RedSelected = true;

                            // 아이템 선택, 효과 
                            RedUseItem4 = true;
                            Debug.Log("레드 아이템 3");

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateRedItemImage(3);

                            // 턴 리셋
                            StartCoroutine(TurnManager.Instance.ResetTurn(true, false));

                            // 아이템 창 비활성화
                            CloseItemPanel(0);
                        }
                    }
                }
                else
                {
                    Debug.Log("레드-이미 아이템 선택함.");
                }
            }

        }
        else
        {
            Player blue = players[1].GetComponent<Player>();

            if (blue != null)
            {
                if (!BlueSelected)
                {
                    if (blue.currentPlayerDirection == Player.PlayerDirection.Attack)
                    {
                        if (Input.GetKeyDown(KeyCode.W) && !BlueUseItem1)
                        {
                            // 아이템 복수 선택 방지 
                            BlueSelected = true;

                            // 아이템 선택, 효과 
                            BlueUseItem1 = true;
                            Debug.Log("블루 아이템 1");
                            blueState.Item1Effect = true;

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateBlueItemImage(0);

                            // 파티클(소켓)
                            blueSnowballSocket.ToggleSocketParticle();

                            // 아이템 창 비활성화
                            CloseItemPanel(1);

                        }
                        else if (Input.GetKeyDown(KeyCode.A) && !BlueUseItem2 && TurnManager.Instance.isRedReady)
                        {
                            // 아이템 복수 선택 방지 
                            BlueSelected = true;

                            // 아이템 선택, 효과 
                            BlueUseItem2 = true;
                            Debug.Log("레드 아이템 2");
                            blueState.Item2Effect = true;

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateBlueItemImage(1);

                            // 아이템 창 비활성화
                            CloseItemPanel(1);

                            // 힌트 패널 활성화
                            UIManager.Instance.VisibleHintPanel(1, TurnManager.Instance.RedSelectedOption);
                        }
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.S) && !BlueUseItem3)
                        {
                            // 아이템 복수 선택 방지 
                            BlueSelected = true;

                            // 아이템 선택, 효과 
                            BlueUseItem3 = true;
                            Debug.Log("블루 아이템 2");
                            blueState.Item3Effect = true;

                            // 스노우맨 머리 활성화
                            blueSMHead.ToggleSnowHead();

                            // 장착 효과음
                            blueAudio.PlayWearSnowHead();

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateBlueItemImage(2);

                            // 아이템 창 비활성화
                            CloseItemPanel(1);

                        }
                        else if (Input.GetKeyDown(KeyCode.D) && !BlueUseItem4)
                        {
                            // 아이템 복수 선택 방지 
                            BlueSelected = true;

                            // 아이템 선택, 효과 
                            BlueUseItem4 = true;
                            Debug.Log("블루 아이템 3");

                            // 아이템 ui 비활성화
                            UIManager.Instance.UpdateBlueItemImage(3);

                            // 턴 리셋
                            StartCoroutine(TurnManager.Instance.ResetTurn(true, false));

                            // 아이템 창 비활성화
                            CloseItemPanel(1);
                        }
                    }
                }
                else
                {
                    Debug.Log("블루-이미 아이템 선택함");
                }
            }
        }
    }

    void CloseItemPanel(int playerIndex)
    {
        StartCoroutine(ClosePanelAfterFrame(playerIndex));
    }
    #endregion

    #region 아이템 선택 상태 리셋
    public void ResetSelected()
    {
        RedSelected = false;
        BlueSelected = false;
    }
    #endregion

    #region 아이템 효과 제거 
    public void DeleteItemEffect()
    {
        Debug.Log("모든 아이템 효과 제거");

        // 레드 플레이어 
        if (redState.Item1Effect)
        {
            redSnowballSocket.ToggleSocketParticle();
        }
        else if (redState.Item2Effect)
        {
            UIManager.Instance.UnvisibleHintPanel(0);
        }
        else if (redState.Item3Effect)
        {
            // 스노우맨 머리 비활성화
            redSMHead.ToggleSnowHead();

            // 장착 효과음
            redAudio.PlayWearSnowHead();
        }

        // 블루 플레이어 
        if (blueState.Item1Effect)
        {
            blueSnowballSocket.ToggleSocketParticle();
        }
        else if (blueState.Item2Effect)
        {
            UIManager.Instance.UnvisibleHintPanel(1);
        }
        else if (blueState.Item3Effect)
        {
            // 스노우맨 머리 비활성화
            blueSMHead.ToggleSnowHead();

            // 장착 효과음
            blueAudio.PlayWearSnowHead();
        }

        redState.Item1Effect = false;
        redState.Item3Effect = false;

        blueState.Item1Effect = false;
        blueState.Item3Effect = false;
    }
    #endregion
}