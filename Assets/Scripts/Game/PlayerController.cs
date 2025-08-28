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
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        GetComponent<NetworkTransform>().AuthorityMode=NetworkTransform.AuthorityModes.Owner;
        OnplayerPosition?.Invoke(transform);
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal")*speed;
        vertical = Input.GetAxis("Vertical")* speed;
        isJump = Input.GetKeyDown(KeyCode.Space);
        Jump();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(horizontal* Time.fixedDeltaTime, rb.linearVelocity.y,vertical * Time.fixedDeltaTime);
    }
    private void Jump()
    {
        if(isJump&&CheckGround())
        {
            rb.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
        }
    }
    private bool CheckGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, distance, layer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
    }
}
