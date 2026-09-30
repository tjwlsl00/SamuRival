using UnityEngine;
using DG.Tweening;

public class TonTon_CameraManager : MonoBehaviour
{
    [SerializeField] Camera red;
    [SerializeField] Camera blue;

    // 게임 종료 시 카메라 위치, 원래 카메라 위치 
    private Vector3 redOriginalPosition;
    private Vector3 blueOriginalPosition;
    [SerializeField] private Vector3 redTargetPosition;
    [SerializeField] private Vector3 blueTargetPosition;

    // 이동 수치 
    [SerializeField] private float moveDuration = 2.0f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;

    void Awake()
    {
        redOriginalPosition = red.transform.position;
        blueOriginalPosition = red.transform.position;
    }

    // 게임 종료 시 카메라 
    public void EndGameCamera()
    {
        red.transform.DOMove(redTargetPosition, moveDuration).SetEase(moveEase);
        red.transform.DORotate(new Vector3(0, 0, 0), moveDuration);

        blue.transform.DOMove(blueTargetPosition, moveDuration).SetEase(moveEase);
        blue.transform.DORotate(new Vector3(0, 0, 0), moveDuration);
    }
}
