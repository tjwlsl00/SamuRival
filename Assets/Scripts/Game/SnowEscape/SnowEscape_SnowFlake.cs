using UnityEngine;

public class SnowEscape_SnowFlake : MonoBehaviour
{
    [Header("추적 관련")]
    public Transform targetTransform;
    [SerializeField] Vector3 offset;
    private Quaternion fixedRotation;
    private float initialTargetY;
    private float initialTargetZ;

    void Start()
    {
        // 회전 방지
        fixedRotation = transform.rotation;

        // 오프셋 위치로 초기화
        transform.position = offset;

        // z축 위치 고정
        if (targetTransform != null)
        {
            initialTargetZ = targetTransform.position.y;
            initialTargetZ = targetTransform.position.z;
        }
    }

    void LateUpdate()
    {
        if (targetTransform != null)
        {
            transform.rotation = fixedRotation;

            float targetMoveY = targetTransform.position.y - initialTargetY;

            float targetMoveZ = targetTransform.position.z - initialTargetZ;


            float finalX = offset.x;
            float finalY = offset.y + targetMoveY;
            float finalZ = offset.z + targetMoveZ;

            transform.position = new Vector3(finalX, finalY, finalZ);
        }
    }
}
