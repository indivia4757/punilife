using System;

public sealed class DexManager
{
    public void Unlock(SaveData data, PuniEvolutionType type)
    {
        if (type == PuniEvolutionType.None)
        {
            return;
        }

        data.EnsureDefaults();
        foreach (PuniDexEntry entry in data.dexEntries)
        {
            if (entry.type != type)
            {
                continue;
            }

            if (!entry.unlocked)
            {
                entry.unlocked = true;
                entry.unlockedAt = DateTime.UtcNow.ToString("O");
            }

            return;
        }
    }

    public int CountUnlocked(SaveData data)
    {
        int count = 0;
        foreach (PuniDexEntry entry in data.dexEntries)
        {
            if (entry.unlocked)
            {
                count++;
            }
        }

        return count;
    }
}
