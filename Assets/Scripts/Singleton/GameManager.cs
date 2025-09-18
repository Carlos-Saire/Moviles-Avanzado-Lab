using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public GameObject playerPrefab;
    public Dictionary<string, PlayerData> playerStatesByAccountID = new();

    public Action OnConnection;

    [SerializeField] private float radius = 10f;
    private void OnEnable()
    {
        PlayerController.OnDead += Set;
    }
    private void OnDisable()
    {
        PlayerController.OnDead -= Set;

    }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;

        OnConnection?.Invoke();
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
    }
    void Start()
    {

    }
    private PlayerData Set()
    {
        return new PlayerData("accountID", GetRandomPosition(), 100, 5);
    }
    private void HandleDisconnect(ulong clientID)
    {

        print("El jugador" + clientID + "Se a desconectado");
    }

    [Rpc(SendTo.Server)]
    public void RegisterPlayerServerRpc(string accountID, ulong ID)
    {
        if (!playerStatesByAccountID.TryGetValue(accountID, out PlayerData data))
        {
            PlayerData NewData = new PlayerData(accountID, GetRandomPosition(), 100, 5);
            playerStatesByAccountID[accountID] = NewData;
            SpawnPlayerServer(ID, NewData);
            print("Nueva id creada con el nombre de " + accountID);
        }
        else
        {
            print("Se encontro cuenta con el nombre de " + accountID);
            SpawnPlayerServer(ID, data);
        }
    }
    public Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        print(randomDirection);
        randomDirection += transform.position;
        print(randomDirection);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            Debug.Log("Entro");
            return hit.position;
        }

        Debug.LogWarning("No se encontró posición válida en el NavMesh.");
        return transform.position;
    }
    public void SpawnPlayerServer(ulong ID, PlayerData data)
    {
        if (!IsServer) return;
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(ID, true);
        player.GetComponent<PlayerController>().SetData(data);
    }


}