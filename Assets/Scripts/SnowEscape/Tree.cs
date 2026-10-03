using UnityEngine;

public class Tree : MonoBehaviour
{
    // 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snowball"))
        {
            Debug.Log("나무 - 스노우볼 충돌");
            Destroy(gameObject);
        }
    }
}