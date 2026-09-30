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
    private Menu_AnimationManager menu_AnimationManager;

    void Awake()
    {
        menu_UIManager = GetComponent<Menu_UIManager>();
        menu_AnimationManager = GetComponent<Menu_AnimationManager>();
    }

    void Update()
    {
        // 질문 패널 활성화 시 모든 입력 방지 
        if (menu_UIManager.isQuestionPanelOpened) return;

        // 1단계: 플레이어 둘 중 하나라도 레디가 안 되었을 때
        if (!isRedReady || !isBlueReady)
        {
            PlayerReadyInput();
        }
        // 2단계: 둘 다 레디 완료했고, 아직 모드 선택 전일 때 (isMod == true)
        else if (!menu_UIManager.isModSelected)
        {
            menu_UIManager.InputToSelectMod(true);
        }
        // 3단계: 모드 선택은 완료되었고, 게임(맵) 선택 중일 때 (isMod == false)
        else if (menu_UIManager.isModSelected && !menu_UIManager.isGameSelected)
        {
            menu_UIManager.InputToSelectMod(false);
        }
    }

    #region 플레이어 레디 키 입력 / 레디 체크 
    void PlayerReadyInput()
    {
        // -----
        // 레드
        // -----
        if (!isRedReady && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isRedReady = true;
            Debug.Log("레드 레디 완");
            menu_AnimationManager.PlayStretchAnim(0);
            CheckPlayerReadyStatus();
        }

        // -----
        // 블루
        // -----
        if (!isBlueReady && Input.GetKeyDown(KeyCode.W))
        {
            isBlueReady = true;
            Debug.Log("블루 레디 완");
            menu_AnimationManager.PlayStretchAnim(1);
            CheckPlayerReadyStatus();
        }
    }

    void CheckPlayerReadyStatus()
    {
        if (isRedReady && isBlueReady)
        {
            // 모드 선택 패널 등장
            menu_UIManager.VisibleModSelectPanel();
        }
    }
    #endregion

    #region 게임 시작 / 종료
    public void StartGame(bool isMod, int index)
    {
        Debug.Log("게임으로 이동합니다.");

        if (isMod)
        {
            if (index == 0)
            {
                Debug.Log("개별 모드 선택함");

                Global_DirectionManager.Instance.isIndivisual = true;
            }
            else if (index == 1)
            {
                Debug.Log("랜덤 모드 선택함");

                Global_DirectionManager.Instance.isIndivisual = false;

                if (isSceneLoading) return;
                isSceneLoading = true;

                // 맵 이동 
                SceneManager.LoadScene("Map");
            }
            else
            {
                Debug.Log("협동 모드 선택함");
            }
        }
        else
        {
            if (isSceneLoading) return;
            isSceneLoading = true;

            // 맵 인덱스 설정 
            Global_DirectionManager.Instance.SelectedMapIndex = index;

            // 맵 이동 
            SceneManager.LoadScene("Loading");
        }
    }

    public void GameEnd()
    {
        Debug.Log("게임 종료 되었습니다");

        Application.Quit();
    }
    #endregion
}