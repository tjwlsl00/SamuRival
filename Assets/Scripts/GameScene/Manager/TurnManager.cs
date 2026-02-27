using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private int RedSelectedOption = 0;
    private int BlueSelectedOption = 0;
    public bool isRedReady = false;
    public bool isBlueReady = false;
    public bool isRedStun = false;
    public bool isBlueStun = false;
    private bool isStunTurn = false;
    private bool SwitchTurn = false;
    [SerializeField] public float TurnPlayTime = 3f;
    private Coroutine resetTurnCoroutine;

    // 외부
    [SerializeField] GameObject[] players;
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
    // 코인
    [SerializeField] GameObject Coin;

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

        // 외부 (플레이어)
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
                    isStunTurn = true;
                    resetTurnCoroutine = StartCoroutine(ResetTurn(false, true));
                }
            }
        }
    }

    #region 게임 셋업
    public void SetPlayerDirection(int Direction)
    {
        if (Direction == 0)
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

        // 게임 셋업 준비(코인 비활성화, 게임 상태 레디, 카메라 설정)
        StartCoroutine(SetupGameToStart(Direction));
    }

    IEnumerator SetupGameToStart(int Direction)
    {
        //coin 안보이게 
        yield return new WaitForSeconds(1f);
        Coin.gameObject.SetActive(false);

        // 선공 알림이
        UIManager.Instance.VisibleFirstAttackPanel(Direction);

        // 오디오 효과
        if (Direction == 0)
        {
            redAudio.PlayGoodLuck();
        }
        else
        {
            blueAudio.PlayGoodLuck();
        }

        //동전 비활성화 2초 후 -> 게임 상태 Ready/카메라 세팅
        yield return new WaitForSeconds(2f);
        GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;
        CameraManager.Instance.SwitchTurnAndCamera();
    }
    #endregion

    #region 옵션 선택 값 가져오기 -> 레디 체크 후 -> 턴플레이 
    public void GetRedOptionNum(int num)
    {
        isRedReady = true;
        RedSelectedOption = num;

        CheckBothReady();

        // UI 적용
        UIManager.Instance.ShowReadyUI();
        // 사운드 효과
        UIEffect.Instance.PlayReady();
    }

    public void GetBlueOptionNum(int num)
    {
        isBlueReady = true;
        BlueSelectedOption = num;

        CheckBothReady();

        // UI 적용
        UIManager.Instance.ShowReadyUI();
        // 사운드 효과
        UIEffect.Instance.PlayReady();
    }

    // 준비 상태 체크 
    private void CheckBothReady()
    {
        if (GameManager.Instance.currentDirection == GameManager.GameDirection.GameEnd) return;

        if (isRedReady && isBlueReady)
        {
            Debug.Log("양쪽 모두 레디를 완료했습니다.");
            TurnPlay();
            StartCoroutine(ResetTurn(false, false));
        }
    }
    #endregion

    #region 턴 플레이 / 초기화
    private void TurnPlay()
    {
        if (red.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            redAnimation.PlayThrow();
            redAudio.PlayThrowClip();
        }
        else
        {
            redAnimation.PlayAvoid(RedSelectedOption);
        }

        if (blue.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            blueAnimation.PlayThrow();
            blueAudio.PlayThrowClip();
        }
        else
        {
            blueAnimation.PlayAvoid(BlueSelectedOption);
        }
    }

    // 턴 초기화
    public IEnumerator ResetTurn(bool SwitchTurn, bool StunTurn)
    {
        // 턴 정상 진행 후 리셋 
        if (!SwitchTurn && !StunTurn)
        {
            GameManager.Instance.currentDirection = GameManager.GameDirection.Play;

            // 공격 연출 기달
            yield return new WaitForSeconds(TurnPlayTime);

            if (GameManager.Instance.currentDirection == GameManager.GameDirection.GameEnd)
            {
                resetTurnCoroutine = null;
                yield break;
            }

            // 모든 플레이어 아이템 선택 상태, 기존 효과 제거 
            ItemManager.Instance.ResetSelected();
            ItemManager.Instance.DeleteItemEffect();

            // 어느 한쪽이 눈덩이를 맞은 라운드
            bool isAttackSucess = false;

            if(red.currentPlayerDirection == Player.PlayerDirection.Attack)
            {
                isAttackSucess = blueState.isHit;
            }
            else
            {
                isAttackSucess = redState.isHit;
            }
            
            if (isAttackSucess)
            {
                if (redState.isHit)
                {
                    StartCoroutine(redState.ResetHitBool());
                }

                if (blueState.isHit)
                {
                    StartCoroutine(blueState.ResetHitBool());
                }
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
            }

            // 레디 상태, 옵션 선택 값 초기화(데이터 초기화)
            isRedReady = false;
            isBlueReady = false;
            RedSelectedOption = 0;
            BlueSelectedOption = 0;

            // 플레이 -> 레디 상태
            GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

            // 화면에 출력된 ready효과 제거 
            UIManager.Instance.HideReadyUI();

            // 코루틴 변수 null로 초기화 
            resetTurnCoroutine = null;
        }
        else if (SwitchTurn && !StunTurn)
        {
            // 레디 상태, 옵션 선택 값 초기화
            isRedReady = false;
            isBlueReady = false;
            RedSelectedOption = 0;
            BlueSelectedOption = 0;

            // 화면에 출력된 ready효과 제거 
            UIManager.Instance.HideReadyUI();

            // 공수 전환
            red.ChangeDirection();
            blue.ChangeDirection();

            // 카메라 전환
            CameraManager.Instance.SwitchTurnAndCamera();

            // 플레이 -> 레디 상태
            GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

            // 코루틴 null초기화 
            resetTurnCoroutine = null;
        }
        else if (!SwitchTurn && StunTurn)
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

            yield return new WaitForSeconds(2f);

            // 공수 전환
            red.ChangeDirection();
            blue.ChangeDirection();

            // 카메라 전환
            CameraManager.Instance.SwitchTurnAndCamera();

            yield return new WaitForSeconds(1.5f);

            isRedStun = false;
            isBlueStun = false;

            isStunTurn = false;

            // 플레이 -> 레디 상태
            GameManager.Instance.currentDirection = GameManager.GameDirection.Ready;

            // 코루틴 null초기화 
            resetTurnCoroutine = null;
        }
    }
    #endregion

    #region 최종 승자 체크
    public void CheckFinalWinner()
    {
        if (redState.currentHP > 0 && blueState.currentHP <= 0)
        {
            // 카메라 전환
            StartCoroutine(CameraManager.Instance.SwitchFinalCamera(0));
            // 승리 패널 시각화
            UIManager.Instance.VisibleVictroyPanel(0);
        }
        else if (blueState.currentHP > 0 && redState.currentHP <= 0)
        {
            // 카메라 전환
            StartCoroutine(CameraManager.Instance.SwitchFinalCamera(1));
            // 승리 패널 시각화
            UIManager.Instance.VisibleVictroyPanel(1);
        }
    }
    #endregion
}