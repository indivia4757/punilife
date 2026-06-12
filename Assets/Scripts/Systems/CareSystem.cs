using UnityEngine;

public sealed class CareSystem
{
    public bool Apply(CareActionType action, SaveData data, out string message)
    {
        PuniStatus status = data.status;
        PuniGrowthStats growth = data.growthStats;
        message = string.Empty;
        CareActionEffect effect = GetEffect(action);

        if (!IsUnlocked(action, data.stage))
        {
            message = "푸니가 더 성장하면 사용할 수 있어요.";
            return false;
        }

        if (effect.coin < 0 && status.coin < -effect.coin)
        {
            message = "코인이 부족해요.";
            return false;
        }

        if (effect.energy < 0 && status.energy < -effect.energy)
        {
            message = "푸니가 피곤해요.";
            return false;
        }

        if (status.stress >= 85 && action != CareActionType.Sleep && action != CareActionType.Play)
        {
            message = "피로가 너무 쌓여서 쉬어야 해요.";
            return false;
        }

        status.hunger += effect.hunger;
        status.happiness += effect.happiness;
        status.cleanliness += effect.cleanliness;
        status.energy += effect.energy;
        status.affection += effect.affection;
        status.stress += effect.stress;
        status.coin += effect.coin;
        status.AddExp(effect.exp);

        growth.intelligence += effect.intelligence;
        growth.strength += effect.strength;
        growth.sensitivity += effect.sensitivity;
        growth.courage += effect.courage;
        growth.kindness += effect.kindness;
        message = effect.successMessage;

        ReduceNeglectAfterCare(data);
        data.totalCareActions++;
        status.Clamp();
        growth.Clamp();
        return true;
    }

    public CareActionEffect GetEffect(CareActionType action)
    {
        return action switch
        {
            CareActionType.Feed => new CareActionEffect(
                hunger: 25,
                affection: 3,
                stress: -2,
                coin: -10,
                kindness: 1,
                successMessage: "푸니가 간식을 맛있게 먹었어요."),
            CareActionType.Play => new CareActionEffect(
                happiness: 20,
                energy: -10,
                affection: 5,
                stress: -8,
                exp: 5,
                sensitivity: 3,
                kindness: 2,
                successMessage: "푸니가 신나게 놀았어요."),
            CareActionType.Clean => new CareActionEffect(
                cleanliness: 30,
                happiness: 5,
                stress: -4,
                exp: 3,
                kindness: 4,
                successMessage: "푸니가 깨끗해졌어요."),
            CareActionType.Sleep => new CareActionEffect(
                hunger: -5,
                energy: 40,
                stress: -30,
                sensitivity: 1,
                successMessage: "푸니가 푹 쉬었어요."),
            CareActionType.Study => new CareActionEffect(
                happiness: -3,
                energy: -10,
                stress: 10,
                coin: -12,
                exp: 8,
                intelligence: 5,
                sensitivity: 1,
                successMessage: "푸니가 수업을 들었어요."),
            CareActionType.Train => new CareActionEffect(
                energy: -15,
                happiness: -3,
                stress: 14,
                coin: -8,
                exp: 8,
                strength: 5,
                courage: 3,
                successMessage: "푸니가 씩씩하게 훈련했어요."),
            CareActionType.Work => new CareActionEffect(
                hunger: -4,
                happiness: -6,
                cleanliness: -4,
                energy: -18,
                stress: 18,
                coin: 28,
                exp: 4,
                courage: 2,
                kindness: 1,
                successMessage: "푸니가 알바를 마치고 코인을 벌었어요."),
            _ => new CareActionEffect(successMessage: "푸니가 평온해요.")
        };
    }

    public bool IsUnlocked(CareActionType action, PuniStage stage)
    {
        if (stage == PuniStage.Evolved)
        {
            return action == CareActionType.Feed ||
                   action == CareActionType.Play ||
                   action == CareActionType.Clean ||
                   action == CareActionType.Sleep ||
                   action == CareActionType.Work;
        }

        if (stage == PuniStage.Egg)
        {
            return action == CareActionType.Feed || action == CareActionType.Play;
        }

        if (stage == PuniStage.Baby)
        {
            return action == CareActionType.Feed ||
                   action == CareActionType.Play ||
                   action == CareActionType.Clean ||
                   action == CareActionType.Sleep ||
                   action == CareActionType.Work;
        }

        return true;
    }

    private static void ReduceNeglectAfterCare(SaveData data)
    {
        if (data.status.hunger > 30 && data.status.happiness > 30 && data.status.cleanliness > 30)
        {
            data.growthStats.neglect = Mathf.Max(0, data.growthStats.neglect - 1);
        }
    }
}
