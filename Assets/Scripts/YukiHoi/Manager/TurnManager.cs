using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    // 싱글톤 
    public static TurnManager Instance;

    [Header("오브젝트 참조")]
    [SerializeField] GameObject[] players;
    [SerializeField] GameObject Coin;

    [Header("플레이어 선택 옵션")]
    public int RedSelectedOption = 0;
    public int BlueSelectedOption = 0;

    [Header("변수")]
    [SerializeField] public float TurnPlayTime = 2f;

    // bool
    public bool isRedReady = false;
    public bool isBlueReady = false;
    public bool isRedStun = false;
    public bool isBlueStun = false;

    // 코루틴
    private Coroutine resetTurnCoroutine;

    [Header("스크립트 참조")]
    public Player red;
    private PlayerState redState;
    public PlayerAnimation redAnimation;
    private StunStar redStunStar;
    public PlayerAudio redAudio;
    public Player blue;
    private PlayerState blueState;
    public PlayerAnimation blueAnimation;
    private StunStar blueStunStar;
    public PlayerAudio blueAudio;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 스크립트 참조 
        red = players[0].GetComponent<Player>();
        redState = players[0].GetComponent<PlayerState>();
        redAnimation = players[0].GetComponent<PlayerAnimation>();
        redStunStar = players[0].GetComponent<StunStar>();
        redAudio = players[0].GetComponent<PlayerAudio>();

        blue = players[1].GetComponent<Player>();
        blueState = players[1].GetComponent<PlayerState>();
        blueAnimation = players[1].GetComponent<PlayerAnimation>();
        blueStunStar = players[1].GetComponent<StunStar>();
        blueAudio = players[1].GetComponent<PlayerAudio>();
    }

    void Update()
    {
        if (GameManager.Instance.currentDirection == GameManager.GameDirection.GameEnd)
        {
            if (resetTurnCoroutine != null)
            {
                StopCoroutine(resetTurnCoroutine);
                resetTurnCoroutine = null;
            }
        }
        else
        {
            bool shouldSkipDueToStun = (isRedStun && red.currentPlayerDirection == Player.PlayerDirection.Attack) || (isBlueStun && blue.currentPlayerDirection == Player.PlayerDirection.Attack);

            if (shouldSkipDueToStun)
            {
                if (resetTurnCoroutine == null)
                {
                    resetTurnCoroutine = StartCoroutine(ResetTurn(false, true));
                }
            }
        }
    }

    #region 선공 결정 
    public void SetPlayerDirection(int direction)
    {
        if (direction == 0)
        {
            Debug.Log("레드가 선공입니다");

            red.currentPlayerDirection = Player.PlayerDirection.Attack;
            blue.currentPlayerDirection = Player.PlayerDirection.Defense;
        }
        else
        {
            Debug.Log("블루가 선공입니다");

            red.currentPlayerDirection = Player.PlayerDirection.Defense;
            blue.currentPlayerDirection = Player.PlayerDirection.Attack;
        }

        // 게임 셋업 
        StartCoroutine(SetupGameToStart(direction));
    }

    IEnumerator SetupGameToStart(int direction)
    {
        yield return new WaitForSeconds(1f);

        //코인 비활성화 
        Coin.gameObject.SetActive(false);

        // 효과 불러오기 
        UIManager.Instance.VisibleFirstAttackPanel(direction);
        PlayerAudio targetAudio = (direction == 0) ? redAudio : blueAudio;
        if (targetAudio != null)
        {
            targetAudio.PlayGoodLuck();
        }

        yield return new WaitForSeconds(2f);

        // 게임 상태 업데이트 
        GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

        // 카메라 전환(게임 카메라)
        CameraManager.Instance.SwitchTurnAndCamera();
    }
    #endregion

    #region 플레이어 선택 값 참조 / 레디 체크
    public void GetPlayerOptionValue(int playerNum, int optionIndex)
    {
        if (playerNum == 0)
        {
            isRedReady = true;
            RedSelectedOption = optionIndex;
        }
        else
        {
            isBlueReady = true;
            BlueSelectedOption = optionIndex;
        }

        // 레디 상태 체크 
        CheckBothReady();

        // 효과 불러오기
        UIManager.Instance.ShowReadyUI();
        UIEffect.Instance.PlayReady();
    }

    private void CheckBothReady()
    {
        if (isRedReady && isBlueReady)
        {
            Debug.Log("양쪽 모두 레디를 완료했습니다.");

            // 턴 플레이 
            TurnPlay();
        }
    }
    #endregion

    #region 턴 플레이 / 초기화
    private void TurnPlay()
    {
        // 레드 
        if (red.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            redAnimation.PlayThrow();
            redAudio.PlayThrowClip();
        }
        else
        {
            redAnimation.PlayAvoid(RedSelectedOption);
        }

        // 블루
        if (blue.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            blueAnimation.PlayThrow();
            blueAudio.PlayThrowClip();
        }
        else
        {
            blueAnimation.PlayAvoid(BlueSelectedOption);
        }

        // 턴 초기화
        resetTurnCoroutine = StartCoroutine(ResetTurn(false, false));
    }

    public IEnumerator ResetTurn(bool switchTurn, bool stunTurn)
    {
        // 일반 턴 
        if (!switchTurn && !stunTurn)
        {
            yield return StartCoroutine(PlayNormalTurn());
        }
        // 체인지 턴 
        else if (switchTurn && !stunTurn)
        {
            yield return StartCoroutine(PlaySwitchTurn());
        }
        // 스턴 턴 
        else if (!switchTurn && stunTurn)
        {
            yield return StartCoroutine(PlayStunTurn());
        }
    }

    private IEnumerator PlayNormalTurn()
    {
        // 게임 상태 변경 
        GameManager.Instance.currentDirection = GameManager.GameDirection.Play;

        // 공격 연출 기달
        yield return new WaitForSeconds(TurnPlayTime);

        if (GameManager.Instance.currentDirection == GameManager.GameDirection.GameEnd)
        {
            resetTurnCoroutine = null;
            yield break;
        }

        // 어느 한쪽이 눈덩이를 맞은 라운드
        bool isAttackSucess = false;

        if (red.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            if (blueState.isHit && !blueState.Item3Effect)
            {
                isAttackSucess = true;
                // 공격 성공 효과
                UIManager.Instance.SuccessAttack(0);
            }
        }
        else
        {
            if (redState.isHit && !redState.Item3Effect)
            {
                isAttackSucess = true;
                // 공격 성공 효과
                UIManager.Instance.SuccessAttack(1);
            }
        }

        // 모든 플레이어 아이템 선택 상태, 기존 효과 제거 
        ItemManager.Instance.ResetSelected();
        ItemManager.Instance.DeleteItemEffect();

        if (isAttackSucess)
        {
            if (redState.isHit) yield return StartCoroutine(redState.ResetHitBool());
            if (blueState.isHit) yield return StartCoroutine(blueState.ResetHitBool());
        }
        // 어느 한쪽도 눈덩이를 안맞은 라운드
        else
        {
            Debug.Log("어느 한쪽도 안맞음.");
            // 공수 전환
            red.ChangeDirection();
            blue.ChangeDirection();

            // 카메라 전환
            CameraManager.Instance.SwitchTurnAndCamera();

            if (redState.isHit) yield return StartCoroutine(redState.ResetHitBool());
            if (blueState.isHit) yield return StartCoroutine(blueState.ResetHitBool());
        }

        // 레디 상태, 옵션 선택 값 초기화(데이터 초기화)
        isRedReady = false;
        isBlueReady = false;
        RedSelectedOption = 0;
        BlueSelectedOption = 0;

        // 게임 상태 변경
        GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

        // 화면에 출력된 ready효과 제거 
        UIManager.Instance.HideReadyUI();

        // 코루틴 변수 null로 초기화 
        resetTurnCoroutine = null;
    }

    private IEnumerator PlaySwitchTurn()
    {
        // 레디 상태, 옵션 선택 값 초기화
        isRedReady = false;
        isBlueReady = false;
        RedSelectedOption = 0;
        BlueSelectedOption = 0;

        // 화면에 출력된 ready효과 제거 
        UIManager.Instance.HideReadyUI();

        // 턴 체인지 효과음
        UIEffect.Instance.PlayTurnChange();

        // 턴 체인지 패널 UI
        UIManager.Instance.VisibleTurnChangePanel();

        // 모든 플레이어 아이템 선택 상태, 기존 효과 제거 
        ItemManager.Instance.ResetSelected();
        ItemManager.Instance.DeleteItemEffect();

        // 공수 전환
        red.ChangeDirection();
        blue.ChangeDirection();

        // 카메라 전환
        CameraManager.Instance.SwitchTurnAndCamera();

        // 게임 상태 변경
        GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

        // 코루틴 null초기화 
        resetTurnCoroutine = null;

        yield return null;
    }

    private IEnumerator PlayStunTurn()
    {
        if (isRedStun)
        {
            redAnimation.PlayStun();
            redStunStar.PlayStunEffect();
            redAudio.PlayStunClip();
        }
        else if (isBlueStun)
        {
            blueAnimation.PlayStun();
            blueStunStar.PlayStunEffect();
            blueAudio.PlayStunClip();
        }

        // 공격 찬스 패널 UI
        UIManager.Instance.VisibleAttackChancePanel();

        // 공격 찬스 효과음
        UIEffect.Instance.PlayAttackChance();

        yield return new WaitForSeconds(2f);

        // 공수 전환
        red.ChangeDirection();
        blue.ChangeDirection();

        // 카메라 전환
        CameraManager.Instance.SwitchTurnAndCamera();

        yield return new WaitForSeconds(1.5f);

        isRedStun = false;
        isBlueStun = false;

        // 게임 상태 변경
        GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

        // 코루틴 null초기화 
        resetTurnCoroutine = null;
    }
    #endregion

    #region 최종 승자 체크
    public void CheckFinalWinner()
    {
        if (redState.currentHP > 0 && blueState.currentHP <= 0)
        {
            // 카메라 전환
            StartCoroutine(CameraManager.Instance.SwitchFinalCamera(0));

            // 점수 추가 
            Global_ScoreManger.Instance.redGetScore ++;
        }
        else if (blueState.currentHP > 0 && redState.currentHP <= 0)
        {
            // 카메라 전환
            StartCoroutine(CameraManager.Instance.SwitchFinalCamera(1));

            // 점수 추가 
            Global_ScoreManger.Instance.blueGetScore ++;
        }

        // 로딩 씬으로 이동 
        StartCoroutine(GameSceneManager.Instance.MoveToScoreScene());
    }
    #endregion
}