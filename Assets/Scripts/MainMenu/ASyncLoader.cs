using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ASyncLoader : MonoBehaviour
{
    // 싱글톤
    public static ASyncLoader Instance { get; private set; }

    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;

    [SerializeField] private float fillSpeed = 1.5f;

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
    }

    public void StartAsyncLoading(string sceneName)
    {
        VisibleLoadingScreen();
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void VisibleLoadingScreen()
    {
        if (mainMenuScreen == null) mainMenuScreen = GameObject.Find("mainMenuScreen");
        if (loadingScreen == null) loadingScreen = GameObject.Find("loadingScreen");

        if (mainMenuScreen != null) mainMenuScreen.SetActive(false);
        if (loadingScreen != null) loadingScreen.SetActive(true);

        // 슬라이더 초기화
        if (loadingSlider != null) loadingSlider.value = 0f;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.allowSceneActivation = false;

        float targetProgress = 0f;

        while (true)
        {
            // 유니티 실제 로딩 진행률 계산
            float realProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // 연출용 타겟 진행률 설정
            targetProgress = realProgress;

            if (loadingSlider != null)
            {
                // 현재 슬라이더 값에서 targetProgress까지 fillSpeed 속도로 부드럽게 이동
                loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, targetProgress, Time.deltaTime * fillSpeed);
            }

            if (asyncOperation.progress >= 0.9f && Mathf.Approximately(loadingSlider.value, 1f))
            {
                // 게이지가 다 찬 것을 확인할 수 있도록 마지막 눈요기 대기 시간
                yield return new WaitForSeconds(0.3f);

                // 실제 씬 전환 허용
                asyncOperation.allowSceneActivation = true;

                yield return new WaitUntil(() => asyncOperation.isDone);
                yield break;
            }
            yield return null;
        }
    }
}