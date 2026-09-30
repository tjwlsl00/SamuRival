using UnityEngine;
using System.Collections;

public class UIEffect : MonoBehaviour
{
    public static UIEffect Instance;

    private AudioSource audioSource;
    [SerializeField] AudioClip ReadyClip;
    [SerializeField] AudioClip SnowballClip;
    [SerializeField] AudioClip AttackChanceClip;
    [SerializeField] AudioClip TurnChangeClip;
    [SerializeField] AudioClip ItemActiveClip;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayReady()
    {
        audioSource.clip = ReadyClip;
        audioSource.Play();
    }

    public void PlaySnowball()
    {
        audioSource.clip = SnowballClip;
        audioSource.Play();
    }

    public void PlayAttackChance()
    {
        audioSource.clip = AttackChanceClip;
        audioSource.Play();
    }

    public void PlayTurnChange()
    {
        audioSource.clip = TurnChangeClip;
        audioSource.Play();
    }

    public void PlayItemActive()
    {
        audioSource.clip = ItemActiveClip;
        audioSource.Play();
    }
}
