using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class Map_UIManager : MonoBehaviour
{
    // 싱글톤 
    public static Map_UIManager Instance;

    // UI
    [SerializeField] private GameObject Title;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject[] mapCards;
    [SerializeField] private float duration;

    private List<GameObject> spawnedCards = new List<GameObject>();
    // 스크립트 참조
    private Map_RouletteManager map_RouletteManager;
    private Map_SoundManager map_SoundManager;

    void Awake()
    {
        // 싱글톤 정의 
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        map_RouletteManager = GetComponent<Map_RouletteManager>();
        map_SoundManager = GetComponent<Map_SoundManager>();
    }

    void Start()
    {
        // 타이틀 비활성화로 시작
        Title.gameObject.SetActive(false);

        // 타이틀 효과
        StartCoroutine(EffectMapTitle(Title));

        // 콘텐츠 안보이는 상태에서 시작 
        content.gameObject.SetActive(false);
    }

    IEnumerator EffectMapTitle(GameObject gameObject)
    {
        if (gameObject == null) yield break;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        // 고무줄 효과
        rectTransform.DOScale(1f, 1f).SetEase(Ease.OutElastic);
    }

    #region 맵 로테이션 효과
    public IEnumerator PlayRoulette(int targetIndex)
    {
        // 과연? 사운드 효과
        map_SoundManager.PlayPredictClip();
        yield return new WaitForSeconds(1.5f);

        // 초기화 및 준비
        foreach (var card in spawnedCards) if (card != null) Destroy(card);
        spawnedCards.Clear();
        content.gameObject.SetActive(true);

        // 연출용 카드 생성 5세트 
        int repeatCount = 3;
        for (int i = 0; i < repeatCount; i++)
        {
            for (int j = 0; j < mapCards.Length; j++)
            {
                GameObject go = Instantiate(mapCards[j], content);
                spawnedCards.Add(go);
            }
        }

        // 레이아웃 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        // 애니메이션 카드 훑기(왼쪽 -> 오른쪽)
        float contentWidth = content.rect.width;
        content.anchoredPosition = new Vector2(-contentWidth, 0);

        // 사운드 
        map_SoundManager.PlaySlotClip();

        content.DOKill();
        content.DOAnchorPosX(0, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 연출 카드 비활성화
                content.gameObject.SetActive(false);

                // 선택된 카드 연출 
                ShowFinalResult(targetIndex);
            });
    }

    private void ShowFinalResult(int targetIndex)
    {
        GameObject resultCard = Instantiate(mapCards[targetIndex], viewport);
        spawnedCards.Add(resultCard);

        RectTransform rectTransform = resultCard.GetComponent<RectTransform>();
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.zero;

        // 캔버스 부모 설정
        resultCard.transform.SetParent(canvasRect, true);

        // 중앙에서 띠링! 하고 나타남
        rectTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // 사운드 효과 
            map_SoundManager.PlaySelectedMapClip(targetIndex + 1);

            // 확대 연출
            StartCoroutine(FinalStep(resultCard));
        });
    }

    IEnumerator FinalStep(GameObject selectedMapCard)
    {
        // 카드 확대 연출
        selectedMapCard.transform.DOScale(Vector3.one * 30f, 1f).SetEase(Ease.InCubic);

        // 연출 완료 대기 
        yield return new WaitForSeconds(0.8f);

        // 로딩 씬 이동
        SceneManager.LoadScene("Loading");
    }
    #endregion
}