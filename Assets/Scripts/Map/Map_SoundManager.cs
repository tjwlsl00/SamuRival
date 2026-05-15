using UnityEngine;

public class Map_SoundManager : MonoBehaviour
{
    private AudioSource audioSourece;
    [SerializeField] AudioClip SlotClip;
    [SerializeField] AudioClip PredictClip;
    [SerializeField] AudioClip Map1Clip;
    [SerializeField] AudioClip Map2Clip;
    [SerializeField] AudioClip Map3Clip;
    [SerializeField] AudioClip Map4Clip;
    [SerializeField] AudioClip Map5Clip;

    void Awake()
    {
        audioSourece = GetComponent<AudioSource>();
    }

    public void PlaySlotClip()
    {
        if (SlotClip != null)
            audioSourece.clip = SlotClip;
        audioSourece.PlayDelayed(0.3f);
        audioSourece.Play();
    }

    public void PlayPredictClip()
    {
        if (PredictClip != null)
            audioSourece.clip = PredictClip;
        audioSourece.Play();
    }

    public void PlaySelectedMapClip(int mapIndex)
    {
        switch (mapIndex)
        {
            case 1:
                if (Map1Clip != null)
                    audioSourece.PlayOneShot(Map1Clip);
                break;
            case 2:
                if (Map2Clip != null)
                    audioSourece.PlayOneShot(Map2Clip);
                break;
            case 3:
                if (Map3Clip != null)
                    audioSourece.PlayOneShot(Map3Clip);
                break;
            case 4:
                if (Map4Clip != null)
                    audioSourece.PlayOneShot(Map4Clip);
                break;
            case 5:
                if (Map5Clip != null)
                    audioSourece.PlayOneShot(Map5Clip);
                break;
        }
    }
}
