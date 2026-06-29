using UnityEngine;

public class Loading_SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] AudioClip[] YukiHoi;
    [SerializeField] AudioClip[] TonTon;
    [SerializeField] AudioClip[] Archering;
    [SerializeField] AudioClip[] IceSumo;

    // 오디오 배열 
    public AudioClip[] GetNarrationArray(int narrationIndex)
    {
        switch (narrationIndex)
        {
            case 0:
                return YukiHoi;
            case 1:
                return TonTon;
            case 2:
                return Archering;
            case 3:
                return IceSumo;
            case 4:
                return TonTon;
            default:
                return null;
        }
    }
}
