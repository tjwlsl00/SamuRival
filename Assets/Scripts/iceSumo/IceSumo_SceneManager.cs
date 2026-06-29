using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class IceSumo_SceneManager : MonoBehaviour
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

    // 씬 재로드 
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoBackMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public IEnumerator MoveToScene()
    {
        // 5초 대기
        yield return new WaitForSeconds(5f);

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
}
