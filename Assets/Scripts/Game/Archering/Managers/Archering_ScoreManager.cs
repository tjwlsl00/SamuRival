using System.Collections;
using UnityEngine;

public class Archering_ScoreManager : MonoBehaviour
{
    // 싱글톤 
    public static Archering_ScoreManager Instance;

    [Header("과녁 중심 오브젝트")]
    [SerializeField] Transform CenterPoint;
    [SerializeField] float[] RadiusRanges;
    [SerializeField] int[] Scores;
    public int redFinalScore;
    public int blueFinalScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 점수 측정 / 필드 오브젝트 찾기 
    public int ScoreJudgment(Transform stonePosition)
    {
        // 과녁과 말 사이의 거리 계산
        float dist = Vector2.Distance(new Vector2(CenterPoint.position.x, CenterPoint.position.z), new Vector2(stonePosition.position.x, stonePosition.position.z));

        // 거리에 따른 점수 
        for (int i = 0; i < RadiusRanges.Length; i++)
        {
            if (dist <= RadiusRanges[i])
            {
                // 해당 구간 점수 반환
                Debug.Log(Scores[i] + "점 획득!");
                return Scores[i];
            }
        }

        // 과녁 밖
        return 0;
    }

    public IEnumerator CalculateScoreWhenStoppedRoutine(System.Action onComplete = null)
    {
        GameObject[] redStones = GameObject.FindGameObjectsWithTag("redStone");
        GameObject[] blueStones = GameObject.FindGameObjectsWithTag("blueStone");

        bool isMoving = true;
        while (isMoving)
        {
            isMoving = false;

            // 3D Rigidbody 속도 체크 (2D인 경우 Rigidbody2D로 변경)
            foreach (var stone in redStones)
            {
                if (stone != null && stone.TryGetComponent<Rigidbody>(out var rb))
                {
                    if (rb.linearVelocity.sqrMagnitude > 0.001f || rb.angularVelocity.sqrMagnitude > 0.001f)
                    {
                        isMoving = true;
                        break;
                    }
                }
            }

            if (!isMoving)
            {
                foreach (var stone in blueStones)
                {
                    if (stone != null && stone.TryGetComponent<Rigidbody>(out var rb))
                    {
                        if (rb.linearVelocity.sqrMagnitude > 0.001f || rb.angularVelocity.sqrMagnitude > 0.001f)
                        {
                            isMoving = true;
                            break;
                        }
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        // 모든 스톤이 정지한 후 최종 점수 집계
        FindOnStageStones();

        // 후속 작업(게임 종료 판정 등) 실행
        onComplete?.Invoke();
    }

    // 필드 위 스톤 참조 
    public void FindOnStageStones()
    {
        redFinalScore = 0;
        blueFinalScore = 0;

        GameObject[] onStageRedStones = GameObject.FindGameObjectsWithTag("redStone");
        if (onStageRedStones != null && onStageRedStones.Length >= 1)
        {
            foreach (GameObject obj in onStageRedStones)
            {
                redFinalScore += ScoreJudgment(obj.transform);
            }
        }

        Debug.Log("레드 최종 점수:" + redFinalScore);

        GameObject[] onStageBlueStones = GameObject.FindGameObjectsWithTag("blueStone");
        if (onStageBlueStones != null && onStageBlueStones.Length >= 1)
        {
            foreach (GameObject obj in onStageBlueStones)
            {
                blueFinalScore += ScoreJudgment(obj.transform);
            }
        }

        Debug.Log("블루 최종 점수:" + blueFinalScore);
    }
    #endregion
}