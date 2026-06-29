using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    [Header("스톤 종류")]
    public Category category;
    public enum Category { DecideStone, Stone }

    [Header("팀 설정")]
    public Team myTeam;
    private KeyCode keyCode;
    public enum Team { RedStone, BlueStone };

    [Header("조작 상태")]
    [SerializeField] private StoneState currentState = StoneState.Ready;
    enum StoneState { Ready, Charging, Launched }

    [Header("UI 연결")]
    [SerializeField] RectTransform arrowTransform;
    [SerializeField] Slider forceSlider;

    [Header("방향 설정")]
    [SerializeField] float maxAngle = 45f;
    [SerializeField] float pingPongSpeed = 2f;
    [SerializeField] float currentAngle;

    [Header("게이지 설정")]
    [SerializeField] float maxForce = 20f;
    [SerializeField] float gaugeSpeed = 1.5f;
    [SerializeField] float currentForceRatio;
    private bool gaugeUp = false;

    [Header("스톤 발사")]
    private Rigidbody stoneRigidbody;
    private float stopTimer = 0f;
    private const float stopDuration = 0.5f;

    // bool 
    private bool isFired = false;

    void Awake()
    {
        stoneRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        SetUpKey();
    }

    void Update()
    {
        if (currentState == StoneState.Launched && isFired)
        {
            CheckStoneStop();
            return;
        }

        // 조작 중 
        bool isMyTurn = (myTeam == Team.RedStone && Archering_TurnManager.Instance.gameTurn == Archering_TurnManager.GameTurn.RedTurn ||
                        myTeam == Team.BlueStone && Archering_TurnManager.Instance.gameTurn == Archering_TurnManager.GameTurn.BlueTurn);

        if (isMyTurn)
        {
            SwitchStoneState();
        }
        else
        {
            return;
        }
    }

    public void InitUI(RectTransform arrow, Slider slider)
    {
        arrowTransform = arrow;
        forceSlider = slider;

        if (arrowTransform != null)
        {
            arrowTransform.gameObject.SetActive(true);
            arrowTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        if (forceSlider != null)
        {
            forceSlider.value = 0f;
            forceSlider.gameObject.SetActive(false);
        }

        // 상태 초기화
        currentState = StoneState.Ready;
        isFired = false;
    }

    #region 입력 키 세팅 
    private void SetUpKey()
    {
        if (myTeam == Team.RedStone)
        {
            keyCode = KeyCode.UpArrow;
        }
        else
        {
            keyCode = KeyCode.W;
        }
    }
    #endregion

    #region 화살표 / 게이지 / 발사
    private void UpdateDirection()
    {
        if (arrowTransform == null) return;

        currentAngle = Mathf.PingPong(Time.time * pingPongSpeed, maxAngle * 2) - maxAngle;
        arrowTransform.localRotation = Quaternion.Euler(90f, 0f, currentAngle);
    }

    private void UpdateForceGauge()
    {
        if (gaugeUp)
        {
            currentForceRatio += Time.deltaTime * gaugeSpeed;
            if (currentForceRatio >= 1f)
            {
                gaugeUp = false;
            }
        }
        else
        {
            currentForceRatio -= Time.deltaTime * gaugeSpeed;
            if (currentForceRatio <= 0f)
            {
                gaugeUp = true;
            }
        }
        forceSlider.value = currentForceRatio;
    }

    private void FireStone()
    {
        if (forceSlider != null) forceSlider.gameObject.SetActive(false);
        if (arrowTransform != null) arrowTransform.gameObject.SetActive(false);

        Vector3 fireDirection = arrowTransform.TransformDirection(Vector3.up);
        fireDirection.y = 0f;
        fireDirection.Normalize();

        float finalForce = currentForceRatio * maxForce;

        if (stoneRigidbody != null)
        {
            stoneRigidbody.AddForce(fireDirection * finalForce, ForceMode.Impulse);
        }
    }
    #endregion

    #region 조작 상태 / 정지 체크
    private void SwitchStoneState()
    {
        bool isKeyPressed = Input.GetKeyDown(keyCode);

        switch (currentState)
        {
            case StoneState.Ready:
                UpdateDirection();
                if (isKeyPressed)
                {
                    forceSlider.gameObject.SetActive(true);
                    currentState = StoneState.Charging;

                    // 사운드 효과
                    Archering_SoundManager.Instance.PlayButtnClip();
                }
                break;
            case StoneState.Charging:
                UpdateForceGauge();
                if (isKeyPressed)
                {
                    FireStone();

                    // 발사 됨 
                    isFired = true;
                    currentState = StoneState.Launched;

                    // 카메라 세팅 
                    Archering_CameraManager.Instance.SetFollowTarget(this.transform);

                    // 사운드 효과
                    Archering_SoundManager.Instance.PlayFireStone();
                }
                break;
        }
    }

    // 스톤이 멈췄을때 턴 종료 함수 호출 
    private void CheckStoneStop()
    {
        if (!isFired) return;

        if (stoneRigidbody != null)
        {
            if (stoneRigidbody.linearVelocity.magnitude < 0.1f)
            {
                stopTimer += Time.deltaTime;

                if (stopTimer >= stopDuration)
                {
                    isFired = false;

                    // 물리 정지 
                    stoneRigidbody.linearVelocity = Vector3.zero;
                    stoneRigidbody.angularVelocity = Vector3.zero;

                    // 순서 결정전 
                    if (category == Category.DecideStone)
                    {
                        // 던진 상태 업데이트
                        if (myTeam == Team.RedStone)
                        {
                            Archering_GameManager.Instance.isD_RThrowed = true;

                            // 디사이드 블루 스톤 스폰 
                            SpawnManager.Instance.SetUpDecideStone(1);

                            // 턴 초기화 
                            Archering_TurnManager.Instance.OnTurnEnd();
                        }
                        else
                        {
                            Archering_GameManager.Instance.isD_BThrowed = true;
                        }
                    }
                    // 턴 종료 
                    else
                    {
                        if (Archering_TurnManager.Instance != null)
                        {
                            // 턴 전환, 카메라 초기화 
                            Archering_TurnManager.Instance.OnTurnEnd();
                        }
                    }
                }
            }
            arrowTransform = null;
            forceSlider = null;
        }
    }
    #endregion

    #region 충돌 이벤트(다른 말이랑 충돌시)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("redStone") || collision.gameObject.CompareTag("blueStone") || collision.gameObject.CompareTag("decideStone"))
        {
            Rigidbody targetStone = collision.gameObject.GetComponent<Rigidbody>();

            // 밀어내기 
            if (targetStone != null)
            {
                float impactSpeed = collision.relativeVelocity.magnitude;

                Vector3 forceDirection = -collision.contacts[0].normal;

                float bonusForce = impactSpeed * 0.5f;

                targetStone.AddForce(forceDirection * bonusForce, ForceMode.Impulse);
            }

            // 충돌 효과음
            Archering_SoundManager.Instance.PlayStoneCrash();
        }
    }
    #endregion
}