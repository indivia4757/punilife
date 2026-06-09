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
    private AudioManager audioManager;

    public PuniController Puni { get; private set; }
    public string LastMessage { get; private set; } = "푸니 라이프에 오신 것을 환영해요.";
    public int LastOfflineHours { get; private set; }
    public OfflineProgressResult LastOfflineProgress { get; private set; }

    private void Awake()
    {
        saveManager = new SaveManager();
        offlineProgressSystem = new OfflineProgressSystem();
        evolutionSystem = new EvolutionSystem();
        dexManager = new DexManager();
        gardenManager = new GardenManager();
        audioManager = GetComponent<AudioManager>();
        if (audioManager == null)
        {
            audioManager = gameObject.AddComponent<AudioManager>();
        }

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
        PuniStage previousStage = Puni.Data.stage;
        PuniEvolutionType previousEvolution = Puni.Data.evolutionType;
        if (Puni.PerformCare(action, out string message))
        {
            LastMessage = message;
            if (Puni.Data.stage == PuniStage.Evolved &&
                (previousStage != PuniStage.Evolved || previousEvolution != Puni.Data.evolutionType))
            {
                audioManager.PlayEvolution();
            }
            else
            {
                audioManager.PlayCareSuccess();
            }

            Save();
        }
        else
        {
            LastMessage = message;
            audioManager.PlayError();
        }

        uiManager.Refresh();
    }

    public void RenamePuni(string rawName)
    {
        string name = string.IsNullOrWhiteSpace(rawName) ? "푸니" : rawName.Trim();
        if (name.Length > 10)
        {
            name = name.Substring(0, 10);
        }

        Puni.Data.puniName = name;
        LastMessage = $"{name}(으)로 이름을 정했어요.";
        audioManager.PlayReward();
        Save();
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
        LastMessage = $"스낵 탭 보상으로 코인 {coin}개를 받았어요.";
        audioManager.PlayReward();
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
            LastMessage = "푸니가 무료 간식을 받았어요.";
            audioManager.PlayReward();
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
            LastMessage = "푸니의 컨디션이 회복됐어요.";
            audioManager.PlayReward();
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
        LastMessage = result.evolved ? $"디버그: {PuniText.EvolutionName(Puni.Data.evolutionType)} 진화" : $"디버그: 경험치 +{amount}";
        if (result.evolved)
        {
            audioManager.PlayEvolution();
        }

        Save();
        uiManager.Refresh();
    }

    public void DebugSimulateOfflineHours(int hours)
    {
        Puni.Data.lastSavedAt = DateTime.UtcNow.AddHours(-hours).ToString("O");
        LastOfflineProgress = offlineProgressSystem.Apply(Puni.Data);
        LastOfflineHours = LastOfflineProgress.hours;
        LastMessage = $"디버그: 오프라인 {LastOfflineHours}시간 적용";
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
        LastMessage = result.evolved ? $"디버그: {PuniText.EvolutionName(Puni.Data.evolutionType)} 진화" : "디버그: 진화 조건이 부족해요.";
        if (result.evolved)
        {
            audioManager.PlayEvolution();
        }

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
        EvolutionUpdateResult evolutionResult = Puni.RefreshProgress();
        gardenManager.UpdateGardenLevel(data, dexManager);
        LastMessage = LastOfflineProgress.HasProgress
            ? BuildOfflineMessage(evolutionResult)
            : "푸니 라이프에 오신 것을 환영해요.";
    }

    private string BuildOfflineMessage(EvolutionUpdateResult evolutionResult)
    {
        if (evolutionResult.evolved)
        {
            return $"{LastOfflineHours}시간 동안 조금 성장해서 {PuniText.EvolutionName(Puni.Data.evolutionType)}로 진화했어요.";
        }

        if (evolutionResult.stageChanged)
        {
            return $"{LastOfflineHours}시간 동안 조금 성장해서 {PuniText.StageName(Puni.Data.stage)}가 됐어요.";
        }

        return $"{LastOfflineHours}시간 동안 조금 성장했어요. 경험치 +{LastOfflineProgress.expDelta}";
    }
}
