using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] public int currentHP;
    [SerializeField] public int maxHP = 100;
    [SerializeField] RectTransform PlayerStatePanel;

    private Player player;
    private PlayerAnimation playerAnimation;
    private PlayerAudio playerAudio;
    private PlayerUI playerUI;

    // bool(눈덩이 맞은 상태/공격 실패/스킬 효과)
    public bool isHit = false;
    public bool Item1Effect = false;
    public bool Item2Effect = false;

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

    public void TakeDamage(int amount, bool isReflection = false)
    {
        // 아이템 상태에 따른 데미지 처리
        if (Item2Effect && !isReflection)
        {
            enemyState.TakeDamage(amount * 2);

            Debug.Log(enemyPlayer.teamSide + "에게 데미지 반사");

            return;
        }

        if (!isReflection)
        {
            isHit = true;
        }

        // 최종 데미지
        int finalDamage = amount;

        // 적이 2배 눈덩이 사용한 상태
        if (!isReflection && enemyState.Item1Effect)
        {
            finalDamage = amount * 2;
            Debug.Log(player.teamSide + "2배 눈덩이에 맞음");
        }
        // 적이 일반적으로 맞췄을때
        else if (!isReflection)
        {
            Debug.Log(player.teamSide + "일반 눈덩이에 맞음");
        }

        // 데미지 처리
        currentHP -= finalDamage;

        // 데미지 애니메이션, UI 업데이트 
        playerAnimation.PlayTakeDamage();
        playerAudio.PlayHitClip();
        playerUI.UpdateGaze();
        UIManager.Instance.ShakeMyUI(PlayerStatePanel);

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log(player.teamSide + "체력 0");
            playerAnimation.PlayDefeat();

            // 게임 상태 GameEnd
            GameManager.Instance.currentDirection = GameManager.GameDirection.GameEnd;
        }
    }

    public IEnumerator ResetHitBool()
    {
        yield return new WaitForSeconds(1.5f);
        isHit = false;

        Debug.Log(enemyPlayer.teamSide + "의 공격 턴이 이어집니다.");
    }
}