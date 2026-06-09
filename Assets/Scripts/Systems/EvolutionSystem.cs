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
            return $"{PuniText.EvolutionName(data.evolutionType)}를 발견했어요.";
        }

        if (data.growthStats.neglect >= 35)
        {
            return "방치가 길어져 그림자 진화에 가까워졌어요.";
        }

        if (data.growthStats.kindness >= Constants.ForestKindnessThreshold && data.status.cleanliness >= 60)
        {
            return "친절하고 깨끗해서 숲의 진화에 가까워졌어요.";
        }

        if (data.growthStats.intelligence >= Constants.ScholarIntelligenceThreshold)
        {
            return "공부를 많이 해서 학자 진화에 가까워졌어요.";
        }

        if (data.growthStats.strength >= Constants.BraveStrengthThreshold || data.growthStats.courage >= Constants.BraveCourageThreshold)
        {
            return "훈련을 많이 해서 용감 진화에 가까워졌어요.";
        }

        return "돌봄에 따라 푸니의 미래가 달라져요.";
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
