using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class IceSumo_SceneManager : MonoBehaviour
{
    // bool
    private bool isChanged = false;

    // 스크립트 참조
    private IceSumo_RoundManager iceSumo_RoundManager;

    void Awake()
    {
        iceSumo_RoundManager = GetComponent<IceSumo_RoundManager>();
    }

    void Update()
    {
        // 맵 전환 테스트 시 활용
        if (isChanged) return;
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isChanged = true;
            StartCoroutine(MoveToScene());
        }
    }

    #region 씬 이동
    // ------
    // 메뉴
    // ------
    public void GoBackMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // ------
    // 재시작
    // ------
    public void RestartScene()
    {
        Time.timeScale = 1f;

        DOTween.KillAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ------
    // 테스트
    // ------
    public IEnumerator MoveToScene()
    {
        // 5초 대기
        yield return new WaitForSeconds(5f);

        // 데이터 초기화
        if(iceSumo_RoundManager != null)
        {
            iceSumo_RoundManager.ResetData();
        }

        // 스코어 맵 이동
        SceneManager.LoadScene("Score");
    }
    #endregion
}