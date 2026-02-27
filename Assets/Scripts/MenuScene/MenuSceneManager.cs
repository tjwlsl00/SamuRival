using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSceneManager : MonoBehaviour
{
    // 외부 
    [SerializeField] GameObject CheckManagerObj;
    private CheckManager checkManager;
    private AudioSource audioSource;
    [SerializeField] AudioClip buttonClip;
    // bool 
    private bool isGameStarted = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        checkManager = CheckManagerObj.GetComponent<CheckManager>();
    }

    public void GoGameScene()
    {
        if (checkManager == null) return;
        if (isGameStarted) return;
        isGameStarted = true;

        StartCoroutine(WaitforMenuPlayer());

        // 사운드 효과
        audioSource.clip = buttonClip;
        audioSource.Play();
    }

    IEnumerator WaitforMenuPlayer()
    {
        checkManager.isAllPlayerReady = true;
        checkManager.CheckMenuPlayerReady();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameScene");
    }

    public void GameEnd()
    {
        Debug.Log("게임 종료 되었습니다");
        Application.Quit();
    }
}