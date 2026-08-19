using UnityEngine;

public class IceSumo_PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayWinAnim()
    {
        Debug.Log("승리 애니메이션 재생");

        animator.SetBool("isWin", true);
    }
}