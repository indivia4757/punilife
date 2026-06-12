using System;
using UnityEngine;

[Serializable]
public sealed class PuniStatus
{
    public int hunger = Constants.InitialHunger;
    public int happiness = Constants.InitialHappiness;
    public int cleanliness = Constants.InitialCleanliness;
    public int energy = Constants.InitialEnergy;
    public int affection = Constants.InitialAffection;
    public int stress;
    public int level = 1;
    public int exp;
    public int coin = Constants.InitialCoin;
    public bool isHungry;
    public bool isDirty;
    public bool isSulking;
    public bool isSick;

    public int NextExp => Mathf.Max(20, level * 20);

    public void Clamp()
    {
        hunger = Mathf.Clamp(hunger, Constants.StatusMin, Constants.StatusMax);
        happiness = Mathf.Clamp(happiness, Constants.StatusMin, Constants.StatusMax);
        cleanliness = Mathf.Clamp(cleanliness, Constants.StatusMin, Constants.StatusMax);
        energy = Mathf.Clamp(energy, Constants.StatusMin, Constants.StatusMax);
        affection = Mathf.Clamp(affection, Constants.StatusMin, Constants.StatusMax);
        stress = Mathf.Clamp(stress, Constants.StatusMin, Constants.StatusMax);
        coin = Mathf.Max(0, coin);
        level = Mathf.Clamp(level, 1, Constants.EvolutionLevel);
        exp = Mathf.Max(0, exp);
        if (level >= Constants.EvolutionLevel)
        {
            exp = Mathf.Min(exp, NextExp);
        }

        RefreshConditionFlags();
    }

    public void AddExp(int amount)
    {
        if (level >= Constants.EvolutionLevel)
        {
            exp = NextExp;
            Clamp();
            return;
        }

        exp += Mathf.Max(0, amount);
        while (exp >= NextExp && level < Constants.EvolutionLevel)
        {
            exp -= NextExp;
            level++;
        }

        if (level >= Constants.EvolutionLevel)
        {
            exp = NextExp;
        }

        Clamp();
    }

    public void RefreshConditionFlags()
    {
        isHungry = hunger <= Constants.LowStatusThreshold;
        isDirty = cleanliness <= Constants.LowStatusThreshold;
        isSulking = happiness <= Constants.LowStatusThreshold;

        int badStateCount = 0;
        if (isHungry)
        {
            badStateCount++;
        }

        if (isDirty)
        {
            badStateCount++;
        }

        if (isSulking)
        {
            badStateCount++;
        }

        isSick = badStateCount >= 2 || stress >= 90;
    }
}
