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
    [SerializeField] private Transform targetCamera;
    public Rigidbody rb;
    private Animator animator;

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
        animator = GetComponent<Animator>();
        cameraMain = Camera.main.transform;
    }
    private void Start()
    {

        if (IsOwner )
            OnplayerPosition?.Invoke(targetCamera);
    }
    private void Update()
    {
        if (!IsOwner) return;

        horizontal = Input.GetAxis("Horizontal")*speed;
        vertical = Input.GetAxis("Vertical")* speed;
        isJump = Input.GetKeyDown(KeyCode.Space);
        direction = GetMoveDirection();
        JumpRpc();

    }
    private void FixedUpdate()
    {
        Debug.Log(direction);
        MoveRpc(direction);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
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
    [Rpc(SendTo.Server)]
    private void JumpRpc()
    {
        if (isJump && CheckGround())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        JumpAnimatorRpc();
    }
    [Rpc(SendTo.Server)]
    private void JumpAnimatorRpc()
    {
        if (CheckGround())
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("FreeFall", false);
        }
        else
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("FreeFall", true);
        }
    }
    [Rpc(SendTo.Server)]
    private void MoveRpc(Vector3 direction)
    {
        Debug.Log("RPC :" + direction);

        rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
    }
}
