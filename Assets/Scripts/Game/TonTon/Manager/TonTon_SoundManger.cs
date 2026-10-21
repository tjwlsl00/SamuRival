using System.Collections;
using UnityEngine;

public class TonTon_SoundManger : MonoBehaviour
{
    private AudioSource audioSouce;
    [SerializeField] AudioClip GameStartClip;
    [SerializeField] AudioClip GameEndClip;
    [SerializeField] AudioClip HammerClip;
    [SerializeField] AudioClip WrongClip;
    [SerializeField] AudioClip Winner1Clip;
    [SerializeField] AudioClip Winner2Clip;
    [SerializeField] AudioClip FreezingClip;
    [SerializeField] AudioClip BreakFreezinClip;
    [SerializeField] AudioClip[] ComboClips;
    [SerializeField] AudioClip StopClip;
    [SerializeField] AudioClip alarmClip;

    // bool 
    private bool isStopped = false;

    void Awake()
    {
        audioSouce = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (TonTon_GameManager.Instance.tonton_GameDirection == TonTon_GameManager.TonTon_GameDirection.End)
        {
            PlayAlarmClip();
        }
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

    public void PlayWrongClip()
    {
        audioSouce.PlayOneShot(WrongClip, 1f);
    }

    void PlayAlarmClip()
    {
        if (isStopped) return;
        isStopped = true;
        audioSouce.PlayOneShot(alarmClip, 1f);
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
