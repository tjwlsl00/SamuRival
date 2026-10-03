using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Loading_UIManager : MonoBehaviour
{
    // UI 연결
    [SerializeField] GameObject[] DescriptionSets;

    // 외부 
    private Loading_SceneManager loading_SceneManager;
    private Loading_SoundManager loading_SoundManager;

    void Awake()
    {
        loading_SceneManager = GetComponent<Loading_SceneManager>();
        loading_SoundManager = GetComponent<Loading_SoundManager>();
    }

    IEnumerator Start()
    {
        // 전역변수 데이터 저장 및 맵 로드 준비
        int targetIndex = Global_DirectionManager.Instance.SelectedMapIndex;
        loading_SceneManager.PreloadMap(targetIndex);

        // 전체 설명 패널 비활성화
        foreach (var set in DescriptionSets)
        {
            set.SetActive(false);
        }

        // 선택된 맵 인덱스 UI 활성화 
        GameObject activePanel = DescriptionSets[targetIndex];
        activePanel.SetActive(true);

        // 오디오 배열 가져오기
        AudioClip[] currentNarrationArray = loading_SoundManager.GetNarrationArray(targetIndex);

        // 자식 오브젝트 오브젝트 참조 
        int childCount = Mathf.Min(3, activePanel.transform.childCount);
        RectTransform[] childTransform = new RectTransform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            childTransform[i] = activePanel.transform.GetChild(i).GetComponent<RectTransform>();
            if (childTransform[i] != null)
            {
                childTransform[i].localScale = Vector3.zero;
                childTransform[i].gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.6f);

        // 자식 오브젝트 애니메이션 등장 
        for (int i = 0; i < childCount; i++)
        {
            if (currentNarrationArray != null && i < currentNarrationArray.Length)
            {
                AudioClip currentClip = currentNarrationArray[i];
                if (currentClip != null)
                {
                    loading_SoundManager.audioSource.clip = currentClip;
                    loading_SoundManager.audioSource.Play();
                }
            }

            // 소리 재생 동시에 스케일 연출 
            if (childTransform[i] != null)
            {
                childTransform[i].DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
            }

            // 마지막 자신 연출 이후 안기다리고 넘어감 
            if (i < childCount - 1)
            {
                yield return null;

                while (loading_SoundManager.audioSource.isPlaying)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(0.3f);
            }
        }

        // 남은 나레이션 대기 
        while (loading_SoundManager.audioSource.isPlaying)
        {
            yield return null;
        }

        // 씬 전환
        loading_SceneManager.StartMoveToMap();
    }
}
