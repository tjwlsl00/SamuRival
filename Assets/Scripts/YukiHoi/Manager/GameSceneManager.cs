using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class GameSceneManager : MonoBehaviour
{
    // 싱글톤
    public static GameSceneManager Instance;

    // 외부 
    private AudioSource audioSource;
    [SerializeField] AudioClip buttonClip;

    // bool 
    private bool isChanged = false;

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

    void Update()
    {
        // 맵 전환 테스트 시 활용
        if (isChanged) return;
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isChanged = true;
            StartCoroutine(MoveToScoreScene());
        }
    }


    #region 씬 이동
    // --------
    // 메뉴
    // --------
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

    // --------
    // 재시작
    // --------
    public void RestartScene()
    {
        Time.timeScale = 1f;

        DOTween.KillAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --------
    // 다음 씬 이동(게임 종료 후)
    // --------
    public IEnumerator MoveToScoreScene()
    {
        // 5초 대기
        yield return new WaitForSeconds(5f);

        if (Global_ScoreManger.Instance.redGetScore >= Global_ScoreManger.Instance.maxScore || Global_ScoreManger.Instance.blueGetScore >= Global_ScoreManger.Instance.maxScore)
        {
            DOTween.KillAll();

            // 맵 로드 데이터 설정 
            Map_RouletteManager.mapPool.Clear();
            Map_RouletteManager.mapPool.AddRange(new int[] { 0, 1, 2, 3 });

            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            SceneManager.LoadScene("Score");
        }
    }
    #endregion
}
