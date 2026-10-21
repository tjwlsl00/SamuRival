using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loading_SceneManager : MonoBehaviour
{
    // 오디오 
    private AudioSource audioSource;

    // 백그라운드 준비(렉 방지)
    private AsyncOperation nextSceneProcess;

    // 맵 인덱스 저장 전역 변수
    private int currentTargetIndex;

    // 맵 이동 코루틴
    private Coroutine moveCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    #region 씬 로드 준비
    public void PreloadMap(int targetIndex)
    {
        // 전역변수에 저장 
        currentTargetIndex = targetIndex;
        Debug.Log(currentTargetIndex);

        string sceneName = GetSceneName(targetIndex);
        nextSceneProcess = SceneManager.LoadSceneAsync(sceneName);
        nextSceneProcess.allowSceneActivation = false;
    }

    private string GetSceneName(int index)
    {
        return index switch
        {
            0 => "YukiHoi",
            1 => "TonTon",
            2 => "Archering",
            3 => "IceSumo",
            4 => "SnowEscape",
            _ => "DefaultScene"
        };
    }

    public void StartMoveToMap()
    {
        moveCoroutine = StartCoroutine(MoveToMap());
    }

    public IEnumerator MoveToMap()
    {
        yield return new WaitForSeconds(3f);

        ExecuteMapChange();
    }

    // 설명 후 맵 이동 
    private void ExecuteMapChange()
    {
        if (nextSceneProcess != null)
        {
            nextSceneProcess.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(GetSceneName(currentTargetIndex));
        }
    }
    #endregion
}