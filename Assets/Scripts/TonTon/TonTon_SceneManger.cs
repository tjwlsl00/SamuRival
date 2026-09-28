using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TonTon_SceneManger : MonoBehaviour
{
    // bool
    private bool isChanged = false;

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

        if (!Global_DirectionManager.Instance.isIndivisual)
        {
            SceneManager.LoadScene("Score");
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
    #endregion
}