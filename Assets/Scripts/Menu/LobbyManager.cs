using UnityEngine;
using Unity.Netcode;
using System;
[RequireComponent(typeof(NetworkObject))]
public class LobbyManager : NetworkBehaviour
{
    [Header("Positon")]
    [SerializeField] private Vector3 positionInitial;
    [SerializeField] private float distance;

    public int currentClient => NetworkManager.Singleton.ConnectedClientsList.Count-1;
    private void OnEnable()
    {
        GameManager.OnPlayerPosition += GetPositionPlayer;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerPosition -= GetPositionPlayer;
    }
    private Vector3 GetPositionPlayer()
    {
        return  positionInitial + Vector3.right * currentClient * distance; 
    }
}
