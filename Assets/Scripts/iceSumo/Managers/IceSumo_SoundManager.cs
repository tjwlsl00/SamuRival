using UnityEngine;
using System.Collections;

public class IceSumo_SoundManager : MonoBehaviour
{
    // 싱글톤
    public static IceSumo_SoundManager Instance;

    [Header("오디오 참조")]
    private AudioSource audioSource;
    [SerializeField] AudioClip gameStartClip;
    [SerializeField] AudioClip penguinClip;
    [SerializeField] AudioClip drawClip;
    [SerializeField] AudioClip becarefulClip;
    [SerializeField] AudioClip reduceMapClip;
    [SerializeField] AudioClip gameOverClip;
    [SerializeField] AudioClip redWinClip;
    [SerializeField] AudioClip blueWinClip;

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

    void Start()
    {
        PlayGameStartClip();
        StartCoroutine(PlayPenguinClip(gameStartClip));
    }

    IEnumerator PlayPenguinClip(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
        if (penguinClip != null)
        {
            audioSource.PlayOneShot(penguinClip, 0.8f);
        }
    }

    private void PlayGameStartClip()
    {
        if (gameStartClip != null)
            audioSource.PlayOneShot(gameStartClip);
    }

    public void PlayDrawClip()
    {
        if (drawClip != null)
            audioSource.PlayOneShot(drawClip);
    }

    public void PlayBeCarefulClip()
    {
        if (becarefulClip != null)
            audioSource.PlayOneShot(becarefulClip);

        StartCoroutine(PlayReduceMapClip(becarefulClip));
    }

    IEnumerator PlayReduceMapClip(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);

        if (reduceMapClip != null)
            audioSource.PlayOneShot(reduceMapClip);
    }

    public IEnumerator PlayWinnerClip(int playerIndex)
    {
        if (gameOverClip != null)
        {
            audioSource.PlayOneShot(gameOverClip);

            AudioClip targetClip = (playerIndex == 0) ? redWinClip : blueWinClip;
            
            if (targetClip != null)
            {
                yield return new WaitForSeconds(gameOverClip.length);
                audioSource.PlayOneShot(targetClip);
            }
        }
    }
}