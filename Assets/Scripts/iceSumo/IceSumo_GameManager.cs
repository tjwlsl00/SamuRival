using UnityEngine;
using System.Collections;

public class IceSumo_GameManager : MonoBehaviour
{
    [Header("게임 상태 참조")]
    public GameDirection gameDirection;
    public enum GameDirection { Ready, Play, End };

    [Header("플레이어 참조")]
    [SerializeField] GameObject[] players;

    // 싱글톤 
    public static IceSumo_GameManager Instance;

    // 스크립트 참조
    private IceSumo_UIManager iceSumo_UIManager;
    private IceSumo_CameraManager iceSumo_CameraManager;
    private IceSumo_RoundManager iceSumo_RoundManager;
    private IceSumo_SoundManager iceSumo_SoundManager;
    private IceSumo_SceneManager iceSumo_SceneManager;

    private IceSumo_PlayerAnimation redAnimation;
    private IceSumo_PlayerAnimation blueAnimation;

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

        iceSumo_UIManager = GetComponent<IceSumo_UIManager>();
        iceSumo_CameraManager = GetComponent<IceSumo_CameraManager>();
        iceSumo_RoundManager = GetComponent<IceSumo_RoundManager>();
        iceSumo_SoundManager = GetComponent<IceSumo_SoundManager>();
        iceSumo_SceneManager = GetComponent<IceSumo_SceneManager>();
    }

    void Start()
    {
        gameDirection = GameDirection.Ready;
        StartCoroutine(GameStartRoutine());
    }

    IEnumerator GameStartRoutine()
    {
        yield return new WaitForSeconds(3f);
        gameDirection = GameDirection.Play;
    }

    public void GameEnd()
    {
        // 게임 상태 업데이트
        gameDirection = GameDirection.End;

        // 승자 카메라 세팅
        iceSumo_CameraManager.SetUpFinalWinnerCamera();

        // 최종 UI 불러오기 
        iceSumo_UIManager.FinalUISetting();

        // 애니메이션 사운드(승자)
        redAnimation = players[0].GetComponent<IceSumo_PlayerAnimation>();
        blueAnimation = players[1].GetComponent<IceSumo_PlayerAnimation>();
        if (iceSumo_RoundManager.isRedWin)
        {
            if (redAnimation != null)
            {
                redAnimation.PlayWinAnim();
            }

            StartCoroutine(iceSumo_SoundManager.PlayWinnerClip(0));
        }
        else
        {
            if (blueAnimation != null)
            {
                blueAnimation.PlayWinAnim();
            }

            StartCoroutine(iceSumo_SoundManager.PlayWinnerClip(1));
        }

        // 씬 이동 
        StartCoroutine(iceSumo_SceneManager.MoveToScene());
    }
}
