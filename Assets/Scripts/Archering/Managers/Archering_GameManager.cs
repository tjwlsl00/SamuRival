using UnityEngine;
using System.Collections;

public class Archering_GameManager : MonoBehaviour
{
    // 게임 상태 
    public enum GameDirection { TurnDecide, Playing, LastSpurt };
    public GameDirection gameDirection;

    // 싱글톤
    public static Archering_GameManager Instance;

    [Header("과녘 위치")]
    [SerializeField] Transform centerPoint;

    [Header("거리 관련 변수")]
    private float redDist = 0f;
    private float blueDist = 0f;

    // bool 
    public bool isPaused = false;
    public bool isD_RThrowed = false;
    public bool isD_BThrowed = false;
    private bool allDecideStoneThrowed = false;
    public bool isL_RThrowed = false;
    public bool isL_BThrowed = false;
    private bool allLastSpurtStoneThrowed = false;

    // 외부
    private Archering_SceneManager archering_SceneManager;
    private Archering_TurnManager archering_TurnManager;
    private Archering_UIManager archering_UIManager;
    private Archering_CameraManager archering_CameraManager;
    private Archering_SoundManager archering_SoundManager;
    public GameObject spawnPoint;
    private SpawnManager spawnManager;

    void Awake()
    {
        // 싱글톤 
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 스크립트 참조 
        archering_SceneManager = GetComponent<Archering_SceneManager>();
        archering_TurnManager = GetComponent<Archering_TurnManager>();
        archering_UIManager = GetComponent<Archering_UIManager>();
        archering_CameraManager = GetComponent<Archering_CameraManager>();
        archering_SoundManager = GetComponent<Archering_SoundManager>();
        spawnManager = spawnPoint.GetComponent<SpawnManager>();
    }

    void Start()
    {
        // 게임 상태 초기화(디사이더 -> 턴)
        gameDirection = GameDirection.TurnDecide;

        // 빨강 먼저 시작
        spawnManager.SetUpDecideStone(0);
    }

    void Update()
    {
        // 게임 일시정지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"현재 상태: {isPaused}");
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        // 모든 디사이더 스톤 던짐
        if (isD_RThrowed && isD_BThrowed)
        {
            // 거리 측정
            MeasureDeciderPosition(0);
        }

        // 모든 라스트 스퍼트 스톤 던짐
        if (isL_RThrowed && isL_BThrowed)
        {
            // 거리 측정
            MeasureDeciderPosition(1);
        }
    }

