using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (currentSceneName == "MainMenuScene")
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("커서 보임");
    }

    public void HideCursor()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("커서 안보임");
    }
}