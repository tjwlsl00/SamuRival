using UnityEngine;

public class SnowEscape_PlayerAnim : MonoBehaviour
{
    private Animator animator;
    private bool isWin = false;

    // 스크립트 참조
    private SnowEscape_Player snowEscape_Player;

    void Awake()
    {
        animator = GetComponent<Animator>();

        snowEscape_Player = GetComponent<SnowEscape_Player>();
    }

    void Update()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Start)
        {
            if (snowEscape_Player.currentTeamSide == SnowEscape_Player.TeamSide.Red)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    PlayCrowdedAnim(true);
                }
                else
                {
                    PlayCrowdedAnim(false);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.S))
                {
                    PlayCrowdedAnim(true);
                }
                else
                {
                    PlayCrowdedAnim(false);
                }
            }

            if (snowEscape_Player.isUlting)
            {
                if (animator.GetBool("isCrowed") == false)
                {
                    PlayCrowdedAnim(true);
                }
            }
        }
        else if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.End)
        {
            if (animator.GetBool("isCrowed") == true)
            {
                PlayCrowdedAnim(false);
            }
        }
    }

    private void PlayCrowdedAnim(bool isCrowed)
    {
        // 매 프레임 재생 방지 
        if (animator.GetBool("isCrowed") == isCrowed) return;

        animator.SetBool("isCrowed", isCrowed);
    }

    public void PlayResultAnim()
    {
        if (snowEscape_Player != null)
        {
            if (snowEscape_Player.currentTeamSide == SnowEscape_Player.TeamSide.Red)
            {
                isWin = SnowEscape_GameManager.Instance.isRedWin;
            }
            else
            {
                isWin = SnowEscape_GameManager.Instance.isBlueWin;
            }

            if (animator != null)
                if (isWin)
                {
                    Debug.Log("승리 애니메이션 재생");
                    animator.SetTrigger("isWin");
                }
                else
                {
                    Debug.Log("패배 애니메이션 재생");
                    animator.SetTrigger("isLose");
                }
        }
    }
}