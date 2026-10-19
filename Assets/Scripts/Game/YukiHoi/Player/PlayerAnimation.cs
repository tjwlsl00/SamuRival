using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayThrow()
    {
        animator.SetTrigger("isThrow");
    }

    public void PlayAvoid(int num)
    {
        if (num == 0)
        {
            animator.SetTrigger("isRightAvoid");

        }
        else
        {
            animator.SetTrigger("isLeftAvoid");
        }
    }

    public void PlayTakeDamage()
    {
        animator.SetTrigger("isHit");
    }

    public void PlayStun()
    {
        animator.SetBool("isStun", true);
        StartCoroutine(StopStunAnim(1.5f));
    }

    IEnumerator StopStunAnim(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("isStun", false);
    }

    public void PlayDefeat()
    {
        animator.SetTrigger("isDefeat");
    }

    public void PlayWin()
    {
        animator.SetTrigger("isWin");
    }
}