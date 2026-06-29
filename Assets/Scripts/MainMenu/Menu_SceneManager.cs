using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_SceneManager : MonoBehaviour
{
    // 레디 수치 
    public float RedReadyRatio = 0f;
    public float BlueReadyRatio = 0f;
    // bool 
    public bool isRedReady = false;
    public bool isBlueReady = false;
    private bool isSceneLoading = false;
    // 외부 
    [SerializeField] GameObject[] MenuPlayers;
    private Menu_UIManager menu_UIManager;
    private Menu_SoundManager menu_SoundManager;
    private Menu_AnimationManager menu_AnimationManager;

    void Awake()
    {
        menu_UIManager = GetComponent<Menu_UIManager>();
        menu_SoundManager = GetComponent<Menu_SoundManager>();
        menu_AnimationManager = GetComponent<Menu_AnimationManager>();
    }

    void Update()
    {
        // 빨간색 레디 입력 
        if (!isRedReady)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                RedReadyInput();
            }
        }

        // 파란색 레디 입력
        if (!isBlueReady)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                BlueReadyInput();
            }
        }

        // 레디 완 -> 페이드인
        if (isRedReady && isBlueReady)
        {
            StartCoroutine(menu_UIManager.FadeOutAndStart(1f));
        }
    }

    #region 레디 키 입력(중복 방지)
    void RedReadyInput()
    {
        if (isRedReady) return;

        isRedReady = true;
        Debug.Log("레드 레디 완");

        // 애니메이션 실행
        menu_AnimationManager.PlayStretchAnim(0);
    }

    void BlueReadyInput()
    {
        if (isBlueReady) return;

        isBlueReady = true;
        Debug.Log("블루 레디 완");

        // 애니메이션 실행
        menu_AnimationManager.PlayStretchAnim(1);
    }
    #endregion

    #region 게임 시작 / 종료
    public void StartGame()
    {
        if (isSceneLoading) return;
        isSceneLoading = true;
        SceneManager.LoadScene("Map");
        Debug.Log("맵 선택 화면으로 이동합니다.");
    }

    public void GameEnd()
    {
        Application.Quit();
        Debug.Log("게임 종료 되었습니다");
    }
    #endregion
}