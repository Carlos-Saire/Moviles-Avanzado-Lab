using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkRigidbody))]
public class PlayerController : NetworkBehaviour
{
    public static event Action<Transform> OnplayerPosition;

    [Header("Characteristics")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private Rigidbody rb;

    [Header("Input")]
    private float horizontal;
    private float vertical;
    private bool isJump;

    [Header("Raycast Jump")]
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layer;

    [Header("Camera")]
    private Transform cameraMain;
    private Vector3 camForward;
    private Vector3 camRight;

    private Vector3 direction;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        GetComponent<NetworkRigidbody>().AutoUpdateKinematicState = false;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraMain = Camera.main.transform;
    }
    private void Start()
    {
        GetComponent<NetworkTransform>().AuthorityMode=NetworkTransform.AuthorityModes.Owner;

        if (IsOwner)
            OnplayerPosition?.Invoke(transform);
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal")*speed;
        vertical = Input.GetAxis("Vertical")* speed;
        isJump = Input.GetKeyDown(KeyCode.Space);
        direction = GetMoveDirection();
        Jump();

    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
    }
    private void Jump()
    {
        if(isJump&&CheckGround())
        {
            rb.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
        }
    }
    private Vector3 GetMoveDirection()
    {
        camForward = cameraMain.transform.forward.normalized;
        camRight = cameraMain.transform.right.normalized;
        return camRight * horizontal + camForward * vertical;
    }
    private bool CheckGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, distance, layer);
    }
    
}
