using UnityEngine;

public class UIEffect : MonoBehaviour
{
    public static UIEffect Instance;

    private AudioSource audioSource;
    [SerializeField] AudioClip ReadyClip;

    void Awake()
    {
        if(Instance == null)
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
}
