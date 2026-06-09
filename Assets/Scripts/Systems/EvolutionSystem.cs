public sealed class EvolutionSystem
{
    public EvolutionUpdateResult UpdateStageAndEvolution(SaveData data, DexManager dexManager)
    {
        PuniStage previousStage = data.stage;

        if (data.stage == PuniStage.Egg)
        {
            data.stage = PuniStage.Baby;
        }

        if (data.status.level >= Constants.YoungUnlockLevel && data.stage == PuniStage.Baby)
        {
            data.stage = PuniStage.Young;
        }

        if (!CanEvolve(data) || data.stage == PuniStage.Evolved)
        {
            return new EvolutionUpdateResult(
                previousStage != data.stage,
                false,
                previousStage,
                data.stage,
                data.evolutionType);
        }

        data.evolutionType = DecideEvolution(data.growthStats, data.status);
        data.stage = PuniStage.Evolved;
        dexManager.Unlock(data, data.evolutionType);
        return new EvolutionUpdateResult(
            previousStage != data.stage,
            true,
            previousStage,
            data.stage,
            data.evolutionType);
    }

    public bool CanEvolve(SaveData data)
    {
        if (data.status.level >= Constants.EvolutionLevel)
        {
            return true;
        }

        return GetPlayedDays(data) >= Constants.EvolutionPlayDays && data.stage == PuniStage.Young;
    }

    public PuniEvolutionType DecideEvolution(PuniGrowthStats stats, PuniStatus status)
    {
        if (stats.neglect >= Constants.ShadowNeglectThreshold ||
            (status.happiness <= Constants.LowStatusThreshold && stats.neglect >= 35))
        {
            return PuniEvolutionType.Shadow;
        }

        if (stats.kindness >= Constants.ForestKindnessThreshold &&
            status.cleanliness >= 60 &&
            stats.kindness >= stats.intelligence &&
            stats.kindness >= stats.strength)
        {
            return PuniEvolutionType.Forest;
        }

        if (stats.intelligence >= Constants.ScholarIntelligenceThreshold &&
            stats.intelligence >= stats.strength &&
            stats.intelligence >= stats.courage)
        {
            return PuniEvolutionType.Scholar;
        }

        if (stats.strength >= Constants.BraveStrengthThreshold || stats.courage >= Constants.BraveCourageThreshold)
        {
            return PuniEvolutionType.Brave;
        }

        if (status.happiness >= Constants.SunnyHappinessThreshold && status.affection >= Constants.SunnyAffectionThreshold)
        {
            return PuniEvolutionType.Sunny;
        }

        return PuniEvolutionType.Sunny;
    }

    public string GetEvolutionHint(SaveData data)
    {
        if (data.stage == PuniStage.Evolved)
        {
            return $"{data.evolutionType} PUNI discovered";
        }

        if (data.growthStats.neglect >= 35)
        {
            return "Shadow path is close";
        }

        if (data.growthStats.kindness >= Constants.ForestKindnessThreshold && data.status.cleanliness >= 60)
        {
            return "Forest path is close";
        }

        if (data.growthStats.intelligence >= Constants.ScholarIntelligenceThreshold)
        {
            return "Scholar path is close";
        }

        if (data.growthStats.strength >= Constants.BraveStrengthThreshold || data.growthStats.courage >= Constants.BraveCourageThreshold)
        {
            return "Brave path is close";
        }

        return "Care for PUNI to shape its future";
    }

    private static int GetPlayedDays(SaveData data)
    {
        if (!System.DateTime.TryParse(data.firstPlayedAt, out System.DateTime firstPlayedAt))
        {
            return 0;
        }

        double days = (System.DateTime.UtcNow - firstPlayedAt.ToUniversalTime()).TotalDays;
        return UnityEngine.Mathf.FloorToInt((float)days);
    }
}
