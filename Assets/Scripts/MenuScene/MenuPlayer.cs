using UnityEngine;

public enum MenuPlayerType
{
    Blue,
    Red
}

public class MenuPlayer : MonoBehaviour
{
    public MenuPlayerType menuPlayerType;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
   
    public void PlayThrowSnow()
    {
        animator.SetTrigger("isThrow");
    }
}
