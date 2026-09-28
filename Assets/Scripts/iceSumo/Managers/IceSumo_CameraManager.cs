using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class IceSumo_CameraManager : MonoBehaviour
{
    [Header("카메라 참조")]
    [SerializeField] CinemachineBrain brain;
    [SerializeField] CinemachineCamera initCamera;
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera[] mapIndexCameras;
    [SerializeField] CinemachineCamera redWinCamera;
    [SerializeField] CinemachineCamera blueWinCamera;

    // 스크립트 참조
    private IceSumo_RoundManager iceSumo_RoundManager;

    void Awake()
    {
        iceSumo_RoundManager = GetComponent<IceSumo_RoundManager>();
    }

    void Start()
    {
        ResetCamera();
        StartCoroutine(SetUpRoundCamera());
    }

    // 카메라 초기화
    void ResetCamera()
    {
        initCamera.Priority = 10;
        roundCamera.Priority = 0;
        foreach (var mapIndexCamera in mapIndexCameras)
        {
            mapIndexCamera.Priority = 0;
        }
        redWinCamera.Priority = 0;
        blueWinCamera.Priority = 0;
    }

    #region 라운드 카메라 / 변환
    IEnumerator SetUpRoundCamera()
    {
        yield return new WaitForSeconds(3f);
        initCamera.Priority = 0;
        roundCamera.Priority = 10;
    }

    public void ChangeRoundCamera(int mapIndex)
    {
        if (mapIndex == 0)
        {
            roundCamera.Priority = 0;
            mapIndexCameras[0].Priority = 10;
        }
        else if (mapIndex == 1)
        {
            mapIndexCameras[0].Priority = 0;
            mapIndexCameras[1].Priority = 10;
        }
    }
    #endregion

    // 승자 카메라 
    public void SetUpFinalWinnerCamera()
    {
        // 카메라 연출 시간 단축
        brain.DefaultBlend = new CinemachineBlendDefinition(
                brain.DefaultBlend.Style,
                1f
            );

        // 카메라 우선순위
        initCamera.Priority = 0;
        roundCamera.Priority = 0;

        foreach (var mapIndexCamera in mapIndexCameras)
        {
            mapIndexCamera.Priority = 0;
        }

        if (iceSumo_RoundManager.isRedWin)
        {
            redWinCamera.Priority = 10;
        }
        else
        {
            blueWinCamera.Priority = 10;
        }
    }
}