using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_SceneManager : MonoBehaviour
{
    // 레디 수치 
    public float RedReadyRatio = 0f;
    public float BlueReadyRatio = 0f;
    [SerializeField] private float maxReadyTime = 2f;
    // bool 
    public bool isRedReady = false;
    public bool isBlueReady = false;
    private bool isSceneLoading = false;
    // 외부 
    [SerializeField] GameObject[] MenuPlayers;
    private Menu_UIManager menu_UIManager;
    private Menu_SoundManager menu_SoundManager;

    void Awake()
    {
        menu_UIManager = GetComponent<Menu_UIManager>();
        menu_SoundManager = GetComponent<Menu_SoundManager>();
    }

    void Update()
    {
        // 빨간색 레디 입력 
        if (!isRedReady)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                RedReadyInput();
            }
            else if (!isRedReady)
            {
                if (RedReadyRatio > 0f)
                    RedReadyRatio -= Time.deltaTime;
            }
        }

        // 파란색 레디 입력
        if (!isBlueReady)
        {
            if (Input.GetKey(KeyCode.W))
            {
                BlueReadyInput();
            }
            else if (!isBlueReady)
            {
                if (BlueReadyRatio > 0f)
                    BlueReadyRatio -= Time.deltaTime;
            }
        }

        // 레디 완 -> 페이드인
        if (isRedReady && isBlueReady)
        {
            menu_UIManager.FadeOutAndStart();
        }
    }

    #region 꾹 입력 레디
    void RedReadyInput()
    {
        if (isRedReady) return;

        RedReadyRatio += Time.deltaTime;

        if (RedReadyRatio >= maxReadyTime)
        {
            isRedReady = true;
            Debug.Log("레드 레디 완");

        }
    }

    void BlueReadyInput()
    {
        if (isBlueReady) return;

        BlueReadyRatio += Time.deltaTime;

        if (BlueReadyRatio >= maxReadyTime)
        {
            isBlueReady = true;
            Debug.Log("블루 레디 완");

        }
    }
    #endregion

    #region 게임 시작 / 종료
    public void StartGame()
    {
        if (isSceneLoading) return;
        isSceneLoading = true;
        SceneManager.LoadScene("Map");

        Debug.Log("로딩 씬으로 이동합니다.");
    }

    public void GameEnd()
    {
        Application.Quit();
        Debug.Log("게임 종료 되었습니다");
    }
    #endregion
}