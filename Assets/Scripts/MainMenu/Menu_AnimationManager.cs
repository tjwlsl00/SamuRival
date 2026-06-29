using UnityEngine;

public class Menu_AnimationManager : MonoBehaviour
{
    [SerializeField] GameObject[] Players;
    private Animator RedAnimator;
    private Animator BlueAnimator;

    void Awake()
    {
        RedAnimator = Players[0].GetComponent<Animator>();
        BlueAnimator = Players[1].GetComponent<Animator>();
    }

    // 스트래칭 애니메이션 실행
    public void PlayStretchAnim(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            RedAnimator.SetBool("isReady", true);
        }
        else
        {
            BlueAnimator.SetBool("isReady", true);
        }
    }
}