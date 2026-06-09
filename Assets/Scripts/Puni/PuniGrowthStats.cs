using System;
using UnityEngine;

[Serializable]
public sealed class PuniGrowthStats
{
    public int intelligence;
    public int strength;
    public int sensitivity;
    public int courage;
    public int kindness;
    public int neglect;

    public void Clamp()
    {
        intelligence = Mathf.Max(0, intelligence);
        strength = Mathf.Max(0, strength);
        sensitivity = Mathf.Max(0, sensitivity);
        courage = Mathf.Max(0, courage);
        kindness = Mathf.Max(0, kindness);
        neglect = Mathf.Max(0, neglect);
    }
}
