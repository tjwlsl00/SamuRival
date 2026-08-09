using UnityEngine;
using System.Collections;

public class SnowEscape_GameManager : MonoBehaviour
{
    // 싱글톤
    public static SnowEscape_GameManager Instance;

    public enum GameDirection { Ready, Start, End };
    public GameDirection gameDirection;

    [Header("데코 오브젝트 참조")]
    [SerializeField] GameObject decoObj;

    [Header("플레이어 오브젝트 참조")]
    [SerializeField] GameObject[] players;

    // bool 
    private bool isPaused = false;
    public bool isRedWin = false;
    public bool isBlueWin = false;
    private bool isGameOver = false;

    // 스크립트 참조
    private SnowEscape_UIManager snowEscape_UIManager;
    private SnowEscape_CameraManager snowEscape_CameraManager;
    private SnowEscape_SoundManager snowEscape_SoundManager;
    private SnowEscape_SceneManager snowEscape_SceneManager;
    private SnowEscape_PlayerAnim redAnim;
    private SnowEscape_PlayerAnim blueAnim;

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

        snowEscape_UIManager = GetComponent<SnowEscape_UIManager>();
        snowEscape_CameraManager = GetComponent<SnowEscape_CameraManager>();
        snowEscape_SoundManager = GetComponent<SnowEscape_SoundManager>();
        snowEscape_SceneManager = GetComponent<SnowEscape_SceneManager>();
        redAnim = players[0].GetComponent<SnowEscape_PlayerAnim>();
        blueAnim = players[1].GetComponent<SnowEscape_PlayerAnim>();
    }

    void Start()
    {
        gameDirection = GameDirection.Ready;
        StartCoroutine(InitialSettingRoutine());
    }

    void Update()
    {
        if (gameDirection == GameDirection.Start)
        {
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
        }
        else if (gameDirection == GameDirection.End)
        {
            if (isGameOver) return;
            isGameOver = true;
            GameEnd();
        }
    }

    private void GameEnd()
    {
        CheckWinner();
    }

    void CheckWinner()
    {
        if (isRedWin && !isBlueWin)
        {
            FinalSetting(0);
        }
        else if (!isRedWin && isBlueWin)
        {
            FinalSetting(1);
        }
    }

    #region 세팅(초기 / 최종)
    private IEnumerator InitialSettingRoutine()
    {
        yield return new WaitForSeconds(6f);

        // 데코 오브젝트 삭제
        Destroy(decoObj);

        // 게임 상태 업데이트
        gameDirection = GameDirection.Start;
    }

    public void FinalSetting(int playerIndex)
    {
        Debug.Log("승리" + playerIndex);

        // ---------
        // 나무 오브젝트 삭제 
        // ---------
        DestroyAllTreeObj();

        // ---------
        // 카메라 세팅
        // ---------
        snowEscape_CameraManager.SwitchEndCameras();

        // --------------
        // 캐릭터 애니메이션 
        // --------------
        if (redAnim != null && blueAnim != null)
        {
            redAnim.PlayResultAnim();
            blueAnim.PlayResultAnim();
        }

        // -------
        // ui 세팅
        // -------
        snowEscape_UIManager.GameEndUISetting(playerIndex);

        // ---------
        // 사운드 세팅
        // ---------
        snowEscape_SoundManager.PlayWinnerClip(playerIndex);

        // ---------
        // 점수 추가 
        // ---------
        if (playerIndex == 0)
        {
            Global_ScoreManger.Instance.redGetScore++;
        }
        else
        {
            Global_ScoreManger.Instance.blueGetScore++;
        }

        // 씬 이동 
        StartCoroutine(snowEscape_SceneManager.MoveToScene());
    }

    // 모든 나무 오브젝트 삭제 
    void DestroyAllTreeObj()
    {
        Debug.Log("모든 나무 오브젝트 삭제");

        GameObject[] trees = GameObject.FindGameObjectsWithTag("tree");

        foreach (GameObject tree in trees)
        {
            Destroy(tree);
        }
    }
    #endregion
}