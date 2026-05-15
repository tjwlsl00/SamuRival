using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // 싱글톤
    public static GameSceneManager Instance;

    // 외부 
    private AudioSource audioSource;
    [SerializeField] AudioClip buttonClip;

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

        // 오디오
        audioSource = GetComponent<AudioSource>();
    }

    #region 씬 이동(YukiHoi -> MainMenu / 승리 이후 YukiHoi -> Load)
    public void GoMenuScene()
    {
        // 오디오 효과
        audioSource.clip = buttonClip;
        audioSource.Play();

        StartCoroutine(SwitchMenuScene());
    }

    IEnumerator SwitchMenuScene()
    {
        yield return new WaitForSeconds(buttonClip.length);
        SceneManager.LoadScene("MainMenuScene");
    }

    public IEnumerator MoveToScoreScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Score");
    }
    #endregion
}
