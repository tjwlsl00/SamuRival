using UnityEngine;
using DG.Tweening;

public class Score_UIManager : MonoBehaviour
{
    [SerializeField] RectTransform red_Icon;
    [SerializeField] RectTransform blue_Icon;
    [SerializeField] RectTransform[] red_wayPoints;
    [SerializeField] RectTransform[] blue_wayPoints;

    // 외부 
    private Score_SceneManager score_SceneManager;

    void Awake()
    {
        score_SceneManager = GetComponent<Score_SceneManager>();
    }

    void Start()
    {
        UpdateIconRect(Global_ScoreManger.Instance.redGetScore, Global_ScoreManger.Instance.blueGetScore);

        // UI 적용 후 맵 이동
        if (score_SceneManager != null)
            StartCoroutine(score_SceneManager.MoveToMapScene());
    }

    void UpdateIconRect(int currentRedScore, int currentBlueScore)
    {
        if (Global_ScoreManger.Instance.redLastVisualScore < currentRedScore)
        {
            DoAnimPlayerIcon(0, currentRedScore);
            Global_ScoreManger.Instance.redLastVisualScore = currentRedScore;
        }
        else
        {
            SetPositionIconImmediate(0, currentRedScore);
        }

        if (Global_ScoreManger.Instance.blueLastVisualScore < currentBlueScore)
        {
            DoAnimPlayerIcon(1, currentBlueScore);
            Global_ScoreManger.Instance.blueLastVisualScore = currentBlueScore;
        }
        else
        {
            SetPositionIconImmediate(1, currentBlueScore);
        }
    }

    #region UI 애니메이션/위치 고정 
    void DoAnimPlayerIcon(int PlayerNum, int targetScore)
    {
        if (targetScore <= 0) return;

        RectTransform targetIcon = (PlayerNum == 0) ? red_Icon : blue_Icon;
        RectTransform[] wayPoints = (PlayerNum == 0) ? red_wayPoints : blue_wayPoints;
        int LastVisualScore = (PlayerNum == 0) ? Global_ScoreManger.Instance.redLastVisualScore : Global_ScoreManger.Instance.blueLastVisualScore;

        // 새롭게 이동해야하는 UI 위치 인덱스 
        int idx = Mathf.Clamp(targetScore - 1, 0, wayPoints.Length - 1);

        // 이동 전, 기존 움직였던 UI 위치 값 할당
        int preIdx = Mathf.Max(0, LastVisualScore - 1);
        if (LastVisualScore > 0)
            targetIcon.anchoredPosition = wayPoints[preIdx].anchoredPosition;

        // 애니메이션 이동(부드럽게)
        targetIcon.DOKill();
        targetIcon.DOAnchorPos(wayPoints[idx].anchoredPosition, 0.5f).SetEase(Ease.OutQuad);
    }

    void SetPositionIconImmediate(int PlayerNum, int targetScore)
    {
        if (targetScore <= 0) return;

        RectTransform targetIcon = (PlayerNum == 0) ? red_Icon : blue_Icon;
        RectTransform[] wayPoints = (PlayerNum == 0) ? red_wayPoints : blue_wayPoints;

        int idx = Mathf.Clamp(targetScore - 1, 0, wayPoints.Length - 1);
        targetIcon.DOKill();
        targetIcon.anchoredPosition = wayPoints[idx].anchoredPosition;
    }
    #endregion
}
