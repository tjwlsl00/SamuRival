using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SnowEscape_UIManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject HUDPanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject PlayerPanel;
    [SerializeField] Slider mainSlider;
    [SerializeField] RectTransform redIcon;
    [SerializeField] RectTransform blueIcon;
    [SerializeField] RectTransform snowBallIcon;

    [Header("오브젝트 참조")]
    [SerializeField] GameObject[] players;
    private GameObject snowBall;
    // 맵 시작, 끝 시점
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("변수 참조")]
    // 슬라이더 폭계산
    private RectTransform sliderRectTransform;
    [SerializeField] RectTransform fillAreaRect;

    // bool 
    public bool isPaused = false;
    private bool isAllFinded = false;

    // 스크립트 참조
    private SnowEscape_GameManager snowEscape_GameManager;
    private SnowEscape_PlayerUI redPlayerUI;
    private SnowEscape_PlayerUI bluePlayerUI;

    void Awake()
    {
        snowEscape_GameManager = GetComponent<SnowEscape_GameManager>();
        redPlayerUI = players[0].GetComponent<SnowEscape_PlayerUI>();
        bluePlayerUI = players[1].GetComponent<SnowEscape_PlayerUI>();
    }

    void Start()
    {
        StartCoroutine(InitialUISettingRoutine());
    }

    void Update()
    {
        if (snowEscape_GameManager.gameDirection == SnowEscape_GameManager.GameDirection.Start)
        {
            // --------
            // 메뉴 토글 
            // --------
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenuPanel();
            }

            // --------
            // 위치 업데이트 
            // --------
            UpdateObjectsPosition();
        }
    }

    #region UI 초기 세팅
    IEnumerator InitialUISettingRoutine()
    {
        HUDPanel.SetActive(false);
        menuPanel.SetActive(false);
        PlayerPanel.SetActive(false);

        // 연출 시간 대기 
        yield return new WaitForSeconds(5.5f);

        HUDPanel.SetActive(true);
        PlayerPanel.SetActive(true);

        // 연출 시간 대기 
        yield return new WaitForSeconds(3f);

        // 스노우볼 오브젝트 참조 
        FindSnowballObj();

        // 슬라이더 세팅
        SettingSlider();
    }
    #endregion

    // 메뉴 패널 토글 
    private void ToggleMenuPanel()
    {
        if (menuPanel != null)
        {
            if (menuPanel.activeSelf)
            {
                isPaused = false;

                menuPanel.SetActive(false);
            }
            else
            {
                isPaused = true;

                menuPanel.SetActive(true);
            }
        }
    }

    #region 슬라이드 
    // ---------
    // 눈덩이 오브젝트 참조 
    // ---------
    private void FindSnowballObj()
    {
        snowBall = GameObject.FindGameObjectWithTag("Snowball");

        if (snowBall != null)
        {
            isAllFinded = true;
            Debug.Log("눈덩이 참조");
        }
        else
        {
            Debug.Log("눈덩이 못찾음");
        }
    }
    // ---------
    // 슬라이드 세팅 
    // ---------
    private void SettingSlider()
    {
        if (!isAllFinded) return;

        Debug.Log("슬라이더 너비 세팅 시도");

        if (mainSlider != null)
        {
            mainSlider.minValue = 0f;
            mainSlider.maxValue = 1f;

            if (fillAreaRect != null)
            {
                sliderRectTransform = fillAreaRect;
                Debug.Log($"슬라이더 폭 세팅 완료 (FillArea 기준폭: {sliderRectTransform.rect.width})");
            }
            else
            {
                sliderRectTransform = mainSlider.GetComponent<RectTransform>();
                Debug.Log($"슬라이더 폭 세팅 완료 (MainSlider 기준폭: {sliderRectTransform.rect.width})");
            }
        }
    }
    // ---------
    // 비율 계산 공식(Z축)
    // ---------
    private float CalculateProgress(Vector3 targetPosition)
    {
        if (startPoint == null || endPoint == null) return 0f;

        // 각 위치의 Z축 값만 추출
        float startZ = startPoint.position.z;
        float endZ = endPoint.position.z;
        float currentZ = targetPosition.z;

        // 전체 Z축 경로 길이
        float totalPathLength = endZ - startZ;

        // 나누기 0 방지 예외 처리
        if (Mathf.Abs(totalPathLength) <= 0.001f) return 0f;

        // 시작점 기준 현재 Z축 진행 거리
        float currentPathLength = currentZ - startZ;

        // 0~1 사이 값으로 클램프하여 반환
        return Mathf.Clamp01(currentPathLength / totalPathLength);
    }
    // ---------
    // 위치 업데이트 
    // ---------
    private void UpdateObjectsPosition()
    {
        // null상태시 위치 업데이트 종료 
        if (!isAllFinded || players[0] == null || players[1] == null || snowBall == null) return;
        if (startPoint == null || endPoint == null || mainSlider == null || sliderRectTransform == null) return;

        Debug.Log("아이콘 위치 업데이트 중");

        float redProgress = CalculateProgress(players[0].transform.position);
        float blueProgress = CalculateProgress(players[1].transform.position);
        float snowBallProgress = CalculateProgress(snowBall.transform.position);

        // 가장 앞서간거 maxProgress로 추적 
        float maxProgress = Mathf.Max(redProgress, Mathf.Max(blueProgress, snowBallProgress));
        mainSlider.value = maxProgress;

        // 업데이트 
        UpdateIconPosition(redIcon, redProgress);
        UpdateIconPosition(blueIcon, blueProgress);
        UpdateIconPosition(snowBallIcon, snowBallProgress);
    }
    // ---------
    // 아이콘 위치 업데이트 
    // ---------
    private void UpdateIconPosition(RectTransform iconRect, float progress)
    {
        Debug.Log("오브젝트 아이콘 업데이트 중!");

        if (iconRect == null) return;

        // 슬라이더의 전체 가로 길이
        float sliderWidth = sliderRectTransform.rect.width;

        // 피벗 기준 정렬 오차를 줄이기 위해 0 ~ 가로폭 사이의 X 좌표
        float targetX = progress * sliderWidth;

        // 기존 Y축 위치나 앵커 상태는 건드리지 않고, 가로(X)축 위치만 실시간 갱신
        Vector2 anchoredPos = iconRect.anchoredPosition;
        anchoredPos.x = targetX;
        iconRect.anchoredPosition = anchoredPos;
    }
    #endregion

    #region 게임 종료
    public void GameEndUISetting(int playerIndex)
    {
        if (redPlayerUI != null && bluePlayerUI != null)
        {
            // 조작, 속력 패널 비활성화
            redPlayerUI.playerUIPanel.SetActive(false);
            bluePlayerUI.playerUIPanel.SetActive(false);
        }
        // -----------------------

        // --------
        // 결과 패널 활성화 
        // --------
        if (resultPanel.activeSelf)
        {
            resultPanel.transform.GetChild(playerIndex).gameObject.SetActive(true);
        }
    }
    #endregion
}