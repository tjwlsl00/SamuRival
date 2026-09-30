using UnityEngine;
using System.Collections;

public class IceSumo_Player : MonoBehaviour
{
    public enum TeamSide { Red, Blue };
    public TeamSide currentTeamSide = TeamSide.Red;

    private Rigidbody rigidbody;
    private Vector3 moveInput;
    private Vector3 moveVelocity;

    [Header("입력 관련")]
    private string horizontalAxisName;
    private string verticalAxisName;
    private KeyCode chargeKey;

    [Header("움직임 관련")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float turnSpeed = 720f;

    [Header("돌진 관련")]
    public float maxChargeTime = 1.2f;
    public float currentChargeTime = 0f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.3f;

    [Header("추락 관련")]
    [SerializeField] float fallThreshold = -8f;

    [Header("상태 관련")]
    private bool wasMoving = false;
    public bool isCharging = false;
    private bool wasCharging = false;
    public bool isDashing = false;
    public bool isFalling = false;
    private float finalChargeRatio = 0f;

    [Header("넉백 관련")]
    [SerializeField] private AnimationCurve knockBackCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // 스크립트 참조
    private IceSumo_PenguinAnimation iceSumo_PenguinAnimation;
    private IceSumo_PlayerSoundManager iceSumo_PlayerSoundManager;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // 팀에 따른 입력 키 매핑
        MappingInputKey();

        iceSumo_PenguinAnimation = GetComponent<IceSumo_PenguinAnimation>();
        iceSumo_PlayerSoundManager = GetComponent<IceSumo_PlayerSoundManager>();
    }

    #region 플레이어 키 매핑
    void MappingInputKey()
    {
        horizontalAxisName = (currentTeamSide == TeamSide.Red) ? "Horizontal_Red" : "Horizontal_Blue";
        verticalAxisName = (currentTeamSide == TeamSide.Red) ? "Vertical_Red" : "Vertical_Blue";
        chargeKey = (currentTeamSide == TeamSide.Red) ? KeyCode.RightControl : KeyCode.LeftControl;
    }
    #endregion

    void Update()
    {
        // 게임 일시정지
        if(IceSumo_GameManager.Instance.isPaused) return;

        // 게임 종료 시 키 입력 무시 
        if (IceSumo_GameManager.Instance.gameDirection == IceSumo_GameManager.GameDirection.Ready || IceSumo_GameManager.Instance.gameDirection == IceSumo_GameManager.GameDirection.End)
        {
            return;
        }
        else
        {
            // 대쉬 중 입력 무시 
            if (isFalling || isDashing) return;

            // 키 입력
            InputMove();
            InputDashKey();

            // 체크 플레이어 상태 
            CheckPlayerMove();
            CheckPlayerCharging();
            CheckPlayerFalling();
        }
    }

    // 물리 업데이트 
    void FixedUpdate()
    {
        if (IceSumo_GameManager.Instance.gameDirection == IceSumo_GameManager.GameDirection.End)
        {
            // 제자리 멈추기
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            // 걷는 상태면 멈추기 
            iceSumo_PenguinAnimation.PlayWalkAnim(0);
        }
        else
        {
            // 대쉬 중 조작불가
            if (isDashing) return;

            // 차징 속도: 기본 속도/2, 기본 속도: 5f
            float currentMaxSpeed = isCharging ? moveSpeed / 2 : moveSpeed;

            Vector3 targetVelocity = moveInput * currentMaxSpeed;
            moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

            rigidbody.linearVelocity = new Vector3(moveVelocity.x, rigidbody.linearVelocity.y, moveVelocity.z);

            // 회전 처리 
            if (moveInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            }
        }
    }

    #region 상태 체크 (떨어짐 / 움직임 / 차징)
    private void CheckPlayerMove()
    {
        float speed = moveInput.magnitude;
        bool isMoving = moveInput.magnitude > 0;

        // -----------
        // 애니메이션
        // -----------
        if (wasMoving && !isMoving)
        {
            // 걷다가 멈추면 Idle 애니메이션 처음부터 
            iceSumo_PenguinAnimation.animator.Play("Move", 0, 0f);
        }

        wasMoving = isMoving;
        iceSumo_PenguinAnimation.PlayWalkAnim(speed);

        // -----------
        // 사운드
        // -----------
        if (isMoving && !isDashing)
        {
            iceSumo_PlayerSoundManager.PlayWalkClip(true);
        }
        else
        {
            iceSumo_PlayerSoundManager.PlayWalkClip(false);
        }
    }

    private void CheckPlayerCharging()
    {
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Min(currentChargeTime, maxChargeTime);
        }

        int targetIndex = (currentTeamSide == TeamSide.Red) ? 0 : 1;

