using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameDirection
    {
        Coin,
        Ready,
        Play,
        GameEnd
    }
    private GameDirection _currentDirection;
    public GameDirection currentDirection
    {
        get => _currentDirection;
        set
        {
            _currentDirection = value;
            OnStageChanged(_currentDirection);
        }
    }

    // bool 
    private bool isEnd = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentDirection = GameDirection.Coin;
    }

    void OnStageChanged(GameDirection newState)
    {
        if (newState == GameDirection.Ready)
        {
            UIManager.Instance.HideReadyUI();
        }
        else if (newState == GameDirection.GameEnd)
        {
            UIManager.Instance.HideReadyUI();
            WhoIsWinner();
        }
    }

    void WhoIsWinner()
    {
        if (isEnd) return;
        isEnd = true;
        TurnManager.Instance.CheckFinalWinner();
    }

}
