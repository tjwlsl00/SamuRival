using UnityEngine;

public class HeadHitBox : MonoBehaviour
{
    [Header("콜라이더 참조")]
    private Collider hitBox;

    // 스크립트 참조
    private Player player;

    void Awake()
    {
        hitBox = GetComponent<Collider>();

        GameObject rootObject = transform.root.gameObject;
        if (rootObject != null)
        {
            player = rootObject.GetComponent<Player>();
        }
    }

    void Update()
    {
        if(player.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            if(hitBox != null)
            {
                hitBox.enabled = false;
            }
        }
        else
        {
            if(hitBox != null)
            {
                hitBox.enabled = true;
            }
        }
    }

}