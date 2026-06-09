using UnityEngine;
using System;

public sealed class GameManager : MonoBehaviour
{
    private SaveManager saveManager;
    private OfflineProgressSystem offlineProgressSystem;
    private EvolutionSystem evolutionSystem;
    private DexManager dexManager;
    private GardenManager gardenManager;
    private UIManager uiManager;
    private AdManager adManager;

    public PuniController Puni { get; private set; }
    public string LastMessage { get; private set; } = "Welcome to PUNI Life";
    public int LastOfflineHours { get; private set; }
    public OfflineProgressResult LastOfflineProgress { get; private set; }

    private void Awake()
    {
        saveManager = new SaveManager();
        offlineProgressSystem = new OfflineProgressSystem();
        evolutionSystem = new EvolutionSystem();
        dexManager = new DexManager();
        gardenManager = new GardenManager();
        adManager = GetComponent<AdManager>();
        if (adManager == null)
        {
            adManager = gameObject.AddComponent<AdManager>();
        }

        uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            uiManager = new GameObject("UIManager").AddComponent<UIManager>();
        }
        InitializeGame(false);
    }

    private void Start()
    {
        Save();
        uiManager.Bind(this);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void PerformCare(CareActionType action)
    {
        if (Puni.PerformCare(action, out string message))
        {
            LastMessage = message;
            Save();
        }
        else
        {
            LastMessage = message;
        }

        uiManager.Refresh();
    }

    public void AddMiniGameReward(int score, bool doubled)
    {
        Puni.Data.totalMiniGamePlays++;
        int coin = score * 2 * (doubled ? 2 : 1);
        Puni.Data.status.coin += coin;
        Puni.Data.status.happiness += 10;
        Puni.Data.status.AddExp(5);
        Puni.Data.status.Clamp();
        LastMessage = $"Snack Tap reward +{coin} coins";
        Save();
        uiManager.Refresh();
    }

    public void ShowRewardedAd(System.Action onReward)
    {
        adManager.ShowRewarded(RewardedAdPlacement.DoubleMiniGameCoins, onReward);
    }

    public void ShowRewardedAd(RewardedAdPlacement placement, System.Action onReward)
    {
        adManager.ShowRewarded(placement, onReward);
    }

    public bool IsRewardedAdReady()
    {
        return adManager != null && adManager.IsRewardedReady;
    }

    public void WatchAdForFreeSnack()
    {
        ShowRewardedAd(RewardedAdPlacement.FreeSnack, () =>
        {
            Puni.Data.status.hunger += 25;
            Puni.Data.status.affection += 3;
            Puni.Data.growthStats.kindness += 1;
            Puni.Data.status.Clamp();
            Puni.Data.growthStats.Clamp();
            LastMessage = "PUNI received a free snack";
            Save();
            uiManager.Refresh();
        });
    }

    public void WatchAdForRecovery()
    {
        ShowRewardedAd(RewardedAdPlacement.RecoverStatus, () =>
        {
            Puni.Data.status.hunger = Mathf.Max(Puni.Data.status.hunger, 60);
            Puni.Data.status.happiness = Mathf.Max(Puni.Data.status.happiness, 60);
            Puni.Data.status.cleanliness = Mathf.Max(Puni.Data.status.cleanliness, 60);
            Puni.Data.status.energy = Mathf.Max(Puni.Data.status.energy, 60);
            Puni.Data.growthStats.neglect = Mathf.Max(0, Puni.Data.growthStats.neglect - 5);
            Puni.Data.status.Clamp();
            Puni.Data.growthStats.Clamp();
            LastMessage = "PUNI recovered";
            Save();
            uiManager.Refresh();
        });
    }

    public void WatchAdForEvolutionHint()
    {
        ShowRewardedAd(RewardedAdPlacement.EvolutionHint, () =>
        {
            LastMessage = GetEvolutionHint();
            uiManager.Refresh();
        });
    }

    public int GetDexUnlockedCount()
    {
        return dexManager.CountUnlocked(Puni.Data);
    }

    public SaveData GetSaveData()
    {
        return Puni.Data;
    }

    public string GetGardenName()
    {
        return gardenManager.GetGardenName(Puni.Data.currentGardenLevel);
    }

    public string GetEvolutionHint()
    {
        return evolutionSystem.GetEvolutionHint(Puni.Data);
    }

    public void Save()
    {
        saveManager.Save(Puni.Data);
    }

    public string GetSavePath()
    {
        return saveManager.SavePath;
    }

    public void DebugResetSave()
    {
        InitializeGame(true);
        Save();
        uiManager.Refresh();
    }

    public void DebugAddExp(int amount)
    {
        Puni.Data.status.AddExp(amount);
        EvolutionUpdateResult result = Puni.RefreshProgress();
        LastMessage = result.evolved ? $"Debug evolved into {Puni.Data.evolutionType}" : $"Debug EXP +{amount}";
        Save();
        uiManager.Refresh();
    }

    public void DebugSimulateOfflineHours(int hours)
    {
        Puni.Data.lastSavedAt = DateTime.UtcNow.AddHours(-hours).ToString("O");
        LastOfflineProgress = offlineProgressSystem.Apply(Puni.Data);
        LastOfflineHours = LastOfflineProgress.hours;
        LastMessage = $"Debug offline {LastOfflineHours}h applied";
        Save();
        uiManager.Refresh();
    }

    public void DebugForceEvolution()
    {
        Puni.Data.stage = PuniStage.Young;
        Puni.Data.status.level = Constants.EvolutionLevel;
        Puni.Data.status.exp = 0;
        Puni.Data.status.energy = Constants.StatusMax;
        EvolutionUpdateResult result = Puni.RefreshProgress();
        LastMessage = result.evolved ? $"Debug evolved into {Puni.Data.evolutionType}" : "Debug evolution did not trigger";
        Save();
        uiManager.Refresh();
    }

    private void InitializeGame(bool resetSave)
    {
        if (resetSave)
        {
            saveManager.ResetSave();
        }

        SaveData data = resetSave ? SaveData.CreateNew() : saveManager.Load();
        LastOfflineProgress = offlineProgressSystem.Apply(data);
        LastOfflineHours = LastOfflineProgress.hours;
        Puni = new PuniController(data, new CareSystem(), evolutionSystem, dexManager, gardenManager);
        gardenManager.UpdateGardenLevel(data, dexManager);
        LastMessage = LastOfflineProgress.HasProgress
            ? $"{LastOfflineHours} offline hours passed. PUNI missed you."
            : "Welcome to PUNI Life";
    }
}
