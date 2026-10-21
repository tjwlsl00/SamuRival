using UnityEngine;

public class Global_ScoreManger : MonoBehaviour
{
    public static Global_ScoreManger Instance;

    // 변수 
    public int redGetScore = 0;
    public int blueGetScore = 0;
    public int maxScore = 3;
    public int redLastVisualScore = 0;
    public int blueLastVisualScore = 0;

    // bool 
    public bool isRedWin = false;
    public bool isBlueWin = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            redLastVisualScore = redGetScore;
            blueLastVisualScore = blueGetScore;

            // 점수 체크
            CheckPlayerScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckPlayerScore();
    }

    public void AddScore(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            redGetScore++;
            Debug.Log("레드 점수가 추가" + redGetScore);
        }
        else
        {
            blueGetScore++;
            Debug.Log("블루 점수 추가" + blueGetScore);
        }
    }

    public void CheckPlayerScore()
    {
        isRedWin = redGetScore >= maxScore && blueGetScore < maxScore;
        isBlueWin = blueGetScore >= maxScore && redGetScore < maxScore;
    }

    public int ConsumeScoreIncrease()
    {
        bool redIncreased = redGetScore > redLastVisualScore;
        bool blueIncreased = blueGetScore > blueLastVisualScore;

        redLastVisualScore = redGetScore;
        blueLastVisualScore = blueGetScore;

        if (redIncreased && !blueIncreased) return 0;
        if (!redIncreased && blueIncreased) return 1;
        return -1;
    }
}