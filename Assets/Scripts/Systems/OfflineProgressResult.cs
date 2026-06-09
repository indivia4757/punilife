using System;

[Serializable]
public readonly struct OfflineProgressResult
{
    public static readonly OfflineProgressResult None = new OfflineProgressResult(0, 0, 0, 0, 0, 0, 0);

    public readonly int hours;
    public readonly int hungerDelta;
    public readonly int happinessDelta;
    public readonly int cleanlinessDelta;
    public readonly int energyDelta;
    public readonly int neglectDelta;
    public readonly int lowStatusCount;

    public bool HasProgress => hours > 0;

    public OfflineProgressResult(
        int hours,
        int hungerDelta,
        int happinessDelta,
        int cleanlinessDelta,
        int energyDelta,
        int neglectDelta,
        int lowStatusCount)
    {
        this.hours = hours;
        this.hungerDelta = hungerDelta;
        this.happinessDelta = happinessDelta;
        this.cleanlinessDelta = cleanlinessDelta;
        this.energyDelta = energyDelta;
        this.neglectDelta = neglectDelta;
        this.lowStatusCount = lowStatusCount;
    }
}
