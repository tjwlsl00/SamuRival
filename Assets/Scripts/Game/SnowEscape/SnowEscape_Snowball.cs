using UnityEngine;
using Unity.Cinemachine;

public class SnowEscape_Snowball : MonoBehaviour
{
    [Header("변수 참조")]
    public float moveSpeed = 20f;
    private bool isMoving = false;

    private Rigidbody rb;

    [Header("진동 관련")]
    [SerializeField] float minShackDistance = 5f;
    [SerializeField] float minShackIntensity = 5f;

    // ---------
    // 플레이어 참조
    // ---------
    private GameObject redPlayer;
    private GameObject bluePlayer;
    // ---------
    // 카메라 참조
    // ---------
    private CinemachineCamera redVirtualCamera;
    private CinemachineCamera blueVirtualCamera;
    private CinemachineBasicMultiChannelPerlin redCameraNoise;
    private CinemachineBasicMultiChannelPerlin blueCameraNoise;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // 플레이어 참조
        FindPlayer();

        // 카메라 참조 
        FindPlayersCamera();
    }

    void Update()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Start)
        {
            isMoving = true;
            rb.isKinematic = false;

            if (isMoving)
            {
                ShakePlayerCamera();
            }
        }
        else
        {
            isMoving = false;
            rb.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
        }
    }

    #region 이동
    private void Move()
    {
        // 이동 방향
        Vector3 moveDirection = transform.forward;
        rb.AddForce(moveDirection * moveSpeed);

        // --------
        // 가속 방지 
        // --------
        float currentZSpeed = rb.linearVelocity.z;

        if (Mathf.Abs(currentZSpeed) > moveSpeed)
        {
            // 현재 굴러가고 있는 방향 
            float directionSign = Mathf.Sign(currentZSpeed);

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, moveSpeed * directionSign);
        }
    }
    #endregion

    #region 플레이어 참조
    private void FindPlayer()
    {
        Debug.Log("플레이어 참조");

        redPlayer = GameObject.FindGameObjectWithTag("Red");
        bluePlayer = GameObject.FindGameObjectWithTag("Blue");
    }
    #endregion

    #region 카메라 
    // ---------
    // 카메라 참조 
    // ---------
    private void FindPlayersCamera()
    {
        Debug.Log("플레이어 카메라 참조");

        GameObject redCamera = GameObject.FindGameObjectWithTag("redCamera");
        if (redCamera != null)
        {
            redVirtualCamera = redCamera.GetComponent<CinemachineCamera>();
            if (redVirtualCamera != null)
            {
                redCameraNoise = redVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        GameObject blueCamera = GameObject.FindGameObjectWithTag("blueCamera");
        if (blueCamera != null)
        {
            blueVirtualCamera = blueCamera.GetComponent<CinemachineCamera>();
            if (blueVirtualCamera != null)
            {
                blueCameraNoise = blueVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }
    }
    // ---------
    // 카메라 진동
    // ---------
    private void ShakePlayerCamera()
    {
        // ---------
        // 위치 값
        // ---------
        if (redPlayer != null)
        {
            Transform redTransform = redPlayer.transform;

            float distance = Vector3.Distance(transform.position, redTransform.position);

            if (distance <= minShackDistance)
            {
                float distanceRatio = 1f - (distance / minShackDistance);
                float currentIntensity = distanceRatio * minShackIntensity;

                if (redCameraNoise != null)
                {
                    Debug.Log("카메라 흔들기");
                    redCameraNoise.AmplitudeGain = currentIntensity;
                }
            }
            else
            {
                if (redCameraNoise != null)
                {
                    Debug.Log("카메라 멈추기");
                    redCameraNoise.AmplitudeGain = 0f;
                }
            }
        }

        if (bluePlayer != null)
        {
            Transform blueTransform = bluePlayer.transform;

            float distance = Vector3.Distance(transform.position, blueTransform.position);

            if (distance <= minShackDistance)
            {
                float distanceRatio = 1f - (distance / minShackDistance);
                float currentIntensity = distanceRatio * minShackIntensity;

                if (blueCameraNoise != null)
                {
                    Debug.Log("카메라 흔들기");
                    blueCameraNoise.AmplitudeGain = currentIntensity;
                }
            }
            else
            {
                if (blueCameraNoise != null)
                {
                    Debug.Log("카메라 멈추기");
                    blueCameraNoise.AmplitudeGain = 0f;
                }
            }
        }
    }
    #endregion

    #region 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        if (!isMoving) return;

        if (other.gameObject.CompareTag("Red"))
        {
            HandleCommonTrigger();

            SnowEscape_GameManager.Instance.isRedWin = false;
            SnowEscape_GameManager.Instance.isBlueWin = true;
        }
        else if (other.gameObject.CompareTag("Blue"))
        {
            HandleCommonTrigger();

            SnowEscape_GameManager.Instance.isRedWin = true;
            SnowEscape_GameManager.Instance.isBlueWin = false;
        }
    }

    // 물리 멈춤 및 게임 종료 처리 
    private void HandleCommonTrigger()
    {
        Debug.Log("물리 멈춤 및 게임 종료 처리");
        
        // ---------
        // 물리 멈추기 
        // ---------
        isMoving = false;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 게임 상태 변경
        if (SnowEscape_GameManager.Instance != null)
        {
            SnowEscape_GameManager.Instance.gameDirection = SnowEscape_GameManager.GameDirection.End;
        }
    }
    #endregion
}