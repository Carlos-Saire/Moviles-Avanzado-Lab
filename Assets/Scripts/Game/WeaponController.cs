using UnityEngine;
using Unity.Netcode;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private Transform bullet;
    private Transform _cam;
    private void Start()
    {
        if (!IsOwner) return;

        _cam = Camera.main.transform;
    }
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
        go.GetComponent<BulletController>().DirectionBullet(_cam.forward);
    }
}
