using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public int experience;
    public int experienceToNextLevel;
    public int skillPoints;

    public int strength;
    public int defense;
    public int agility;

    public PlayerData(string name)
    {
        playerName = string.IsNullOrEmpty(name) ? "Nuevo Jugador" : name;
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
        skillPoints = 0;

        strength = 1;
        defense = 1;
        agility = 1;
    }
}

public class UnityPlayerAuth : MonoBehaviour
{
    public event Action<PlayerInfo, string> OnSingedIn;
    public event Action<string> OnUpdateName;

    private PlayerInfo playerInfo;
    public PlayerData currentPlayerData;

    private const string PLAYER_DATA_KEY = "PlayerData";

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();
        PlayerAccountService.Instance.SignedIn += SignIn;
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
        };

        AuthenticationService.Instance.SignInFailed += (err) => Debug.LogError(err);
        AuthenticationService.Instance.SignedOut += () => Debug.Log("Player log out");
        AuthenticationService.Instance.Expired += () => Debug.Log("Session expired");
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    private async void SignIn()
    {
        try
        {
            await SignInWithUnityAuth();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async Task SignInWithUnityAuth()
    {
        string accessToken = PlayerAccountService.Instance.AccessToken;
        await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
        playerInfo = AuthenticationService.Instance.PlayerInfo;

        var name = await AuthenticationService.Instance.GetPlayerNameAsync();
        OnSingedIn?.Invoke(playerInfo, name);

        await LoadOrCreatePlayerDataAsync(name);
    }

    public async Task UpdateName(string newName)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
        var name = await AuthenticationService.Instance.GetPlayerNameAsync();

        if (currentPlayerData != null)
        {
            currentPlayerData.playerName = name;
            await SavePlayerDataAsync();
        }

        OnUpdateName?.Invoke(name);
    }

    [Button(ButtonSizes.Large), GUIColor(0.5f, 1f, 0.5f)]
    public async Task SavePlayerDataAsync()
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("No hay datos de jugador para guardar.");
            return;
        }

        string jsonData = JsonUtility.ToJson(currentPlayerData, true);

        var data = new Dictionary<string, object>
        {
            { PLAYER_DATA_KEY, jsonData }
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        Debug.Log("Datos del jugador guardados como JSON en Cloud Save.");
    }

    [Button(ButtonSizes.Large), GUIColor(0.8f, 0.9f, 1f)]
    public async Task LoadOrCreatePlayerDataAsync(string playerName)
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PLAYER_DATA_KEY });

            if (result != null && result.ContainsKey(PLAYER_DATA_KEY))
            {
                string jsonData = result[PLAYER_DATA_KEY].Value.GetAsString();
                currentPlayerData = JsonUtility.FromJson<PlayerData>(jsonData);
                Debug.Log(" Datos del jugador cargados desde JSON.");
            }
            else
            {
                Debug.Log("No se encontraron datos. Creando nuevos...");
                currentPlayerData = new PlayerData(playerName);
                await SavePlayerDataAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(" Error al cargar datos: " + ex.Message);
            currentPlayerData = new PlayerData(playerName);
        }
    }

    [Button(ButtonSizes.Medium), GUIColor(1f, 0.9f, 0.4f)]
    public void AddExperienceTest()
    {
        AddExperience(50);
    }

    public void AddExperience(int amount)
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("No hay datos cargados.");
            return;
        }

        currentPlayerData.experience += amount;

        if (currentPlayerData.experience >= currentPlayerData.experienceToNextLevel)
        {
            currentPlayerData.experience -= currentPlayerData.experienceToNextLevel;
            currentPlayerData.level++;
            currentPlayerData.skillPoints += 2;
            currentPlayerData.experienceToNextLevel = Mathf.RoundToInt(currentPlayerData.experienceToNextLevel * 1.2f);
            Debug.Log(" ¡Subiste de nivel!");
        }
    }

    [Button(ButtonSizes.Medium)]
    public void SpendSkillPoint(string stat)
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("No hay datos cargados.");
            return;
        }

        if (currentPlayerData.skillPoints <= 0)
        {
            Debug.LogWarning("No hay puntos disponibles.");
            return;
        }

        switch (stat.ToLower())
        {
            case "fuerza": currentPlayerData.strength++; break;
            case "defensa": currentPlayerData.defense++; break;
            case "agilidad": currentPlayerData.agility++; break;
            default: Debug.LogWarning("Estadística inválida."); return;
        }

        currentPlayerData.skillPoints--;
        Debug.Log($"Aumentaste {stat}. Puntos restantes: {currentPlayerData.skillPoints}");
    }
}
