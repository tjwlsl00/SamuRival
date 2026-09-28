using UnityEngine;
using System.Collections;

public class SnowEscape_SoundManager : MonoBehaviour
{
    [Header("사운드 참조")]
    private AudioSource audioSource;
    [SerializeField] AudioClip earthquakeClip;
    [SerializeField] AudioClip runawayClip;
    [SerializeField] AudioClip redWin;
    [SerializeField] AudioClip blueWin;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(DecoSoundRoutine());
    }

    IEnumerator DecoSoundRoutine()
    {
        yield return null;

        PlayEarthquakeClip();

        yield return new WaitForSeconds(4f);

        PlayRunawayClip();
    }

    public void PlayEarthquakeClip()
    {
        if (earthquakeClip != null)
        {
            audioSource.PlayOneShot(earthquakeClip, 0.5f);
        }
    }

    public void PlayRunawayClip()
    {
        if (runawayClip != null)
        {
            audioSource.PlayOneShot(runawayClip);
        }
    }

    public void PlayWinnerClip(int soundIndex)
    {
        if (soundIndex == 0)
        {
            audioSource.PlayOneShot(redWin);
        }
        else
        {
            audioSource.PlayOneShot(blueWin);
        }
    }
}
