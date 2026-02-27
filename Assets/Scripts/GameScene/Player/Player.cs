using UnityEngine;

public enum TeamSide { Red, Blue }

public class Player : MonoBehaviour
{
    public TeamSide teamSide;

    public enum PlayerDirection
    {
        Attack,
        Defense
    }
    public PlayerDirection currentPlayerDirection;

    // 내부 변수 
    public int sideAngle;

    void Update()
    {
        if (GameManager.Instance.currentDirection == GameManager.GameDirection.Ready)
        {
            if (TurnManager.Instance.isBlueStun || TurnManager.Instance.isRedStun) return;

            // 옵션 선택(공격/방어)
            SelectOption();
        }
    }

    public void SelectOption()
    {
        if (teamSide == TeamSide.Red)
        {
            if (TurnManager.Instance.isRedReady || ItemManager.Instance.isRedItemGoActive) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("왼 선택");

                TurnManager.Instance.GetRedOptionNum(0);

                if (currentPlayerDirection == PlayerDirection.Attack)
                {
                    sideAngle = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("오른 선택");

                TurnManager.Instance.GetRedOptionNum(1);

                if (currentPlayerDirection == PlayerDirection.Attack)
                {
                    sideAngle = 1;
                }
            }
        }
        else
        {
            if (TurnManager.Instance.isBlueReady || ItemManager.Instance.isBlueItemGoActive) return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("왼 선택");

                TurnManager.Instance.GetBlueOptionNum(0);

                if (currentPlayerDirection == PlayerDirection.Attack)
                {
                    sideAngle = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("오른 선택");

                TurnManager.Instance.GetBlueOptionNum(1);

                if (currentPlayerDirection == PlayerDirection.Attack)
                {
                    sideAngle = 1;
                }
            }
        }
    }

    // 턴 변경 
    public void ChangeDirection()
    {
        if (currentPlayerDirection == PlayerDirection.Attack)
        {
            currentPlayerDirection = PlayerDirection.Defense;
        }
        else
        {
            currentPlayerDirection = PlayerDirection.Attack;
        }
    }
}
