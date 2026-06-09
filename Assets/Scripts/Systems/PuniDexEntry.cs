using System;

[Serializable]
public sealed class PuniDexEntry
{
    public PuniEvolutionType type;
    public bool unlocked;
    public string unlockedAt;

    public PuniDexEntry()
    {
    }

    public PuniDexEntry(PuniEvolutionType type)
    {
        this.type = type;
        unlocked = false;
        unlockedAt = string.Empty;
    }
}
