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
        float dist = Vector2.Distance(new Vector2(CenterPoint.transform.position.x, CenterPoint.position.z), new Vector2(stonePosition.transform.position.x, stonePosition.position.z));

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

    // 필드 위 스톤 참조 
    public void FindOnStageStones()
    {
        // 라운드 종료 시점(기본 최종 점수)
        redFinalScore = 0;
        blueFinalScore = 0;
     
        // 빨강 스톤 점수 계산 
        GameObject[] onStageRedStones = GameObject.FindGameObjectsWithTag("redStone");

        if (onStageRedStones != null && onStageRedStones.Length >= 1)
        {
            foreach (GameObject obj in onStageRedStones)
            {
                redFinalScore += ScoreJudgment(obj.transform);
            }
        }

        Debug.Log("레드 최종 점수:" + redFinalScore);

        // 파랑 스톤 점수 게산
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
