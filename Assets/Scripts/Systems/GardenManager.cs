public sealed class GardenManager
{
    public int UpdateGardenLevel(SaveData data, DexManager dexManager)
    {
        int unlocked = dexManager.CountUnlocked(data);
        data.currentGardenLevel = unlocked;
        return data.currentGardenLevel;
    }

    public string GetGardenName(int level)
    {
        return level switch
        {
            <= 0 => "Forgotten Garden",
            1 => "Flower Corner",
            2 => "Young Tree",
            3 => "Clear Pond",
            4 => "Starlight Garden",
            _ => "PUNI House"
        };
    }
}
