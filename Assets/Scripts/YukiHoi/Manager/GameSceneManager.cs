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

        // 모든 맵 플레이-> 메인메뉴 / 아니면 Map 선택으로 
        if (Map_RouletteManager.mapPool != null && Map_RouletteManager.mapPool.Count > 0)
        {
            SceneManager.LoadScene("Map");
        }
        else
        {
            DOTween.KillAll();

            // 맵 로드 데이터 설정 
            Map_RouletteManager.mapPool.Clear();
            Map_RouletteManager.mapPool.AddRange(new int[] { 0, 1, 2, 3 });

            SceneManager.LoadScene("MainMenuScene");
        }
    }
    #endregion
}
