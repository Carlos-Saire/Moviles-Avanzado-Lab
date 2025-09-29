using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    private Dictionary<ulong, bool> playerReadyStates = new Dictionary<ulong, bool>();

    [Header("Player")]
    [SerializeField] private Transform playerPrefab;
    public static Func<Vector3> OnPlayerPosition;

    [SerializeField] private int numList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject == null)
        {
            CreatePlayerForClient(clientId);
        }
    }

    private void CreatePlayerForClient(ulong clientId)
    {
        Transform player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        if (!playerReadyStates.ContainsKey(clientId))
            playerReadyStates.Add(clientId, false);

        player.position = OnPlayerPosition?.Invoke() ?? Vector3.zero;
    }

    [Rpc(SendTo.Server)]
    public void SetPlayerReadyRpc(ulong clientId, bool isReady)
    {
        if (playerReadyStates.ContainsKey(clientId))
            playerReadyStates[clientId] = isReady;

        if (AreAllPlayersReady()&& numList == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

    private bool AreAllPlayersReady()
    {
        foreach (var kvp in playerReadyStates)
        {
            if (!kvp.Value) return false;
        }
        return true;
    }
}
