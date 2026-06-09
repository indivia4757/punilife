using UnityEngine;
using UnityEngine.UI;
using System;

public sealed class DexGardenPanel
{
    private readonly GameObject root;
    private readonly Text titleText;
    private readonly Text gardenText;
    private readonly Text[] entryTexts;

    public DexGardenPanel(Transform parent)
    {
        root = new GameObject("DexGardenPanel");
        root.transform.SetParent(parent, false);

        var image = root.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.96f);

        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 15f);
        rect.sizeDelta = new Vector2(610f, 760f);

        titleText = CreateText(root.transform, "Title", new Vector2(0f, 320f), new Vector2(540f, 48f), 30, TextAnchor.MiddleCenter);
        gardenText = CreateText(root.transform, "Garden", new Vector2(0f, 255f), new Vector2(540f, 88f), 22, TextAnchor.MiddleCenter);

        entryTexts = new Text[5];
        entryTexts[0] = CreateText(root.transform, "Sunny", new Vector2(0f, 145f), new Vector2(500f, 46f), 22, TextAnchor.MiddleLeft);
        entryTexts[1] = CreateText(root.transform, "Scholar", new Vector2(0f, 85f), new Vector2(500f, 46f), 22, TextAnchor.MiddleLeft);
        entryTexts[2] = CreateText(root.transform, "Brave", new Vector2(0f, 25f), new Vector2(500f, 46f), 22, TextAnchor.MiddleLeft);
        entryTexts[3] = CreateText(root.transform, "Forest", new Vector2(0f, -35f), new Vector2(500f, 46f), 22, TextAnchor.MiddleLeft);
        entryTexts[4] = CreateText(root.transform, "Shadow", new Vector2(0f, -95f), new Vector2(500f, 46f), 22, TextAnchor.MiddleLeft);

        CreateCloseButton(root.transform);
        root.SetActive(false);
    }

    public void Show(SaveData data, string gardenName, int unlockedCount)
    {
        Refresh(data, gardenName, unlockedCount);
        root.SetActive(true);
    }

    public void Refresh(SaveData data, string gardenName, int unlockedCount)
    {
        titleText.text = "PUNI Dex";
        gardenText.text = $"Garden: {gardenName}\nRestoration {unlockedCount}/5";

        SetEntry(entryTexts[0], data, PuniEvolutionType.Sunny, "Sunny PUNI", "Happy and loved");
        SetEntry(entryTexts[1], data, PuniEvolutionType.Scholar, "Scholar PUNI", "Raised through study");
        SetEntry(entryTexts[2], data, PuniEvolutionType.Brave, "Brave PUNI", "Raised through training");
        SetEntry(entryTexts[3], data, PuniEvolutionType.Forest, "Forest PUNI", "Kind and clean");
        SetEntry(entryTexts[4], data, PuniEvolutionType.Shadow, "Shadow PUNI", "Lonely but precious");
    }

    private void CreateCloseButton(Transform parent)
    {
        var buttonObject = new GameObject("CloseButton");
        buttonObject.transform.SetParent(parent, false);
        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.43f, 0.66f, 0.90f);

        var button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(() => root.SetActive(false));

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -290f);
        rect.sizeDelta = new Vector2(190f, 58f);

        var text = CreateText(buttonObject.transform, "Text", Vector2.zero, new Vector2(190f, 58f), 22, TextAnchor.MiddleCenter);
        text.text = "Close";
        text.color = Color.white;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;
    }

    private static void SetEntry(Text text, SaveData data, PuniEvolutionType type, string name, string hint)
    {
        PuniDexEntry entry = FindEntry(data, type);
        bool unlocked = entry != null && entry.unlocked;
        text.text = unlocked ? $"{name}  {FormatDate(entry.unlockedAt)}" : $"????  {hint}";
        text.color = unlocked ? new Color(0.18f, 0.22f, 0.24f) : new Color(0.55f, 0.58f, 0.62f);
    }

    private static string FormatDate(string isoDate)
    {
        if (!DateTime.TryParse(isoDate, out DateTime dateTime))
        {
            return "Unlocked";
        }

        return dateTime.ToLocalTime().ToString("yyyy-MM-dd");
    }

    private static PuniDexEntry FindEntry(SaveData data, PuniEvolutionType type)
    {
        foreach (PuniDexEntry entry in data.dexEntries)
        {
            if (entry.type == type)
            {
                return entry;
            }
        }

        return null;
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
        text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
    }
}
