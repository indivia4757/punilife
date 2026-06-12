using UnityEngine;
using System;

public sealed class GameManager : MonoBehaviour
{
    private SaveManager saveManager;
    private OfflineProgressSystem offlineProgressSystem;
    private EvolutionSystem evolutionSystem;
    private DexManager dexManager;
    private GardenManager gardenManager;
    private UIManager uiManager;
    private AdManager adManager;
    private AudioManager audioManager;

    public PuniController Puni { get; private set; }
    public string LastMessage { get; private set; } = "푸니 라이프에 오신 것을 환영해요.";
    public string PuniSpeech { get; private set; } = "안녕... 나를 돌봐줄래?";
    public int LastOfflineHours { get; private set; }
    public OfflineProgressResult LastOfflineProgress { get; private set; }

    private void Awake()
    {
        saveManager = new SaveManager();
        offlineProgressSystem = new OfflineProgressSystem();
        evolutionSystem = new EvolutionSystem();
        dexManager = new DexManager();
        gardenManager = new GardenManager();
        audioManager = GetComponent<AudioManager>();
        if (audioManager == null)
        {
            audioManager = gameObject.AddComponent<AudioManager>();
        }

        adManager = GetComponent<AdManager>();
        if (adManager == null)
        {
            adManager = gameObject.AddComponent<AdManager>();
        }

        uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            uiManager = new GameObject("UIManager").AddComponent<UIManager>();
        }
        InitializeGame(false);
    }

    private void Start()
    {
        Save();
        uiManager.Bind(this);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void PerformCare(CareActionType action)
    {
        PuniStage previousStage = Puni.Data.stage;
        PuniEvolutionType previousEvolution = Puni.Data.evolutionType;
        if (Puni.PerformCare(action, out string message))
        {
            LastMessage = message;
            if (Puni.Data.stage == PuniStage.Evolved &&
                (previousStage != PuniStage.Evolved || previousEvolution != Puni.Data.evolutionType))
            {
                PuniSpeech = "두근두근... 나, 새로운 모습이 됐어!";
                audioManager.PlayEvolution();
            }
            else
            {
                PuniSpeech = GetCareSpeech(action);
                audioManager.PlayCareSuccess();
            }

            Save();
        }
        else
        {
            LastMessage = message;
            PuniSpeech = GetCareBlockedSpeech(action);
            audioManager.PlayError();
        }

        uiManager.Refresh();
    }

    public void RenamePuni(string rawName)
    {
        string name = string.IsNullOrWhiteSpace(rawName) ? "푸니" : rawName.Trim();
        if (name.Length > 10)
        {
            name = name.Substring(0, 10);
        }

        Puni.Data.puniName = name;
        LastMessage = $"{name}(으)로 이름을 정했어요.";
        PuniSpeech = $"{name}... 좋은 이름이야!";
        audioManager.PlayReward();
        Save();
        uiManager.Refresh();
    }

    public void TalkToPuni()
    {
        PuniSpeech = BuildConversationSpeech(Puni.Data);
        audioManager.PlayButtonClick();
        uiManager.Refresh();
    }

    public void RunWeeklyPlan()
    {
        CareActionType[] plan = BuildWeeklyPlan(Puni.Data);
        int appliedCount = 0;
        CareActionType lastApplied = CareActionType.Play;
        PuniStage previousStage = Puni.Data.stage;
        PuniEvolutionType previousEvolution = Puni.Data.evolutionType;

        for (int i = 0; i < plan.Length; i++)
        {
            if (Puni.PerformCare(plan[i], out _))
            {
                appliedCount++;
                lastApplied = plan[i];
            }
        }

        if (appliedCount == 0)
        {
            LastMessage = "이번 주 일정을 진행하지 못했어요. 컨디션과 코인을 확인해주세요.";
            PuniSpeech = "오늘은 조금 무리인 것 같아.";
            audioManager.PlayError();
            uiManager.Refresh();
            return;
        }

        if (Puni.Data.stage == PuniStage.Evolved &&
            (previousStage != PuniStage.Evolved || previousEvolution != Puni.Data.evolutionType))
        {
            LastMessage = $"이번 주 일정을 마치고 {PuniText.EvolutionName(Puni.Data.evolutionType)}로 진화했어요.";
            PuniSpeech = "이번 주가 나를 바꿨어!";
            audioManager.PlayEvolution();
        }
        else if (Puni.Data.stage != previousStage)
        {
            LastMessage = $"이번 주 일정을 마치고 {PuniText.StageName(Puni.Data.stage)}로 성장했어요.";
            PuniSpeech = "나 조금 자란 것 같지?";
            audioManager.PlayReward();
        }
        else
        {
            LastMessage = $"이번 주 일정 {appliedCount}개를 마쳤어요. 마지막 일정: {GetScheduleName(lastApplied)}";
            PuniSpeech = BuildWeeklySpeech(lastApplied);
            audioManager.PlayReward();
        }

        Save();
        uiManager.Refresh();
    }

    public void StartNewEgg()
    {
        SaveData data = Puni.Data;
        if (data.stage != PuniStage.Evolved)
        {
            LastMessage = "최종 진화 후 새 알을 받을 수 있어요.";
            PuniSpeech = "아직 조금 더 자라야 해.";
            audioManager.PlayError();
            uiManager.Refresh();
            return;
        }

        string previousName = data.puniName;
        int coin = data.status.coin;
        data.status = new PuniStatus();
        data.status.coin = coin;
        data.growthStats = new PuniGrowthStats();
        data.stage = PuniStage.Egg;
        data.evolutionType = PuniEvolutionType.None;
        data.puniName = "푸니";
        data.lastSavedAt = DateTime.UtcNow.ToString("O");
        data.EnsureDefaults();
        gardenManager.UpdateGardenLevel(data, dexManager);
        LastMessage = $"{previousName}의 기록을 도감에 남기고 새 알을 받았어요.";
        PuniSpeech = "새로운 만남이 시작됐어...";
        audioManager.PlayReward();
        Save();
        uiManager.Refresh();
    }

    public void AddMiniGameReward(int score, bool doubled)
    {
        Puni.Data.totalMiniGamePlays++;
        int coin = score * 2 * (doubled ? 2 : 1);
        Puni.Data.status.coin += coin;
        Puni.Data.status.happiness += 10;
        Puni.Data.status.AddExp(5);
        EvolutionUpdateResult result = Puni.RefreshProgress();
        Puni.Data.status.Clamp();
        LastMessage = result.evolved
            ? $"스낵 탭 보상으로 성장해서 {PuniText.EvolutionName(Puni.Data.evolutionType)}로 진화했어요."
            : $"스낵 탭 보상으로 코인 {coin}개를 받았어요.";
        if (result.evolved)
        {
            PuniSpeech = "간식 힘으로 반짝반짝 변했어!";
            audioManager.PlayEvolution();
        }
        else
        {
            PuniSpeech = "스낵 더 먹고 싶어!";
            audioManager.PlayReward();
        }

        Save();
        uiManager.Refresh();
    }

    public void ShowRewardedAd(System.Action onReward)
    {
        adManager.ShowRewarded(RewardedAdPlacement.DoubleMiniGameCoins, onReward);
    }

    public void ShowRewardedAd(RewardedAdPlacement placement, System.Action onReward)
    {
        adManager.ShowRewarded(placement, onReward);
    }

    public bool IsRewardedAdReady()
    {
        return adManager != null && adManager.IsRewardedReady;
    }

    public void WatchAdForFreeSnack()
    {
        ShowRewardedAd(RewardedAdPlacement.FreeSnack, () =>
        {
            Puni.Data.status.hunger += 25;
            Puni.Data.status.affection += 3;
            Puni.Data.growthStats.kindness += 1;
            Puni.Data.status.Clamp();
            Puni.Data.growthStats.Clamp();
            LastMessage = "푸니가 무료 간식을 받았어요.";
            PuniSpeech = "냠냠... 고마워!";
            audioManager.PlayReward();
            Save();
            uiManager.Refresh();
        });
    }

    public void WatchAdForRecovery()
    {
        ShowRewardedAd(RewardedAdPlacement.RecoverStatus, () =>
        {
            Puni.Data.status.hunger = Mathf.Max(Puni.Data.status.hunger, 60);
            Puni.Data.status.happiness = Mathf.Max(Puni.Data.status.happiness, 60);
            Puni.Data.status.cleanliness = Mathf.Max(Puni.Data.status.cleanliness, 60);
            Puni.Data.status.energy = Mathf.Max(Puni.Data.status.energy, 60);
            Puni.Data.growthStats.neglect = Mathf.Max(0, Puni.Data.growthStats.neglect - 5);
            Puni.Data.status.Clamp();
            Puni.Data.growthStats.Clamp();
            LastMessage = "푸니의 컨디션이 회복됐어요.";
            PuniSpeech = "몸이 한결 가벼워졌어.";
            audioManager.PlayReward();
            Save();
            uiManager.Refresh();
        });
    }

    public void WatchAdForEvolutionHint()
    {
        ShowRewardedAd(RewardedAdPlacement.EvolutionHint, () =>
        {
            LastMessage = GetEvolutionHint();
            uiManager.Refresh();
        });
    }

    public int GetDexUnlockedCount()
    {
        return dexManager.CountUnlocked(Puni.Data);
    }

    public SaveData GetSaveData()
    {
        return Puni.Data;
    }

    public string GetGardenName()
    {
        return gardenManager.GetGardenName(Puni.Data.currentGardenLevel);
    }

    public string GetEvolutionHint()
    {
        return evolutionSystem.GetEvolutionHint(Puni.Data);
    }

    public void Save()
    {
        if (saveManager == null || Puni == null || Puni.Data == null)
        {
            return;
        }

        saveManager.Save(Puni.Data);
    }

    public string GetSavePath()
    {
        return saveManager.SavePath;
    }

    public void DebugResetSave()
    {
        InitializeGame(true);
        Save();
        uiManager.Refresh();
    }

    public void DebugAddExp(int amount)
    {
        Puni.Data.status.AddExp(amount);
        EvolutionUpdateResult result = Puni.RefreshProgress();
        LastMessage = result.evolved ? $"디버그: {PuniText.EvolutionName(Puni.Data.evolutionType)} 진화" : $"디버그: 경험치 +{amount}";
        if (result.evolved)
        {
            PuniSpeech = "나, 변한 것 같아!";
            audioManager.PlayEvolution();
        }
        else
        {
            PuniSpeech = "조금 더 자란 느낌이야.";
        }

        Save();
        uiManager.Refresh();
    }

    public void DebugSimulateOfflineHours(int hours)
    {
        Puni.Data.lastSavedAt = DateTime.UtcNow.AddHours(-hours).ToString("O");
        LastOfflineProgress = offlineProgressSystem.Apply(Puni.Data);
        LastOfflineHours = LastOfflineProgress.hours;
        EvolutionUpdateResult result = Puni.RefreshProgress();
        LastMessage = result.evolved
            ? $"디버그: 오프라인 성장으로 {PuniText.EvolutionName(Puni.Data.evolutionType)} 진화"
            : $"디버그: 오프라인 {LastOfflineHours}시간 적용";
        if (result.evolved)
        {
            PuniSpeech = "기다리는 동안 나도 자랐어!";
            audioManager.PlayEvolution();
        }
        else
        {
            PuniSpeech = "혼자 기다렸어... 다시 와줘서 좋아.";
        }

        Save();
        uiManager.Refresh();
    }

    public void DebugForceEvolution()
    {
        Puni.Data.stage = PuniStage.Young;
        Puni.Data.status.level = Constants.EvolutionLevel;
        Puni.Data.status.exp = 0;
        Puni.Data.status.energy = Constants.StatusMax;
        EvolutionUpdateResult result = Puni.RefreshProgress();
        LastMessage = result.evolved ? $"디버그: {PuniText.EvolutionName(Puni.Data.evolutionType)} 진화" : "디버그: 진화 조건이 부족해요.";
        if (result.evolved)
        {
            PuniSpeech = "짜잔! 새로운 푸니야!";
            audioManager.PlayEvolution();
        }

        Save();
        uiManager.Refresh();
    }

    private void InitializeGame(bool resetSave)
    {
        if (resetSave)
        {
            saveManager.ResetSave();
        }

        SaveData data = resetSave ? SaveData.CreateNew() : saveManager.Load();
        LastOfflineProgress = offlineProgressSystem.Apply(data);
        LastOfflineHours = LastOfflineProgress.hours;
        Puni = new PuniController(data, new CareSystem(), evolutionSystem, dexManager, gardenManager);
        EvolutionUpdateResult evolutionResult = Puni.RefreshProgress();
        gardenManager.UpdateGardenLevel(data, dexManager);
        LastMessage = LastOfflineProgress.HasProgress
            ? BuildOfflineMessage(evolutionResult)
            : "푸니 라이프에 오신 것을 환영해요.";
        PuniSpeech = LastOfflineProgress.HasProgress
            ? BuildOfflineSpeech(evolutionResult)
            : BuildIdleSpeech(data);
    }

    private string BuildOfflineMessage(EvolutionUpdateResult evolutionResult)
    {
        if (evolutionResult.evolved)
        {
            return $"{LastOfflineHours}시간 동안 조금 성장해서 {PuniText.EvolutionName(Puni.Data.evolutionType)}로 진화했어요.";
        }

        if (evolutionResult.stageChanged)
        {
            return $"{LastOfflineHours}시간 동안 조금 성장해서 {PuniText.StageName(Puni.Data.stage)}가 됐어요.";
        }

        return $"{LastOfflineHours}시간 동안 조금 성장했어요. 경험치 +{LastOfflineProgress.expDelta}";
    }

    private string BuildOfflineSpeech(EvolutionUpdateResult evolutionResult)
    {
        if (evolutionResult.evolved)
        {
            return "기다리는 동안 이렇게 변했어. 어때?";
        }

        if (Puni.Data.status.isSick)
        {
            return "오래 기다렸더니 몸이 안 좋아...";
        }

        if (Puni.Data.status.isHungry)
        {
            return "배에서 꼬르륵 소리가 나...";
        }

        if (evolutionResult.stageChanged)
        {
            return "나 조금 커진 것 같아!";
        }

        return "다시 와줘서 기뻐.";
    }

    private static string BuildIdleSpeech(SaveData data)
    {
        if (data.status.isSick)
        {
            return "오늘은 힘이 별로 없어...";
        }

        if (data.status.isHungry)
        {
            return "간식 먹고 싶어.";
        }

        if (data.status.isDirty)
        {
            return "몸이 끈적끈적해...";
        }

        if (data.status.isSulking)
        {
            return "나랑 조금만 놀아줘.";
        }

        return data.stage switch
        {
            PuniStage.Egg => "톡톡... 안에서 네 목소리가 들려.",
            PuniStage.Baby => "오늘은 뭐 하고 놀까?",
            PuniStage.Young => "나 어떤 푸니가 될까?",
            PuniStage.Evolved => "내 기록을 도감에 남겨줘.",
            _ => "안녕!"
        };
    }

    private static CareActionType[] BuildWeeklyPlan(SaveData data)
    {
        if (data.status.isSick)
        {
            return new[] { CareActionType.Sleep, CareActionType.Feed, CareActionType.Play };
        }

        if (data.status.hunger < 55)
        {
            return new[] { CareActionType.Feed, CareActionType.Study, CareActionType.Sleep };
        }

        if (data.status.energy < 45)
        {
            return new[] { CareActionType.Sleep, CareActionType.Play, CareActionType.Feed };
        }

        if (data.stage == PuniStage.Egg)
        {
            return new[] { CareActionType.Feed, CareActionType.Play, CareActionType.Feed };
        }

        if (data.stage == PuniStage.Young)
        {
            return data.growthStats.intelligence <= data.growthStats.strength
                ? new[] { CareActionType.Study, CareActionType.Play, CareActionType.Sleep }
                : new[] { CareActionType.Train, CareActionType.Clean, CareActionType.Sleep };
        }

        return new[] { CareActionType.Play, CareActionType.Clean, CareActionType.Sleep };
    }

    private static string GetScheduleName(CareActionType action)
    {
        return action switch
        {
            CareActionType.Feed => "식사",
            CareActionType.Play => "자유시간",
            CareActionType.Clean => "생활관리",
            CareActionType.Sleep => "휴식",
            CareActionType.Study => "수업",
            CareActionType.Train => "훈련",
            _ => "일정"
        };
    }

    private static string BuildWeeklySpeech(CareActionType action)
    {
        return action switch
        {
            CareActionType.Feed => "든든하게 먹으니까 다음 일정도 할 수 있어.",
            CareActionType.Play => "자유시간이 있어서 버틸 수 있었어.",
            CareActionType.Clean => "생활이 정돈되니까 마음도 편해졌어.",
            CareActionType.Sleep => "푹 쉬니까 다시 해볼 수 있을 것 같아.",
            CareActionType.Study => "수업은 어렵지만 머리가 반짝이는 느낌이야.",
            CareActionType.Train => "훈련은 힘들지만 조금 강해졌어.",
            _ => "이번 주도 지나갔어."
        };
    }

    private static string GetCareSpeech(CareActionType action)
    {
        return action switch
        {
            CareActionType.Feed => "냠냠! 맛있어!",
            CareActionType.Play => "헤헤, 더 놀고 싶어!",
            CareActionType.Clean => "반짝반짝해졌어!",
            CareActionType.Sleep => "잠깐 꿈나라 다녀올게...",
            CareActionType.Study => "새로운 걸 배웠어!",
            CareActionType.Train => "나 조금 강해진 것 같아!",
            _ => "고마워!"
        };
    }

    private static string GetCareBlockedSpeech(CareActionType action)
    {
        return action switch
        {
            CareActionType.Feed => "지금은 간식을 못 먹겠어.",
            CareActionType.Play => "에너지가 조금 부족해...",
            CareActionType.Clean => "아직은 혼자 씻기 어려워.",
            CareActionType.Sleep => "아직 잠들 준비가 안 됐어.",
            CareActionType.Study => "조금 더 자라면 공부할래.",
            CareActionType.Train => "훈련은 아직 어려워.",
            _ => "지금은 안 될 것 같아."
        };
    }

    private static string BuildConversationSpeech(SaveData data)
    {
        if (data.status.isSick)
        {
            return Pick("조금 쉬고 싶어...", "옆에 있어주면 나아질 것 같아.", "회복하면 다시 놀자.");
        }

        if (data.status.isHungry)
        {
            return Pick("간식 냄새가 나는 것 같아.", "꼬르륵... 들렸어?", "먹으면 힘이 날 것 같아.");
        }

        if (data.status.isDirty)
        {
            return Pick("깨끗해지면 기분이 좋아질 것 같아.", "몸이 조금 찝찝해.", "씻고 나면 반짝일 수 있어.");
        }

        if (data.status.isSulking)
        {
            return Pick("나랑 이야기해줘서 좋아.", "혼자 있으면 심심해.", "조금만 더 같이 있어줘.");
        }

        return data.stage switch
        {
            PuniStage.Egg => Pick("톡톡... 여기 있어.", "밖은 어떤 곳이야?", "따뜻해서 좋아."),
            PuniStage.Baby => Pick("오늘도 같이 있어줄 거지?", "나 많이 컸어?", "작은 것도 전부 신기해."),
            PuniStage.Young => Pick("어떤 모습으로 자랄지 궁금해.", "공부도 훈련도 해보고 싶어.", "네가 고르는 길을 따라가볼래."),
            PuniStage.Evolved => Pick("이 모습 마음에 들어?", "내 기록을 도감에 남겨줘.", "다음 푸니도 만나보고 싶어."),
            _ => Pick("안녕!", "헤헤.", "오늘은 좋은 날이야.")
        };
    }

    private static string Pick(params string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return "...";
        }

        return lines[UnityEngine.Random.Range(0, lines.Length)];
    }
}
