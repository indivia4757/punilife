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
            <= 0 => "잊힌 정원",
            1 => "꽃 피는 모퉁이",
            2 => "어린 나무 정원",
            3 => "맑은 연못",
            4 => "별빛 정원",
            _ => "푸니 하우스"
        };
    }
}
