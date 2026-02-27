using UnityEngine;
using UnityEngine.UI;

public class SnowSplat : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip SplatClip;
    [SerializeField] Image splatImage;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void VisibleSnowSplat()
    {
        splatImage.gameObject.SetActive(true);

        // 눈덩이 스플릿 효과음 
        audioSource.clip = SplatClip;
        audioSource.Play();
    }
}
