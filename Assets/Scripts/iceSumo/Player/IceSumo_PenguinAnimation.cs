using UnityEngine;

public class IceSumo_PenguinAnimation : MonoBehaviour
{
    public Animator animator;

    // 스크립트 참조 
    private IceSumo_Player iceSumo_Player;

    void Awake()
    {
        animator = GetComponent<Animator>();
        iceSumo_Player = GetComponent<IceSumo_Player>();
    }

    public void PlayWalkAnim(float speed)
    {
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }
}