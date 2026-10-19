using UnityEngine;

public class DecoObjSpawner : MonoBehaviour
{
    [Header("오브젝트 참조")]
    [SerializeField] GameObject mapObject; //달리는 필드
    [SerializeField] GameObject[] decoMapObjects; //데코 필드 
    [SerializeField] GameObject fencePrefab;
    [SerializeField] GameObject treePrefab;

    [Header("변수 관련")]
    [SerializeField] private float fenceWidth = 4f;
    [SerializeField] private float treeWidth = 10f;
    [SerializeField] private float widthOffset = 0; // 울타리 좌우 위치 조절 오프셋 
    [SerializeField] private float additionalRotationY = 0f;

    void Start()
    {
        if (mapObject == null || fencePrefab == null)
        {
            return;
        }
        else
        {
            Debug.Log("맵과 울타리가 모두 참조되었습니다.");

            SpawnFencesDynamic();
            SpawnTreesDynamic();
        }
    }

    private void SpawnFencesDynamic()
    {
        // 맵 스케일 정보 참조(너비, 길이)
        float mapWidth = mapObject.transform.localScale.x;
        float mapTotalLength = mapObject.transform.localScale.z;

        // 맵 길이 맞춘 울타리 갯수 자동 계산 
        int requiredFenceCount = Mathf.FloorToInt(mapTotalLength / fenceWidth);

        // 울타리 회전값 가져오기 
        Quaternion prefabRotation = fencePrefab.transform.rotation;

        // 맵 로컬 방향(앞, 오른쪽, 중앙)
        Vector3 mapForward = mapObject.transform.forward;
        Vector3 mapRight = mapObject.transform.right;
        Vector3 mapCenter = mapObject.transform.position;

        // 코너 계산 
        Vector3 leftCornerStart = mapCenter - (mapForward * (mapTotalLength / 2f)) - (mapRight * (mapWidth / 2f));
        Vector3 rightCornerStart = mapCenter - (mapForward * (mapTotalLength / 2f)) + (mapRight * (mapWidth / 2f));

        // 울타리 자체의 중심 축 정렬 보정 (경사면을 타고 반 칸 앞으로 전진)
        leftCornerStart += mapForward * (fenceWidth / 2f);
        rightCornerStart += mapForward * (fenceWidth / 2f);

        // 좌우 폭 오프셋 적용
        leftCornerStart += mapRight * widthOffset;
        rightCornerStart -= mapRight * widthOffset;

        // 부모 오브젝트 생성
        GameObject leftGroup = new GameObject("Left_Fences");
        GameObject rightGroup = new GameObject("Right_Fences");
        leftGroup.transform.parent = this.transform;
        rightGroup.transform.parent = this.transform;

        // 울타리 소환 
        for (int i = 0; i < requiredFenceCount; i++)
        {
            Vector3 forwardOffset = mapForward * (i * fenceWidth);

            // 왼쪽 꼭지점 라인 울타리 생성
            Vector3 leftSpawnPos = leftCornerStart + forwardOffset;
            GameObject leftFence = Instantiate(fencePrefab, leftSpawnPos, prefabRotation);
            leftFence.transform.parent = leftGroup.transform;
            leftFence.name = $"LeftFence_{i}";

            // 오른쪽 꼭지점 라인 울타리 생성
            Vector3 rightSpawnPos = rightCornerStart + forwardOffset;
            GameObject rightFence = Instantiate(fencePrefab, rightSpawnPos, prefabRotation);
            rightFence.transform.parent = rightGroup.transform;
            rightFence.name = $"RightFence_{i}";
        }
    }

    private void SpawnTreesDynamic()
    {
        // 울타리 회전값 가져오기 
        Quaternion prefabRotation = fencePrefab.transform.rotation;

        foreach (GameObject decoMapObject in decoMapObjects)
        {
            if (decoMapObject == null) continue;

            // 맵 스케일 정보 참조(길이)
            float mapTotalLength = decoMapObject.transform.localScale.z;

            // 맵 길이 맞춘 울타리 갯수 자동 계산 
            int requiredFenceCount = Mathf.FloorToInt(mapTotalLength / treeWidth);

            Vector3 mapForward = decoMapObject.transform.forward;
            Vector3 mapCenter = decoMapObject.transform.position;

            Vector3 mapStartPos = mapCenter - (mapForward * (mapTotalLength * 0.5f));

            for (int i = 0; i < requiredFenceCount; i++)
            {
                Vector3 forwardOffset = mapForward * (i * treeWidth + (treeWidth * 0.5f));

                // 스폰 위치 
                Vector3 spawnPos = mapStartPos + forwardOffset;

                // 나무 생성 
                GameObject tree = Instantiate(treePrefab, spawnPos, prefabRotation);

                tree.transform.SetParent(decoMapObject.transform);
            }
        }
    }
}