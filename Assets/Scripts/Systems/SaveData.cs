using System;
using System.Collections.Generic;

[Serializable]
public sealed class SaveData
{
    public int version = Constants.SaveDataVersion;
    public PuniStatus status = new PuniStatus();
    public PuniGrowthStats growthStats = new PuniGrowthStats();
    public PuniStage stage = PuniStage.Egg;
    public PuniEvolutionType evolutionType = PuniEvolutionType.None;
    public List<PuniDexEntry> dexEntries = new List<PuniDexEntry>();
    public int currentGardenLevel;
    public int currentRoomThemeId;
    public List<int> ownedItemIds = new List<int>();
    public bool tutorialCompleted;
    public int totalCareActions;
    public int totalMiniGamePlays;
    public string firstPlayedAt = string.Empty;
    public string lastSavedAt = string.Empty;

    public static SaveData CreateNew()
    {
        var data = new SaveData();
        data.EnsureDefaults();
        return data;
    }

    public void EnsureDefaults()
    {
        if (status == null)
        {
            status = new PuniStatus();
        }

        if (growthStats == null)
        {
            growthStats = new PuniGrowthStats();
        }

        if (dexEntries == null)
        {
            dexEntries = new List<PuniDexEntry>();
        }

        if (ownedItemIds == null)
        {
            ownedItemIds = new List<int>();
        }
        version = Constants.SaveDataVersion;

        EnsureDexEntry(PuniEvolutionType.Sunny);
        EnsureDexEntry(PuniEvolutionType.Scholar);
        EnsureDexEntry(PuniEvolutionType.Brave);
        EnsureDexEntry(PuniEvolutionType.Forest);
        EnsureDexEntry(PuniEvolutionType.Shadow);

        string now = DateTime.UtcNow.ToString("O");
        if (string.IsNullOrEmpty(firstPlayedAt))
        {
            firstPlayedAt = now;
        }

        if (string.IsNullOrEmpty(lastSavedAt))
        {
            lastSavedAt = now;
        }

        status.Clamp();
        growthStats.Clamp();
    }

    private void EnsureDexEntry(PuniEvolutionType type)
    {
        for (int i = 0; i < dexEntries.Count; i++)
        {
            if (dexEntries[i].type == type)
            {
                return;
            }
        }

        dexEntries.Add(new PuniDexEntry(type));
    }
}
