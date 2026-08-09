using System.Collections;
using UnityEngine;

public class Archering_TurnManager : MonoBehaviour
{
    public enum GameTurn { RedTurn, BlueTurn };
    public GameTurn gameTurn;

    // 싱글톤
    public static Archering_TurnManager Instance;

    [Header("플레이어 턴 기회")]
    public int currentRedTurn = 0;
    public int currentBlueTurn = 0;
    public int leftRedCount;
    public int leftBlueCount;
    private int maxCount = 10;

    [Header("타겟 참조")]
    private int targetStoneIndex = 0;

    // 외부
    public GameObject spawnPoint;
    private SpawnManager spawnManager;
    private Archering_GameManager archering_GameManager;
    private Archering_UIManager archering_UIManager;
    private Archering_CameraManager archering_CameraManager;
    private Archering_ScoreManager archering_ScoreManager;
    private Archering_SoundManager archering_SoundManager;

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
        spawnManager = spawnPoint.GetComponent<SpawnManager>();
        archering_GameManager = GetComponent<Archering_GameManager>();
        archering_UIManager = GetComponent<Archering_UIManager>();
        archering_CameraManager = GetComponent<Archering_CameraManager>();
        archering_ScoreManager = GetComponent<Archering_ScoreManager>();
        archering_SoundManager = GetComponent<Archering_SoundManager>();

        // 카운트 설정 
        leftRedCount = maxCount;
        leftBlueCount = maxCount;
    }

    #region 턴 준비 / 시작
    // ---------
    // 디사이드 
    // ---------
    public IEnumerator PrepareStartGame(int turnIndex)
    {
        // 사운드 효과
        archering_SoundManager.PlayWhoIsFirst(turnIndex);

        // 디사이드 결과값 패널 
        yield return StartCoroutine(archering_UIManager.EffectDecideResultPanel(turnIndex));

        yield return new WaitForSeconds(0.5f);

        // 디사이드 오브젝트 삭제 
        DeleteDecideStones();

        // 카메라 리셋
        archering_CameraManager.ResetCam();

        // 턴 상태 패널 활성화 
        if (archering_UIManager != null)
        {
            archering_UIManager.VisibleTurnStatusPanel();
        }

        // 게임 상태 변경 
        archering_GameManager.gameDirection = Archering_GameManager.GameDirection.Playing;

        // 준비 후 턴 시작 
        StartTurn(turnIndex);
    }
    private void DeleteDecideStones()
    {
        GameObject[] decideStones = GameObject.FindGameObjectsWithTag("decideStone");
        foreach (GameObject stone in decideStones)
        {
            Destroy(stone);
        }
        Debug.Log("화면 위의 다사이드 스톤 제거");
    }

    // ---------
    // 일반
    // ---------
    public void StartTurn(int turnIndex)
    {
        gameTurn = (turnIndex == 0) ? GameTurn.RedTurn : GameTurn.BlueTurn;
        targetStoneIndex = (turnIndex == 0) ? leftRedCount : leftBlueCount;

        // 첫빠따 스톤 스폰
        if (spawnManager != null)
        {
            spawnManager.SetUpStone(turnIndex, targetStoneIndex);
        }
    }
    #endregion

    #region 턴 끝남 / 카운트 증가 / 턴 체인지
    public void OnTurnEnd()
    {
        // ---------
        // 디사이드, 라스트 스퍼트
        // ---------
        if (archering_GameManager.gameDirection == Archering_GameManager.GameDirection.TurnDecide || archering_GameManager.gameDirection == Archering_GameManager.GameDirection.LastSpurt)
        {
            // 기본 턴 카메라 
            archering_CameraManager.ResetCam();

            if (gameTurn == GameTurn.RedTurn)
            {
                gameTurn = GameTurn.BlueTurn;
            }
        }
        // ---------
        // 일반
        // ---------
        else
        {
            int playerNum = (gameTurn == GameTurn.RedTurn) ? 0 : 1;
            DecreaseTurnCount(playerNum);

            if (leftRedCount > 0 || leftBlueCount > 0)
            {
                // 기본 턴 카메라 
                archering_CameraManager.ResetCam();

                ChangeTurn();
            }
            else
            {
                // 점수 체크 
                CheckPlayerScore();
            }
        }
    }

    public void DecreaseTurnCount(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            if (leftRedCount <= 0)
            {
                return;
            }
            else
            {
                // 남은 기회
                leftRedCount--;

                // 현재 턴 카운팅
                currentRedTurn++;

                Debug.Log("남은 기회 : " + leftRedCount + "현재 턴 진행 : " + currentRedTurn);

                // 턴 카운팅 UI 업데이트 
                archering_UIManager.UpdateTurnStatus(0, false);
            }
        }
        else
        {
            if (leftBlueCount <= 0)
            {
                return;
            }
            else
            {
                // 남은 기회 
                leftBlueCount--;

                // 현재 카운팅
                currentBlueTurn++;

                Debug.Log("남은 기회 : " + leftBlueCount + "현재 턴  진행 : " + currentBlueTurn);

                // 턴 카운팅 UI 업데이트 
                archering_UIManager.UpdateTurnStatus(1, false);
            }
        }
    }

    public void ChangeTurn()
    {
        // 턴 업데이트 
        gameTurn = (gameTurn == GameTurn.RedTurn) ? GameTurn.BlueTurn : GameTurn.RedTurn;

        // 플레이 준비
        StartCoroutine(ReadyToPlay());
    }

    IEnumerator ReadyToPlay()
    {
        // 스폰 포인트 내부 오브젝트 청소
        Archering_DeadZone.Instance.DeleteStone();

        // 컬링 겹치는 버그 방지  
        yield return new WaitForSeconds(1f);

        int playerNum = (gameTurn == GameTurn.RedTurn) ? 0 : 1;
        int leftCount = (gameTurn == GameTurn.RedTurn) ? leftRedCount : leftBlueCount;

        if (spawnManager != null)
        {
            spawnManager.SetUpStone(playerNum, leftCount);
        }
    }
    #endregion

    #region 점수 체크
    private void CheckPlayerScore()
    {
        // 스톤 찾기
        archering_ScoreManager.FindOnStageStones();

        // -------------
        // 승패 갈리면 정상 게임 종료, 아니면 승자 결정전
        // -------------
        if (archering_ScoreManager.redFinalScore != archering_ScoreManager.blueFinalScore)
        {
            archering_GameManager.GameEnd(true);
        }
        else
        {
            archering_GameManager.GameEnd(false);
        }
    }
    #endregion
}