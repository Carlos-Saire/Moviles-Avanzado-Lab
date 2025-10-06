using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Transform loginPanel;
    [SerializeField] private Transform userPanel;

    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text playerIDTxt;
    [SerializeField] private TMP_Text playerNameTxt;

    [SerializeField] private TMP_InputField UpdateNameIF;
    [SerializeField] private Button updateNameBtn;

    [SerializeField] private UnityPlayerAuth unityPlayerAuth;

    [Header("Player Stats UI")]
    [SerializeField] private TMP_Text levelTxt;
    [SerializeField] private TMP_Text expTxt;
    [SerializeField] private TMP_Text skillPointsTxt;
    [SerializeField] private TMP_Text statsTxt;
    [SerializeField] private Button gainExpButton;
    [SerializeField] private Button addStrengthButton;
    [SerializeField] private Button addDefenseButton;
    [SerializeField] private Button addAgilityButton;

    void Start()
    {
        loginPanel.gameObject.SetActive(true);
        userPanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        loginButton?.onClick.AddListener(LoginButton);
        unityPlayerAuth.OnSingedIn += UnityPlayerOnSignedIn;

        updateNameBtn.onClick.AddListener(UpdateName);
        unityPlayerAuth.OnUpdateName += UpdateNameVisual;

        if (gainExpButton != null) gainExpButton.onClick.AddListener(GainExperience);
        if (addStrengthButton != null) addStrengthButton.onClick.AddListener(() => AddStat("fuerza"));
        if (addDefenseButton != null) addDefenseButton.onClick.AddListener(() => AddStat("defensa"));
        if (addAgilityButton != null) addAgilityButton.onClick.AddListener(() => AddStat("agilidad"));
    }

    private async void UpdateName()
    {
        await unityPlayerAuth.UpdateName(UpdateNameIF.text);
    }

    private void UpdateNameVisual(string newName)
    {
        playerNameTxt.text = newName;
    }

    private void UnityPlayerOnSignedIn(PlayerInfo playerInfo, string PlayerName)
    {
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);

        playerIDTxt.text = "ID: " + playerInfo.Id;
        playerNameTxt.text = PlayerName;

        Invoke(nameof(UpdateUI), 1f);
    }

    private async void LoginButton()
    {
        await unityPlayerAuth.InitSignIn();
    }

    private void OnDisable()
    {
        loginButton?.onClick.RemoveListener(LoginButton);
        unityPlayerAuth.OnSingedIn -= UnityPlayerOnSignedIn;

        if (gainExpButton != null) gainExpButton.onClick.RemoveListener(GainExperience);
        if (addStrengthButton != null) addStrengthButton.onClick.RemoveAllListeners();
        if (addDefenseButton != null) addDefenseButton.onClick.RemoveAllListeners();
        if (addAgilityButton != null) addAgilityButton.onClick.RemoveAllListeners();
    }
    private async void GainExperience()
    {
        unityPlayerAuth.AddExperience(25);
        await unityPlayerAuth.SavePlayerDataAsync();
        UpdateUI();
    }

    private async void AddStat(string stat)
    {
        unityPlayerAuth.SpendSkillPoint(stat);
        await unityPlayerAuth.SavePlayerDataAsync();
        UpdateUI();
    }

    private void UpdateUI()
    {
        var data = unityPlayerAuth.currentPlayerData;
        if (data == null) return;

        if (levelTxt != null) levelTxt.text = "Nivel: " + data.level;
        if (expTxt != null) expTxt.text = $"Exp: {data.experience}/{data.experienceToNextLevel}";
        if (skillPointsTxt != null) skillPointsTxt.text = "Puntos: " + data.skillPoints;
        if (statsTxt != null)
            statsTxt.text = $"Fuerza: {data.strength}\nDefensa: {data.defense}\nAgilidad: {data.agility}";
    }
}
