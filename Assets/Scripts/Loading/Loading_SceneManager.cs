using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading_SceneManager : MonoBehaviour
{
    // 백그라운드 준비(렉 방지)
    private AsyncOperation nextSceneProcess;

    #region 씬 로드 준비
    private string GetSceneName(int index)
    {
        return index switch
        {
            0 => "YukiHoi",
            1 => "TonTon",
            2 => "Archering",
            3 => "IceSumo",
            4 => "TonTon",
            _ => "DefaultScene"
        };
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