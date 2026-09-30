using UnityEngine;
using DG.Tweening;

public class BuildObj : MonoBehaviour
{
    private Material material;
    private static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");

    private float minFill = 0f;
    private float maxFill = 1f;
    private float currentDisplayFill;
    private float lastTargetFill = -999f;
    [SerializeField] private float changeSpeed = 2.0f;

    // 오브젝트
    private Vector3 originalScale;
    private Renderer objRenderer;
    private MaterialPropertyBlock propBlock;

    // 흔들림 효과 변수
    [SerializeField] private float bounceHeight = 0.25f;
    [SerializeField] private float bounceDuration = 0.1f;
    [SerializeField] private float returnDuration = 0.2f;

    private Tween fillTween;
    private Sequence bounceSequence;

    void Awake()
    {
        // 오브젝트 원래 크기 저장
        originalScale = transform.localScale;

        objRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock(); 
    }

    void Start()
    {
        if (material != null)
        {
            // 셰이더 fillamount 0에서 시작
            currentDisplayFill = minFill - 0.1f;
            material.SetFloat(FillAmountID, currentDisplayFill);
            lastTargetFill = minFill;
        }
    }

    #region 건물 셰이더 게이지 / 흔들림
    public void SetGaugeStep(float currentGaze)
    {
        if (objRenderer == null) return;

        // 목표값 매핑 (0~100 -> 0~1) 및 보정
        float targetFill = Map(currentGaze, 0f, 100f, minFill, maxFill);
        // 0일 때 확실히 제거
        if (currentGaze <= 0.1f) targetFill = minFill - 0.2f; 
        // 100일 때 확실히 노출
        else if (currentGaze >= 99.9f) targetFill = maxFill + 0.2f; 

        // 중복 호출 방지
        if (Mathf.Approximately(targetFill, lastTargetFill)) return;
        lastTargetFill = targetFill;

        // 기존 트윈 중단 (부드러운 방향 전환용)
        if (fillTween != null && fillTween.IsActive()) fillTween.Kill();

        // 동적 시간 계산 (성공/실패 속도 차이를 주고 싶다면 changeSpeed 조절)
        float duration = Mathf.Clamp(Mathf.Abs(targetFill - currentDisplayFill) / changeSpeed, 0.25f, 0.8f);

        // DOTween 실행
        fillTween = DOTween.To(() => currentDisplayFill, x =>
        {
            currentDisplayFill = x;
            ApplyPropertyBlock(currentDisplayFill);
        }, targetFill, duration).SetEase(Ease.OutCubic); 
    }

    private void ApplyPropertyBlock(float value)
    {
        // 렌더러에서 현재 블록을 가져와서 수정한 뒤 다시 세팅
        objRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(FillAmountID, value);
        objRenderer.SetPropertyBlock(propBlock);
    }

    private float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    // 흔들림
    public void ShackBuildObj()
    {
        // 기존 연출 초기화
        if (bounceSequence != null && bounceSequence.IsActive())
        {
            bounceSequence.Kill();
        }

        transform.localScale = originalScale;

        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y + bounceHeight, originalScale.z);

        bounceSequence = DOTween.Sequence();
        bounceSequence.Append(transform.DOScale(targetScale, bounceDuration).SetEase(Ease.OutQuad));
        bounceSequence.Append(transform.DOScale(originalScale, returnDuration).SetEase(Ease.InOutQuad));

        bounceSequence.Play();
    }
    #endregion

    // 메모리 누수 방지
    private void OnDestroy()
    {
        fillTween.Kill();
        bounceSequence.Kill();
    }
}
