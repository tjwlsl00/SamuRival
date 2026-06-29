using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TonTon_UIManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] TextMeshProUGUI GameTimer;
    [SerializeField] GameObject GameStartPanel;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] RectTransform RedScreen;
    [SerializeField] RectTransform BlueScreen;
    [SerializeField] GameObject RedFrost;
    [SerializeField] GameObject BlueFrost;
    [SerializeField] TextMeshProUGUI RedFronzenTimer;
    [SerializeField] TextMeshProUGUI BlueFronzenTimer;
    [SerializeField] Image RedGaze;
    [SerializeField] Image BlueGaze;
    [SerializeField] TextMeshProUGUI RedGaze_Text;
    [SerializeField] TextMeshProUGUI BlueGaze_Text;
    [SerializeField] GameObject RedCommandPanel;
    [SerializeField] GameObject BlueCommandPanel;
    [SerializeField] GameObject RedHammer;
    [SerializeField] GameObject BlueHammer;
    [SerializeField] RectTransform RedComboPanel;
    [SerializeField] GameObject[] RedComboIndexs;
    [SerializeField] RectTransform BlueComboPanel;
    [SerializeField] GameObject[] BlueComboIndexs;

    [Header("변수 참조")]
    [SerializeField] float FreezingTime;
    private float currentRedFreezingTime;
    private float currentBlueFreezingTime;
    private Vector3 originalTextScale;

    [Header("좌표 값 참조")]
    // 숨김, 표지 상태 RectTransform PosX 좌표 
    [SerializeField] float RCTargetPosX = 150f;
    [SerializeField] float RCHiddenPosX = -150f;
    [SerializeField] float BCTargetPosX = -150f;
    [SerializeField] float BCHiddenPosX = 150f;
    [SerializeField] float Animduration = 0.5f;

    // bool 
    public bool isRedFrost = false;
    public bool isBlueFrost = false;

    // 스크립트 참조 
    private TonTon_SoundManger tonTon_SoundManger;
    private TonTon_CommandManager tonTon_CommandManager;

    void Awake()
    {
        tonTon_SoundManger = GetComponent<TonTon_SoundManger>();
        tonTon_CommandManager = GetComponent<TonTon_CommandManager>();
    }

    void Start()
    {
        originalTextScale = GameTimer.transform.localScale;

        InitialUISetting();
    }

    void Update()
    {
        // 메뉴 패널 토글 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPanel.activeSelf)
            {
                menuPanel.SetActive(false);
            }
            else
            {
                menuPanel.SetActive(true);
            }
        }

        if (TonTon_GameManager.Instance.tonton_GameDirection == TonTon_GameManager.TonTon_GameDirection.End) return;

        InputWhileFrozen();
    }

    #region 게임 초기 세팅
    private void InitialUISetting()
    {
        // 초기 게이지 퍼센트 업데이트 
        UpdatePlayerBuildGazeUI(0);
        UpdatePlayerBuildGazeUI(1);

        // UI 비활성화 
        menuPanel.SetActive(false);
        RedHammer.SetActive(false);
        RedFrost.SetActive(false);
        BlueHammer.SetActive(false);
        BlueFrost.SetActive(false);
        GameStartPanel.SetActive(false);
        GameOverPanel.SetActive(false);

        StartCoroutine(EffectGameStartPanel(GameStartPanel));
    }

    IEnumerator EffectGameStartPanel(GameObject gameObject)
    {
        if (gameObject == null) yield break;

        RectTransform rect = gameObject.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;
        gameObject.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        // 잠시 대기
        yield return new WaitForSeconds(1.2f);

        // 퇴장 애니메이션
        rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 애니메이션이 완전히 끝난 후 비활성화
            gameObject.SetActive(false);
        });
    }
    #endregion

    #region UI(게이지, 커맨드 쉐이크, 망치 질, 종료 패널)
    // 건설 게이지 
    public void UpdatePlayerBuildGazeUI(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            // MaxGaze에서 비율 계산
            float targetFill = TonTon_GameManager.Instance.CurrentRedGaze / TonTon_GameManager.Instance.MaxGaze;

            if (RedGaze != null)
            {
                // 이전 애니메이션 있으면 중지
                RedGaze.DOKill();

                // 0.3초 동안 부드럽게 수치 변화
                RedGaze.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutCubic);
            }

            if (RedGaze_Text != null)
            {
                RedGaze_Text.text = (targetFill * 100f).ToString("F0") + "%";
            }

        }
        else
        {
            // MaxGaze에서 비율 계산
            float targetFill = TonTon_GameManager.Instance.CurrentBlueGaze / TonTon_GameManager.Instance.MaxGaze;

            if (BlueGaze != null)
            {
                // 이전 애니메이션 있으면 중지
                BlueGaze.DOKill();

                // 0.3초 동안 부드럽게 수치 변화
                BlueGaze.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutCubic);
            }

            if (BlueGaze_Text != null)
            {
                BlueGaze_Text.text = (targetFill * 100f).ToString("F0") + "%";
            }
        }
    }

    // 커맨드 입력 실패 시 패널 흔들기
    public void ShakeCommandPanel(int PlayerNum)
    {
        GameObject target = null;

        if (PlayerNum == 0)
        {
            target = RedCommandPanel;
        }
        else
        {
            target = BlueCommandPanel;
        }

        if (target != null)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            rectTransform.DOKill(true);
            rectTransform.DOShakePosition(0.5f, 5f);
        }
    }

    public void SucessCommandPanel(int PlayerNum)
    {
        GameObject target = (PlayerNum == 0) ? RedCommandPanel : BlueCommandPanel;

        if (target != null)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();

            rectTransform.DOKill(true);

            Sequence successSeq = DOTween.Sequence();

            // 크기 작아짐
            successSeq.Append(rectTransform.DOScale(0f, 0.25f).SetEase(Ease.InQuad));

            // 0.05초 대기 
            successSeq.AppendInterval(0.05f);

            // 다시 커짐(원 상태)
            successSeq.Append(rectTransform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
        }
    }

    // 망치 
    public void VisibleHammerUI(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            StartCoroutine(EffectHammerUI(RedHammer));
        }
        else
        {
            StartCoroutine(EffectHammerUI(BlueHammer));
        }
    }

    IEnumerator EffectHammerUI(GameObject panel)
    {
        if (panel == null) yield break;
        panel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        panel.SetActive(false);
    }

    // 콤보 패널
    public void ShowComboPanel(int PlayerNum)
    {
        // 데이터 타겟 설정
        RectTransform targetComboPanel = (PlayerNum == 0) ? RedComboPanel : BlueComboPanel;
        GameObject[] targetComboIndexs = (PlayerNum == 0) ? RedComboIndexs : BlueComboIndexs;
        CanvasGroup canvasGroup = targetComboPanel.GetComponent<CanvasGroup>();

        if (targetComboPanel == null || targetComboIndexs == null) return;

        // 현재 콤보 값 가져오기 및 보정
        int currentCombo = (PlayerNum == 0) ? tonTon_CommandManager.RedCurrentCombo : tonTon_CommandManager.BlueCurrentCombo;

        if (currentCombo > 0)
        {
            float targetPosX = (PlayerNum == 0) ? RCTargetPosX : BCTargetPosX;
            float hiddenPosX = (PlayerNum == 0) ? RCHiddenPosX :
            BCHiddenPosX;

            // 해당 번째까지 패널 활성화
            for (int i = 0; i < targetComboIndexs.Length; i++)
            {
                if (targetComboIndexs[i] != null)
                {
                    bool isActive = (i == currentCombo - 1);
                    if (targetComboIndexs[i].activeSelf != isActive)
                    {
                        targetComboIndexs[i].SetActive(isActive);
                    }
                }
            }

            if (canvasGroup != null)
            {
                canvasGroup.DOKill(); // 알파 애니메이션도 중단
                canvasGroup.alpha = 1f; // 즉시 보이게 설정
            }

            // 위치 즉시 초기화
            targetComboPanel.anchoredPosition = new Vector2(hiddenPosX, targetComboPanel.anchoredPosition.y);

            // 해당 번째 인덱스만 활성화
            for (int i = 0; i < targetComboIndexs.Length; i++)
            {
                if (targetComboIndexs[i] != null)
                {
                    bool isActive = (i == currentCombo - 1);
                    if (targetComboIndexs[i].activeSelf != isActive)
                    {
                        targetComboIndexs[i].SetActive(isActive);
                    }
                }
            }

            // 애니메이션
            targetComboPanel.DOAnchorPosX(targetPosX, Animduration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // 등장 완료 후 1초 뒤에 페이드 아웃
                    if (canvasGroup != null)
                    {
                        canvasGroup.DOFade(0f, 1f).SetDelay(0.5f);
                    }
                });

            // 콤보 사운드 효과 
            tonTon_SoundManger.PlayComboClip(currentCombo);

            // 맥스 콤보 달성(달성 효과/초기화)
            if (currentCombo >= tonTon_CommandManager.maxCombo)
            {
                PlayerCompleteCombo(PlayerNum);
                tonTon_CommandManager.ResetPlayerCombo(PlayerNum);
            }
            else
            {
                return;
            }
        }
    }

    // 게임 종료 UI처리 
    public void EndGameUISetting()
    {
        RedCommandPanel.SetActive(false);
        BlueCommandPanel.SetActive(false);
        RedFrost.SetActive(false);
        BlueFrost.SetActive(false);

        StartCoroutine(EffectGameEndPanel(GameOverPanel));
    }

    IEnumerator EffectGameEndPanel(GameObject panel)
    {
        yield return null;

        if (panel != null)
        {
            RectTransform rect = panel.GetComponent<RectTransform>();

            //크기를 0으로 만들고 활성화
            rect.localScale = Vector3.zero;
            panel.SetActive(true);

            // 등장 애니메이션
            rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

            // 잠시 대기
            yield return new WaitForSeconds(1.2f);

            // 퇴장 애니메이션
            rect.DOScale(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                // 애니메이션이 완전히 끝난 후 비활성화
                panel.SetActive(false);
            });
        }
    }
    #endregion

    #region 콤보 성공 성에 패널 
    public void PlayerCompleteCombo(int PlayerNum)
    {
        StartCoroutine(EffectCombo(PlayerNum));
    }

    IEnumerator EffectCombo(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            isBlueFrost = true;

            // 성에 패널 페이드 인 효과
            if (BlueFrost != null)
            {
                BlueFrost.SetActive(true);
                CanvasGroup canvasGroup = BlueFrost.GetComponent<CanvasGroup>();

                if (canvasGroup != null)
                    canvasGroup.alpha = 0;
                canvasGroup.DOFade(1f, 1f);
            }

            // 어는 사운드 효과
            tonTon_SoundManger.PlayFreezingClip();

            // 시간 값 받아오기 
            currentBlueFreezingTime = FreezingTime;

            // 얼어있는 동안에만 실행되는 루프
            while (currentBlueFreezingTime > 0)
            {
                // 시간 차감
                currentBlueFreezingTime -= Time.deltaTime;

                // UI 업데이트
                UpdateFronzenTimerUI(BlueFronzenTimer, currentBlueFreezingTime);

                // 다음 프레임까지 대기
                yield return null;
            }

            // 시간이 다 되면 초기화
            currentBlueFreezingTime = 0;

            isBlueFrost = false;

            // 성에 패널 페이드 아웃 효과
            if (BlueFrost != null)
            {
                if (BlueFrost.activeSelf)
                {
                    CanvasGroup canvasGroup = BlueFrost.GetComponent<CanvasGroup>();

                    if (canvasGroup != null)
                        canvasGroup.alpha = 1;
                    canvasGroup.DOFade(0f, 1f).OnComplete(() =>
                    {
                        BlueFrost.SetActive(false);
                    });
                }
            }

            // 녹는 사운드 효과
            tonTon_SoundManger.PlayBreakFreezingClip();
        }
        else
        {
            isRedFrost = true;

            // 성에 패널 페이드 인 효과
            if (RedFrost != null)
            {
                RedFrost.SetActive(true);
                CanvasGroup canvasGroup = RedFrost.GetComponent<CanvasGroup>();

                if (canvasGroup != null)
                    canvasGroup.alpha = 0;
                canvasGroup.DOFade(1f, 1f);
            }

            // 어는 사운드 효과
            tonTon_SoundManger.PlayFreezingClip();

            // 시간 값 받아오기 
            currentRedFreezingTime = FreezingTime;

            while (currentRedFreezingTime > 0)
            {
                currentRedFreezingTime -= Time.deltaTime;
                UpdateFronzenTimerUI(RedFronzenTimer, currentRedFreezingTime);
                yield return null;
            }

            // 시간 다 되면 초기화
            currentRedFreezingTime = 0;

            isRedFrost = false;

            // 성에 패널 페이드 아웃 효과
            if (RedFrost != null)
            {
                if (RedFrost.activeSelf)
                {
                    CanvasGroup canvasGroup = RedFrost.GetComponent<CanvasGroup>();

                    if (canvasGroup != null)
                        canvasGroup.alpha = 1;
                    canvasGroup.DOFade(0f, 1f).OnComplete(() =>
                    {
                        RedFrost.SetActive(false);
                    });
                }
            }

            // 녹는 사운드 효과
            tonTon_SoundManger.PlayBreakFreezingClip();
        }

        yield return new WaitForSeconds(0.5f);
    }
    #endregion

    #region 언상태(키 입력시 화면 흔들림 / 프로징 타이머)
    private void InputWhileFrozen()
    {
        // 언상태 
        if (isRedFrost)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
        Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                PanelShake(RedScreen);
            }
        }

        if (isBlueFrost)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
        Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                PanelShake(BlueScreen);
            }
        }
    }

    private void PanelShake(RectTransform targetPanel)
    {
        if (targetPanel != null)
        {
            targetPanel.DOKill(true);
            targetPanel.DOShakePosition(0.2f, 2f);
        }
    }

    private void UpdateFronzenTimerUI(TextMeshProUGUI targetTimerUI, float remainingTime)
    {
        if (targetTimerUI != null)
            targetTimerUI.text = remainingTime.ToString("F1") + "秒";

        // 텍스트 
        targetTimerUI.transform.localScale = originalTextScale;
    }
    #endregion

}
