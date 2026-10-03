using UnityEngine;
using System.Collections.Generic;

public enum PlayerType { Red, Blue }
public enum MoveDirection { Left, Right }

public class CountAttackManager : MonoBehaviour
{
    // 싱글톤
    public static CountAttackManager Instance;

    [SerializeField] int maxStackCount = 4;

    // 플레이어 카운트 데이터
    private class PlayerCountData
    {
        public int stackCount = 0;

        public void Reset()
        {
            stackCount = 0;
        }
    }

    // 플레이어 타입 키 딕셔너리
    private Dictionary<PlayerType, PlayerCountData> playerRecords = new Dictionary<PlayerType, PlayerCountData>();

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 데이터 초기화
        playerRecords.Add(PlayerType.Red, new PlayerCountData());
        playerRecords.Add(PlayerType.Blue, new PlayerCountData());
    }

    #region 카운팅 처리 / 패널티
    public void AddCount(PlayerType playerType)
    {
        if (!playerRecords.ContainsKey(playerType)) return;
        PlayerCountData data = playerRecords[playerType];

        data.stackCount++;

        // 스택 체크
        CheckStackCount(playerType, data.stackCount);

        // UI 업데이트
        UIManager.Instance.UpdateCounterAttack(playerType, data.stackCount);
    }

    public void CheckStackCount(PlayerType playerType, int count)
    {
        if (count >= maxStackCount)
        {
            Debug.Log("역공 찬스가 주어집니다!");

            if (playerType == PlayerType.Red)
            {
                TurnManager.Instance.isBlueStun = true;
                UIManager.Instance.UpdateCounterAttack(playerType, 0);
            }
            else
            {
                TurnManager.Instance.isRedStun = true;
                UIManager.Instance.UpdateCounterAttack(playerType, 0);
            }

            // 패널티 적용 후 데이터 초기화 
            playerRecords[playerType].Reset();
        }
    }
    #endregion

}
