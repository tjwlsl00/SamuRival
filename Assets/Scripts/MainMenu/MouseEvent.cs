using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;

public class MouseEvent : MonoBehaviour
{
    public static MouseEvent Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursor();
        }
    }

    private void ToggleCursor()
    {
        // 커서가 안 보이거나 잠겨있는 상태라면 보이게 전환
        if (!Cursor.visible || Cursor.lockState == CursorLockMode.Locked)
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;

        if (currentSceneName == "MainMenuScene" || currentSceneName == "SelectMod")
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current != null)
        {
            // 기존 선택 초기화
            EventSystem.current.SetSelectedGameObject(null);

            // 마우스 재인식
            var inputModule = EventSystem.current.GetComponent<BaseInputModule>();
            if (inputModule != null)
            {
                inputModule.enabled = false;
                inputModule.enabled = true;
            }
        }

        Debug.Log("커서 보임 + Lock 해제");
    }

    public void HideCursor()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // 화면 밖으로 탈출하지 않게 하려면 Locked만 써도 충분합니다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("커서 안보임 + Lock 설정");
    }
}