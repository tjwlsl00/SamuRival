using UnityEngine;

public class Snowball : MonoBehaviour
{
    [Header("변수 참조")]
    [SerializeField] int snowDamage = 20;

    [Header("파티클 참조")]
    [SerializeField] GameObject snowParticle;

    #region 파티클 토글
    public void ToggleSnowballParticle()
    {
        Debug.Log("스노우 볼 파티클 토글");

        snowParticle.SetActive(!snowParticle.activeSelf);
    }
    #endregion

    #region 충돌 이벤트 
    void OnTriggerEnter(Collider other)
    {
        // 플레이어 히트 박스 
        if (other.gameObject.CompareTag("HeadHitBox"))
        {
            PlayerState playerState = other.transform.root.GetComponent<PlayerState>();

            if (playerState != null)
            {
                // 데미지 호출 
                playerState.TakeDamage(snowDamage);

                // 오브젝트 삭제 
                Destroy(gameObject);
            }
        }
        // 벽
        else if (other.gameObject.CompareTag("Wall"))
        {
            Player player = other.transform.root.GetComponent<Player>();

            if (player != null)
            {
                // 스턴 카운트 증가 
                if (player.teamSide == Player.TeamSide.Red)
                {
                    CountAttackManager.Instance.AddCount(PlayerType.Blue);
                }
                else
                {
                    CountAttackManager.Instance.AddCount(PlayerType.Red);
                }

                // 오브젝트 삭제 
                Destroy(gameObject);
            }
        }
    }
    #endregion
}
