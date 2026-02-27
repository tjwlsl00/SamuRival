using Unity.Mathematics;
using UnityEngine;

public class SnowballSocket : MonoBehaviour
{
    [SerializeField] GameObject snowballPrefab;
    [SerializeField] Transform Socket;
    [SerializeField] float throwForce = 10f;
    // 외부 
    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    private GameObject currentSnowball;

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

        Destroy(currentSnowball, 3f);
        currentSnowball = null;

        if(currentSnowball == null)
        {
            player.sideAngle = 0;
        }
    }
}
