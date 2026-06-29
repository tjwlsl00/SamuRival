using UnityEngine;

public class DecoSnowball : MonoBehaviour
{
    private Rigidbody rb;

    [Header("물리 변수")]
    [SerializeField] float moveSpeed = 300f;
    [SerializeField] float torqueMultiplier = 1.5f;
    private float ballRadius;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.maxAngularVelocity = 150f;
        ballRadius = transform.localScale.y / 2f;
    }

    void FixedUpdate()
    {
        // 전진
        Vector3 moveDirection = rb.linearVelocity.normalized;
        if (moveDirection == Vector3.zero)
        {
            moveDirection = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).normalized;
        }
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

        // 회전
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDirection).normalized;
        float calculatedTorque = (moveSpeed / ballRadius) * torqueMultiplier;
        rb.AddTorque(rotationAxis * calculatedTorque, ForceMode.Force);
    }
}
