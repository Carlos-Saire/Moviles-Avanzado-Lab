using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    private Dictionary<int,PlayerCustomizationData> a;
    [Header("Player")]
    [SerializeField] private Transform playerPrefab;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        print(NetworkManager.Singleton.LocalClientId);
        CreatePlayerRpc(NetworkManager.Singleton.LocalClientId);
    }
    [Rpc(SendTo.Server)]
    private void CreatePlayerRpc(ulong id)
    {
         Transform player = Instantiate(playerPrefab);
         player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
    }
}
