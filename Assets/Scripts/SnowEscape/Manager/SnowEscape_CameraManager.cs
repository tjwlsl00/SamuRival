using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class SnowEscape_CameraManager : MonoBehaviour
{
    [Header("카메라 참조")]
    [SerializeField] CinemachineCamera deco1Cam;
    [SerializeField] CinemachineCamera deco2Cam;
    [SerializeField] CinemachineBrain decoBrain;
    private CinemachineBasicMultiChannelPerlin deco1CameraNoise;
    private CinemachineBasicMultiChannelPerlin deco2CameraNoise;
    [SerializeField] CinemachineBrain redCamBrain;
    [SerializeField] CinemachineBrain blueCamBrain;
    [SerializeField] CinemachineCamera redRaceCam;
    [SerializeField] CinemachineCamera redEndCam;
    [SerializeField] CinemachineCamera blueRaceCam;
    [SerializeField] CinemachineCamera blueEndCam;

    void Awake()
    {
        DisableAllCineCamera();

        // 노이즈 컴포넌트 참조 
        if (deco1Cam != null)
        {
            deco1CameraNoise = deco1Cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    IEnumerator Start()
    {
        // 카메라 활성화
        EnableDecoCamera();

        // Deco1 카메라 진동 시작 
        ShakeDeco1Camera();

        yield return new WaitForSeconds(4f);

        // 초기 세팅
        InitialSetting(1);
        
        // Deco2 카메라 진동 시작 
        ShakeDeco2Camera();

        yield return new WaitForSeconds(1.5f);

        // 카메라 비활성화 
        DisableDecoCamera();

        // 레이싱 카메라 전환
        SwitchGameRaceCameras();
    }

    #region 데코 카메라
    void EnableDecoCamera()
    {
        deco1Cam.gameObject.SetActive(true);
        deco2Cam.gameObject.SetActive(true);
        if (decoBrain != null) decoBrain.gameObject.SetActive(true);
    }

    void DisableDecoCamera()
    {
        deco1Cam.gameObject.SetActive(false);
        deco2Cam.gameObject.SetActive(false);
        if (decoBrain != null) decoBrain.gameObject.SetActive(false);
    }

    // 진동
    void ShakeDeco1Camera()
    {
        if (deco1CameraNoise != null)
        {
            StartCoroutine(ShakeRoutine(0, 4f, 2f));
        }
    }

    void ShakeDeco2Camera()
    {
        if (deco2CameraNoise != null)
        {
            StartCoroutine(ShakeRoutine(1, 4f, 2f));
        }
    }

    IEnumerator ShakeRoutine(int decoIndex, float duration, float intensity)
    {
        CinemachineBasicMultiChannelPerlin targetNoise = (decoIndex == 0) ? deco1CameraNoise : deco2CameraNoise;

        if (targetNoise != null)
        {
            // 진동 켜기
            targetNoise.AmplitudeGain = intensity;

            // 4초 대기
            yield return new WaitForSeconds(duration);

            // 진동 끄기
            targetNoise.AmplitudeGain = 0f;
        }
    }
    #endregion

    #region 초기 설정
    void DisableAllCineCamera()
    {
        deco1Cam.gameObject.SetActive(false);
        deco2Cam.gameObject.SetActive(false);
        redCamBrain.gameObject.SetActive(false);
        blueCamBrain.gameObject.SetActive(false);

        InitialSetting(0);
    }

    void InitialSetting(int setIndex)
    {
        if (setIndex == 0)
        {
            Debug.Log("연출 1");

            deco1Cam.Priority = 10;
            deco2Cam.Priority = 0;
        }
        else
        {
            Debug.Log("연출 2");

            deco1Cam.Priority = 0;
            deco2Cam.Priority = 10;
        }
    }
    #endregion

    // 게임 씬 카메라 전환 
    void SwitchGameRaceCameras()
    {
        Debug.Log("레이스 카메라 ");

        redCamBrain.gameObject.SetActive(true);
        blueCamBrain.gameObject.SetActive(true);

        // ------
        // 카메라 우선순위 
        // ------
        redRaceCam.Priority = 10;
        redEndCam.Priority = 0;
        blueRaceCam.Priority = 10;
        blueEndCam.Priority = 0;
    }

    public void SwitchEndCameras()
    {
        Debug.Log("레이스 종료 카메라");

        // ------
        // 카메라 우선순위 
        // ------
        redRaceCam.Priority = 0;
        redEndCam.Priority = 10;
        blueRaceCam.Priority = 0;
        blueEndCam.Priority = 10;
    }
}