using UnityEngine;

public class Archering_SoundManager : MonoBehaviour
{
    // 싱글톤
    public static Archering_SoundManager Instance;

    private AudioSource audioSource;
    [Header("사운드 참조")]
    [SerializeField] AudioClip turnDecideClip;
    [SerializeField] AudioClip[] whoIsFirstClips;
    [SerializeField] AudioClip resultClip;
    [SerializeField] AudioClip drawClip;
    [SerializeField] AudioClip[] whoIsWinner;
    [SerializeField] AudioClip buttonClip;
    [SerializeField] AudioClip fireStone;
    [SerializeField] AudioClip stoneCrashClip;

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
        
        // 컴포넌트 참조 
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayTurnDecideClip();
    }

    #region 나레이션
    // 순서를 정할거야!
    public void PlayTurnDecideClip()
    {
        if (turnDecideClip != null)
        {
            audioSource.PlayOneShot(turnDecideClip, 1f);
        }
    }

    // []부터 시작!
    public void PlayWhoIsFirst(int playerNum)
    {
        if (whoIsFirstClips != null && whoIsFirstClips.Length >= 2)
        {
            audioSource.PlayOneShot(whoIsFirstClips[playerNum], 0.8f);
        }
    }

    // 과연 결과는?
    public void PlayResultClip()
    {
        if (resultClip != null)
        {
            audioSource.PlayOneShot(resultClip, 1f);
        }
    }

    // 무승부
    public void PlayDrawClip()
    {
        if (drawClip != null)
        {
            audioSource.PlayOneShot(drawClip, 1f);
        }
    }

    // 승자
    public void PlayWhoIsWinner(int playerNum)
    {
        if (whoIsWinner != null && whoIsWinner.Length >= 2)
        {
            audioSource.PlayOneShot(whoIsWinner[playerNum], 1f);
        }
    }
    #endregion

    #region 게임 효과음
    // 버튼 클릭
    public void PlayButtnClip()
    {
        if (buttonClip != null)
        {
            audioSource.PlayOneShot(buttonClip, 1f);
        }
    }

    // 스톤 발사 
    public void PlayFireStone()
    {
        if (fireStone != null)
        {
            audioSource.PlayOneShot(fireStone, 1f);
        }
    }

    public void PlayStoneCrash()
    {
        if (stoneCrashClip != null)
        {
            audioSource.PlayOneShot(stoneCrashClip, 0.7f);
        }
    }
    #endregion
}
