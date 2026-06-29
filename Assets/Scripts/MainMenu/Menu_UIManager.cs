using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Menu_UIManager : MonoBehaviour
{
    [SerializeField] GameObject BlinkRedPanel;
    [SerializeField] Image RedReady;
    [SerializeField] GameObject BlinkBluePanel;
    [SerializeField] Image BlueReady;
    [SerializeField] Image FadeOutPanel;
    [SerializeField] GameObject QuestionPanel;
    [SerializeField] GameObject[] GameDescriptionPanels;
    private int panelIndex = 0;

    // bool
    private bool isFadeOutStarted = false;

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
        // 각 플레이어 레디 아이콘 비활성화 
        RedReady.gameObject.SetActive(false);
        BlueReady.gameObject.SetActive(false);

        // 비활성화(질문/페이드 아웃)
        QuestionPanel.SetActive(false);
        FadeOutPanel.gameObject.SetActive(false);

        // 블링크 패널 깜빡임 
        VisibleBlinkPanel();
    }

    #region 블링크 패널 깜박임
    void VisibleBlinkPanel()
    {
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
        VisibleBlinkPanel();
    }
    #endregion

    void Update()
    {
        UpdateMenuPlayerState();
    }

    #region 업데이트 
    void UpdateMenuPlayerState()
    {
        if (menu_SceneManager.isRedReady)
        {
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

        if (menu_SceneManager.isBlueReady)
        {
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

    // 질문 패널 토글 
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
        GameDescriptionPanels[panelIndex].SetActive(false);
        panelIndex = newIndex;
        GameObject targetObj = GameDescriptionPanels[panelIndex];
        targetObj.SetActive(true);
    }

    // 페이드 아웃 효과
    public IEnumerator FadeOutAndStart(float waitTime)
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
            menu_SceneManager.StartGame();
        });
    }
}
