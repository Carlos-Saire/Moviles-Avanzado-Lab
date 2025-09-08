using UnityEngine;
using Unity.Netcode;
public class BulletController : NetworkBehaviour
{
    private Rigidbody rb;
    private NetworkObject thisObject;
    [SerializeField] private float speed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void DirectionBullet(Vector3 direction)
    {
        rb.linearVelocity = direction*speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        DestroyRpc();
    }

    [Rpc(SendTo.Server)]
    private void DestroyRpc()
    {
        thisObject.Despawn();
    }
}
