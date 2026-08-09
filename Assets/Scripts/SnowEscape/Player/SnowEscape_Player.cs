using UnityEngine;
using System.Collections;

public class SnowEscape_Player : MonoBehaviour
{
    public enum TeamSide { Red, Blue };
    public TeamSide currentTeamSide = TeamSide.Red;

    private Rigidbody rb;
    private Vector3 moveInput;

    [Header("적 플레이어 참조/오브젝트 게이지")]
    [SerializeField] GameObject enemyPlayer;

    [Header("입력 관련")]
    private string horizontalAxisName;
    private KeyCode dashKey;

    [Header("움직임 변수")]
    [SerializeField] float moveSpeed;
    [SerializeField] float stunSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float ultSpeed;
    public float velocitySpeed = 0f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float turnSpeed = 360f;
    // 궁극기
    public float maxGauge = 30f;
    private float initialGauge = 0f;
    public float currentGauge;
    [SerializeField] float ultTime = 5f;

    [Header("상태 관련")]
    public bool isMoving = false;
    public bool isDashing = false;
    public bool isStun = false;
    public bool isUlting = false;

    [Header("효과 관련")]
    public SkinnedMeshRenderer characterRenderer;
    [SerializeField] ParticleSystem stunStar;

    // 스크립트 참조 
    private SnowEscape_PlayerUI snowEscape_PlayerUI;
    private SnowEscape_PlayerSoundManager snowEscape_PlayerSoundManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        characterRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // 스크립트 참조 
        snowEscape_PlayerUI = GetComponent<SnowEscape_PlayerUI>();
        snowEscape_PlayerSoundManager = GetComponent<SnowEscape_PlayerSoundManager>();

        // 팀에 따른 입력 키 매핑
        MappingInputKey();

