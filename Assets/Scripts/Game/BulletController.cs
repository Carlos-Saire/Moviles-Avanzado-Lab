using UnityEngine;
using Unity.Netcode;
public class BulletController : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Invoke("DestroyRpc", 5);
    }
    public void DirectionBullet(Vector3 direction)
    {
        rb.linearVelocity = direction*speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Damage(1);
            DestroyRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void DestroyRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
