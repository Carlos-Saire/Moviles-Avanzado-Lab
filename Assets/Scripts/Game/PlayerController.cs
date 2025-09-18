using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Collections;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    public static event Action<Transform> OnplayerPosition;
    public static event Func<PlayerData> OnDead;

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

    public NetworkVariable<FixedString32Bytes> accountID = new();
    public NetworkVariable<int> health = new();
    public NetworkVariable<int> attack = new();

    public bool isDead => health.Value <= 0;
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        GetComponent<NetworkRigidbody>().AutoUpdateKinematicState = false;
    }
    private void OnEnable()
    {

        InputReader.OnJump+= JumpRpc;
     
        
    }
    private void OnDisable()
    {

        InputReader.OnJump-= JumpRpc;
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
        JumpAnimatorRpc();

    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;

        MoveRpc(direction);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
    }
    public override void OnNetworkDespawn()
    {
        GameManager.Instance.playerStatesByAccountID[accountID.Value.ToString()] = new PlayerData(accountID.ToString(), transform.position, health.Value, attack.Value);
        print("Jugador se ha desconectado: " + NetworkManager.Singleton.LocalClientId + ", y se ha guardado la data de: " + accountID.Value);
    }
    public void Damage(int damage)
    {
        health.Value -= damage;

        if (isDead)
        {
            PlayerData playerData = OnDead?.Invoke();
            health.Value = playerData.health;
            attack.Value = playerData.attack;
            transform.position = playerData.position;
        }
    }
    private Vector3 GetMoveDirection()
    {
        camForward = cameraMain.transform.forward.normalized;
        camRight = cameraMain.transform.right.normalized;
        return camRight * horizontal + camForward * vertical;
    }
    public void SetData(PlayerData playerData)
    {
        accountID.Value = playerData.accountID;
        health.Value = playerData.health;
        attack.Value = playerData.attack;
        transform.position = playerData.position;
    }

    [Rpc(SendTo.Server)]
    private void JumpRpc(bool isJump)
    {
        if (!IsOwner) return;

        if (isJump && Physics.Raycast(transform.position, Vector3.down, distance, layer))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    [Rpc(SendTo.Server)]
    private void JumpAnimatorRpc()
    {
        if (Physics.Raycast(transform.position, Vector3.down, distance, layer))
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

        rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
    }
}
public class PlayerData
{
    public string accountID;
    public Vector3 position;
    public int health;
    public int attack;

    public PlayerData(string id, Vector3 pos, int hp, int atk)
    {
        accountID = id;
        position = pos;
        health = hp;
        attack = atk;
    }
}
