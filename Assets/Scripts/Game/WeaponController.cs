using UnityEngine;
using Unity.Netcode;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private Transform bullet;
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0))
        {
            InstanceBulletRpc();
        }
    }
    [Rpc(SendTo.Server)]
    private void InstanceBulletRpc()
    {
        Transform go = Instantiate(bullet, transform.position, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn(true);
        go.GetComponent<BulletController>().DirectionBullet(transform.forward);
    }
}
