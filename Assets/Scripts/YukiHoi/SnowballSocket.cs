using UnityEngine;

public class SnowballSocket : MonoBehaviour
{
    [Header("오브젝트 참조")]
    [SerializeField] GameObject snowballPrefab;
    [SerializeField] GameObject SocketParticle;
    [SerializeField] Transform Socket;
    private Snowball snowball;
    private GameObject currentSnowball;

    [Header("변수 참조")]
    [SerializeField] float throwForce = 10f;

    // 스크립트 참조 
    private Player player;
    private PlayerState playerState;

    void Awake()
    {
        player = GetComponent<Player>();
        playerState = GetComponent<PlayerState>();
    }

    #region 소켓 파티클 토글
    public void ToggleSocketParticle()
    {   
        Debug.Log("소켓 파티클 토글 ");

        SocketParticle.SetActive(!SocketParticle.activeSelf);
    }
    #endregion
    
    #region 눈덩이 생성 / 삭제 
    public void OnCreateSnowball()
    {
        currentSnowball = Instantiate(snowballPrefab, Socket.position, Socket.rotation);

        // 부모 설정
        currentSnowball.transform.SetParent(Socket);

        Rigidbody rigidbody = currentSnowball.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        // 파티클(눈덩이)
        snowball = currentSnowball.GetComponent<Snowball>();
        if (snowball != null)
        {
            if (playerState.Item1Effect)
            {
                snowball.ToggleSnowballParticle();
            }
        }
    }

    public void OnReleaseSnowball()
    {
        if (currentSnowball == null) return;

        float horizontalOffset = 0f;

        // 던지는 각도 설정
        if (player.sideAngle == 0)
        {
            horizontalOffset = -10f;
            Debug.Log("눈덩이 각도 왼쪽 -0.5f");
        }
        else
        {
            horizontalOffset = 10f;
            Debug.Log("눈덩이 각도 오른쪽 -0.5f");
        }

        // 부모 해제 
        currentSnowball.transform.SetParent(null);

        Rigidbody rigidbody = currentSnowball.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;

            // 던지는 방향 정면 + 위(포물선)
            Vector3 baseDirection = (transform.forward + transform.up * 0.2f).normalized;

            // 쿼터니언 각도 회전(왼쪽/오른쪽)
            Quaternion rotation = Quaternion.Euler(0, horizontalOffset, 0);

            // 최종 방향 
            Vector3 finalDirection = rotation * baseDirection;

            // 던지기 
            rigidbody.AddForce(finalDirection * throwForce, ForceMode.Impulse);
        }

        // 파티클 토글
        snowball = currentSnowball.GetComponent<Snowball>();
        if (snowball != null && playerState.Item1Effect)
        {
            snowball.ToggleSnowballParticle();
        }

        // 오브젝트 삭제 
        Destroy(currentSnowball, 3f);

        currentSnowball = null;

        if (currentSnowball == null)
        {
            player.sideAngle = 0;
        }
    }
    #endregion
}
