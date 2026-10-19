using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeSpawnManager : MonoBehaviour
{

    [Header("이동 관련")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool isMoving = false;

    [Header("스폰 관련")]
    [SerializeField] GameObject treePrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float spawnInterval = 5f;
    [SerializeField] private int poolSize = 10;

    [Header("충돌 방지 설정")]
    [SerializeField] private float overlapCheckRadius = 0.5f;
    [SerializeField] private LayerMask collisionLayer;

    // 풀 리스트 
    private List<GameObject> treePool = new List<GameObject>();

    void Start()
    {
        // 풀 초기화
        initializePool();

        // 스폰 루틴 시작 
        StartCoroutine(SpawnRoutine());
    }

    // --------------
    // 풀 초기화
    // --------------
    private void initializePool()
    {
        // 리스트에 넣어놓고 비활성화 
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(treePrefab);
            obj.SetActive(false);
            treePool.Add(obj);
        }
    }

    // 비활성화된 나무 가져오기 
    private GameObject GetTreeFromPool()
    {
        // 리스트 순회하며 정리
        for (int i = treePool.Count - 1; i >= 0; i--)
        {
            if (treePool[i] == null)
            {
                // 사라진 오브젝트 리스트에서 제거 
                treePool.RemoveAt(i);
                continue;
            }

            // 상태 확인 
            if (!treePool[i].activeInHierarchy)
            {
                return treePool[i];
            }
        }

        // 풀이 부족하면 새로 생성 
        GameObject newObj = Instantiate(treePrefab);
        treePool.Add(newObj);
        return newObj;
    }

    void Update()
    {
        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Start)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // 이동
        if (isMoving)
        {
            Move();
        }
        else
        {
            return;
        }
    }

    #region 이동
    private void Move()
    {
        transform.Translate(transform.forward.normalized * moveSpeed * Time.deltaTime, Space.World);
    }
    #endregion

    #region 오브젝트 스폰 
    // -----------
    // 스폰 루틴 
    // -----------
    private IEnumerator SpawnRoutine()
    {
        // 카메라 연출 + 게임 초반 대기 
        yield return new WaitForSeconds(10f);

        while (true)
        {
            if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Start)
            {
                Debug.Log("나무 소환");

                // 소환
                SpawnTree();

                // 스폰 주기 대기 
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // Start 상태가 아니면 잠시 대기 후 다시 상태 체크
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    // -----------
    // 나무 스폰 
    // -----------
    void SpawnTree()
    {
        // 점유되지 않은 포인터 필터링
        List<Transform> availablePoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            if (!Physics.CheckSphere(point.position, overlapCheckRadius, collisionLayer))
            {
                availablePoints.Add(point);
            }
        }

        // 소환 개수 결정 
        int spawnCount = Random.Range(1, 3);
        if (spawnCount > availablePoints.Count) spawnCount = availablePoints.Count;
        if (spawnCount == 0) return;

        // 셔플
        ShuffleList(availablePoints);

        // 풀에서 나무 꺼내기 
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject tree = GetTreeFromPool();
            if (tree != null)
            {
                tree.transform.position = availablePoints[i].position;
                tree.transform.rotation = availablePoints[i].rotation;

                // 활성화
                tree.SetActive(true);
            }
        }
    }
    // -------------
    // 스폰 포인트 셔플 
    // -------------
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    #endregion

    #region 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("GoalLine"))
        {
            Debug.Log("트리 스포너 삭제");

            Destroy(this);
        }
    }
    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.cyan;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                // 스폰 위치
                Gizmos.DrawWireSphere(point.position, overlapCheckRadius);

                // 겹침 확인 영역
                Gizmos.DrawCube(point.position, Vector3.one * 0.1f);
            }
        }
    }
    #endregion
}