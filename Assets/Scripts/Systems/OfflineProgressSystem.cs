using System;
using UnityEngine;

public sealed class OfflineProgressSystem
{
    public OfflineProgressResult Apply(SaveData data)
    {
        if (!DateTime.TryParse(data.lastSavedAt, out DateTime lastSavedAt))
        {
            return OfflineProgressResult.None;
        }

        TimeSpan elapsed = DateTime.UtcNow - lastSavedAt.ToUniversalTime();
        int hours = Mathf.Clamp(Mathf.FloorToInt((float)elapsed.TotalHours), 0, Constants.MaxOfflineHours);
        if (hours <= 0)
        {
            return OfflineProgressResult.None;
        }

        int hungerDelta = -(hours * 5);
        int happinessDelta = -(hours * 3);
        int cleanlinessDelta = -(hours * 4);
        int energyDelta = hours * 5;
        int expDelta = Mathf.Clamp(Mathf.CeilToInt(hours * 0.5f), 1, Constants.MaxOfflineExp);

        data.status.hunger += hungerDelta;
        data.status.happiness += happinessDelta;
        data.status.cleanliness += cleanlinessDelta;
        data.status.energy += energyDelta;
        data.status.AddExp(expDelta);

        int lowStatusCount = 0;
        if (data.status.hunger <= 0)
        {
            lowStatusCount++;
        }

        if (data.status.happiness <= 0)
        {
            lowStatusCount++;
        }

        if (data.status.cleanliness <= 0)
        {
            lowStatusCount++;
        }

        int neglectDelta = Mathf.Max(1, hours / 3) + lowStatusCount * 2;
        data.growthStats.neglect += neglectDelta;
        data.status.Clamp();
        data.growthStats.Clamp();

        return new OfflineProgressResult(
            hours,
            hungerDelta,
            happinessDelta,
            cleanlinessDelta,
            energyDelta,
            expDelta,
            neglectDelta,
            lowStatusCount);
    }
}
