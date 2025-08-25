using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }
    [Rpc(SendTo.Server)]
    public void InstancePlayerRpc()
    {

    }
}
