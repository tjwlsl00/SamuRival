using UnityEngine;
using System.Collections.Generic;

public class Map_RouletteManager : MonoBehaviour
{
    // 사용 가능한 맵 리스트(씬 재로드시에도 값 유지)
    public static List<int> mapPool = new List<int> { 0, 1, 2,3};

    // 외부 
    private Map_UIManager map_UIManager;

    void Awake()
    {
        map_UIManager = GetComponent<Map_UIManager>();
    }

    void Start()
    {
        StartRoulette();
    }

    #region 맵 룰렛
    void StartRoulette()
    {
        if (mapPool.Count > 0)
        {
            int randomIndex = Random.Range(0, mapPool.Count);
            int selectedMap = mapPool[randomIndex];

            // 맵 중복 방지
            mapPool.RemoveAt(randomIndex);

            // UI 룰렛 효과 
            StartCoroutine(map_UIManager.PlayRoulette(selectedMap));

            // 선택된 맵 인자값 전달 
            Debug.Log(selectedMap + "번째 맵이 선택되었습니다.");
            Global_DirectionManager.Instance.SelectedMapIndex = selectedMap;
        }
        else
        {
            Debug.Log("모든 맵 플레이 완료");
            return;
        }
    }
    #endregion
}
