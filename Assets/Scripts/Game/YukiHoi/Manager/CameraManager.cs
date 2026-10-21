using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    // 카메라
    [SerializeField] CinemachineCamera vcamRed;
    [SerializeField] CinemachineCamera vcamBlue;
    [SerializeField] CinemachineCamera vcamRedWin;
    [SerializeField] CinemachineCamera vcamBlueWin;

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

    #region 카메라 시점 변경(플레이 중) / 최종 승자
    public void SwitchTurnAndCamera()
    {
        Debug.Log("카메라 전환 함수 호출됨!");

        if (TurnManager.Instance.red.currentPlayerDirection == Player.PlayerDirection.Attack)
        {
            vcamRed.Priority = 20;
            vcamBlue.Priority = 10;
        }
        else if (TurnManager.Instance.red.currentPlayerDirection == Player.PlayerDirection.Defense)
        {
            vcamRed.Priority = 10;
            vcamBlue.Priority = 20;
        }
    }

    public IEnumerator SwitchFinalCamera(int num)
    {
        Debug.Log("최종 카메라");

        yield return new WaitForSeconds(2f);

        // 모든 카메라 우선순위 초기화
        vcamRed.Priority = 10;
        vcamBlue.Priority = 10;
        if(vcamRedWin != null) vcamRedWin.Priority = 10;
        if(vcamBlueWin != null) vcamBlueWin.Priority = 10;

        if (num == 0)
        {
            TurnManager.Instance.redAnimation.PlayWin();
            TurnManager.Instance.redAudio.PlayWinClip();

            vcamRedWin.Priority = 20;
        }
        else
        {
            TurnManager.Instance.blueAnimation.PlayWin();
            TurnManager.Instance.blueAudio.PlayWinClip();

            vcamBlueWin.Priority = 20;
        }
    }
    #endregion
}