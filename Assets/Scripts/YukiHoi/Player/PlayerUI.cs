using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] Image[] hpHearts;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("변수 참조")]
    private int lastHp;

    // 스크립트 참조 
    private PlayerState playerState;

    void Awake()
    {
        playerState = GetComponent<PlayerState>();
    }

    void Start()
    {
        if (playerState != null)
        {
            lastHp = playerState.currentHP;
        }

        UpdateGaze(true);
    }

    void Update()
    {
        if (playerState != null && playerState.currentHP != lastHp)
        {
            lastHp = playerState.currentHP;
            UpdateGaze(false);
        }
    }

    #region 체력 상태 업데이트 
    public void UpdateGaze(bool isInit)
    {
        if (playerState == null) return;

        for (int i = 0; i < hpHearts.Length; i++)
        {
            if (hpHearts[i] == null) continue;

            RectTransform rt = hpHearts[i].rectTransform;

            rt.DOKill(true);

            rt.localScale = Vector3.one;

            // 현재 체력에 해당하는 UI는 하트 스프라이트, 그밖에는 껍데기
            if (i < playerState.currentHP)
            {
                if (hpHearts[i].sprite != fullHeartSprite)
                {
                    hpHearts[i].sprite = fullHeartSprite;

                    if (!isInit)
                    {
                        rt.DOPunchScale(Vector3.one * 0.2f, 0.25f, 5, 0.5f);
                    }
                }
            }
            else
            {
                if (hpHearts[i].sprite != emptyHeartSprite)
                {
                    hpHearts[i].sprite = emptyHeartSprite;

                    if (!isInit)
                    {
                        rt.DOShakeAnchorPos(0.2f, 8f, 15);
                    }
                }
            }
        }
    }
    #endregion
}