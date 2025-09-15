using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }
}