    #region 거리 측정(디사이더 / 라스트 스퍼트)
    public void MeasureDeciderPosition(int gameIndex)
    {
        Debug.Log("거리 측정을 시작합니다.");

        // 거리 저장값 초기화 
        redDist = -1f;
        blueDist = -1f;

        // -------
        // 디사이더
        // -------
        if (gameIndex == 0)
        {
            if (allDecideStoneThrowed) return;

            allDecideStoneThrowed = true;

            GameObject[] decideStones = GameObject.FindGameObjectsWithTag("decideStone");

            if (decideStones != null && decideStones.Length >= 2)
            {
                Vector2 center2D = new Vector2(centerPoint.position.x, centerPoint.position.z);

                foreach (GameObject decideStone in decideStones)
                {
                    Stone stone = decideStone.GetComponent<Stone>();
                    if (stone != null)
                    {
                        Vector2 stone2D = new Vector2(decideStone.transform.position.x, decideStone.transform.position.z);

                        float distance = Vector2.Distance(center2D, stone2D);

                        if (stone.myTeam == Stone.Team.RedStone)
                        {
                            redDist = distance;
                        }
                        else
                        {
                            blueDist = distance;
                        }
                    }
                }

                // 순서 결정 
                TurnDecide(redDist, blueDist);
            }
            else
            {
                Debug.Log("모든 디사이더 스톤이 발견되지 않음.");

                allDecideStoneThrowed = false;
            }
        }
        // -------
        // 라스트 스퍼트 
        // -------
        else
        {
            if (allLastSpurtStoneThrowed) return;

            allLastSpurtStoneThrowed = true;

            GameObject[] lastSpurtStones = GameObject.FindGameObjectsWithTag("lastSpurtStone");

            Debug.Log("라스톤 찾았습니다.");

            if (lastSpurtStones != null && lastSpurtStones.Length >= 2)
            {
                Vector2 center2D = new Vector2(centerPoint.position.x, centerPoint.position.z);

                foreach (GameObject lastSpurtStone in lastSpurtStones)
                {
                    Stone stone = lastSpurtStone.GetComponent<Stone>();

                    if (stone != null)
                    {
                        float distance = Vector2.Distance(center2D, new Vector2(lastSpurtStone.transform.position.x, lastSpurtStone.transform.position.z));
                        if (stone.myTeam == Stone.Team.RedStone) redDist = distance;
                        else blueDist = distance;
                    }
                }

                if (redDist > blueDist && redDist != -1f && blueDist != -1f)
                {
                    Debug.Log("라스트 스퍼트: 블루 승리");

                    StartCoroutine(archering_UIManager.VisibleLastSpurtResultPanel(1));

                    // 점수 추가 
                    Global_ScoreManger.Instance.blueGetScore++;
                }
                else if (redDist < blueDist && redDist != -1f && blueDist != -1f)
                {
                    Debug.Log("라스트 스퍼트: 레드 승리");

                    StartCoroutine(archering_UIManager.VisibleLastSpurtResultPanel(0));

                    // 점수 추가 
                    Global_ScoreManger.Instance.redGetScore++;
                }
                else
                {
                    Debug.Log("라스트 스퍼트: 무승부 혹은 측정 실패");
                }
            }
            else
            {
                Debug.Log("모든 라스트 스퍼트 스톤이 발견되지 않음.");

                allLastSpurtStoneThrowed = false;
            }

            // 다음 맵 이동
            StartCoroutine(archering_SceneManager.MoveToScene());
        }
    }
    #endregion

    #region 거리값에 의한 순서 결정
    private void TurnDecide(float redDist, float blueDist)
    {
        if (redDist < blueDist)
        {
            Debug.Log("블루 먼저 시작");

            // 게임 준비 
            StartCoroutine(archering_TurnManager.PrepareStartGame(1));
        }
        else
        {
            Debug.Log("레드 먼저 시작");

            // 게임 준비 
            StartCoroutine(archering_TurnManager.PrepareStartGame(0));
        }
    }
    #endregion

    #region 게임 종료(일반 / 라스트 스퍼트)
    public void GameEnd(bool isGameEnd)
    {
        // 점수 카메라
        archering_CameraManager.SwitchScoreCam();

        // 사운드 효과
        archering_SoundManager.PlayResultClip();

        if (isGameEnd)
        {
            // UI 세팅
            archering_UIManager.FinalUISetting(true);

            // 다음 맵 이동
            StartCoroutine(archering_SceneManager.MoveToScene());
        }
        else
        {
            // UI 세팅
            archering_UIManager.FinalUISetting(false);

            // 라스트 스퍼트 
            StartCoroutine(LastSpurtGame());
        }
    }

    IEnumerator LastSpurtGame()
    {
        yield return new WaitForSeconds(2f);

        // 오디오 재생
        archering_SoundManager.PlayDrawClip();

        yield return new WaitForSeconds(3f);

        // 맵에 존재하는 스톤 삭제 
        DeleteAllStones();

        // 게임 모드 세팅
        gameDirection = GameDirection.LastSpurt;

        // 레드 먼저 시작 
        archering_TurnManager.gameTurn = Archering_TurnManager.GameTurn.RedTurn;

        // 카메라 리셋
        archering_CameraManager.ResetCam();

        // 스톤 세팅
        SpawnManager.Instance.SetUpLastSpurtStone(0);

        // 오디오 재생
        archering_SoundManager.PlayDecideTheWinnerClip();
    }

    // 스톤 제거
    private void DeleteAllStones()
    {
        GameObject[] redStones = GameObject.FindGameObjectsWithTag("redStone");
        foreach (GameObject stone in redStones)
        {
            Destroy(stone);
        }

        GameObject[] blueStones = GameObject.FindGameObjectsWithTag("blueStone");
        foreach (GameObject stone in blueStones)
        {
            Destroy(stone);
        }

        Debug.Log("화면 위의 모든 스톤 제거");
    }
    #endregion
}