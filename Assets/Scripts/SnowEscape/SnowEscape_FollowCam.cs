using UnityEngine;

public class SnowEscape_FollowCam : MonoBehaviour
{
    // 플레이어 참조 
    [SerializeField] GameObject player;

    [Header("대쉬 파티클 참조")]
    [SerializeField] ParticleSystem speedLineParticle;

    // bool 
    private bool isTurnOff = false;

    // 스크립트
    private SnowEscape_Player snowEscape_Player;

    void Awake()
    {
        snowEscape_Player = player.GetComponent<SnowEscape_Player>();
    }

    void Update()
    {
        if (isTurnOff) return;

        if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.Start)
        {
            ChangeParticleEmission();
        }
        else if (SnowEscape_GameManager.Instance.gameDirection == SnowEscape_GameManager.GameDirection.End)
        {
            isTurnOff = true;
            speedLineParticle.Stop();
        }
    }

    void ChangeParticleEmission()
    {
        var emission = speedLineParticle.emission;

        if (snowEscape_Player.isUlting)
        {
            emission.rateOverTime = 70f;
        }
        else if(!snowEscape_Player.isUlting && snowEscape_Player.isDashing)
        {
            emission.rateOverTime = 40f;
        }
        else
        {
            emission.rateOverTime = 20f;
        }
    }
}