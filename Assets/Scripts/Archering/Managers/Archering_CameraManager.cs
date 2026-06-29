using UnityEngine;
using Unity.Cinemachine;

public class Archering_CameraManager : MonoBehaviour
{
    // 싱글톤
    public static Archering_CameraManager Instance;

    [SerializeField] CinemachineCamera freeLockCam;
    [SerializeField] CinemachineCamera redFollowCam;
    [SerializeField] CinemachineCamera blueFollowCam;
    [SerializeField] CinemachineCamera finalCam;

    // 외부 
    private Archering_TurnManager archering_TurnManager;

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

        // 스크립트 참조 
        archering_TurnManager = GetComponent<Archering_TurnManager>();
    }

    void Start()
    {
        ResetCam();
    }

    #region 카메라 초기 세팅
    public void ResetCam()
    {
        freeLockCam.Priority = 1;
        redFollowCam.Priority = 0;
        blueFollowCam.Priority = 0;
        finalCam.Priority = 0;
    }
    #endregion

    #region 추적 카메라 / 점수 카메라
    public void SetFollowTarget(Transform targetObj)
    {
        Debug.Log("추적 카메라로 변경");
        
        if (archering_TurnManager.gameTurn == Archering_TurnManager.GameTurn.RedTurn)
        {
            // 우선 순위 변경
            redFollowCam.Priority = 10;
            blueFollowCam.Priority = 0;

            if (targetObj != null)
            {
                // 타겟 설정
                redFollowCam.Follow = targetObj;
                redFollowCam.LookAt = targetObj;
            }
        }
        else
        {
            redFollowCam.Priority = 0;
            blueFollowCam.Priority = 10;

            if (targetObj != null)
            {
                // 타겟 설정
                blueFollowCam.Follow = targetObj;
                blueFollowCam.LookAt = targetObj;
            }
        }
    }

    public void SwitchScoreCam()
    {
        Debug.Log("점수 카메라로 변경");

        if (finalCam != null)
        {
            freeLockCam.Priority = 0;
            redFollowCam.Priority = 0;
            blueFollowCam.Priority = 0;
            finalCam.Priority = 20;
        }
    }
    #endregion
}
