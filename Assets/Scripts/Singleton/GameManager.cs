using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    private GameManager instance;

    [Header("Prefab Player")]
    [SerializeField] private Transform prefabPlayer;
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
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }
    public GameManager Instance => instance;
}
