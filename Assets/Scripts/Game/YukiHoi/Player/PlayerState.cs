using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    [SerializeField] public int currentHP;
    [SerializeField] public int maxHP = 10;

    private Player player;
    private PlayerAnimation playerAnimation;
    private PlayerAudio playerAudio;
    private PlayerUI playerUI;

    // bool(눈덩이 맞은 상태/공격 실패/스킬 효과)
    public bool isHit = false;
    public bool Item1Effect = false;
    public bool Item2Effect = false;
    public bool Item3Effect = false;

    // 외부 
    [SerializeField] GameObject enemyObj;
    private Player enemyPlayer;
    private PlayerState enemyState;

    void Awake()
    {
        player = GetComponent<Player>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerUI = GetComponent<PlayerUI>();
        playerAudio = GetComponent<PlayerAudio>();

        enemyPlayer = enemyObj.GetComponent<Player>();
        enemyState = enemyObj.GetComponent<PlayerState>();
    }

    void Start()
    {
        currentHP = maxHP;
    }

    #region 데미지 처리 
    public void TakeDamage(int amount, bool isReflection = false)
    {
        // 최종 데미지
        int finalDamage = amount;

        // 반사 아이템 사용 
        if (Item3Effect && !isReflection)
        {
            Debug.Log(enemyPlayer.teamSide + "에게 데미지 반사");

            if (enemyState != null)
            {
                enemyState.TakeDamage(finalDamage, true);

                isHit = true;
                return;
            }
        }
        // 적이 2배 데미지 아이템 사용
        else if (enemyState.Item1Effect && !isReflection)
        {
            Debug.Log(player.teamSide + "2배 눈덩이에 맞음");

            finalDamage = amount * 2;
        }
        // 일반 
        else if (!isReflection)
        {
            Debug.Log(player.teamSide + "일반 눈덩이에 맞음");
        }

        if (!isReflection)
        {
            isHit = true;
        }

        // 데미지 처리
        currentHP -= finalDamage;

        // 체력 상태 체크 
        CheckPlayerLastHP();

        // 효과(사운드 / UI)
        playerAnimation.PlayTakeDamage();
        playerAudio.PlayHitClip();
        playerUI.UpdateGaze(false);
    }

    void CheckPlayerLastHP()
    {
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log(player.teamSide + "체력 0");
            playerAnimation.PlayDefeat();

            // 게임 상태 GameEnd
            GameManager.Instance.currentDirection = GameManager.GameDirection.GameEnd;
        }
        else
        {
            return;
        }
    }
    #endregion

    public IEnumerator ResetHitBool()
    {
        yield return new WaitForSeconds(1.5f);
        isHit = false;

        Debug.Log(enemyPlayer.teamSide + "의 공격 턴이 이어집니다.");
    }
}