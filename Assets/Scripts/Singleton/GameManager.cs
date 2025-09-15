using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField] private Transform playerPregab;
    
    [SerializeField]
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        GeneratePlayerRpc();
    }

    [Rpc(SendTo.Server)]
    private void GeneratePlayerRpc()
    {
        Transform player = Instantiate(playerPregab);
        player.GetComponent<NetworkObject>().Spawn();
    }
}


