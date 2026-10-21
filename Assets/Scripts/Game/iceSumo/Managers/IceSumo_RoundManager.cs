using UnityEngine;
using System.Collections;

public class IceSumo_RoundManager : MonoBehaviour
{
    // 싱글톤
    public static IceSumo_RoundManager Instance;

    [Header("점수 관련")]
    public static int redScore = 0;
    public static int blueScore = 0;
    [SerializeField] int maxScore = 2;

    [Header("점수 관련")]
    public static int roundCount = 0;
    public static int[] roundHistroy = new int[3];

    // bool
    public bool isRedGetRound = false;
    public bool isBlueGetRound = false;
    public bool isRedWin = false;
    public bool isBlueWin = false;

    [Header("스크립트 관련")]
    private IceSumo_GameManager iceSumo_GameManager;
    private IceSumo_UIManager iceSumo_UIManager;
    private IceSumo_SceneManager iceSumo_SceneManager;

    void Awake()
    {
        // 싱글톤 선언
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 스크립트 참조
        iceSumo_GameManager = GetComponent<IceSumo_GameManager>();
        iceSumo_UIManager = GetComponent<IceSumo_UIManager>();
        iceSumo_SceneManager = GetComponent<IceSumo_SceneManager>();
    }

    #region 점수 증가 / 체크 / 기록
    public void IncreaseScore(int playerNum)
    {
        if (playerNum == 0)
        {
            Debug.Log("레드 점수 추가");

            redScore++;
            isRedGetRound = true;
        }
        else
        {
            Debug.Log("블루 점수 추가");

            blueScore++;
            isBlueGetRound = true;
        }

        // 점수 체크 
        CheckPlayerScore();

        // 라운드 UI 업데이트
        iceSumo_UIManager.UpdateRoundUI();
    }

    private void CheckPlayerScore()
    {
        if (redScore >= maxScore || blueScore >= maxScore)
        {
            Debug.Log("승부남");

            if (redScore > blueScore)
            {
                Debug.Log("레드 승리");

                isRedWin = true;
            }
            else if (redScore < blueScore)
            {
                Debug.Log("블루 승리");

                isBlueWin = true;
            }

            // 라운드 기록
            RecordRoundHistroy();

            // 게임 종료 함수 호출 
            iceSumo_GameManager.GameEnd();
        }
        else
        {
            // 라운드 기록
            RecordRoundHistroy();

            // 리붓
            RebootRound();
        }
    }

    private void RecordRoundHistroy()
    {
        Debug.Log("라운드 기록 완료");

        int currentRound = roundCount;

        if (currentRound >= 0 && currentRound < roundHistroy.Length)
        {
            if (isRedGetRound)
            {
                roundHistroy[currentRound] = 1;
            }
            else if (isBlueGetRound)
            {
                roundHistroy[currentRound] = 2;
            }
        }

        // 기록 후 라운드 카운트 증가 
        roundCount++;
    }
    #endregion

    #region 라운드 리붓
    public void RebootRound()
    {
        // 라운드 플래그 초기화 
        ResetRoundFlag();

        // 페이드 인 효과 
        StartCoroutine(iceSumo_UIManager.FadeInAfterRound(() =>
        {
            // 씬 재로드
            iceSumo_SceneManager.RestartScene();
        }));
    }

    private void ResetRoundFlag()
    {
        isRedGetRound = false;
        isBlueGetRound = false;
    }
    #endregion

    #region 데이터 초기화
    public void ResetData()
    {
        redScore = 0;
        blueScore = 0;
        roundCount = 0;
        isRedWin = false;
        isBlueWin = false;
        
        if (roundHistroy != null)
        {
            System.Array.Clear(roundHistroy, 0, roundHistroy.Length);
        }
    }
    #endregion
}