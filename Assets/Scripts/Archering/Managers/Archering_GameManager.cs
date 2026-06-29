using UnityEngine;

public class Archering_GameManager : MonoBehaviour
{
    public enum GameDirection { TurnDecide, Playing };
    public GameDirection gameDirection;

    // 싱글톤
    public static Archering_GameManager Instance;

    [Header("과녘 위치")]
    [SerializeField] Transform centerPoint;

    [Header("디사이더 스톤 위치")]
    private float decideRedStoneDist;
    private float decideBlueStoneDist;

    // bool 
    public bool isD_RThrowed = false;
    public bool isD_BThrowed = false;
    private bool allDecideStoneThrowed = false;

    // 외부
    private Archering_SceneManager archering_SceneManager;
    private Archering_TurnManager archering_TurnManager;
    private Archering_UIManager archering_UIManager;
    private Archering_CameraManager archering_CameraManager;
    private Archering_ScoreManager archering_ScoreManager;
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
        archering_ScoreManager = GetComponent<Archering_ScoreManager>();
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
        // 모든 디사이드 스톤 던져짐
        if (isD_RThrowed && isD_BThrowed)
        {
            if (allDecideStoneThrowed) return;
            MeasureDeciderPosition();
        }
    }

    #region 거리 측정(표적 - 디사이더 스톤 위치)
    public void MeasureDeciderPosition()
    {
        allDecideStoneThrowed = true;

        GameObject[] decideStones = GameObject.FindGameObjectsWithTag("decideStone");

        if (decideStones != null && decideStones.Length >= 2)
        {
            float redDist = 0f;
            float blueDist = 0f;
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
            allDecideStoneThrowed = false;
            Debug.Log("모든 디사이더 스톤이 발견되지 않음.");
        }
    }
    #endregion

    #region 거리값에 의한 순서 결정
    private void TurnDecide(float redDist, float blueDist)
    {
        if (redDist < blueDist)
        {
            Debug.Log("레드 먼저 시작");

            // 게임 준비 
            StartCoroutine(archering_TurnManager.PrepareStartGame(0));
        }
        else
        {
            Debug.Log("블루 먼저 시작");

            // 게임 준비 
            StartCoroutine(archering_TurnManager.PrepareStartGame(1));
        }
    }
    #endregion

    #region 게임 재시작 / 종료 
    public void GameEnd(bool isGameEnd)
    {
        // 점수 카메라
        archering_CameraManager.SwitchScoreCam();

        // 사운드 효과
        archering_SoundManager.PlayResultClip();

        // 최종 UI 세팅
        archering_UIManager.FinalUISetting();

        if (isGameEnd)
        {
            // 다음 맵 이동
            StartCoroutine(archering_SceneManager.MoveToScene());
        }
        else
        {
            // 재시작
            StartCoroutine(archering_SceneManager.RebootGameScene());
        }
    }
    #endregion
}
