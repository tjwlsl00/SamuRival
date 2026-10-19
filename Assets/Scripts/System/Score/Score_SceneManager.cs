using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class Score_SceneManager : MonoBehaviour
{
    void Start()
    {
        if (!Global_ScoreManger.Instance.isRedWin && !Global_ScoreManger.Instance.isBlueWin)
        {
            StartCoroutine(MoveToMapScene());
        }
    }

    #region 맵 이동 
    // Map
    public IEnumerator MoveToMapScene()
    {
        // 스코어 UI 연출 시간 대기     
        yield return new WaitForSeconds(3f);

        if (Map_RouletteManager.mapPool != null && Map_RouletteManager.mapPool.Count > 0)
        {
            Debug.Log("맵 로테이션 하러 이동!");
            SceneManager.LoadScene("Map");
        }
    }
    // MainMenu
    public void GobackMenu()
    {
        DOTween.KillAll();

        // 맵 로드 데이터 설정 
        Map_RouletteManager.mapPool.Clear();
        Map_RouletteManager.mapPool.AddRange(new int[] { 0, 1, 2, 3 });

        SceneManager.LoadScene("MainMenuScene");
    }
    #endregion
}