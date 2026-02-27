using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip GoodLuckClip;
    [SerializeField] AudioClip[] ThrowClips;
    [SerializeField] AudioClip HitClip;
    [SerializeField] AudioClip StunClip;
    [SerializeField] AudioClip WinClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayThrowClip()
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            audioSource.clip = ThrowClips[0];
            audioSource.Play();
        }
        else
        {
            audioSource.clip = ThrowClips[1];
            audioSource.Play();
        }
    }

    public void PlayHitClip()
    {
        audioSource.clip = HitClip;
        audioSource.Play();
    }

    public void PlayStunClip()
    {
        audioSource.loop = true;
        audioSource.clip = StunClip;
        audioSource.Play();
        StartCoroutine(StopStunClip(3.5f));
    }

    IEnumerator StopStunClip(float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.loop = false;
        audioSource.Stop();
    }

    public void PlayWinClip()
    {
        audioSource.clip = WinClip;
        audioSource.Play();
    }

    public void PlayGoodLuck()
    {
        audioSource.clip = GoodLuckClip;
        audioSource.Play();
    }
}