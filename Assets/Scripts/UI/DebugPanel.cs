using UnityEngine;
using UnityEngine.UI;

public sealed class DebugPanel
{
    private readonly GameObject root;
    private readonly Text infoText;
    private GameManager gameManager;

    public DebugPanel(Transform parent)
    {
        root = new GameObject("DebugPanel");
        root.transform.SetParent(parent, false);

        var image = root.AddComponent<Image>();
        image.color = new Color(0.12f, 0.14f, 0.16f, 0.94f);

        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 0f);
        rect.sizeDelta = new Vector2(620f, 760f);

        var title = CreateText(root.transform, "Title", new Vector2(0f, 320f), new Vector2(540f, 44f), 28, TextAnchor.MiddleCenter);
        title.text = "디버그";
        title.color = Color.white;

        infoText = CreateText(root.transform, "Info", new Vector2(0f, 245f), new Vector2(540f, 110f), 19, TextAnchor.MiddleCenter);
        infoText.color = new Color(0.88f, 0.92f, 0.95f);

        CreateButton(root.transform, "경험치 +50", new Vector2(-150f, 130f), () => Run(() => gameManager.DebugAddExp(50)));
        CreateButton(root.transform, "12시간 경과", new Vector2(150f, 130f), () => Run(() => gameManager.DebugSimulateOfflineHours(12)));
        CreateButton(root.transform, "강제 진화", new Vector2(-150f, 40f), () => Run(() => gameManager.DebugForceEvolution()));
        CreateButton(root.transform, "저장 초기화", new Vector2(150f, 40f), () => Run(() => gameManager.DebugResetSave()));
        CreateButton(root.transform, "닫기", new Vector2(0f, -265f), Hide);

        root.SetActive(false);
    }

    public void Show(GameManager manager)
    {
        gameManager = manager;
        Refresh();
        root.SetActive(true);
    }

    public void Refresh()
    {
        if (gameManager == null)
        {
            return;
        }

        SaveData data = gameManager.GetSaveData();
        infoText.text =
            $"단계 {PuniText.StageName(data.stage)} / 진화 {PuniText.EvolutionName(data.evolutionType)}\n" +
            $"{FormatLevel(data.status)} 코인 {data.status.coin}\n" +
            $"INT {data.growthStats.intelligence} STR {data.growthStats.strength} SEN {data.growthStats.sensitivity} " +
            $"COU {data.growthStats.courage} KIN {data.growthStats.kindness} NEG {data.growthStats.neglect}";
    }

    private static string FormatLevel(PuniStatus status)
    {
        return status.level >= Constants.EvolutionLevel
            ? $"Lv.{status.level} 최고 레벨"
            : $"Lv.{status.level} 경험치 {status.exp}/{status.NextExp}";
    }

    private void Run(System.Action action)
    {
        action?.Invoke();
        Refresh();
    }

    private void Hide()
    {
        root.SetActive(false);
    }

    private static void CreateButton(Transform parent, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        var buttonObject = new GameObject($"{label}Button");
        buttonObject.transform.SetParent(parent, false);
        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.43f, 0.66f, 0.90f);

        var button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(210f, 60f);

        var text = CreateText(buttonObject.transform, "Text", Vector2.zero, new Vector2(210f, 60f), 21, TextAnchor.MiddleCenter);
        text.text = label;
        text.color = Color.white;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;
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
        text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
    }
}
