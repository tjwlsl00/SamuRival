using UnityEngine;
using System.Collections.Generic;

public class BoardMeshTrail : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float rayDistance = 1.5f;
    [SerializeField] private float pointSpacing = 0.05f;
    [SerializeField] private float lifeTime = 2f;
    private LineRenderer lineRenderer;

    private struct TrailPoint
    {
        public Vector3 position;
        public float timeCreated;
        public TrailPoint(Vector3 pos, float time) { position = pos; timeCreated = time; }
    }

    private List<TrailPoint> points = new List<TrailPoint>();
    private bool isGroundedLastFrame = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        // 보드 중심 아래로 레이캐스트
        bool isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.3f, Vector3.down, out hit, rayDistance, groundLayer);

        if (isGrounded)
        {
            // 제트파이팅(깜빡임) 방지 오프셋 적용한 정확한 바닥 좌표
            Vector3 currentGroundPos = hit.point + hit.normal * 0.015f;

            // 공중에 있다가 처음 착지했거나, 마지막 마디와의 거리가 설정한 간격(pointSpacing)을 넘었을 때만 좌표 추가
            if (!isGroundedLastFrame || points.Count == 0 || Vector3.Distance(points[points.Count - 1].position, currentGroundPos) > pointSpacing)
            {
                // 착지 순간 공중에서부터 선이 길게 튀어 꼽히는 현상 방지
                if (!isGroundedLastFrame && points.Count > 0)
                {
                    // 공중 상태였다면 이전 기록을 한 번 리셋하거나 끊기
                    points.Clear();
                }

                points.Add(new TrailPoint(currentGroundPos, Time.time));
            }
        }

        isGroundedLastFrame = isGrounded;

        // 시간이 지난 오래된 좌표들은 리스트에서 제거
        while (points.Count > 0 && Time.time - points[0].timeCreated > lifeTime)
        {
            points.RemoveAt(0);
        }

        // 라인 렌더러에 최종 좌표들 그려주기
        DrawLine();
    }

    void DrawLine()
    {
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}