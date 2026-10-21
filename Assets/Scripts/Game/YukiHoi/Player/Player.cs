using UnityEngine;

public class Player : MonoBehaviour
{
    public enum TeamSide { Red, Blue }
    public TeamSide teamSide;

    public enum PlayerDirection
    {
        Attack,
        Defense
    }
    public PlayerDirection currentPlayerDirection;

    // 입력 키 참조 
    private KeyCode leftKey;
    private KeyCode rightKey;
    private int teamIndex;

    [Header("변수 참조")]
    public int sideAngle;

    void Start()
    {
        MappingInputKey();
    }

    #region 키 매핑
    private void MappingInputKey()
    {
        if (teamSide == TeamSide.Red)
        {
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            teamIndex = 0;
        }
        else
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            teamIndex = 1;
        }
    }
    #endregion

    void Update()
    {
        // 게임 일시정지
        if(GameManager.Instance.isPaused) return;
        
        if (GameManager.Instance.currentDirection != GameManager.GameDirection.Ready) return;
        if (TurnManager.Instance.isBlueStun || TurnManager.Instance.isRedStun) return;
        if (TurnManager.Instance.isBlueStun || TurnManager.Instance.isRedStun) return;

        // 옵션 선택 가능(공격/방어)
        SelectOption();
    }

    #region 옵션 선택(공격/방어)
    public void SelectOption()
    {
        if (teamSide == TeamSide.Red && (TurnManager.Instance.isRedReady || ItemManager.Instance.isRedItemGoActive)) return;
        if (teamSide == TeamSide.Blue && (TurnManager.Instance.isBlueReady || ItemManager.Instance.isBlueItemGoActive)) return;

        if (Input.GetKeyDown(leftKey))
        {
            HandleInput(0);
        }
        else if (Input.GetKeyDown(rightKey))
        {
            HandleInput(1);
        }
    }

    private void HandleInput(int directionValue)
    {
        Debug.Log(directionValue == 0 ? "왼쪽" : "오른쪽");

        // 인풋값 전달
        TurnManager.Instance.GetPlayerOptionValue(teamIndex, directionValue);

        // 앵글 저장 
        sideAngle = directionValue;
    }
    #endregion

    // 턴 변경 
    public void ChangeDirection()
    {
        currentPlayerDirection = (currentPlayerDirection == PlayerDirection.Attack)
            ? PlayerDirection.Defense
            : PlayerDirection.Attack;
    }
}
