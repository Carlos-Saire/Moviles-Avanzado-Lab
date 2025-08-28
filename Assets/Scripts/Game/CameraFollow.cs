using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    [Header("Settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed;
    private Vector3 desiredPosition;
    private void OnEnable()
    {
        PlayerController.OnplayerPosition += SetTarget;
    }
    private void OnDisable()
    {
        PlayerController.OnplayerPosition -= SetTarget;
    }
    private void LateUpdate()
    {
        if (target == null) return;

        desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
    private void SetTarget(Transform player)
    {
        target = player;
    }
}
