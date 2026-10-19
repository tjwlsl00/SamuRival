using UnityEngine;
using UnityEngine.SceneManagement;

public class Global_Master : MonoBehaviour
{
    public enum InputMod { Keyboard, Controller }
    public InputMod currentInputMod;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //기본 세팅 
        currentInputMod = InputMod.Keyboard;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("입력 변경:키보드");
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("입력 변경:컨트롤러 ");
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
        if (Global_ScoreManger.Instance.isRedWin || Global_ScoreManger.Instance.isBlueWin)
        {
            if (scene.name == "Score")
            {
                Destroy(gameObject);
            }
        }
    }
}