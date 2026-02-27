using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip buttonClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void GoMenuScene()
    {
        // 오디오 효과
        audioSource.clip = buttonClip;
        audioSource.Play();

        StartCoroutine(LoadMenuScene());
    }

    IEnumerator LoadMenuScene()
    {
        yield return new WaitForSeconds(buttonClip.length);
        SceneManager.LoadScene("MenuScene");
    }

    public void ReStartScene()
    {
        // 오디오 효과
        audioSource.clip = buttonClip;
        audioSource.Play();

        StartCoroutine(LoadCurrentScene());
    }

    IEnumerator LoadCurrentScene()
    {
        yield return new WaitForSeconds(buttonClip.length);
        string CurrentSceneName = SceneManager.GetActiveScene().name;

        if (CurrentSceneName != null)
        {
            // 현재 씬 리로드 
            SceneManager.LoadScene(CurrentSceneName);
        }
    }
}
