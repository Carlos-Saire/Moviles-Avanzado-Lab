using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UiManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonCostomizer;

    [Header("Text")]
    private TMP_Text textReady;
    private bool isReady;

    private void Awake()
    {
        buttonCostomizer.onClick.AddListener(Costomizer);
        buttonReady.onClick.AddListener(Ready);
        textReady = buttonReady.GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        buttonCostomizer.onClick.RemoveListener(Costomizer);
        buttonReady.onClick.RemoveListener(Ready);
    }

    private void Ready()
    {
        isReady = !isReady;
        textReady.text = isReady ? "READY" : "NOT READY";

        if (NetworkManager.Singleton.IsClient)
        {
            GameManager.instance.SetPlayerReadyRpc(NetworkManager.Singleton.LocalClientId, isReady);
        }
    }
    private void Costomizer()
    {

    }
}
