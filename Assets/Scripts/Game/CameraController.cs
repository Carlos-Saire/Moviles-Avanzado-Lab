using Unity.Cinemachine;
using UnityEngine;
using System;
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cam;
    public static Func<Transform> OnPlayerTransform;
    private void OnEnable()
    {
        PlayerController.OnplayerPosition += SetTarget;
    }
    private void OnDisable()
    {
        PlayerController.OnplayerPosition -= SetTarget;
    }
    private void SetTarget(Transform player)
    {
        _cam.Target.TrackingTarget = player;
    }

}
