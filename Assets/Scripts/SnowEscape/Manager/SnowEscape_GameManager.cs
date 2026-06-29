using UnityEngine;

public class SnowEscape_GameManager : MonoBehaviour
{
    // 싱글톤
    public static SnowEscape_GameManager Instance;

    public enum GameDirection { Ready, Start, End };
    public GameDirection gameDirection;

    [Header("데코 오브젝트 참조")]
    [SerializeField] GameObject decoObj;

    [Header("플레이어 오브젝트 참조")]
    [SerializeField] GameObject[] players;

    // 스크립트 참조
    private SnowEscape_SpawnManager snowEscape_SpawnManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        snowEscape_SpawnManager = GetComponent<SnowEscape_SpawnManager>();
    }

    void Start()
    {
        gameDirection = GameDirection.Ready;
    }

    public void InitialGameSetting()
    {
        // 데코 오브젝트 삭제
        Destroy(decoObj);

        // 게임 상태 업데이트
        gameDirection = GameDirection.Start;

        // 스노우볼 소환
        snowEscape_SpawnManager.SpawnSnowball();
    }

    public void FinalSetting(int playerIndex)
    {
        // 게임 상태 변경
        gameDirection = GameDirection.End;

        if (playerIndex == 0)
        {
            // 카메라 세팅 -


            // 캐릭터 애니메이션 


            // ui 세팅 -


            // 사운드 세팅 -
        }
        else
        {
            // 카메라 세팅 -


            // 캐릭터 애니메이션 


            // ui 세팅 -


            // 사운드 세팅 -
        }
    }
}