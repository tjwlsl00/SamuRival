using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image GazeBar;
    // 외부 
    private PlayerState playerState;

    void Awake()
    {
        playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        UpdateGaze();
    }

    public void UpdateGaze()
    {
        float ratio = (float)playerState.currentHP / (float)playerState.maxHP;
        GazeBar.DOFillAmount(ratio, 0.5f);
    }
    
}