        // 게이지 초기화
        currentGauge = initialGauge;
    }

    #region 플레이어 키 매핑
    private void MappingInputKey()
    {
        // 이동 방향키 
        horizontalAxisName = (currentTeamSide == TeamSide.Red) ? "Horizontal_Red" : "Horizontal_Blue";

        // 대쉬 키 
        dashKey = (currentTeamSide == TeamSide.Red) ? KeyCode.DownArrow : KeyCode.S;
    }
    #endregion

    void Update()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Ready || SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.End)
        {
            rb.isKinematic = true;
            isMoving = false;
        }
        else
        {
            rb.isKinematic = false;
            isMoving = true;

            // -------------
            // 방향키 입력(좌/우)
            // -------------
            InputMove();

            // 궁극기 
            Ult();

            if (!isUlting && !isStun)
            {
                // -------------
                // 대쉬 
                // -------------
                if (Input.GetKey(dashKey))
                {
                    isDashing = true;
                }
                else
                {
                    isDashing = false;
                }
            }
        }
    }

    // 물리 처리 
    void FixedUpdate()
    {
        if (isMoving)
        {
            // 캐릭터 회전 처리 
            RotationProcessing();

            // 캐릭터 움직임 처리 
            MotionProcessing();
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    #region 키 입력 / 움직임 처리 / 회전 처리 
    // ---------
    // 방향키 입력 
    // ---------
    private void InputMove()
    {
        float h = Input.GetAxisRaw(horizontalAxisName);
        moveInput = new Vector3(h, 0f, 0f).normalized;
    }
    // ---------
    // 움직임 처리  
    // ---------
    private void MotionProcessing()
    {
        // 방향, 속도 
        Vector3 moveDirection = transform.forward;
        float currentSpeed = moveSpeed;

        // -------------
        // 상태별 목표 속도 
        // -------------
        if (isUlting)
        {
            currentSpeed = ultSpeed;
        }
        else
        {
            if (isStun)
            {
                currentSpeed = stunSpeed;
            }
            else if (isDashing)
            {
                currentSpeed = dashSpeed;
            }
        }

        // -----------------------
        // 상태별 속도 순간이동  연산 
        // -----------------------
        if (isStun && velocitySpeed > stunSpeed)
        {
            velocitySpeed = stunSpeed;
        }
        else if (isUlting && velocitySpeed < ultSpeed)
        {
            velocitySpeed = ultSpeed;
        }
        else
        {
            velocitySpeed = Mathf.Lerp(velocitySpeed, currentSpeed, acceleration * Time.deltaTime);
        }

        Vector3 targetVelocity = moveDirection * velocitySpeed;
        Vector3 currentVelocity = rb.linearVelocity;

        float currentAcc = acceleration;

        if (isStun || isUlting)
        {
            currentAcc = acceleration * 2f;
        }

        // 관성 
        float smoothX = Mathf.MoveTowards(currentVelocity.x, targetVelocity.x, currentAcc * Time.fixedDeltaTime);
        float smoothZ = Mathf.MoveTowards(currentVelocity.z, targetVelocity.z, currentAcc * Time.fixedDeltaTime);

        // 이동 적용
        rb.linearVelocity = new Vector3(smoothX, currentVelocity.y, smoothZ);
    }
    // ---------
    // 회전 처리  
    // ---------
    private void RotationProcessing()
    {
        // 회전값 변수 
        float targetY = 0f;

        if (moveInput.x > 0f)
        {
            targetY = 25f;
        }
        else if (moveInput.x < 0f)
        {
            targetY = -25f;
        }
        else
        {
            targetY = 0f;
        }

        // 최종 회전 목표 값 설정 
        Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);

        // 현재 회전 -> 목표 회전 계산 
        Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        // 리지드 바디 회전 적용
        rb.MoveRotation(nextRotation);

        // 물리 충돌로 인한 회전 속도 0으로 초기화 
        rb.angularVelocity = Vector3.zero;
    }
    #endregion

    #region 궁극기
    void Ult()
    {
        float autoChargeCauge = 0f;

        if (this.transform.position.z > enemyPlayer.transform.position.z)
        {
            autoChargeCauge = 0.5f;
        }
        else
        {
            autoChargeCauge = 1f;
        }

        currentGauge += autoChargeCauge * Time.deltaTime;

        // -------
        // Max 게이지 달성 
        // -------
        if (!isUlting && currentGauge >= maxGauge)
        {
            currentGauge = initialGauge;
            isUlting = true;

            StartCoroutine(UltRoutine());
        }
    }

    IEnumerator UltRoutine()
    {
        Debug.Log("3초간 속도 80");

        // 궁극기 발동 효과

        // -----------
        // UI
        // -----------
        if (snowEscape_PlayerUI != null)
        {
            StartCoroutine(snowEscape_PlayerUI.UltEffectRoutine(snowEscape_PlayerUI.ultEffectPanel));
        }

        // -----------
        // 사운드
        // -----------
        if (snowEscape_PlayerSoundManager != null)
        {
            snowEscape_PlayerSoundManager.PlayUltClip();
        }

        yield return new WaitForSeconds(ultTime);

        isUlting = false;
    }
    #endregion

    #region 장애물 충돌 처리 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tree"))
        {
            if (!isUlting)
            {
                // 스턴 중복 실행 방지 
                if (!isStun)
                {
                    Debug.Log("장애물에 부딫힘");

                    // 깜빡임
                    StartCoroutine(BlinkRoutine());

                    // 사운드 
                    snowEscape_PlayerSoundManager.PlayStunClip();
                }
            }

            // 충돌 장애물 삭제 
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("GoalLine"))
        {
            if (currentTeamSide == TeamSide.Red)
            {
                SnowEscape_GameManager.Instance.isRedWin = true;
            }
            else
            {
                SnowEscape_GameManager.Instance.isBlueWin = true;
            }

            SnowEscape_GameManager.Instance.gameDirection = SnowEscape_GameManager.GameDirection.End;
        }
    }

    IEnumerator BlinkRoutine()
    {
        Debug.Log("스턴 시작");

        isStun = true;

        // ---------
        // 게이지 추가 
        // ---------
        if (enemyPlayer != null)
        {
            float objGauge = 0f;

            // Z축 기준 
            if (this.transform.position.z > enemyPlayer.transform.position.z)
            {
                objGauge = 1.5f;
            }
            else
            {
                objGauge = 3f;
            }

            // 게이지 증가 
            currentGauge += objGauge;

            if (currentGauge > maxGauge) currentGauge = maxGauge;
        }

        // ---------
        // 파티클 재생
        // ---------
        if (stunStar != null)
        {
            stunStar.gameObject.SetActive(true);
            stunStar.Play();
        }

        // 깜빡임 횟수와 간격 
        int blinkCount = 4;
        float blinkInterval = 0.25f;

        for (int i = 0; i < blinkCount; i++)
        {
            // 투명
            characterRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);

            // 불투명
            characterRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        Debug.Log("스턴 종료");

        isStun = false;

        // ---------
        // 파티클 멈추기
        // ---------
        if (stunStar != null)
        {
            stunStar.Stop();
            stunStar.gameObject.SetActive(false);
        }
    }
    #endregion
}