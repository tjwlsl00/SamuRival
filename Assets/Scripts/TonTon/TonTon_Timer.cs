using UnityEngine;

public class TonTon_Timer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip clockClip;
    [SerializeField] AudioClip alarmClip;

    // bool
    private bool isStopped = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayClockClip();
    }

    void Update()
    {
        if (TonTon_GameManager.Instance.tonton_GameDirection == TonTon_GameManager.TonTon_GameDirection.End)
        {
            PlayAlarmClip();
        }
    }

    void PlayClockClip()
    {
        audioSource.clip = clockClip;
        audioSource.Play();
    }

    void PlayAlarmClip()
    {
        if (isStopped) return;
        isStopped = true;

        audioSource.loop = false;
        audioSource.clip = alarmClip;
        audioSource.Play();
    }
}
