using System.Collections;
using UnityEngine;

public class TonTon_SoundManger : MonoBehaviour
{
    private AudioSource audioSouce;
    [SerializeField] AudioClip GameStartClip;
    [SerializeField] AudioClip GameEndClip;
    [SerializeField] AudioClip HammerClip;
    [SerializeField] AudioClip Winner1Clip;
    [SerializeField] AudioClip Winner2Clip;
    [SerializeField] AudioClip BonusTimeClip;
    [SerializeField] AudioClip[] CountClips;
    [SerializeField] AudioClip FreezingClip;
    [SerializeField] AudioClip BreakFreezinClip;
    [SerializeField] AudioClip[] ComboClips;
    [SerializeField] AudioClip StopClip;

    // bool 
    private bool isCountClipPlayed = false;
    private bool isBonusTimePlayed = false;

    void Awake()
    {
        audioSouce = GetComponent<AudioSource>();
    }

    public void PlayGameStartClip()
    {
        audioSouce.PlayOneShot(GameStartClip);
    }

    public void PlayGameEndClip()
    {
        audioSouce.PlayOneShot(GameEndClip);
    }

    public void PlayHammerClip()
    {
        audioSouce.PlayOneShot(HammerClip, 0.2f);
    }

    public void PlayBonusTimeClip()
    {
        if (isBonusTimePlayed) return;
        isBonusTimePlayed = true;
        audioSouce.PlayOneShot(BonusTimeClip);
    }

    public void PlayLeftTimeClip()
    {
        if (isCountClipPlayed) return;

        isCountClipPlayed = true;
        StartCoroutine(LeftTimeCoroutine());
    }

    public IEnumerator LeftTimeCoroutine()
    {
        yield return null;

        if (CountClips.Length > 0)
        {
            for (int i = 0; i < CountClips.Length; i++)
            {
                audioSouce.PlayOneShot(CountClips[i]);
                yield return new WaitForSeconds(1f);
            }
        }

        // 재사용 위해서    
        isCountClipPlayed = false;
    }

    // 어는/녹는 소리
    public void PlayFreezingClip()
    {
        if (FreezingClip != null)
            audioSouce.PlayOneShot(FreezingClip, 1f);
    }

    public void PlayBreakFreezingClip()
    {
        if (BreakFreezinClip != null)
            audioSouce.PlayOneShot(BreakFreezinClip, 1f);
    }

    // 콤보 
    public void PlayComboClip(int ComboIndex)
    {
        if (ComboClips != null)
        {
            audioSouce.PlayOneShot(ComboClips[ComboIndex - 1], 0.3f);

            if (ComboIndex >= 5)
            {
                if (StopClip != null)
                {
                    audioSouce.PlayOneShot(StopClip, 1f);
                }
            }
            else
            {
                return;
            }
        }
    }
}
