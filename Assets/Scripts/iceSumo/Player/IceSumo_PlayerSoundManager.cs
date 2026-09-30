using UnityEngine;

public class IceSumo_PlayerSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource dashAudioSource;

    [Header("오디오 참조")]
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip crashClip;
    [SerializeField] AudioClip fallingClip;
    [SerializeField] AudioClip walkClip;

    // 스크립트 참조
    private IceSumo_Player iceSumo_Player;

    void Awake()
    {
        iceSumo_Player = GetComponent<IceSumo_Player>();
    }

    public void PlayWalkClip(bool isWalking)
    {
        if (walkClip != null)
        {
            audioSource.clip = walkClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            if (isWalking && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (!isWalking && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // 차징 상태에 따른 피치 조절
            if (audioSource.isPlaying)
            {
                if (iceSumo_Player.isCharging)
                {
                    audioSource.pitch = 0.8f;
                }
                else
                {
                    audioSource.pitch = 1f;
                }
            }
        }
    }

    public void PlayDashClip()
    {
        if (dashClip != null)
        {
            dashAudioSource.PlayOneShot(dashClip, 0.5f);
        }
    }

    public void PlayCrashClip()
    {
        if (crashClip != null)
        {
            audioSource.PlayOneShot(crashClip, 0.7f);
        }
    }

    public void PlayFallingClip()
    {
        if (fallingClip != null)
        {
            audioSource.PlayOneShot(fallingClip, 0.5f);
        }
    }
}