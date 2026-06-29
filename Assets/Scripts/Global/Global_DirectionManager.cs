using UnityEngine;

public class Global_DirectionManager : MonoBehaviour
{
    // 싱글톤
    public static Global_DirectionManager Instance;

    // 맵 인덱스 
    public int SelectedMapIndex;

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
