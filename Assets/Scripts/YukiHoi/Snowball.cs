using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] int snowDamage = 20;
    // 파티클
    [SerializeField] GameObject snowParticle;

    #region 파티클 비주얼 생성 / 제거 
    public void OnOffParticle(bool isVisible)
    {
        if (isVisible == true)
        {
            Debug.Log("눈덩이 파티클 켬");
        }
        else
        {
            Debug.Log("눈덩이 파티클 끔");
        }

        snowParticle.SetActive(isVisible);
    }
    #endregion

    // 충돌 이벤트 
    void OnTriggerEnter(Collider other)
    {
        // 플레이어 머리 트리거에 맞은 눈덩이가 맞은 경우 
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
        // 플레이어를 못 맞추고 뒤에 있는 벽에 눈덩이가 맞은 경우
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
