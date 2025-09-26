using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private Vector2 direction;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        InputReader.OnMovePlayer += SetDirection;
    }
    private void OnDisable()
    {
        InputReader.OnMovePlayer -= SetDirection;
    }
    private void Update()
    {
        if(!IsOwner) return;
        float h = Input.GetAxis("Vertical");
        MoveAnimationRpc(h);
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
