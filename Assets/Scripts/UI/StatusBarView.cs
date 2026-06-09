using UnityEngine;
using UnityEngine.UI;

public sealed class StatusBarView
{
    private readonly Slider slider;
    private readonly Text label;
    private readonly Image fill;
    private readonly Color normalColor;
    private readonly string name;

    public StatusBarView(Transform parent, string name, Vector2 position, Color normalColor)
    {
        this.name = name;
        this.normalColor = normalColor;

        var root = new GameObject($"{name}Status");
        root.transform.SetParent(parent, false);
        var rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(560f, 38f);

        label = CreateText(root.transform, "Label", TextAnchor.MiddleLeft, new Vector2(130f, 38f));
        label.rectTransform.anchorMin = new Vector2(0f, 0f);
        label.rectTransform.anchorMax = new Vector2(0f, 1f);

        var sliderObject = new GameObject("Slider");
        sliderObject.transform.SetParent(root.transform, false);
        slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;

        var sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0f, 0f);
        sliderRect.anchorMax = new Vector2(1f, 1f);
        sliderRect.offsetMin = new Vector2(145f, 7f);
        sliderRect.offsetMax = new Vector2(0f, -6f);

        var background = CreateImage(sliderObject.transform, "Background", new Color(0.82f, 0.86f, 0.88f));
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.offsetMin = Vector2.zero;
        background.rectTransform.offsetMax = Vector2.zero;

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        fill = CreateImage(fillArea.transform, "Fill", normalColor);
        fill.rectTransform.anchorMin = Vector2.zero;
        fill.rectTransform.anchorMax = Vector2.one;
        fill.rectTransform.offsetMin = Vector2.zero;
        fill.rectTransform.offsetMax = Vector2.zero;

        slider.fillRect = fill.rectTransform;
        slider.targetGraphic = fill;
        slider.interactable = false;
    }

    public void SetValue(int value)
    {
        slider.value = value;
        label.text = $"{name} {value}";
        fill.color = value <= Constants.LowStatusThreshold ? new Color(0.88f, 0.28f, 0.30f) : normalColor;
    }

    private static Image CreateImage(Transform parent, string name, Color color)
    {
        var imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);
        var image = imageObject.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(Transform parent, string name, TextAnchor alignment, Vector2 size)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.alignment = alignment;
        text.fontSize = 20;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.rectTransform.sizeDelta = size;
        return text;
    }
}
