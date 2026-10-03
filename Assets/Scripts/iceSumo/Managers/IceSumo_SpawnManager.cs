using UnityEngine;

public class IceSumo_SpawnManager : MonoBehaviour
{
    [Header("플레이어 참조")]
    [SerializeField] GameObject[] players;
    [SerializeField] Transform[] spawnPoints;

    void Start()
    {
        SettingPlayerPosition();
    }

    void SettingPlayerPosition()
    {
        if (players == null || spawnPoints == null)
        {
            return;
        }

        int loopCount = Mathf.Min(players.Length, spawnPoints.Length);

        for (int i = 0; i < loopCount; i++)
        {
            if (players[i] != null && spawnPoints[i] != null)
            {
                // 플레이어 비활성화(떨어짐 방지)
                players[i].SetActive(false);

                // 위치 이동
                players[i].transform.position = spawnPoints[i].transform.position;

                // 회전 
                players[i].transform.rotation = spawnPoints[i].transform.rotation;

                Rigidbody rigidbody = players[i].GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    // 물리 연산 잠시 끄기 속도 초기화
                    rigidbody.isKinematic = true;
                    rigidbody.linearVelocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;

                    // 위치, 회전 값 
                    rigidbody.position = spawnPoints[i].position;
                    rigidbody.rotation = spawnPoints[i].rotation;
                }

                // Transform 위치 회전 Rigidbody와 동기화
                players[i].transform.position = spawnPoints[i].position;
                players[i].transform.rotation = spawnPoints[i].rotation;

                // 오브젝트 활성화
                players[i].SetActive(true);

                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;
                }
            }
            else
            {
                Debug.LogWarning($"{i}번째 플레이어 또는 스폰 포인트가 null");
            }
        }
    }
}
