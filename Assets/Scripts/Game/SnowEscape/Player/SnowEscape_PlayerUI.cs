using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class SnowEscape_PlayerUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject playerUIPanel;
    [SerializeField] TextMeshProUGUI velocity;
    [SerializeField] Image[] keyIcons;
    [SerializeField] Image ultGauge;
    public GameObject ultEffectPanel;
    [SerializeField] ParticleSystem ultParticle;

    // 스크립트 참조
    private SnowEscape_Player snowEscape_Player;

    void Awake()
    {
        snowEscape_Player = GetComponent<SnowEscape_Player>();
    }

    void Update()
    {
        // 속력
        UpdatePlayerVelocity();

        // 키 입력
        UpdateKeyAlpha();

        // 궁극기 게이지 업데이트 
        UpdateUltGauge();

        // 궁극기 파티클 업데이트
        SettingUltParticle();
    }

    #region 현재 속력
    void UpdatePlayerVelocity()
    {
        if (velocity != null)
        {
            velocity.text = snowEscape_Player.velocitySpeed.ToString("F0") + "km/h";
        }
    }
    #endregion

    #region 키 입력시 UI 어둡게 처리 
    public void UpdateKeyAlpha()
    {
        if (keyIcons == null || keyIcons.Length < 3) return;

        if (snowEscape_Player.currentTeamSide == SnowEscape_Player.TeamSide.Red)
        {
            SetKeyIconColor(0, Input.GetKey(KeyCode.LeftArrow));
            SetKeyIconColor(1, Input.GetKey(KeyCode.DownArrow));
            SetKeyIconColor(2, Input.GetKey(KeyCode.RightArrow));
        }
        else
        {
            SetKeyIconColor(0, Input.GetKey(KeyCode.A));
            SetKeyIconColor(1, Input.GetKey(KeyCode.S));
            SetKeyIconColor(2, Input.GetKey(KeyCode.D));
        }
    }

    // 어둡게 처리 
    void SetKeyIconColor(int keyIndex, bool isPushed)
    {
        if (keyIcons[keyIndex] == null) return;

        if (isPushed)
        {
            keyIcons[keyIndex].color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            keyIcons[keyIndex].color = Color.white;
        }
    }
    #endregion

    #region 궁극기 게이지 업데이트 
    private void UpdateUltGauge()
    {
        float fillFill = snowEscape_Player.currentGauge / snowEscape_Player.maxGauge;

        ultGauge.fillAmount = Mathf.Lerp(ultGauge.fillAmount, fillFill, Time.deltaTime * 10f);
    }
    #endregion

    #region 궁극기 파티클 ON
    private void SettingUltParticle()
    {
        if (ultParticle.gameObject.activeSelf != snowEscape_Player.isUlting)
        {
            ultParticle.gameObject.SetActive(snowEscape_Player.isUlting);
        }
    }
    #endregion

    #region 궁극기 발동 효과UI
    public IEnumerator UltEffectRoutine(GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();

        //크기를 0으로 만들고 활성화
        rect.localScale = Vector3.zero;
        panel.SetActive(true);

        // 등장 애니메이션
        rect.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        // 잠시 대기
        yield return new WaitForSeconds(4f);

        // 퇴장 애니메이션
        rect.DOScale(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 애니메이션이 완전히 끝난 후 비활성화
            panel.SetActive(false);
        });
    }
    #endregion
}