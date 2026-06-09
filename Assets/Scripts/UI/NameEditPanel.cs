using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class NameEditPanel
{
    private readonly GameObject root;
    private readonly InputField inputField;
    private Action<string> onSave;

    public NameEditPanel(Transform parent)
    {
        root = new GameObject("NameEditPanel");
        root.transform.SetParent(parent, false);

        var image = root.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.97f);

        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(560f, 390f);

        var title = CreateText(root.transform, "Title", new Vector2(0f, 135f), new Vector2(480f, 48f), 30, TextAnchor.MiddleCenter);
        title.text = "이름 짓기";

        var hint = CreateText(root.transform, "Hint", new Vector2(0f, 82f), new Vector2(480f, 38f), 20, TextAnchor.MiddleCenter);
        hint.text = "푸니에게 불러줄 이름을 정해주세요.";

        inputField = CreateInputField(root.transform);

        CreateButton(root.transform, "저장", new Vector2(-105f, -125f), Save);
        CreateButton(root.transform, "취소", new Vector2(105f, -125f), Hide);
        root.SetActive(false);
    }

    public void Show(string currentName, Action<string> saveAction)
    {
        onSave = saveAction;
        inputField.text = currentName;
        root.SetActive(true);
        root.transform.SetAsLastSibling();
        inputField.ActivateInputField();
    }

    private void Save()
    {
        onSave?.Invoke(inputField.text);
        Hide();
    }

    private void Hide()
    {
        root.SetActive(false);
    }

    private static InputField CreateInputField(Transform parent)
    {
        var inputObject = new GameObject("NameInput");
        inputObject.transform.SetParent(parent, false);
        var image = inputObject.AddComponent<Image>();
        image.color = new Color(0.93f, 0.96f, 0.98f);

        var rect = inputObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 5f);
        rect.sizeDelta = new Vector2(420f, 62f);

        var input = inputObject.AddComponent<InputField>();
        input.characterLimit = 10;

        var text = CreateText(inputObject.transform, "Text", Vector2.zero, new Vector2(380f, 54f), 26, TextAnchor.MiddleCenter);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = new Vector2(18f, 4f);
        text.rectTransform.offsetMax = new Vector2(-18f, -4f);
        input.textComponent = text;

        var placeholder = CreateText(inputObject.transform, "Placeholder", Vector2.zero, new Vector2(380f, 54f), 24, TextAnchor.MiddleCenter);
        placeholder.text = "예: 모찌";
        placeholder.color = new Color(0.55f, 0.58f, 0.62f);
        placeholder.rectTransform.anchorMin = Vector2.zero;
        placeholder.rectTransform.anchorMax = Vector2.one;
        placeholder.rectTransform.offsetMin = new Vector2(18f, 4f);
        placeholder.rectTransform.offsetMax = new Vector2(-18f, -4f);
        input.placeholder = placeholder;

        return input;
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
        rect.sizeDelta = new Vector2(170f, 58f);

        var text = CreateText(buttonObject.transform, "Text", Vector2.zero, new Vector2(170f, 58f), 22, TextAnchor.MiddleCenter);
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
