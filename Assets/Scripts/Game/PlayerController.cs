using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerController : NetworkBehaviour
{
    public static event Action<Transform> OnplayerPosition;

    private Vector2 direction;
    private Animator animator;
    private void OnEnable()
    {
        InputReader.OnMovePlayer += SetDirection;
    }
    private void OnDisable()
    {
        InputReader.OnMovePlayer -= SetDirection;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (IsOwner)
            OnplayerPosition?.Invoke(transform);
    }
    private void Update()
    {
        if (!IsOwner) return;
        MoveAnimationRpc(direction.y);
    }
    private void SetDirection(Vector2 value)
    {
        direction = value;
    }
    [Rpc(SendTo.Server)]
    private void MoveAnimationRpc(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
}
