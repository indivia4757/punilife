using System;
using UnityEngine;

public sealed class AdManager : MonoBehaviour
{
    public bool IsRewardedReady => true;
    public RewardedAdPlacement LastPlacement { get; private set; }

    public void ShowRewarded(RewardedAdPlacement placement, Action onReward)
    {
        LastPlacement = placement;
        onReward?.Invoke();
    }
}
