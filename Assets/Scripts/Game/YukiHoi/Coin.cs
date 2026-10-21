using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    private bool isFlipped = false;
    [SerializeField] float jumpHeight;
    [SerializeField] float flipDuration = 2.0f;
    private int DirectionNum = 0;

    private AudioSource audioSource;
    [SerializeField] AudioClip coinClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(TossCoin());
    }

    IEnumerator TossCoin()
    {
        yield return new WaitForSeconds(2f);
        if (isFlipped) yield break;

        // 사운드 효과
        audioSource.clip = coinClip;
        audioSource.Play();

        bool isHeads = Random.value > 0.5f;

        StartCoroutine(FlipRoutine(isHeads));
    }

    IEnumerator FlipRoutine(bool isHeads)
    {
        isFlipped = true;

        // 처음 오브젝트 위치 값 저장.
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float elapsed = 0;

        // 공중에서 동전 회전 수 
        float totalRotation = isHeads ? 1800f : 1980f;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / flipDuration;

            // y축(포물선 그리기- 삼각함수Sin)
            // sin0(바닥), sin90(최고 높이), sin180(바닥)
            // 바닥 -> 최고 높이 -> 바닥으로 포물선 이동 
            float yOffset = Mathf.Sin(time * Mathf.PI) * jumpHeight;
            transform.position = startPos + new Vector3(0, yOffset, 0);

            // 회전
            // time 비율에 맞춰 부드럽게 회전
            float currentRotation = Mathf.Lerp(0, totalRotation, time);
            transform.rotation = Quaternion.Euler(currentRotation, 0, 0);

            yield return null;
        }

        // 처음 있던 위치 
        transform.position = startPos;

        // 루프 이후 앞(0) / 뒤(180) 하나로 결정
        transform.rotation = isHeads ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(180, 0, 0);

        isFlipped = false;
        DetermineResult();
    }

    void DetermineResult()
    {
        if (Vector3.Dot(transform.up, Vector3.up) > 0)
        {
            Debug.Log("앞면입니다.");
            DirectionNum = 0;
        }
        else
        {
            Debug.Log("뒷면입니다.");
            DirectionNum = 1;
        }

        // 선/후공 결과
        TurnManager.Instance.SetPlayerDirection(DirectionNum);
    }
}
