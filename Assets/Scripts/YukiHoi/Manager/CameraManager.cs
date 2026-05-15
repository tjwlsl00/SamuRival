using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    // 카메라
    [SerializeField] float targetFOV = 30f;
    [SerializeField] CinemachineCamera vcamRed;
    [SerializeField] CinemachineCamera vcamBlue;

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
        if (num == 0)
        {
            TurnManager.Instance.redAnimation.PlayWin();
            TurnManager.Instance.redAudio.PlayWinClip();

            //정면 시점
            vcamRed.Priority = 10;
            vcamBlue.Priority = 20;

            var lensSettings = vcamBlue.Lens;
            lensSettings.FieldOfView = targetFOV;
            vcamBlue.Lens = lensSettings;
        }
        else
        {
            TurnManager.Instance.blueAnimation.PlayWin();
            TurnManager.Instance.blueAudio.PlayWinClip();

            //정면 시점
            vcamRed.Priority = 20;
            vcamBlue.Priority = 10;

            var lensSettings = vcamRed.Lens;
            lensSettings.FieldOfView = targetFOV;
            vcamRed.Lens = lensSettings;
        }
    }
    #endregion
}