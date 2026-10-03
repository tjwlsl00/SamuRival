using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class IceSumo_GhostEffect : MonoBehaviour
{
    [Header("잔상 참조")]
    [SerializeField] float ghostDelay = 0.05f;
    [SerializeField] float ghostLifeTime = 0.5f;
    [SerializeField] float startAlpha = 0.6f;
    [SerializeField] float positionOffset = 0.3f;
    [SerializeField] Material ghostMaterial;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    // bool 참조
    private bool isEffectRunning = false;

    // 스크립트 참조 
    private IceSumo_Player iceSumo_Player;

    void Awake()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        iceSumo_Player = GetComponent<IceSumo_Player>();
    }

    void Update()
    {
        if (iceSumo_Player == null) return;

        if (iceSumo_Player.isDashing && !isEffectRunning)
        {
            isEffectRunning = true;
            StartCoroutine(GhostCoroutine());
        }
        else if (!iceSumo_Player.isDashing && isEffectRunning)
        {
            isEffectRunning = false;
        }
    }

    IEnumerator GhostCoroutine()
    {
        while (isEffectRunning)
        {
            foreach (var smr in skinnedMeshRenderers)
            {
                if (!smr.gameObject.activeInHierarchy) continue;

                Vector3 spawnPosition = smr.transform.position - (smr.transform.forward * positionOffset);

                GameObject ghostObject = new GameObject("GhostSnapShot");

                ghostObject.transform.position = spawnPosition;
                ghostObject.transform.rotation = smr.transform.rotation;
                ghostObject.transform.localScale = smr.transform.localScale;

                MeshFilter meshFilter = ghostObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = ghostObject.AddComponent<MeshRenderer>();

                // 현재 포즈 메쉬 굽기
                Mesh mesh = new Mesh();
                smr.BakeMesh(mesh);
                meshFilter.mesh = mesh;

                Material[] originalMaterials = smr.materials;
                List<Material> ghostMaterials = new List<Material>();

                foreach (var originalMaterial in originalMaterials)
                {
                    Material newMat = new Material(originalMaterial);

                    SetupURPTransparentMaterial(newMat);

                    if (newMat.HasProperty("_BaseColor"))
                    {
                        Color c = newMat.GetColor("_BaseColor");
                        c.a = startAlpha;
                        newMat.SetColor("_BaseColor", c);
                    }

                    ghostMaterials.Add(newMat);
                }

                meshRenderer.materials = ghostMaterials.ToArray();

                StartCoroutine(FadeOutGhost(meshRenderer, ghostLifeTime));
            }
            yield return new WaitForSeconds(ghostDelay);
        }
    }

    // URP 불투명 재질을 투명 재질로 실시간 변환해주는 함수
    void SetupURPTransparentMaterial(Material material)
    {
        material.SetFloat("_Surface", 1); // 1 이 Transparent(투명)를 의미합니다.
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    IEnumerator FadeOutGhost(MeshRenderer meshRenderer, float lifeTime)
    {
        float elapsedTime = 0f;
        List<Color> startColors = new List<Color>();
        foreach (var mat in meshRenderer.materials)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                startColors.Add(mat.GetColor("_BaseColor"));
            }
            else
            {
                startColors.Add(Color.white);
            }
        }

        while (elapsedTime < lifeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / ghostLifeTime;

            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Material material = meshRenderer.materials[i];
                if (material.HasProperty("_BaseColor"))
                {
                    Color startColor = startColors[i];
                    float alpha = Mathf.Lerp(startColor.a, 0f, progress);
                    material.SetColor("_BaseColor", new Color(startColor.r, startColor.g, startColor.b, alpha));
                }
            }
            yield return null;
        }

        if (meshRenderer != null)
        {
            // 메모리 해제 및 오브젝트 삭제
            MeshFilter mf = meshRenderer.GetComponent<MeshFilter>();
            if (mf != null && mf.mesh != null) Destroy(mf.mesh);

            foreach (var mat in meshRenderer.materials) Destroy(mat);
            Destroy(meshRenderer.gameObject);
        }
    }
}
