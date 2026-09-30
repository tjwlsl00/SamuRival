using UnityEngine;

public class Cylinder : MonoBehaviour
{
    // 변수 선언 
    [SerializeField] private float targetSize;

    // 매쉬렌더러 참조 
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("오브젝트에 메쉬(MeshFilter)가 없습니다!");
        }
    }

    void Update()
    {
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Bounds localBounds = meshFilter.sharedMesh.bounds;
        float currentX = localBounds.size.x * transform.localScale.x;
        float currentZ = localBounds.size.z * transform.localScale.z;

        bool isHide = false;

        if (currentX <= targetSize && currentZ <= targetSize)
        {
            isHide = true;
        }

        if (isHide)
        {
            if (meshRenderer.enabled) meshRenderer.enabled = false;
        }
        else
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
        }
    }
}