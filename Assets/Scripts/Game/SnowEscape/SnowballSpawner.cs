using UnityEngine;
using System.Collections;

public class SnowballSpawner : MonoBehaviour
{

    [Header("오브젝트 참조")]
    [SerializeField] GameObject snowballPrefab;
    [SerializeField] Transform snowballSpawnPoint;

    void Start()
    {
        if (snowballPrefab != null && snowballSpawnPoint != null)
        {
            StartCoroutine(SnowballSpawnRoutine());
        }
    }

    IEnumerator SnowballSpawnRoutine()
    {
        // 연출 + 초반 대기 
        yield return new WaitForSeconds(8f);

        // 소환 
        Instantiate(snowballPrefab, snowballSpawnPoint.position, snowballSpawnPoint.rotation);
    }
}