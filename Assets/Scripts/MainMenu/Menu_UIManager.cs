using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Menu_UIManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] GameObject BlinkRedPanel;
    [SerializeField] GameObject RedReady;
    [SerializeField] GameObject BlinkBluePanel;
    [SerializeField] GameObject BlueReady;
    [SerializeField] Image FadeOutPanel;
    [SerializeField] GameObject QuestionPanel;
    [SerializeField] GameObject[] GameDescriptionPanels;
    public GameObject modSelectPanel;
    [SerializeField] Button[] modButtons;
    [SerializeField] GameObject gameSelectPanel;
    [SerializeField] Button[] gameButtons;

    // 변수 참조 
    private int panelIndex = 0;
    private int currentModIndex = 0;
    private int currentGameIndex = 0;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = Color.black;

    // bool
    private bool isRedReadyTriggered = false;
    private bool isBlueReadyTriggered = false;
    private bool isBlinking = false;
    public bool isQuestionPanelOpened = false;
    private bool isFadeOutStarted = false;
    // 모드 선택
    public bool isModSelected = false;
    public bool isGameSelected = false;
    public bool isLeftSelected = false;

    // 스크립트 참조
    private Menu_SceneManager menu_SceneManager;
    private Menu_SoundManager menu_SoundManager;

    void Awake()
    {
        menu_SceneManager = GetComponent<Menu_SceneManager>();
        menu_SoundManager = GetComponent<Menu_SoundManager>();
    }

    void Start()
    {
        // 초기 UI 세팅
        InitalUISetting();

        // 블링크 패널 깜빡임 
        VisibleBlinkPanel();
    }

    void Update()
    {
        UpdateMenuPlayerState();
    }

    #region 초기 UI 세팅
    private void InitalUISetting()
    {
        // 각 플레이어 레디 아이콘 비활성화 
        RedReady.gameObject.SetActive(false);
        BlueReady.gameObject.SetActive(false);

        // 비활성화(질문/페이드 아웃)
        QuestionPanel.SetActive(false);
        FadeOutPanel.gameObject.SetActive(false);
    }
    #endregion

    #region 블링크 패널 깜박임
    void VisibleBlinkPanel()
    {
        if (!isBlinking) return;

        BlinkRedPanel.SetActive(true);
        BlinkBluePanel.SetActive(true);
        StartCoroutine(UnvisibleBlinkPanel());
    }

    IEnumerator UnvisibleBlinkPanel()
    {
        yield return new WaitForSeconds(1f);
        BlinkRedPanel.gameObject.SetActive(false);
        BlinkBluePanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        if (isBlinking)
        {
            VisibleBlinkPanel();
        }
    }
    #endregion

    #region 업데이트 
    void UpdateMenuPlayerState()
    {
        if (menu_SceneManager.isRedReady && !isRedReadyTriggered)
        {
            isRedReadyTriggered = true; // 플래그 잠금

            // 블링크 패널 비활성화
            BlinkRedPanel.SetActive(false);

            // 레디 아이콘 활성황
            if (RedReady != null)
            {
                RectTransform rectTransform = RedReady.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.zero;

                    // 활성화 
                    RedReady.gameObject.SetActive(true);

                    // 애니메이션 
                    rectTransform.DOScale(1f, 0.8f).SetEase(Ease.OutBack);
                }
            }

            // 사운드 재생
            menu_SoundManager.PlayReadyClip(0);
        }

        if (menu_SceneManager.isBlueReady && !isBlueReadyTriggered)
        {
            isBlueReadyTriggered = true; // 플래그 잠금

            // 블링크 패널 비활성화
            BlinkBluePanel.SetActive(false);

            // 레디 아이콘 활성화
            if (BlueReady != null)
            {
                RectTransform rectTransform = BlueReady.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.zero;

                    // 활성화 
                    BlueReady.gameObject.SetActive(true);

                    // 애니메이션 
                    rectTransform.DOScale(1f, 0.8f).SetEase(Ease.OutBack);
                }
            }

            // 사운드 재생
            menu_SoundManager.PlayReadyClip(1);
        }
    }
    #endregion

    #region 질문 패널 
    public void ToggleQuestionPanel()
    {
        if (QuestionPanel != null)
            QuestionPanel.SetActive(!QuestionPanel.activeSelf);

        if (QuestionPanel.activeSelf)
        {
            for (int i = 0; i < GameDescriptionPanels.Length; i++)
            {
                GameDescriptionPanels[i].SetActive(i == 0);
            }
        }

        // 효과음
        menu_SoundManager.PlayBtnClip();
    }

    public void NextPanel()
    {
        int nextIndex = (panelIndex + 1) % GameDescriptionPanels.Length;
        SwitchPanel(nextIndex);

        // 효과음
        menu_SoundManager.PlayBtnClip();
    }

    public void PrevPanel()
    {
        int prevIndex = (panelIndex - 1 + GameDescriptionPanels.Length) % GameDescriptionPanels.Length;
        SwitchPanel(prevIndex);

        // 효과음
        menu_SoundManager.PlayBtnClip();
    }

    public void SwitchPanel(int newIndex)
    {
        // [기존 일반 전환]
        GameDescriptionPanels[panelIndex].SetActive(false);
        panelIndex = newIndex;
        GameObject targetObj = GameDescriptionPanels[panelIndex];
        targetObj.SetActive(true);
    }
    #endregion

    #region 선택(모드/게임)
    // -----
    // 시각화
    // -----
    public void VisibleModSelectPanel()
    {
        Debug.Log("모드 선택 창이 열렸습니다.");
        modSelectPanel.SetActive(true);
        MouseEvent.Instance.HideCursor();

        // 첫 진입 0번째 선택된 상태로 
        SelectedEffect(true, 0);
    }
    private void VisibleGameSelectPanel()
    {
        modSelectPanel.SetActive(false);
        gameSelectPanel.SetActive(true);

        // 게임 기본 선택 상태
        SelectedEffect(false, 2);
    }

    private void SelectedEffect(bool isMod, int selectedIndex)
    {
        // 기존 포커스 해제
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        if (isMod)
        {
            currentModIndex = Mathf.Clamp(selectedIndex, 0, modButtons.Length - 1);

            for (int i = 0; i < modButtons.Length; i++)
            {
                if (modButtons[i] == null) continue;

                ColorBlock cb = modButtons[i].colors;

                Color targetColor = (i == currentModIndex) ? selectedColor : normalColor;

                cb.normalColor = targetColor;
                cb.highlightedColor = targetColor;
                cb.pressedColor = targetColor;
                cb.selectedColor = targetColor;

                modButtons[i].colors = cb;

                Image btnImage = modButtons[i].GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = targetColor;
                }

                if (i == currentModIndex)
                {
                    modButtons[i].Select();
                }
            }
        }
        else
        {
            currentGameIndex = Mathf.Clamp(selectedIndex, 0, gameButtons.Length - 1);

            for (int i = 0; i < gameButtons.Length; i++)
            {
                if (gameButtons[i] == null) continue;

                Image btnImage = gameButtons[i].GetComponent<Image>();
                if (btnImage == null) continue;

                Color targetColor = btnImage.color;

                if (i == currentGameIndex)
                {
                    targetColor.a = 1f;    // 활성화
                }
                else
                {
                    targetColor.a = 0.7f;  // 비활성화
                }

                btnImage.color = targetColor;

                ColorBlock cb = gameButtons[i].colors;
                cb.normalColor = targetColor;
                cb.highlightedColor = targetColor;
                cb.pressedColor = targetColor;
                cb.selectedColor = targetColor;
                gameButtons[i].colors = cb;

                if (i == currentGameIndex)
                {
                    gameButtons[i].Select();
                }
            }
        }
    }
    // -----
    // 키 입력
    // -----
    public void InputToSelectMod(bool isMod)
    {
        // 왼
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (isMod)
            {
                int nextIndex = currentModIndex - 1;

                if (nextIndex < 0)
                {
                    nextIndex = modButtons.Length - 1;
                }

                SelectedEffect(true, nextIndex);
            }
            else
            {
                int nextIndex = currentGameIndex - 1;

                if (nextIndex < 0)
                {
                    nextIndex = gameButtons.Length - 1;
                }

                SelectedEffect(false, nextIndex);
            }
        }

        // 오
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (isMod)
            {
                int nextIndex = currentModIndex + 1;

                if (nextIndex >= modButtons.Length)
                {
                    nextIndex = 0;
                }

                SelectedEffect(true, nextIndex);
            }
            else
            {
                int nextIndex = currentGameIndex + 1;

                if (nextIndex >= gameButtons.Length)
                {
                    nextIndex = 0;
                }

                SelectedEffect(false, nextIndex);
            }
        }

        // 확정
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isMod) // 1단계: 모드 선택창에서 엔터를 눌렀을 때
            {
                DeselectAllModButtons(true);

                if (currentModIndex == 0) // [개별 모드] 선택 시
                {
                    Debug.Log("개별 모드 선택 완료 -> 게임 선택 패널 오픈");
                    isModSelected = true; // 씬 매니저가 다음 Update에서 false 상태 입력을 받도록 플래그 전환

                    modSelectPanel.SetActive(false);
                    gameSelectPanel.SetActive(true);
                    SelectedEffect(false, 0); // 게임 선택창의 첫 번째 요소 활성화
                }
                else if (currentModIndex == 1) // [랜덤 모드] 선택 시
                {
                    Debug.Log("랜덤 모드 선택 완료 -> 페이드 후 바로 Map으로 전환");
                    isModSelected = true;
                    modSelectPanel.SetActive(false);

                    Global_DirectionManager.Instance.isIndivisual = false;
                    StartCoroutine(FadeOutAndStart(true, 0.5f, 1));
                }
            }
            else // 2단계: 게임(맵) 선택창에서 엔터를 눌렀을 때
            {
                Debug.Log($"게임 확정: {currentGameIndex}번 선택됨. 페이드 후 로딩씬 이동");
                isGameSelected = true;
                gameSelectPanel.SetActive(false);

                Global_DirectionManager.Instance.isIndivisual = true;
                StartCoroutine(FadeOutAndStart(false, 0.5f, currentGameIndex));
            }
        }
    }
    // -----
    // 버튼 선택 상태 해제
    // -----
    private void DeselectAllModButtons(bool isMod)
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        if (isMod)
        {
            for (int i = 0; i < modButtons.Length; i++)
            {
                if (modButtons[i] == null) continue;

                ColorBlock cb = modButtons[i].colors;
                cb.normalColor = normalColor;
                cb.highlightedColor = normalColor;
                cb.pressedColor = normalColor;
                cb.selectedColor = normalColor;
                modButtons[i].colors = cb;

                Image btnImage = modButtons[i].GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = normalColor;
                }
            }
        }
        else
        {
            for (int i = 0; i < gameButtons.Length; i++)
            {
                if (gameButtons[i] == null) continue;

                ColorBlock cb = gameButtons[i].colors;
                cb.normalColor = normalColor;
                cb.highlightedColor = normalColor;
                cb.pressedColor = normalColor;
                cb.selectedColor = normalColor;
                gameButtons[i].colors = cb;

                Image btnImage = gameButtons[i].GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = normalColor;
                }
            }
        }
    }
    #endregion
    // 페이드 아웃 효과
    public IEnumerator FadeOutAndStart(bool isMod, float waitTime, int index)
    {
        if (isFadeOutStarted) yield break;
        isFadeOutStarted = true;

        // 레디 아이콘 업데이트 기달 
        yield return new WaitForSeconds(waitTime);

        FadeOutPanel.gameObject.SetActive(true);
        FadeOutPanel.color = new Color(0, 0, 0, 0);
        FadeOutPanel.DOFade(1, 1f)
        .OnComplete(() =>
        {
            // 효과 이후 게임 시작
            menu_SceneManager.StartGame(isMod, index);
        });
    }
}