using UnityEngine;

public class Score_SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip scoreClip;
    [SerializeField] AudioClip congraClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayScoreClip()
    {
        if (scoreClip != null)
        {
            audioSource.PlayOneShot(scoreClip);
        }
    }

    public void PlayCongraClip()
    {
        if (congraClip != null)
        {
            audioSource.PlayOneShot(congraClip);
        }
    }
}