        // -----------
        // UI
        // -----------
        // 차징을 시작한 최초의 순간
        if (isCharging && !wasCharging)
        {
            IceSumo_UIManager.Instance.ShackChargePanel(targetIndex);
        }
        // 차징을 멈춘(손을 뗀) 최초의 순간
        else if (!isCharging && wasCharging)
        {
            IceSumo_UIManager.Instance.StopShackChargePanel(targetIndex);
        }

        // 현재 상태를 기록하여 다음 프레임 비교용으로 저장
        wasCharging = isCharging;
    }

    private void CheckPlayerFalling()
    {
        // ----------
        // 사운드 
        // ----------
        if (this.transform.position.y < fallThreshold)
        {
            isFalling = true;
            iceSumo_PlayerSoundManager.PlayFallingClip();
        }
        else
        {
            isFalling = false;
        }
    }
    #endregion

    #region 이동
    private void InputMove()
    {
        // 움직임
        float h = Input.GetAxisRaw(horizontalAxisName);
        float v = Input.GetAxisRaw(verticalAxisName);
        moveInput = new Vector3(h, 0f, v).normalized;
    }
    #endregion

    #region 대쉬 
    // 키 입력 
    private void InputDashKey()
    {
        if (Input.GetKeyDown(chargeKey))
        {
            isCharging = true;
            currentChargeTime = 0f;
        }

        if (Input.GetKeyUp(chargeKey) && isCharging)
        {
            // 최종 차지 비율 계산
            finalChargeRatio = Mathf.Max(currentChargeTime / maxChargeTime, 0.1f);

            isCharging = false;
            currentChargeTime = 0f;

            StartCoroutine(DashRoutine());

            // 사운드 효과
            iceSumo_PlayerSoundManager.PlayDashClip();
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;

        // 속도 초기화
        rigidbody.linearVelocity = Vector3.zero;

        // 물리(방향 / 대쉬 힘) 
        Vector3 dashDirection = transform.forward;
        rigidbody.AddForce(dashDirection * dashSpeed * finalChargeRatio, ForceMode.Impulse);

        // 대쉬 유지 시간
        yield return new WaitForSeconds(dashDuration);

        Vector3 startDecelVelocity = rigidbody.linearVelocity;
        float elapsed = 0f;
        float decelTime = 0.2f;

        // 관성
        while (elapsed < decelTime)
        {
            elapsed += Time.fixedDeltaTime;
            rigidbody.linearVelocity = Vector3.Lerp(startDecelVelocity, startDecelVelocity * 0.1f, elapsed / decelTime);
            yield return new WaitForFixedUpdate(); // 물리 프레임 대기
        }

        isDashing = false;
    }
    #endregion

    #region 충돌 처리 지면 / 플레이어 밀기
    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어
        if (isDashing)
        {
            string targetTag = (currentTeamSide == TeamSide.Red) ? "Blue" : "Red";

            // 타겟 태그 오브젝트가 아니면 return
            if(!collision.gameObject.CompareTag(targetTag)) return;

            // 물리
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (enemyRigidbody != null)
            {
                // 밀어낼 방향 계산
                Vector3 pushDirection = (collision.transform.position - transform.position);
                pushDirection.y = 0f;

                if (pushDirection == Vector3.zero)
                {
                    // 플레이어가 바라보는 방향
                    pushDirection = transform.forward;
                }
                else
                {
                    pushDirection.Normalize();
                }

                // 밀려나갈 총 거리 
                float knockBackDistance = dashSpeed / 2 * finalChargeRatio * 1.5f * 0.5f;

                StartCoroutine(KnockBackRoutine(enemyRigidbody, pushDirection, knockBackDistance, 0.25f));

                // 사운드
                iceSumo_PlayerSoundManager.PlayCrashClip();
            }
        }
    }

    IEnumerator KnockBackRoutine(Rigidbody targetRb, Vector3 direction, float distance, float duration)
    {
        if (targetRb != null)
        {
            targetRb.linearVelocity = Vector3.zero;
            targetRb.angularVelocity = Vector3.zero;

            targetRb.isKinematic = true;

            Vector3 startPosition = targetRb.position;
            Vector3 endPosition = startPosition + (direction * distance);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (targetRb == null) yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 애니메이션 커브
                float curveT = knockBackCurve.Evaluate(t);

                // 부드럽게 위치 이동 
                targetRb.MovePosition(Vector3.Lerp(startPosition, endPosition, curveT));

                yield return null;
            }

            if (targetRb != null)
            {
                targetRb.isKinematic = false;
            }
        }
    }

    // 데드존
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("DeadZone"))
        {
            DeadZone deadZone = collider.GetComponent<DeadZone>();

            // 태그 값 전달
            deadZone.PlayerFallen(gameObject.tag);
        }
    }
    #endregion
}