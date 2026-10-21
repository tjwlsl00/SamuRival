using UnityEngine;

public class Global_DirectionManager : MonoBehaviour
{
    // 싱글톤
    public static Global_DirectionManager Instance;

    // 맵 인덱스 
    public int SelectedMapIndex;

    // 모든 선택 상태
    public bool isIndivisual = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}