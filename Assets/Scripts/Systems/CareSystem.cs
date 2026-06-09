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
            message = "This action unlocks when PUNI grows up";
            return false;
        }

        if (effect.coin < 0 && status.coin < -effect.coin)
        {
            message = "Not enough coins";
            return false;
        }

        if (effect.energy < 0 && status.energy < -effect.energy)
        {
            message = "PUNI is tired";
            return false;
        }

        status.hunger += effect.hunger;
        status.happiness += effect.happiness;
        status.cleanliness += effect.cleanliness;
        status.energy += effect.energy;
        status.affection += effect.affection;
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
                coin: -10,
                kindness: 1,
                successMessage: "PUNI enjoyed the snack"),
            CareActionType.Play => new CareActionEffect(
                happiness: 20,
                energy: -10,
                affection: 5,
                exp: 5,
                sensitivity: 3,
                kindness: 2,
                successMessage: "PUNI had fun"),
            CareActionType.Clean => new CareActionEffect(
                cleanliness: 30,
                happiness: 5,
                exp: 3,
                kindness: 4,
                successMessage: "PUNI is clean"),
            CareActionType.Sleep => new CareActionEffect(
                hunger: -5,
                energy: 40,
                sensitivity: 1,
                successMessage: "PUNI rested"),
            CareActionType.Study => new CareActionEffect(
                happiness: -3,
                energy: -10,
                exp: 8,
                intelligence: 5,
                sensitivity: 1,
                successMessage: "PUNI studied"),
            CareActionType.Train => new CareActionEffect(
                energy: -15,
                exp: 8,
                strength: 5,
                courage: 3,
                successMessage: "PUNI trained"),
            _ => new CareActionEffect(successMessage: "PUNI feels normal")
        };
    }

    public bool IsUnlocked(CareActionType action, PuniStage stage)
    {
        if (stage == PuniStage.Evolved)
        {
            return action == CareActionType.Feed ||
                   action == CareActionType.Play ||
                   action == CareActionType.Clean ||
                   action == CareActionType.Sleep;
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
                   action == CareActionType.Sleep;
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
