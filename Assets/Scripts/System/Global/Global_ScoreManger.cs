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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 점수 체크
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

    void CheckPlayerScore()
    {
        // 체크 둘중 하나가 3라운드를 가졌으면 게임 종료
        if (redGetScore == 3 && blueGetScore < 3)
        {
            isRedWin = true;
        }
        else if (blueGetScore == 3 && redGetScore < 3)
        {
            isBlueWin = true;
        }
    }
}