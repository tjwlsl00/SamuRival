using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TonTon_TeamSide { Red, Blue };

public class TonTon_Player : MonoBehaviour
{
    public TonTon_TeamSide tonton_TeamSide;

    // 애니메이션
    private Animator animator;
    // 오디오 
    private AudioSource audioSource;
    [SerializeField] AudioClip CommandClip;
    [SerializeField] AudioClip WinnerClip;

    // 위치 이동
    [SerializeField] List<Transform> waypoints;
    // 각 지점 사이 대기 시간
    private int currentIndex = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    #region 애니메이션 
    public void PlayWorkAnim()
    {
        animator.SetTrigger("isWork");
    }

    private void StopWorkAnim()
    {
        // 애니메이션 강제 전환
        animator.Play("Idle", 0, 0f);

        // 혹시 모를 트리거 예약 취소 
        animator.ResetTrigger("isWork");
    }

    public void PlayVictroyAnim()
    {
        animator.SetTrigger("isVictroy");
    }

    public void PlayDefeatAnim()
    {
        animator.SetTrigger("isDefeat");
    }
    #endregion

    // 오디오
    public IEnumerator PlayWinnerClip()
    {
        yield return new WaitForSeconds(1f);
        audioSource.clip = WinnerClip;
        audioSource.Play();
    }

    public void PlayCommandClip()
    {
        audioSource.clip = CommandClip;
        audioSource.Play();
    }


    // 플레이어 위치 변경(게임 중 / 게임 끝나고)
    public void MoveToWayPoint()
    {
        if (TonTon_GameManager.Instance.tonton_GameDirection == TonTon_GameManager.TonTon_GameDirection.Start)
        {
            transform.position = waypoints[currentIndex].position;
            transform.rotation = waypoints[currentIndex].rotation;

            currentIndex = (currentIndex + 1) % waypoints.Count;

            // 이동 후 애니메이션
            PlayWorkAnim();
        }
        else
        {
            transform.position = waypoints[3].position;
            transform.rotation = Quaternion.Euler(0, -180, 0);
            StopWorkAnim();
        }
    }
}
