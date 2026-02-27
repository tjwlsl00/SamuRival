using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] int snowDamage = 20;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HeadHitBox"))
        {
            // 부모객체에서 스크립트 찾기 
            PlayerState playerState = other.transform.root.GetComponent<PlayerState>();

            if (playerState != null)
            {
                // 플레이어 데미지 입히고 -> 오브젝트 삭제 
                playerState.TakeDamage(snowDamage);
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            // 부모객체에서 스크립트 찾기 
            Player player = other.transform.root.GetComponent<Player>();

            if (player != null)
            {
                // 스턴 카운트 
                if (player.teamSide == TeamSide.Red)
                {
                    CountAttackManager.Instance.AddCount(PlayerType.Blue);
                }
                else
                {
                    CountAttackManager.Instance.AddCount(PlayerType.Red);
                }
                Destroy(gameObject);
            }
        }
    }
}
