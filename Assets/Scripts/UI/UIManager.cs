using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private bool built;
    private Text titleText;
    private Text coinText;
    private Text levelText;
    private Text messageText;
    private Text gardenText;
    private Text dexText;
    private StatusBarView hungerBar;
    private StatusBarView happinessBar;
    private StatusBarView cleanlinessBar;
    private StatusBarView energyBar;
    private StatusBarView affectionBar;
    private PuniView puniView;
    private DexGardenPanel dexGardenPanel;
    private DebugPanel debugPanel;
    private Button feedButton;
    private Button playButton;
    private Button cleanButton;
    private Button sleepButton;
    private Button studyButton;
    private Button trainButton;

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

    public void Refresh()
    {
        SaveData data = gameManager.Puni.Data;
        titleText.text = "PUNI Life";
        coinText.text = $"Coin {data.status.coin}";
        levelText.text = $"Lv.{data.status.level}  EXP {data.status.exp}/{data.status.NextExp}";
        messageText.text = BuildMessage(data);
        gardenText.text = gameManager.GetGardenName();
        dexText.text = $"Dex {gameManager.GetDexUnlockedCount()}/5";
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
            return $"{gameManager.LastMessage}  PUNI feels sick.";
        }

        if (data.status.isHungry)
        {
            return $"{gameManager.LastMessage}  PUNI is hungry.";
        }

        if (data.status.isDirty)
        {
            return $"{gameManager.LastMessage}  PUNI needs a bath.";
        }

        if (data.status.isSulking)
        {
            return $"{gameManager.LastMessage}  PUNI feels lonely.";
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

        var background = CreatePanel(canvasObject.transform, "RoomBackground", new Color(0.94f, 0.97f, 0.92f));
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.offsetMin = Vector2.zero;
        background.rectTransform.offsetMax = Vector2.zero;

        CreateTopBand(canvasObject.transform);

        titleText = CreateText(canvasObject.transform, "Title", new Vector2(0f, -30f), new Vector2(420f, 48f), 34, TextAnchor.MiddleCenter);
        coinText = CreateText(canvasObject.transform, "Coin", new Vector2(250f, -40f), new Vector2(210f, 38f), 22, TextAnchor.MiddleRight);
        levelText = CreateText(canvasObject.transform, "Level", new Vector2(-235f, -40f), new Vector2(250f, 38f), 22, TextAnchor.MiddleLeft);
        gardenText = CreateText(canvasObject.transform, "Garden", new Vector2(-180f, -96f), new Vector2(300f, 34f), 20, TextAnchor.MiddleLeft);
        dexText = CreateText(canvasObject.transform, "Dex", new Vector2(230f, -96f), new Vector2(220f, 34f), 20, TextAnchor.MiddleRight);
        messageText = CreateText(canvasObject.transform, "Message", new Vector2(0f, -145f), new Vector2(610f, 64f), 21, TextAnchor.MiddleCenter);

        puniView = new PuniView(canvasObject.transform);

        hungerBar = new StatusBarView(canvasObject.transform, "Hunger", new Vector2(0f, -760f), new Color(0.96f, 0.55f, 0.45f));
        happinessBar = new StatusBarView(canvasObject.transform, "Happy", new Vector2(0f, -806f), new Color(1f, 0.78f, 0.30f));
        cleanlinessBar = new StatusBarView(canvasObject.transform, "Clean", new Vector2(0f, -852f), new Color(0.34f, 0.72f, 0.92f));
        energyBar = new StatusBarView(canvasObject.transform, "Energy", new Vector2(0f, -898f), new Color(0.46f, 0.72f, 0.50f));
        affectionBar = new StatusBarView(canvasObject.transform, "Love", new Vector2(0f, -944f), new Color(0.92f, 0.48f, 0.76f));

        feedButton = CreateButton(canvasObject.transform, "Feed", new Vector2(-225f, 235f), () => gameManager.PerformCare(CareActionType.Feed));
        playButton = CreateButton(canvasObject.transform, "Play", new Vector2(0f, 235f), () => gameManager.PerformCare(CareActionType.Play));
        cleanButton = CreateButton(canvasObject.transform, "Clean", new Vector2(225f, 235f), () => gameManager.PerformCare(CareActionType.Clean));
        sleepButton = CreateButton(canvasObject.transform, "Sleep", new Vector2(-225f, 150f), () => gameManager.PerformCare(CareActionType.Sleep));
        studyButton = CreateButton(canvasObject.transform, "Study", new Vector2(0f, 150f), () => gameManager.PerformCare(CareActionType.Study));
        trainButton = CreateButton(canvasObject.transform, "Train", new Vector2(225f, 150f), () => gameManager.PerformCare(CareActionType.Train));
        CreateButton(canvasObject.transform, "Snack Tap", new Vector2(120f, 66f), StartMiniGame);
        CreateButton(canvasObject.transform, "Dex", new Vector2(-120f, 66f), ShowDexGarden);
        CreateButton(canvasObject.transform, "Free Snack", new Vector2(-225f, 0f), () => gameManager.WatchAdForFreeSnack());
        CreateButton(canvasObject.transform, "Recover", new Vector2(225f, 0f), () => gameManager.WatchAdForRecovery());
        CreateButton(canvasObject.transform, "Debug", new Vector2(0f, 0f), ShowDebug);
        dexGardenPanel = new DexGardenPanel(canvasObject.transform);
        debugPanel = new DebugPanel(canvasObject.transform);
        built = true;
    }

    private static void CreateTopBand(Transform parent)
    {
        var band = CreatePanel(parent, "TopInfoBand", new Color(1f, 1f, 1f, 0.42f));
        band.rectTransform.anchorMin = new Vector2(0f, 1f);
        band.rectTransform.anchorMax = new Vector2(1f, 1f);
        band.rectTransform.pivot = new Vector2(0.5f, 1f);
        band.rectTransform.anchoredPosition = Vector2.zero;
        band.rectTransform.sizeDelta = new Vector2(0f, 185f);
    }

    private void RefreshButtons(SaveData data)
    {
        SetButtonState(feedButton, data.status.coin >= 10);
        SetButtonState(playButton, data.status.energy >= 10);
        SetButtonState(cleanButton, data.stage != PuniStage.Egg);
        SetButtonState(sleepButton, data.stage != PuniStage.Egg);
        SetButtonState(studyButton, data.stage == PuniStage.Young && data.status.energy >= 10);
        SetButtonState(trainButton, data.stage == PuniStage.Young && data.status.energy >= 15);
    }

    private static void SetButtonState(Button button, bool interactable)
    {
        button.interactable = interactable;
        var image = button.GetComponent<Image>();
        image.color = interactable ? new Color(0.43f, 0.66f, 0.90f) : new Color(0.68f, 0.72f, 0.76f);
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

    private void ShowDexGarden()
    {
        dexGardenPanel.Show(gameManager.GetSaveData(), gameManager.GetGardenName(), gameManager.GetDexUnlockedCount());
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
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        text.rectTransform.pivot = new Vector2(0.5f, 1f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private static Button CreateButton(Transform parent, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        var buttonObject = new GameObject($"{label}Button");
        buttonObject.transform.SetParent(parent, false);
        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.43f, 0.66f, 0.90f);

        var button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(176f, 62f);

        var labelObject = new GameObject("Text");
        labelObject.transform.SetParent(buttonObject.transform, false);
        var text = labelObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = label;
        text.fontSize = 22;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;

        return button;
    }
}
