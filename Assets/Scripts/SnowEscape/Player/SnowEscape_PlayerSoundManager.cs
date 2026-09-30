using UnityEngine;

public class SnowEscape_PlayerSoundManager : MonoBehaviour
{
    [Header("오디오 참조")]
    [SerializeField] AudioSource skiAudioSource;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip ultClip;
    [SerializeField] AudioClip stunClip;

    // 스크립트 참조 
    private SnowEscape_Player snowEscape_Player;

    void Awake()
    {
        // 스크립트 참조 
        snowEscape_Player = GetComponent<SnowEscape_Player>();
    }

    void Update()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Start)
        {
            if (!skiAudioSource.isPlaying)
            {
                skiAudioSource.Play();
            }

            if (snowEscape_Player.isStun)
            {
                skiAudioSource.volume = 0.02f;
            }
            else
            {
                skiAudioSource.volume = 0.04f;
            }
        }
        else
        {
            if (skiAudioSource.isPlaying)
            {
                skiAudioSource.Stop();
            }
        }
    }

    public void PlayUltClip()
    {
        if (ultClip != null)
        {
            audioSource.PlayOneShot(ultClip);
        }
    }

    public void PlayStunClip()
    {
        if (stunClip != null)
        {
            audioSource.PlayOneShot(stunClip);
        }
    }
}
