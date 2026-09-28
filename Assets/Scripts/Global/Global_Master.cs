using UnityEngine;
using UnityEngine.SceneManagement;

public class Global_Master : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
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