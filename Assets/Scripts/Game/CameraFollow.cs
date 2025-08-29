using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineCamera _cam; 
    private void OnEnable()
    {
        PlayerController.OnplayerPosition += SetTarget;
    }
    private void OnDisable()
    {
        PlayerController.OnplayerPosition -= SetTarget;
    }
    private void Awake()
    {
        _cam = GetComponent<CinemachineCamera>();
    }
    private void SetTarget(Transform player)
    {
        _cam.Target.TrackingTarget= player;
    }
}
