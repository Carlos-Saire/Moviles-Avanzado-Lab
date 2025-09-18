using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class UiManager : NetworkBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;

    public GameObject loginPanel;
    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitName);
        loginPanel.SetActive(true);

        GameManager.Instance.OnConnection += () =>
        {
            loginPanel.SetActive(true);
            inputField.text = "";
            submitButton.interactable = true;
            inputField.interactable = true;
        };
    }
    public void OnSubmitName()
    {
        string accountID = inputField.text;
        if (!string.IsNullOrEmpty(accountID))
        {
            GameManager.Instance.RegisterPlayerServerRpc(accountID, NetworkManager.Singleton.LocalClientId);
            submitButton.interactable = false;
            inputField.interactable = false;

            loginPanel.SetActive(false);
        }
    }
}
