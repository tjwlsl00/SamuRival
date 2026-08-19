using UnityEngine;

public class Map_SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] AudioClip SlotClip;
    [SerializeField] AudioClip PredictClip;
    [SerializeField] AudioClip Map1Clip;
    [SerializeField] AudioClip Map2Clip;
    [SerializeField] AudioClip Map3Clip;
    [SerializeField] AudioClip Map4Clip;
    [SerializeField] AudioClip Map5Clip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySlotClip()
    {
        if (SlotClip != null)
            audioSource.clip = SlotClip;
        audioSource.PlayDelayed(0.3f);
        audioSource.Play();
    }

    public void PlayPredictClip()
    {
        if (PredictClip != null)
            audioSource.clip = PredictClip;
        audioSource.Play();
    }

    public void PlaySelectedMapClip(int mapIndex)
    {
        switch (mapIndex)
        {
            case 1:
                if (Map1Clip != null)
                    audioSource.PlayOneShot(Map1Clip);
                break;
            case 2:
                if (Map2Clip != null)
                    audioSource.PlayOneShot(Map2Clip);
                break;
            case 3:
                if (Map3Clip != null)
                    audioSource.PlayOneShot(Map3Clip);
                break;
            case 4:
                if (Map4Clip != null)
                    audioSource.PlayOneShot(Map4Clip);
                break;
            case 5:
                if (Map5Clip != null)
                    audioSource.PlayOneShot(Map5Clip);
                break;
        }
    }
}
