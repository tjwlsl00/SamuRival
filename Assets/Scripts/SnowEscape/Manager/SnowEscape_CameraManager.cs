using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class SnowEscape_CameraManager : MonoBehaviour
{
    [Header("카메라 참조")]
    [SerializeField] CinemachineCamera deco1Cam;
    [SerializeField] CinemachineCamera deco2Cam;
    [SerializeField] CinemachineBrain decoBrain;
    [SerializeField] CinemachineBrain redCamBrain;
    [SerializeField] CinemachineBrain blueCamBrain;

    // 스크립트 참조
    private SnowEscape_GameManager snowEscape_GameManager;

    void Awake()
    {
        DisableAllCineCamera();

        snowEscape_GameManager = GetComponent<SnowEscape_GameManager>();
    }

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

    #region 데코 카메라
    void EnableDecoCamera()
    {
        deco1Cam.gameObject.SetActive(true);
        deco2Cam.gameObject.SetActive(true);
    }

    void DisableDecoCamera()
    {
        deco1Cam.gameObject.SetActive(false);
        deco2Cam.gameObject.SetActive(false);
        decoBrain.gameObject.SetActive(false);
    }
    #endregion

    void SwitchGameSceneCamere()
    {
        Debug.Log("게임 카메라 세팅");

        redCamBrain.gameObject.SetActive(true);
        blueCamBrain.gameObject.SetActive(true);
    }

    IEnumerator Start()
    {
        EnableDecoCamera();

        yield return new WaitForSeconds(4f);

        InitialSetting(1);

        yield return new WaitForSeconds(1.5f);

        snowEscape_GameManager.InitialGameSetting();
        DisableDecoCamera();
        SwitchGameSceneCamere();
    }
}
