using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Score_SceneManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(MoveToMapScene());
    }

    #region 맵 이동 
    public IEnumerator MoveToMapScene()
    {
        if (Global_ScoreManger.Instance.isRedWin)
        {
            // 스코어 UI 연출 시간 대기 
            yield return new WaitForSeconds(3f);

            Debug.Log("레드의 승리로 게임 마무리!");
            SceneManager.LoadScene("RedWin");
        }
        else if (Global_ScoreManger.Instance.isBlueWin)
        {
            // 스코어 UI 연출 시간 대기              
            yield return new WaitForSeconds(3f);

            Debug.Log("블루의 승리로 게임 마무리!");
            SceneManager.LoadScene("BlueWin");
        }
        else
        {
            // 스코어 UI 연출 시간 대기     
            yield return new WaitForSeconds(3f);

            if (Map_RouletteManager.mapPool != null && Map_RouletteManager.mapPool.Count > 0)
            {
                Debug.Log("맵 로테이션 하러 이동!");
                SceneManager.LoadScene("Map");
            }
        }
    }
    #endregion
}