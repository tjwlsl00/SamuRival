using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TonTon_GameManager : MonoBehaviour
{

    // 싱글톤
    public static TonTon_GameManager Instance;

    public enum TonTon_GameDirection { Start, End }
    public TonTon_GameDirection tonton_GameDirection { get; set; }

    // bool 
    private bool isRedWin = false;
    private bool isBlueWin = false;
    private bool isGameOver = false;

    // 게이지 수치 
    private float InitialGaze = 0f;
    public float MaxGaze = 100f;
    public float CurrentRedGaze;
    public float CurrentBlueGaze;

    // 외부 
    private TonTon_UIManager tonTon_UIManager;
    private TonTon_CameraManager tonTon_CameraManager;
    private TonTon_SoundManger tonTon_SoundManger;

    // 플레이어
    [SerializeField] GameObject[] Players;
    private TonTon_Player red;
    private TonTon_Player blue;

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

        tonTon_UIManager = GetComponent<TonTon_UIManager>();
        tonTon_CameraManager = GetComponent<TonTon_CameraManager>();
        tonTon_SoundManger = GetComponent<TonTon_SoundManger>();
        red = Players[0].GetComponent<TonTon_Player>();
        blue = Players[1].GetComponent<TonTon_Player>();
    }

    void Start()
    {
        tonton_GameDirection = TonTon_GameDirection.Start;

        CurrentRedGaze = InitialGaze;
        CurrentBlueGaze = InitialGaze;

        // 오디오 
        tonTon_SoundManger.PlayGameStartClip();
    }

    void Update()
    {
        if (tonton_GameDirection == TonTon_GameDirection.End) return;
        if (CurrentRedGaze >= MaxGaze)
        {
            CurrentRedGaze = MaxGaze;
            CheckWinner();
        }
        else if (CurrentBlueGaze >= MaxGaze)
        {
            CurrentBlueGaze = MaxGaze;
            CheckWinner();
        }
    }

    #region 승자 체크/게임 종료
    public void CheckWinner()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (CurrentRedGaze > CurrentBlueGaze)
        {
            isRedWin = true;
        }
        else if (CurrentRedGaze < CurrentBlueGaze)
        {
            isBlueWin = true;
        }

        GameOver();
    }

    void GameOver()
    {
        tonton_GameDirection = TonTon_GameDirection.End;
        Debug.Log("톤톤 게임 종료");

        // 점수 처리
        if (isRedWin)
        {
            // 애니메이션 
            red.PlayVictroyAnim();
            blue.PlayDefeatAnim();

            // 오디오
            StartCoroutine(red.PlayWinnerClip());

            Debug.Log("레드 승리, 점수 추가");
            Global_ScoreManger.Instance.redGetScore++;
        }
        else if (isBlueWin)
        {
            // 애니메이션 
            blue.PlayVictroyAnim();
            red.PlayDefeatAnim();

            // 오디오
            StartCoroutine(blue.PlayWinnerClip());

            Debug.Log("블루 승리, 점수 추가");
            Global_ScoreManger.Instance.blueGetScore++;
        }

        // 종료 패널을 활성화
        tonTon_UIManager.EndGameUISetting();

        // 플레이어 위치 변경(게임 종료)
        red.MoveToWayPoint();
        blue.MoveToWayPoint();

        // 카메라 
        tonTon_CameraManager.EndGameCamera();

        // 오디오 
        tonTon_SoundManger.PlayGameEndClip();

        // 둘 중 하나 점수 최고점에 도달했을때 
        // if (Global_ScoreManger.Instance.redGetScore >= Global_ScoreManger.Instance.maxScore || Global_ScoreManger.Instance.blueGetScore >= Global_ScoreManger.Instance.maxScore) return;

        StartCoroutine(MoveToScoreScene());
    }

    IEnumerator MoveToScoreScene()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Score");
    }
    #endregion
}
