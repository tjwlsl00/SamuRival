using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading_SceneManager : MonoBehaviour
{
    // 백그라운드 준비(렉 방지)
    private AsyncOperation nextSceneProcess;

    #region 씬 로드 준비
    // 0,2,4번 유키호이 1,3번 톤톤 
    private string GetSceneName(int index)
    {
        return (index % 2 == 0) ? "YukiHoi" : "TonTon";
    }

    public void PreloadMap(int targetIndex)
    {
        string sceneName = GetSceneName(targetIndex);
        nextSceneProcess = SceneManager.LoadSceneAsync(sceneName);
        nextSceneProcess.allowSceneActivation = false;
    }

    public IEnumerator MoveToMap(int index)
    {
        yield return new WaitForSeconds(3f);
        
        if (nextSceneProcess != null)
        {
            nextSceneProcess.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(GetSceneName(index));
        }
    }
    #endregion
}