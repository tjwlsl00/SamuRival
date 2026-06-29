using UnityEngine;

public class SnowEscape_Player : MonoBehaviour
{
    public enum TeamSide { Red, Blue };
    public TeamSide currentTeamSide = TeamSide.Red;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;

    [Header("입력 관련")]
    private string horizontalAxisName;
    private string verticalAxisName;

    [Header("움직임 변수")]
    [SerializeField] float moveSpeed;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float turnSpeed = 360f;

    [Header("상태 관련")]
    private bool isMoving = false;
    private bool isStun = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // 팀에 따른 입력 키 매핑
        MappingInputKey();
    }

    #region 플레이어 키 매핑
    private void MappingInputKey()
    {
        horizontalAxisName = (currentTeamSide == TeamSide.Red) ? "Horizontal_Red" : "Horizontal_Blue";
        verticalAxisName = (currentTeamSide == TeamSide.Red) ? "Vertical_Red" : "Vertical_Blue";
    }
    #endregion

    void Update()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Ready || SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.End) return;

        if (isStun) return;
        InputMove();
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
            // 스턴 중 물리 무시 
            if (isStun) return;

            // 움직임 처리 
            Vector3 targetVelocity = moveInput * moveSpeed;
            moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

            // 회전 처리 
            if (moveInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            }
        }
    }

    #region 이동 / 회전
    private void InputMove()
    {
        float h = Input.GetAxisRaw(horizontalAxisName);
        float v = Input.GetAxisRaw(verticalAxisName);
        moveInput = new Vector3(h, 0f, v).normalized;
    }
    #endregion



}
