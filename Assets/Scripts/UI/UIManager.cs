using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private bool built;
    private Text titleText;
    private Text coinText;
    private Text levelText;
    private Text nameText;
    private Text messageText;
    private Text speechText;
    private Text goalText;
    private Text moodText;
    private Text statsText;
    private Text scheduleText;
    private Text gardenText;
    private Text dexText;
    private StatusBarView hungerBar;
    private StatusBarView happinessBar;
    private StatusBarView cleanlinessBar;
    private StatusBarView energyBar;
    private StatusBarView affectionBar;
    private PuniView puniView;
    private DexGardenPanel dexGardenPanel;
    private GrowthGuidePanel growthGuidePanel;
    private NameEditPanel nameEditPanel;
    private DebugPanel debugPanel;
    private Button feedButton;
    private Button playButton;
    private Button cleanButton;
    private Button sleepButton;
    private Button studyButton;
    private Button trainButton;
    private Button weeklyPlanButton;
    private Button newEggButton;

    public Transform CanvasTransform { get; private set; }

    public void Bind(GameManager manager)
    {
        gameManager = manager;
        if (!built)
        {
            Build();
        }

        Refresh();
    }

    private void Update()
    {
        if (built && puniView != null)
        {
            puniView.Tick(Time.time, Time.deltaTime);
        }
    }

    public void Refresh()
    {
        SaveData data = gameManager.Puni.Data;
        titleText.text = "푸니 라이프";
        coinText.text = $"코인 {data.status.coin}";
        levelText.text = data.stage == PuniStage.Evolved
            ? $"Lv.{data.status.level}  진화 완료"
            : data.status.level >= Constants.EvolutionLevel
            ? $"Lv.{data.status.level}  진화 준비"
            : $"Lv.{data.status.level}  경험치 {data.status.exp}/{data.status.NextExp}";
        nameText.text = data.puniName;
        messageText.text = BuildMessage(data);
        speechText.text = $"\"{gameManager.PuniSpeech}\"";
        goalText.text = BuildDailyGoal(data);
        moodText.text = BuildMoodSummary(data);
        statsText.text = BuildStats(data);
        scheduleText.text = BuildSchedulePreview(data);
        gardenText.text = gameManager.GetGardenName();
        dexText.text = $"도감 {gameManager.GetDexUnlockedCount()}/5";
        hungerBar.SetValue(data.status.hunger);
        happinessBar.SetValue(data.status.happiness);
        cleanlinessBar.SetValue(data.status.cleanliness);
        energyBar.SetValue(data.status.energy);
        affectionBar.SetValue(data.status.affection);
        puniView.Refresh(data);
        dexGardenPanel.Refresh(data, gameManager.GetGardenName(), gameManager.GetDexUnlockedCount());
        RefreshButtons(data);
    }

    private string BuildMessage(SaveData data)
    {
        if (data.status.isSick)
        {
            return "몸이 좋지 않아요. 회복이나 잠으로 컨디션을 먼저 챙겨주세요.";
        }

        if (data.status.isHungry)
        {
            return "배가 고파요. 먹이를 주면 성장 경험치도 조금 올라요.";
        }

        if (data.status.isDirty)
        {
            return "몸이 끈적해요. 씻기면 기분과 청결이 회복돼요.";
        }

        if (data.status.isSulking)
        {
            return "조금 외로워요. 놀아주면 애착과 행복이 올라요.";
        }

        if (data.stage == PuniStage.Egg)
        {
            return "알 속에서 천천히 자라고 있어요. 먹이와 대화로 따뜻하게 돌봐주세요.";
        }

        if (data.status.level >= Constants.EvolutionLevel)
        {
            return gameManager.GetEvolutionHint();
        }

        return "컨디션이 좋아요. 어떤 행동을 자주 하느냐에 따라 진화 성향이 달라져요.";
    }

    private string BuildDailyGoal(SaveData data)
    {
        if (data.stage == PuniStage.Evolved)
        {
            return "다음 목표: 도감에 기록하고 새 알로 다른 성향을 키워보기";
        }

        if (data.status.isSick)
        {
            return "오늘 목표: 회복으로 컨디션을 60 이상까지 끌어올리기";
        }

        if (data.status.hunger < 70)
        {
            return "오늘 목표: 먹이를 줘서 배고픔 70 이상 만들기";
        }

        if (data.status.happiness < 70)
        {
            return "오늘 목표: 놀아주고 행복 70 이상 만들기";
        }

        if (data.status.cleanliness < 70 && data.stage != PuniStage.Egg)
        {
            return "오늘 목표: 씻겨서 청결 70 이상 만들기";
        }

        if (data.status.level >= Constants.EvolutionLevel)
        {
            return "오늘 목표: 성향을 확인하고 진화 결과를 기다리기";
        }

        return $"오늘 목표: 경험치 {data.status.NextExp - data.status.exp} 더 모아 다음 성장 단계 열기";
    }

    private string BuildMoodSummary(SaveData data)
    {
        if (data.status.isSick)
        {
            return "컨디션 나쁨";
        }

        if (data.status.isHungry)
        {
            return "배고픔";
        }

        if (data.status.isDirty)
        {
            return "씻고 싶음";
        }

        if (data.status.isSulking)
        {
            return "외로움";
        }

        if (data.status.energy <= Constants.LowStatusThreshold)
        {
            return "졸림";
        }

        return "기분 좋음";
    }

    private string BuildStats(SaveData data)
    {
        PuniGrowthStats stats = data.growthStats;
        return $"지능 {stats.intelligence}   체력 {stats.strength}   감성 {stats.sensitivity}\n용기 {stats.courage}   다정함 {stats.kindness}   방치 {stats.neglect}";
    }

    private string BuildSchedulePreview(SaveData data)
    {
        if (data.status.isSick)
        {
            return "이번 주 추천: 휴식 → 식사 → 자유시간";
        }

        if (data.status.hunger < 55)
        {
            return "이번 주 추천: 식사 → 수업 → 휴식";
        }

        if (data.status.energy < 45)
        {
            return "이번 주 추천: 휴식 → 자유시간 → 식사";
        }

        if (data.stage == PuniStage.Egg)
        {
            return "이번 주 추천: 식사 → 대화 → 식사";
        }

        if (data.stage == PuniStage.Young)
        {
            return data.growthStats.intelligence <= data.growthStats.strength
                ? "이번 주 추천: 수업 → 자유시간 → 휴식"
                : "이번 주 추천: 훈련 → 생활관리 → 휴식";
        }

        return "이번 주 추천: 자유시간 → 생활관리 → 휴식";
    }

    private void Build()
    {
        var canvasObject = new GameObject("Canvas");
        canvasObject.transform.SetParent(transform, false);
        CanvasTransform = canvasObject.transform;
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(720f, 1280f);
        scaler.matchWidthOrHeight = 0.7f;
        canvasObject.AddComponent<GraphicRaycaster>();

        CreateEnvironment(canvasObject.transform);

        CreateTopBand(canvasObject.transform);

        titleText = CreateText(canvasObject.transform, "Title", new Vector2(0f, -24f), new Vector2(420f, 42f), 31, TextAnchor.MiddleCenter);
        titleText.color = new Color(0.48f, 0.36f, 0.24f);
        coinText = CreateText(canvasObject.transform, "Coin", new Vector2(252f, -42f), new Vector2(205f, 34f), 20, TextAnchor.MiddleRight);
        levelText = CreateText(canvasObject.transform, "Level", new Vector2(-238f, -42f), new Vector2(250f, 34f), 20, TextAnchor.MiddleLeft);
        gardenText = CreateText(canvasObject.transform, "Garden", new Vector2(-215f, -84f), new Vector2(300f, 30f), 18, TextAnchor.MiddleLeft);
        dexText = CreateText(canvasObject.transform, "Dex", new Vector2(228f, -84f), new Vector2(220f, 30f), 18, TextAnchor.MiddleRight);

        CreateNeedCard(canvasObject.transform);
        moodText = CreateText(canvasObject.transform, "Mood", new Vector2(-232f, -137f), new Vector2(160f, 30f), 18, TextAnchor.MiddleLeft);
        messageText = CreateText(canvasObject.transform, "Message", new Vector2(52f, -136f), new Vector2(440f, 42f), 18, TextAnchor.MiddleLeft);

        nameText = CreateText(canvasObject.transform, "PuniName", new Vector2(0f, -216f), new Vector2(420f, 40f), 27, TextAnchor.MiddleCenter);
        speechText = CreateSpeechBubble(canvasObject.transform);

        puniView = new PuniView(canvasObject.transform);
        CreateSimulationBoard(canvasObject.transform);

        Vector2 statusSize = new Vector2(192f, 27f);
        hungerBar = new StatusBarView(canvasObject.transform, "배고픔", new Vector2(-244f, -104f), new Color(0.96f, 0.55f, 0.45f), statusSize, 14, 50f, 56f);
        happinessBar = new StatusBarView(canvasObject.transform, "행복", new Vector2(-40f, -104f), new Color(1f, 0.78f, 0.30f), statusSize, 14, 40f, 46f);
        cleanlinessBar = new StatusBarView(canvasObject.transform, "청결", new Vector2(164f, -104f), new Color(0.34f, 0.72f, 0.92f), statusSize, 14, 40f, 46f);
        energyBar = new StatusBarView(canvasObject.transform, "에너지", new Vector2(-142f, -362f), new Color(0.46f, 0.72f, 0.50f), new Vector2(192f, 27f), 14, 50f, 56f);
        affectionBar = new StatusBarView(canvasObject.transform, "애정", new Vector2(88f, -362f), new Color(0.92f, 0.48f, 0.76f), new Vector2(192f, 27f), 14, 40f, 46f);

        CreateActionDock(canvasObject.transform);
        CreateGoalCard(canvasObject.transform);

        Vector2 careButtonSize = new Vector2(156f, 64f);
        feedButton = CreateButton(canvasObject.transform, "식사", new Vector2(-248f, 164f), () => PerformCare(CareActionType.Feed), careButtonSize, 20, PuniTheme.CreamButton);
        playButton = CreateButton(canvasObject.transform, "자유시간", new Vector2(-83f, 164f), () => PerformCare(CareActionType.Play), careButtonSize, 18, PuniTheme.PeachButton);
        cleanButton = CreateButton(canvasObject.transform, "생활관리", new Vector2(83f, 164f), () => PerformCare(CareActionType.Clean), careButtonSize, 18, PuniTheme.SkyButton);
        sleepButton = CreateButton(canvasObject.transform, "휴식", new Vector2(248f, 164f), () => PerformCare(CareActionType.Sleep), careButtonSize, 20, PuniTheme.LilacButton);

        Vector2 growthButtonSize = new Vector2(154f, 50f);
        studyButton = CreateButton(canvasObject.transform, "수업", new Vector2(-248f, 90f), () => PerformCare(CareActionType.Study), growthButtonSize, 18, PuniTheme.MintButton);
        trainButton = CreateButton(canvasObject.transform, "훈련", new Vector2(-83f, 90f), () => PerformCare(CareActionType.Train), growthButtonSize, 18, PuniTheme.PeachButton);
        weeklyPlanButton = CreateButton(canvasObject.transform, "일주일 진행", new Vector2(83f, 90f), RunWeeklyPlan, new Vector2(154f, 50f), 16, PuniTheme.CreamButton);
        CreateButton(canvasObject.transform, "회복", new Vector2(248f, 90f), () => gameManager.WatchAdForRecovery(), growthButtonSize, 18, PuniTheme.MintButton);

        Vector2 menuButtonSize = new Vector2(120f, 42f);
        CreateButton(canvasObject.transform, "도감", new Vector2(-256f, 34f), ShowDexGarden, menuButtonSize, 16, PuniTheme.MintButton);
        CreateButton(canvasObject.transform, "가이드", new Vector2(-128f, 34f), ShowGrowthGuide, menuButtonSize, 16, PuniTheme.SkyButton);
        CreateButton(canvasObject.transform, "이름", new Vector2(0f, 34f), ShowNameEdit, menuButtonSize, 16, PuniTheme.SkyButton);
        CreateButton(canvasObject.transform, "알바", new Vector2(128f, 34f), StartMiniGame, menuButtonSize, 16, PuniTheme.CreamButton);
        newEggButton = CreateButton(canvasObject.transform, "새 알", new Vector2(256f, 34f), StartNewEgg, menuButtonSize, 16, PuniTheme.LilacButton);
        dexGardenPanel = new DexGardenPanel(canvasObject.transform);
        growthGuidePanel = new GrowthGuidePanel(canvasObject.transform);
        nameEditPanel = new NameEditPanel(canvasObject.transform);
        debugPanel = new DebugPanel(canvasObject.transform);
        built = true;
    }

    private static void CreateTopBand(Transform parent)
    {
        var band = CreatePanel(parent, "TopInfoBand", PuniTheme.SoftPanel);
        PuniTheme.ApplyRounded(band, PuniTheme.SoftPanel);
        band.raycastTarget = false;
        band.rectTransform.anchorMin = new Vector2(0f, 1f);
        band.rectTransform.anchorMax = new Vector2(1f, 1f);
        band.rectTransform.pivot = new Vector2(0.5f, 1f);
        band.rectTransform.anchoredPosition = Vector2.zero;
        band.rectTransform.sizeDelta = new Vector2(0f, 176f);
    }

    private static void CreateNeedCard(Transform parent)
    {
        var card = CreatePanel(parent, "NeedCard", new Color(1f, 0.96f, 0.84f, 0.88f));
        PuniTheme.ApplyRounded(card, new Color(1f, 0.96f, 0.84f, 0.88f));
        PuniTheme.CreateShadow(card.transform, "NeedCardShadow", new Vector2(0f, -5f));
        card.raycastTarget = false;
        card.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        card.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        card.rectTransform.pivot = new Vector2(0.5f, 1f);
        card.rectTransform.anchoredPosition = new Vector2(0f, -122f);
        card.rectTransform.sizeDelta = new Vector2(610f, 76f);

        var label = CreateText(parent, "NeedLabel", new Vector2(-232f, -124f), new Vector2(150f, 28f), 16, TextAnchor.MiddleLeft);
        label.text = "지금 푸니";
        label.color = new Color(0.56f, 0.38f, 0.20f);
    }

    private void CreateGoalCard(Transform parent)
    {
        var card = CreatePanel(parent, "GoalCard", new Color(1f, 0.98f, 0.90f, 0.92f));
        PuniTheme.ApplyRounded(card, new Color(1f, 0.98f, 0.90f, 0.92f));
        PuniTheme.CreateShadow(card.transform, "GoalCardShadow", new Vector2(0f, -4f));
        card.raycastTarget = false;
        card.rectTransform.anchorMin = new Vector2(0.5f, 0f);
        card.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        card.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        card.rectTransform.anchoredPosition = new Vector2(0f, 246f);
        card.rectTransform.sizeDelta = new Vector2(610f, 58f);

        goalText = CreateBottomText(parent, "DailyGoal", new Vector2(0f, 246f), new Vector2(560f, 44f), 18, TextAnchor.MiddleCenter);
    }

    private void CreateSimulationBoard(Transform parent)
    {
        var board = CreatePanel(parent, "SimulationBoard", new Color(1f, 0.98f, 0.92f, 0.82f));
        PuniTheme.ApplyRounded(board, new Color(1f, 0.98f, 0.92f, 0.82f));
        PuniTheme.CreateShadow(board.transform, "SimulationBoardShadow", new Vector2(0f, -5f));
        board.raycastTarget = false;
        board.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        board.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        board.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        board.rectTransform.anchoredPosition = new Vector2(0f, -258f);
        board.rectTransform.sizeDelta = new Vector2(610f, 104f);

        statsText = CreateCenterText(parent, "GrowthStats", new Vector2(-142f, -244f), new Vector2(300f, 66f), 17, TextAnchor.MiddleLeft);
        scheduleText = CreateCenterText(parent, "SchedulePreview", new Vector2(168f, -244f), new Vector2(260f, 66f), 17, TextAnchor.MiddleLeft);
        var label = CreateCenterText(parent, "BoardLabel", new Vector2(0f, -307f), new Vector2(530f, 24f), 15, TextAnchor.MiddleCenter);
        label.text = "일정을 고르면 성향이 쌓이고, 성장 후 진화 결과가 달라집니다.";
        label.color = new Color(0.48f, 0.36f, 0.24f);
    }

    private static void CreateEnvironment(Transform parent)
    {
        var background = CreatePanel(parent, "RoomBackground", new Color(0.96f, 0.98f, 0.93f));
        background.raycastTarget = false;
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.offsetMin = Vector2.zero;
        background.rectTransform.offsetMax = Vector2.zero;

        var backHill = CreateEllipse(parent, "BackGarden", new Vector2(0f, -130f), new Vector2(860f, 340f), PuniTheme.GardenBack);
        backHill.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        backHill.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        backHill.gameObject.AddComponent<PuniBackgroundMotion>().Initialize(0.35f, new Vector2(8f, 2f), 0f);

        var midHill = CreateEllipse(parent, "MiddleGarden", new Vector2(-130f, -205f), new Vector2(720f, 260f), PuniTheme.GardenMid);
        midHill.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        midHill.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        midHill.gameObject.AddComponent<PuniBackgroundMotion>().Initialize(0.45f, new Vector2(10f, 3f), 0f);

        var floor = CreateEllipse(parent, "SoftFloor", new Vector2(0f, -365f), new Vector2(780f, 250f), PuniTheme.Floor);
        floor.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        floor.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        CreateWindow(parent, new Vector2(242f, -350f));
        CreateRoomProp(parent, "BedProp", new Vector2(-250f, -252f), new Vector2(168f, 78f), PuniTheme.LilacButton, new Color(1f, 0.96f, 0.86f, 0.9f));
        CreateRoomProp(parent, "FoodBowlProp", new Vector2(-226f, -388f), new Vector2(120f, 42f), PuniTheme.PeachButton, PuniTheme.CreamButton);
        CreateRoomProp(parent, "WashProp", new Vector2(245f, -248f), new Vector2(126f, 72f), PuniTheme.SkyButton, new Color(0.82f, 0.94f, 0.98f, 0.9f));
        CreateRoomProp(parent, "BookProp", new Vector2(238f, -392f), new Vector2(112f, 48f), PuniTheme.MintButton, new Color(1f, 0.98f, 0.88f, 0.9f));
        CreateLeaf(parent, "LeafLeft", new Vector2(-292f, -372f), new Vector2(52f, 96f), new Color(0.56f, 0.78f, 0.55f), -28f, 0.8f);
        CreateLeaf(parent, "LeafRight", new Vector2(300f, -390f), new Vector2(48f, 88f), new Color(0.47f, 0.72f, 0.58f), 30f, 1.3f);
    }

    private static void CreateActionDock(Transform parent)
    {
        var dock = CreatePanel(parent, "ActionDock", new Color(1f, 0.96f, 0.86f, 0.66f));
        PuniTheme.ApplyRounded(dock, new Color(1f, 0.96f, 0.86f, 0.66f));
        dock.raycastTarget = false;
        dock.rectTransform.anchorMin = new Vector2(0f, 0f);
        dock.rectTransform.anchorMax = new Vector2(1f, 0f);
        dock.rectTransform.pivot = new Vector2(0.5f, 0f);
        dock.rectTransform.anchoredPosition = Vector2.zero;
        dock.rectTransform.sizeDelta = new Vector2(0f, 286f);
    }

    private static void CreateWindow(Transform parent, Vector2 position)
    {
        var window = CreatePanel(parent, "GardenWindow", new Color(0.77f, 0.89f, 0.95f, 0.44f));
        PuniTheme.ApplyRounded(window, new Color(0.77f, 0.89f, 0.95f, 0.44f));
        window.raycastTarget = false;
        window.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        window.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        window.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        window.rectTransform.anchoredPosition = position;
        window.rectTransform.sizeDelta = new Vector2(150f, 120f);
        window.gameObject.AddComponent<PuniBackgroundMotion>().Initialize(0.25f, new Vector2(4f, 2f), 0f);
    }

    private static Image CreateEllipse(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        var image = CreatePanel(parent, name, color);
        PuniTheme.ApplyCircle(image, color);
        image.raycastTarget = false;
        image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        image.rectTransform.anchoredPosition = position;
        image.rectTransform.sizeDelta = size;
        return image;
    }

    private static void CreateLeaf(Transform parent, string name, Vector2 position, Vector2 size, Color color, float rotation, float phase)
    {
        var leaf = CreateEllipse(parent, name, position, size, color);
        leaf.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
        leaf.gameObject.AddComponent<PuniBackgroundMotion>().Initialize(0.8f, new Vector2(4f, 3f), 5f, phase);
    }

    private static void CreateRoomProp(Transform parent, string name, Vector2 position, Vector2 size, Color baseColor, Color accentColor)
    {
        var prop = CreatePanel(parent, name, baseColor);
        PuniTheme.ApplyRounded(prop, new Color(baseColor.r, baseColor.g, baseColor.b, 0.62f));
        prop.raycastTarget = false;
        prop.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        prop.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        prop.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        prop.rectTransform.anchoredPosition = position;
        prop.rectTransform.sizeDelta = size;

        var accent = CreatePanel(prop.transform, $"{name}Accent", accentColor);
        PuniTheme.ApplyRounded(accent, accentColor);
        accent.raycastTarget = false;
        accent.rectTransform.anchorMin = new Vector2(0.12f, 0.18f);
        accent.rectTransform.anchorMax = new Vector2(0.88f, 0.78f);
        accent.rectTransform.offsetMin = Vector2.zero;
        accent.rectTransform.offsetMax = Vector2.zero;
    }

    private void RefreshButtons(SaveData data)
    {
        SetButtonState(feedButton, data.status.coin >= 10);
        SetButtonState(playButton, data.status.energy >= 10);
        SetButtonState(cleanButton, data.stage != PuniStage.Egg);
        SetButtonState(sleepButton, data.stage != PuniStage.Egg);
        SetButtonState(studyButton, data.stage == PuniStage.Young && data.status.energy >= 10);
        SetButtonState(trainButton, data.stage == PuniStage.Young && data.status.energy >= 15);
        SetButtonState(weeklyPlanButton, data.status.coin >= 10 || data.status.energy >= 20);
        SetButtonState(newEggButton, data.stage == PuniStage.Evolved);
    }

    private static void SetButtonState(Button button, bool interactable)
    {
        button.interactable = interactable;
        var visual = button.GetComponent<PuniButtonVisual>();
        if (visual != null)
        {
            visual.SetInteractable(interactable);
        }
    }

    private void StartMiniGame()
    {
        SnackTapGameManager miniGame = FindAnyObjectByType<SnackTapGameManager>();
        if (miniGame == null)
        {
            miniGame = new GameObject("SnackTapGame").AddComponent<SnackTapGameManager>();
        }

        miniGame.StartGame(gameManager, this);
    }

    private void PerformCare(CareActionType action)
    {
        puniView.PlayReaction(action);
        gameManager.PerformCare(action);
    }

    private void RunWeeklyPlan()
    {
        puniView.PlayReaction(CareActionType.Study);
        gameManager.RunWeeklyPlan();
    }

    private void StartNewEgg()
    {
        puniView.PlayCelebrate();
        gameManager.StartNewEgg();
    }

    private void ShowDexGarden()
    {
        dexGardenPanel.Show(gameManager.GetSaveData(), gameManager.GetGardenName(), gameManager.GetDexUnlockedCount());
    }

    private void ShowGrowthGuide()
    {
        growthGuidePanel.Show();
    }

    private void ShowNameEdit()
    {
        nameEditPanel.Show(gameManager.GetSaveData().puniName, gameManager.RenamePuni);
    }

    private void ShowDebug()
    {
        debugPanel.Show(gameManager);
    }

    private static Image CreatePanel(Transform parent, string name, Color color)
    {
        var panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        var image = panel.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.raycastTarget = false;
        text.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        text.rectTransform.pivot = new Vector2(0.5f, 1f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private static Text CreateSpeechBubble(Transform parent)
    {
        var bubble = CreatePanel(parent, "PuniSpeechBubble", PuniTheme.SoftPanel);
        PuniTheme.ApplyRounded(bubble, PuniTheme.SoftPanel);
        PuniTheme.CreateShadow(bubble.transform, "BubbleShadow", new Vector2(0f, -5f));
        bubble.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        bubble.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        bubble.rectTransform.pivot = new Vector2(0.5f, 1f);
        bubble.rectTransform.anchoredPosition = new Vector2(0f, -250f);
        bubble.rectTransform.sizeDelta = new Vector2(560f, 72f);
        var button = bubble.gameObject.AddComponent<Button>();
        button.onClick.AddListener(PlayButtonSound);
        button.onClick.AddListener(() =>
        {
            UIManager manager = Object.FindAnyObjectByType<UIManager>();
            if (manager != null)
            {
                manager.TalkToPuni();
            }
        });

        var textObject = new GameObject("Text");
        textObject.transform.SetParent(bubble.transform, false);
        var text = textObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.fontSize = 21;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.raycastTarget = false;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = new Vector2(18f, 8f);
        text.rectTransform.offsetMax = new Vector2(-18f, -8f);
        return text;
    }

    private static Text CreateBottomText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.raycastTarget = false;
        text.rectTransform.anchorMin = new Vector2(0.5f, 0f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private static Text CreateCenterText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.raycastTarget = false;
        text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private void TalkToPuni()
    {
        gameManager.TalkToPuni();
    }

    private static Button CreateButton(
        Transform parent,
        string label,
        Vector2 anchoredPosition,
        UnityEngine.Events.UnityAction onClick,
        Vector2? size = null,
        int fontSize = 20,
        Color? color = null)
    {
        var buttonObject = new GameObject($"{label}Button");
        buttonObject.transform.SetParent(parent, false);
        PuniTheme.CreateShadow(buttonObject.transform, "Shadow", new Vector2(0f, -5f));
        var image = buttonObject.AddComponent<Image>();
        PuniTheme.ApplyRounded(image, color ?? PuniTheme.CreamButton);
        var visual = buttonObject.AddComponent<PuniButtonVisual>();
        visual.Initialize(image, color ?? PuniTheme.CreamButton);

        var button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(PlayButtonSound);
        button.onClick.AddListener(visual.PlayPress);
        button.onClick.AddListener(onClick);

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        Vector2 buttonSize = size ?? new Vector2(164f, 54f);
        rect.sizeDelta = buttonSize;

        var shineObject = new GameObject("Highlight");
        shineObject.transform.SetParent(buttonObject.transform, false);
        var shine = shineObject.AddComponent<Image>();
        PuniTheme.ApplyRounded(shine, new Color(1f, 1f, 1f, 0.22f));
        shine.raycastTarget = false;
        shine.rectTransform.anchorMin = new Vector2(0.08f, 0.55f);
        shine.rectTransform.anchorMax = new Vector2(0.92f, 0.92f);
        shine.rectTransform.offsetMin = Vector2.zero;
        shine.rectTransform.offsetMax = Vector2.zero;

        var labelObject = new GameObject("Text");
        labelObject.transform.SetParent(buttonObject.transform, false);
        var text = labelObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.text = label;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = PuniTheme.Ink;
        text.raycastTarget = false;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;

        return button;
    }

    private static void PlayButtonSound()
    {
        AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }
    }
}

public sealed class PuniButtonVisual : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;
    private Color normalColor;
    private Vector3 baseScale = Vector3.one;
    private float pressTimer;
    private const float PressDuration = 0.22f;

    public void Initialize(Image targetImage, Color color)
    {
        image = targetImage;
        rectTransform = targetImage.rectTransform;
        baseScale = rectTransform.localScale;
        normalColor = color;
    }

    public void PlayPress()
    {
        pressTimer = PressDuration;
    }

    public void SetInteractable(bool interactable)
    {
        if (image != null)
        {
            image.color = interactable ? normalColor : PuniTheme.Disabled;
        }
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }

        if (pressTimer <= 0f)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, baseScale, Time.deltaTime * 14f);
            return;
        }

        pressTimer = Mathf.Max(0f, pressTimer - Time.deltaTime);
        float progress = 1f - pressTimer / PressDuration;
        float squash = Mathf.Sin(progress * Mathf.PI);
        float rebound = Mathf.Sin(progress * Mathf.PI * 2f) * 0.03f;
        rectTransform.localScale = new Vector3(
            baseScale.x * (1f + squash * 0.07f + rebound),
            baseScale.y * (1f - squash * 0.05f),
            baseScale.z);
    }
}

public sealed class PuniBackgroundMotion : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 basePosition;
    private float speed;
    private Vector2 amplitude;
    private float rotationAmplitude;
    private float phase;

    public void Initialize(float moveSpeed, Vector2 moveAmplitude, float rotateAmplitude, float startPhase = 0f)
    {
        rectTransform = GetComponent<RectTransform>();
        basePosition = rectTransform.anchoredPosition;
        speed = moveSpeed;
        amplitude = moveAmplitude;
        rotationAmplitude = rotateAmplitude;
        phase = startPhase;
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }

        float t = Time.time * speed + phase;
        rectTransform.anchoredPosition = basePosition + new Vector2(Mathf.Sin(t) * amplitude.x, Mathf.Cos(t * 0.8f) * amplitude.y);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 1.2f) * rotationAmplitude);
    }
}
