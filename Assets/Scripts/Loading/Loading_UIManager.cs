using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Loading_UIManager : MonoBehaviour
{
    // UI 연결
    [SerializeField] GameObject[] DescriptionSets;

    // 외부 
    private Loading_SceneManager loading_SceneManager;

    void Awake()
    {
        loading_SceneManager = GetComponent<Loading_SceneManager>();
    }

    void Start()
    {
        // 전체 설명 패널 비활성화
        foreach (var set in DescriptionSets)
        {
            set.SetActive(false);
        }

        int targetIndex = Global_DirectionManager.Instance.SelectedMapIndex;

        // 선택된 맵 데이터에 의거해서 UI 세팅
        DescriptionSets[targetIndex].SetActive(true);

        // 선택된 맵 로드 준비 
        loading_SceneManager.PreloadMap(Global_DirectionManager.Instance.SelectedMapIndex);

        // 맵 로드 
        StartCoroutine(loading_SceneManager.MoveToMap(targetIndex));
    }
}
