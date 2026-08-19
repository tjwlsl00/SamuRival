using UnityEngine;
using System.Collections;

public class IceSumo_MapManager : MonoBehaviour
{
    [Header("맵 파츠 참조")]
    [SerializeField] GameObject[] mapParts;

    [Header("변수 참조")]
    public int reduceCount = 0;
    public int maxReduceCount = 2;
    private float currentTime = 0f;
    [SerializeField] float reduceTime = 10f;

    // bool 
    private bool isReducing = false;

    // 스크립트 참조
    private IceSumo_GameManager iceSumo_GameManager;
    private IceSumo_SoundManager iceSumo_SoundManager;
    private IceSumo_UIManager iceSumo_UIManager;
    private IceSumo_CameraManager iceSumo_CameraManager;

    void Awake()
    {
        iceSumo_GameManager = GetComponent<IceSumo_GameManager>();
        iceSumo_SoundManager = GetComponent<IceSumo_SoundManager>();
        iceSumo_UIManager = GetComponent<IceSumo_UIManager>();
        iceSumo_CameraManager = GetComponent<IceSumo_CameraManager>();
    }

    void Update()
    {
        if (iceSumo_GameManager.gameDirection == IceSumo_GameManager.GameDirection.Ready || iceSumo_GameManager.gameDirection == IceSumo_GameManager.GameDirection.End)
        {
            return;
        }
        else
        {
            if(isReducing) return;
            
            currentTime += Time.deltaTime;

            if (currentTime >= reduceTime)
            {
                if (reduceCount < maxReduceCount)
                {
                    reduceCount++;
                    StartCoroutine(ReduceMapPart());
                }
                else
                {
                    return;
                }
            }
        }
    }

    #region 맵 파츠 크기 축소 
    IEnumerator ReduceMapPart()
    {
        if (isReducing) yield break;
        isReducing = true;

        // UI, 사운드 효과(발조심!)
        StartCoroutine(iceSumo_UIManager.EffectBecarefulPanel());
        iceSumo_SoundManager.PlayBeCarefulClip();

        // 타겟 설정
        int targetIndex = Mathf.Max(0, reduceCount - 1);
        GameObject targetObject = mapParts[targetIndex];
        Renderer targetRenderer = targetObject.GetComponent<Renderer>();

        // -----------------------------
        // 장판 깜빡임 - 3초 
        // -----------------------------
        if (targetRenderer != null)
        {
            Color originalColor = targetRenderer.material.color;

            // 연출 관련 설정 
            int blinkCount = 3;
            float totalTime = 3.0f;
            float blinkDuration = totalTime / (blinkCount * 2f);

            for (int i = 0; i < blinkCount; i++)
            {
                // 빨간색
                targetRenderer.material.color = Color.red;
                yield return new WaitForSeconds(blinkDuration);

                // 원래 색상 
                targetRenderer.material.color = originalColor;
                yield return new WaitForSeconds(blinkDuration);
            }
        }

        // 카메라 전환 
        iceSumo_CameraManager.ChangeRoundCamera(targetIndex);

        // -----------------------------
        // 크기 줄이기 - 7초 
        // -----------------------------
        Vector3 startScale = targetObject.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float shrinkDuration = 7.0f;
        float shrinkTimer = 0f;

        while (shrinkTimer < shrinkDuration)
        {
            shrinkTimer += Time.deltaTime;

            float t = Mathf.Clamp01(shrinkTimer / shrinkDuration);
            targetObject.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // 다음 프레임 대기 
            yield return null;
        }

        // -----------------------------
        // 맵 파츠 삭제 
        // -----------------------------
        Debug.Log("맵이 사라집니다.");
        targetObject.transform.localScale = Vector3.zero;
        targetObject.SetActive(false);

        // 초기화
        currentTime = 0f;
        isReducing = false;
    }
    #endregion
}
