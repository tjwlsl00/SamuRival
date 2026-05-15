using UnityEngine;

public class Menu_SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip BtnClip;
    [SerializeField] AudioClip ReadyClip;
    [SerializeField] AudioClip RedVoicClip;
    [SerializeField] AudioClip BlueVoicClip;
    private bool isBluePlayed = false;
    private bool isRedPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayReadyClip(int PlayerNum)
    {
        bool isAlreadyPlayed = (PlayerNum == 0) ? isRedPlayed : isBluePlayed;

        if (isAlreadyPlayed) return;

        if (audioSource != null)
        {
            // 기본적인 레디음 
            audioSource.PlayOneShot(ReadyClip);

            switch (PlayerNum)
            {
                case 0:
                    if (RedVoicClip != null)
                        audioSource.PlayOneShot(RedVoicClip);
                    isRedPlayed = true;
                    break;
                case 1:
                    if (BlueVoicClip != null)
                        audioSource.PlayOneShot(BlueVoicClip);
                    isBluePlayed = true;
                    break;
            }
        }
    }

    public void PlayBtnClip()
    {
        audioSource.PlayOneShot(BtnClip);
    }
}
