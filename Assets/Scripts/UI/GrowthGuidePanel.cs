using UnityEngine;
using UnityEngine.UI;

public sealed class GrowthGuidePanel
{
    private readonly GameObject root;

    public GrowthGuidePanel(Transform parent)
    {
        root = new GameObject("GrowthGuidePanel");
        root.transform.SetParent(parent, false);

        var image = root.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.97f);

        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 10f);
        rect.sizeDelta = new Vector2(620f, 930f);

        var title = CreateText(root.transform, "Title", new Vector2(0f, 405f), new Vector2(540f, 52f), 32, TextAnchor.MiddleCenter);
        title.text = "성장 가이드";

        var body = CreateText(root.transform, "Body", new Vector2(0f, 40f), new Vector2(540f, 675f), 21, TextAnchor.UpperLeft);
        body.text =
            "푸니는 돌봄과 경험치로 성장합니다.\n\n" +
            "성장 단계\n" +
            "- 알: 먹이와 놀기만 가능\n" +
            "- Lv.4: 어린 푸니로 성장\n" +
            "- Lv.10 또는 3일 플레이: 최종 진화\n\n" +
            "돌봄 효과\n" +
            "- 먹이: 코인 10 사용, 배고픔 회복\n" +
            "- 놀기: 에너지 10 사용, 행복과 경험치 증가\n" +
            "- 청소: 아기 단계부터 가능, 청결 회복\n" +
            "- 잠: 아기 단계부터 가능, 에너지 회복\n" +
            "- 공부: 어린 푸니부터 가능, 학자 진화에 영향\n" +
            "- 훈련: 어린 푸니부터 가능, 용감 진화에 영향\n\n" +
            "오프라인 성장\n" +
            "- 앱을 꺼둔 동안에도 경험치가 조금 쌓입니다.\n" +
            "- 최대 24시간까지 계산되고, 경험치는 최대 12까지 얻습니다.\n" +
            "- 대신 배고픔, 행복, 청결이 떨어지고 방치 수치가 오릅니다.\n\n" +
            "이름\n" +
            "- 이름 버튼으로 푸니의 이름을 바꿀 수 있습니다.\n\n" +
            "진화 방향\n" +
            "- 행복과 애정이 높으면 햇살 푸니\n" +
            "- 공부를 많이 하면 학자 푸니\n" +
            "- 훈련을 많이 하면 용감 푸니\n" +
            "- 친절과 청결이 높으면 숲의 푸니\n" +
            "- 오래 방치하면 그림자 푸니";

        CreateCloseButton(root.transform);
        root.SetActive(false);
    }

    public void Show()
    {
        root.SetActive(true);
        root.transform.SetAsLastSibling();
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
        rect.anchoredPosition = new Vector2(0f, -400f);
        rect.sizeDelta = new Vector2(190f, 58f);

        var text = CreateText(buttonObject.transform, "Text", Vector2.zero, new Vector2(190f, 58f), 22, TextAnchor.MiddleCenter);
        text.text = "닫기";
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
