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
            return $"{gameManager.LastMessage}  푸니가 아파요.";
        }

        if (data.status.isHungry)
        {
            return $"{gameManager.LastMessage}  푸니가 배고파요.";
        }

        if (data.status.isDirty)
        {
            return $"{gameManager.LastMessage}  푸니가 씻고 싶어해요.";
        }

        if (data.status.isSulking)
        {
            return $"{gameManager.LastMessage}  푸니가 외로워해요.";
        }

        return $"{gameManager.LastMessage}  {gameManager.GetEvolutionHint()}";
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

        titleText = CreateText(canvasObject.transform, "Title", new Vector2(0f, -30f), new Vector2(420f, 48f), 34, TextAnchor.MiddleCenter);
        titleText.color = new Color(0.48f, 0.36f, 0.24f);
        coinText = CreateText(canvasObject.transform, "Coin", new Vector2(250f, -40f), new Vector2(210f, 38f), 22, TextAnchor.MiddleRight);
        levelText = CreateText(canvasObject.transform, "Level", new Vector2(-235f, -40f), new Vector2(250f, 38f), 22, TextAnchor.MiddleLeft);
        gardenText = CreateText(canvasObject.transform, "Garden", new Vector2(-180f, -96f), new Vector2(300f, 34f), 20, TextAnchor.MiddleLeft);
        dexText = CreateText(canvasObject.transform, "Dex", new Vector2(230f, -96f), new Vector2(220f, 34f), 20, TextAnchor.MiddleRight);
        messageText = CreateText(canvasObject.transform, "Message", new Vector2(0f, -202f), new Vector2(610f, 48f), 19, TextAnchor.MiddleCenter);
        nameText = CreateText(canvasObject.transform, "PuniName", new Vector2(0f, -236f), new Vector2(420f, 44f), 28, TextAnchor.MiddleCenter);
        speechText = CreateSpeechBubble(canvasObject.transform);

        puniView = new PuniView(canvasObject.transform);

        Vector2 statusSize = new Vector2(220f, 30f);
        hungerBar = new StatusBarView(canvasObject.transform, "배고픔", new Vector2(-230f, -122f), new Color(0.96f, 0.55f, 0.45f), statusSize, 15, 58f, 64f);
        happinessBar = new StatusBarView(canvasObject.transform, "행복", new Vector2(0f, -122f), new Color(1f, 0.78f, 0.30f), statusSize, 15, 44f, 50f);
        cleanlinessBar = new StatusBarView(canvasObject.transform, "청결", new Vector2(230f, -122f), new Color(0.34f, 0.72f, 0.92f), statusSize, 15, 44f, 50f);
        energyBar = new StatusBarView(canvasObject.transform, "에너지", new Vector2(-115f, -156f), new Color(0.46f, 0.72f, 0.50f), statusSize, 15, 58f, 64f);
        affectionBar = new StatusBarView(canvasObject.transform, "애정", new Vector2(115f, -156f), new Color(0.92f, 0.48f, 0.76f), statusSize, 15, 44f, 50f);

        CreateActionDock(canvasObject.transform);

        Vector2 careButtonSize = new Vector2(164f, 54f);
        feedButton = CreateButton(canvasObject.transform, "먹이", new Vector2(-178f, 258f), () => PerformCare(CareActionType.Feed), careButtonSize, 20, PuniTheme.CreamButton);
        playButton = CreateButton(canvasObject.transform, "놀기", new Vector2(0f, 258f), () => PerformCare(CareActionType.Play), careButtonSize, 20, PuniTheme.PeachButton);
        cleanButton = CreateButton(canvasObject.transform, "청소", new Vector2(178f, 258f), () => PerformCare(CareActionType.Clean), careButtonSize, 20, PuniTheme.SkyButton);
        sleepButton = CreateButton(canvasObject.transform, "잠", new Vector2(-178f, 196f), () => PerformCare(CareActionType.Sleep), careButtonSize, 20, PuniTheme.LilacButton);
        studyButton = CreateButton(canvasObject.transform, "공부", new Vector2(0f, 196f), () => PerformCare(CareActionType.Study), careButtonSize, 20, PuniTheme.MintButton);
        trainButton = CreateButton(canvasObject.transform, "훈련", new Vector2(178f, 196f), () => PerformCare(CareActionType.Train), careButtonSize, 20, PuniTheme.PeachButton);

        Vector2 menuButtonSize = new Vector2(164f, 52f);
        CreateButton(canvasObject.transform, "도감", new Vector2(-178f, 124f), ShowDexGarden, menuButtonSize, 20, PuniTheme.MintButton);
        CreateButton(canvasObject.transform, "스낵 탭", new Vector2(0f, 124f), StartMiniGame, menuButtonSize, 20, PuniTheme.CreamButton);
        CreateButton(canvasObject.transform, "가이드", new Vector2(178f, 124f), ShowGrowthGuide, menuButtonSize, 20, PuniTheme.SkyButton);

        Vector2 utilityButtonSize = new Vector2(148f, 50f);
        CreateButton(canvasObject.transform, "무료 간식", new Vector2(-238f, 52f), () => gameManager.WatchAdForFreeSnack(), utilityButtonSize, 18, PuniTheme.CreamButton);
        CreateButton(canvasObject.transform, "회복", new Vector2(-79f, 52f), () => gameManager.WatchAdForRecovery(), utilityButtonSize, 18, PuniTheme.MintButton);
        CreateButton(canvasObject.transform, "이름", new Vector2(80f, 52f), ShowNameEdit, utilityButtonSize, 18, PuniTheme.SkyButton);
        newEggButton = CreateButton(canvasObject.transform, "새 알", new Vector2(239f, 52f), StartNewEgg, utilityButtonSize, 18, PuniTheme.LilacButton);
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
        band.rectTransform.sizeDelta = new Vector2(0f, 255f);
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
        dock.rectTransform.sizeDelta = new Vector2(0f, 310f);
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

    private void RefreshButtons(SaveData data)
    {
        SetButtonState(feedButton, data.status.coin >= 10);
        SetButtonState(playButton, data.status.energy >= 10);
        SetButtonState(cleanButton, data.stage != PuniStage.Egg);
        SetButtonState(sleepButton, data.stage != PuniStage.Egg);
        SetButtonState(studyButton, data.stage == PuniStage.Young && data.status.energy >= 10);
        SetButtonState(trainButton, data.stage == PuniStage.Young && data.status.energy >= 15);
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
        bubble.rectTransform.anchoredPosition = new Vector2(0f, -280f);
        bubble.rectTransform.sizeDelta = new Vector2(540f, 64f);
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
