using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_SceneManager : MonoBehaviour
{
    // bool 
    public bool isRedReady = false;
    public bool isBlueReady = false;
    private bool isSceneLoading = false;

    // 스크립트 참조
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
        // 질문 패널 활성화 시 레디 키 입력 방지
        if (menu_UIManager.isQuestionPanelOpened) return;

        // 플레이어 레디 입력
        PlayerReadyInput();
    }

    #region 플레이어 레디 키 입력 / 레디 체크 
    void PlayerReadyInput()
    {
        // -----
        // 레드
        // -----
        if (!isRedReady)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isRedReady = true;
                Debug.Log("레드 레디 완");

                // 애니메이션 실행
                menu_AnimationManager.PlayStretchAnim(0);
            }
        }

        // -----
        // 블루
        // -----
        if (!isBlueReady)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (isBlueReady) return;

                isBlueReady = true;
                Debug.Log("블루 레디 완");

                // 애니메이션 실행
                menu_AnimationManager.PlayStretchAnim(1);
            }
        }

        // 레디 상태 체크 
        CheckPlayerReadyStatus();
    }
    
    void CheckPlayerReadyStatus()
    {
        if (isRedReady && isBlueReady)
        {
            StartCoroutine(menu_UIManager.FadeOutAndStart(1f));
        }
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