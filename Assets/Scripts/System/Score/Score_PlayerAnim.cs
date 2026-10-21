using UnityEngine;

public class Score_PlayerAnim : MonoBehaviour
{
    public enum TeamSide { Red, Blue };
    public TeamSide currentTeamSide;
    
    private Animator animator;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void PlayWinAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger("isWin");
        }
    }
}