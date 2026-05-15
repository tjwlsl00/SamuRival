using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Menu_UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI RedReadyText;
    [SerializeField] TextMeshProUGUI BlueReadyText;
    [SerializeField] Image RedReadyGaze;
    [SerializeField] Image BlueReadyGaze;
    [SerializeField] GameObject RedReadyBtn;
    [SerializeField] GameObject BlueReadyBtn;
    [SerializeField] Image FadeOutPanel;
    [SerializeField] GameObject QuestionPanel;
    [SerializeField] GameObject[] GameDescriptionPanels;
    private int panelIndex = 0;

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
        // 비활성화(질문/페이드 아웃)
        QuestionPanel.SetActive(false);
        FadeOutPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateMenuPlayerState();
        UpdateMenuPlayerGaze();
    }

    #region 업데이트 
    void UpdateMenuPlayerState()
    {
        if (menu_SceneManager.isRedReady)
        {
            // 레디 버튼 아이콘 비활성화
            if (RedReadyBtn != null)
                RedReadyBtn.SetActive(false);

            // 레디 상태 텍스트 
            RedReadyText.text = "Ready!";

            // 사운드 재생
            menu_SoundManager.PlayReadyClip(0);
        }
        else if (menu_SceneManager.isBlueReady)
        {
            // 레디 버튼 아이콘 비활성화
            if (BlueReadyBtn != null)
                BlueReadyBtn.SetActive(false);

            // 레디 상태 텍스트 
            BlueReadyText.text = "Ready!";

            // 사운드 재생
            menu_SoundManager.PlayReadyClip(1);
        }
    }

    void UpdateMenuPlayerGaze()
    {
        float maxTime = 2f;
        RedReadyGaze.fillAmount = menu_SceneManager.RedReadyRatio / maxTime;
        BlueReadyGaze.fillAmount = menu_SceneManager.BlueReadyRatio / maxTime;
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
    public void FadeOutAndStart()
    {
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
