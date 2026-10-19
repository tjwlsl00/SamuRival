using UnityEngine;

public class Score_PlayerAnim : MonoBehaviour
{
    public enum TeamSide { Red, Blue };
    public TeamSide currentTeamSide;

    private Animator animator;

    private static int redInitialScore = 0;
    private static int blueInitialScore = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        GetCurrentScore();
    }

    private void GetCurrentScore()
    {
        int currentScore = 0;
        int previouScore = 0;

        if (currentTeamSide == TeamSide.Red)
        {
            currentScore = Global_ScoreManger.Instance.redGetScore;
            previouScore = redInitialScore;
            redInitialScore = currentScore;
        }
        else
        {
            currentScore = Global_ScoreManger.Instance.blueGetScore;
            previouScore = blueInitialScore;
            blueInitialScore = currentScore;
        }

        // 점수 변동 시 박수 애니메이션 재생
        bool isScoreIncreased = currentScore > previouScore;
        if (isScoreIncreased)
        {
            int playerIndex = 0;

            if (currentTeamSide == TeamSide.Red)
            {
                playerIndex = 0;
            }
            else
            {
                playerIndex = 1;
            }

            // 텍스트 효과 
            StartCoroutine(Score_UIManager.Instance.ScoreUIAnimRoutine(playerIndex));

            // 애니메이션 
            PlayWinAnim();
        }
    }

    public void PlayWinAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger("isWin");
        }
    }
}