using System.Collections;
using UnityEngine;

public class SnowEscape_Snowball : MonoBehaviour
{
    private Rigidbody rb;

    [Header("움직임 변수")]
    [SerializeField] float moveSpeed = 10f;

    // bool 
    private bool isMove = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        isMove = true;
    }

    void FixedUpdate()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.End)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            if (!isMove) return;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, moveSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Red"))
        {
            Debug.Log("레드 사망");

            SnowEscape_GameManager.Instance.FinalSetting(1);
        }
        else if (other.CompareTag("Blue"))
        {
            Debug.Log("블루 사망");

            SnowEscape_GameManager.Instance.FinalSetting(0);
        }
    }
}