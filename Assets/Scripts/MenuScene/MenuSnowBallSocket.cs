using UnityEngine;

public class MenuSnowBallSocket : MonoBehaviour
{
    [SerializeField] GameObject snowballPrefab;
    [SerializeField] Transform Socket;
    [SerializeField] float throwForce = 10f;
    
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

        // 부모 해제 
        currentSnowball.transform.SetParent(null);

        Rigidbody rigidbody = currentSnowball.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;

            // 던지는 방향 정면 + 위(포물선)
            Vector3 baseDirection = (transform.forward + transform.up * 0.1f).normalized;

            // 던지기 
            rigidbody.AddForce(baseDirection * throwForce, ForceMode.Impulse);
        }

        Destroy(currentSnowball, 3f);
        currentSnowball = null;
    }
}