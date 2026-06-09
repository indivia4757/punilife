using UnityEngine;
using UnityEngine.UI;

public sealed class PuniView
{
    private readonly RectTransform rootRect;
    private readonly Image body;
    private readonly Image shell;
    private readonly Image leftEye;
    private readonly Image rightEye;
    private readonly Image mouth;
    private readonly Image leftCheek;
    private readonly Image rightCheek;
    private readonly Image sproutLeft;
    private readonly Image sproutRight;
    private readonly Image leftWing;
    private readonly Image rightWing;
    private readonly Image[] spots;
    private readonly Text stageText;
    private readonly Sprite circleSprite;

    public PuniView(Transform parent)
    {
        circleSprite = CreateCircleSprite();
        var root = new GameObject("PuniView");
        root.transform.SetParent(parent, false);
        rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(0f, 105f);
        rootRect.sizeDelta = new Vector2(250f, 270f);

        body = root.AddComponent<Image>();
        body.sprite = circleSprite;

        leftWing = CreateShape(root.transform, "LeftWing", new Vector2(-102f, -24f), new Vector2(52f, 82f), new Color(1f, 0.78f, 0.22f), 28f);
        rightWing = CreateShape(root.transform, "RightWing", new Vector2(102f, -24f), new Vector2(52f, 82f), new Color(1f, 0.78f, 0.22f), -28f);
        sproutLeft = CreateShape(root.transform, "SproutLeft", new Vector2(-28f, 128f), new Vector2(42f, 70f), new Color(1f, 0.83f, 0.28f), -32f);
        sproutRight = CreateShape(root.transform, "SproutRight", new Vector2(22f, 128f), new Vector2(42f, 70f), new Color(1f, 0.83f, 0.28f), 38f);

        shell = CreateShape(root.transform, "EggShell", new Vector2(0f, -69f), new Vector2(220f, 96f), new Color(1f, 0.96f, 0.82f), 0f);
        spots = new[]
        {
            CreateShape(shell.transform, "SpotOrange", new Vector2(22f, 6f), new Vector2(32f, 24f), new Color(1f, 0.53f, 0.27f), 0f),
            CreateShape(shell.transform, "SpotMint", new Vector2(-56f, -8f), new Vector2(34f, 24f), new Color(0.50f, 0.78f, 0.74f), -15f),
            CreateShape(shell.transform, "SpotYellow", new Vector2(72f, -12f), new Vector2(24f, 20f), new Color(0.98f, 0.78f, 0.25f), 0f),
            CreateShape(shell.transform, "SpotGreen", new Vector2(-12f, -26f), new Vector2(28f, 20f), new Color(0.62f, 0.76f, 0.35f), 0f)
        };

        leftEye = CreateShape(root.transform, "LeftEye", new Vector2(-46f, 22f), new Vector2(34f, 48f), new Color(0.12f, 0.08f, 0.06f), 0f);
        rightEye = CreateShape(root.transform, "RightEye", new Vector2(46f, 22f), new Vector2(34f, 48f), new Color(0.12f, 0.08f, 0.06f), 0f);
        CreateShape(leftEye.transform, "LeftEyeHighlight", new Vector2(8f, 12f), new Vector2(11f, 14f), Color.white, 0f);
        CreateShape(rightEye.transform, "RightEyeHighlight", new Vector2(8f, 12f), new Vector2(11f, 14f), Color.white, 0f);
        mouth = CreateShape(root.transform, "Mouth", new Vector2(0f, -20f), new Vector2(34f, 26f), new Color(0.72f, 0.12f, 0.07f), 0f);
        leftCheek = CreateShape(root.transform, "LeftCheek", new Vector2(-78f, -12f), new Vector2(32f, 22f), new Color(1f, 0.67f, 0.58f, 0.72f), 0f);
        rightCheek = CreateShape(root.transform, "RightCheek", new Vector2(78f, -12f), new Vector2(32f, 22f), new Color(1f, 0.67f, 0.58f, 0.72f), 0f);

        stageText = CreateText(root.transform, "Stage", TextAnchor.MiddleCenter, 22, new Vector2(0f, -150f), new Vector2(320f, 42f));
    }

    public void Refresh(SaveData data)
    {
        bool isEgg = data.stage == PuniStage.Egg;
        body.color = GetBodyColor(data);
        body.rectTransform.sizeDelta = isEgg ? new Vector2(150f, 205f) : new Vector2(214f, 225f);
        shell.gameObject.SetActive(!isEgg);
        leftWing.gameObject.SetActive(!isEgg);
        rightWing.gameObject.SetActive(!isEgg && data.stage != PuniStage.Baby);
        sproutLeft.gameObject.SetActive(!isEgg);
        sproutRight.gameObject.SetActive(!isEgg);
        leftEye.gameObject.SetActive(!isEgg);
        rightEye.gameObject.SetActive(!isEgg);
        mouth.gameObject.SetActive(!isEgg);
        leftCheek.gameObject.SetActive(!isEgg);
        rightCheek.gameObject.SetActive(!isEgg);

        foreach (Image spot in spots)
        {
            spot.gameObject.SetActive(true);
        }

        if (isEgg)
        {
            shell.gameObject.SetActive(false);
            spots[0].transform.SetParent(rootRect, false);
            spots[0].rectTransform.anchoredPosition = new Vector2(28f, 2f);
            spots[1].transform.SetParent(rootRect, false);
            spots[1].rectTransform.anchoredPosition = new Vector2(-38f, -16f);
            spots[2].transform.SetParent(rootRect, false);
            spots[2].rectTransform.anchoredPosition = new Vector2(12f, -54f);
            spots[3].transform.SetParent(rootRect, false);
            spots[3].rectTransform.anchoredPosition = new Vector2(-12f, 48f);
        }
        else if (spots[0].transform.parent != shell.transform)
        {
            ResetSpotParent();
        }

        ApplyMood(data.status);
        rootRect.localScale = GetScale(data.stage);
        stageText.text = data.stage == PuniStage.Evolved ? PuniText.EvolutionName(data.evolutionType) : PuniText.StageName(data.stage);
    }

    private static Vector3 GetScale(PuniStage stage)
    {
        return stage switch
        {
            PuniStage.Egg => Vector3.one * 0.82f,
            PuniStage.Baby => Vector3.one * 0.92f,
            PuniStage.Young => Vector3.one,
            PuniStage.Evolved => Vector3.one * 1.08f,
            _ => Vector3.one
        };
    }

    private static Color GetBodyColor(SaveData data)
    {
        if (data.stage == PuniStage.Egg)
        {
            return new Color(0.96f, 0.92f, 0.82f);
        }

        return data.evolutionType switch
        {
            PuniEvolutionType.Sunny => new Color(1f, 0.86f, 0.42f),
            PuniEvolutionType.Scholar => new Color(0.58f, 0.72f, 1f),
            PuniEvolutionType.Brave => new Color(1f, 0.54f, 0.48f),
            PuniEvolutionType.Forest => new Color(0.5f, 0.84f, 0.56f),
            PuniEvolutionType.Shadow => new Color(0.5f, 0.48f, 0.66f),
            _ => new Color(1f, 0.90f, 0.58f)
        };
    }

    private void ApplyMood(PuniStatus status)
    {
        mouth.color = new Color(0.72f, 0.12f, 0.07f);
        mouth.rectTransform.sizeDelta = new Vector2(34f, 26f);
        mouth.rectTransform.anchoredPosition = new Vector2(0f, -20f);

        if (status.isSick)
        {
            leftEye.color = new Color(0.25f, 0.22f, 0.24f);
            rightEye.color = leftEye.color;
            mouth.color = new Color(0.18f, 0.20f, 0.24f);
            mouth.rectTransform.sizeDelta = new Vector2(34f, 8f);
            return;
        }

        if (status.isHungry || status.isDirty || status.isSulking)
        {
            mouth.color = new Color(0.18f, 0.20f, 0.24f);
            mouth.rectTransform.sizeDelta = new Vector2(30f, 8f);
            return;
        }

        if (status.energy <= Constants.LowStatusThreshold)
        {
            mouth.color = new Color(0.18f, 0.20f, 0.24f);
            mouth.rectTransform.sizeDelta = new Vector2(24f, 6f);
            return;
        }

        leftEye.color = new Color(0.12f, 0.08f, 0.06f);
        rightEye.color = leftEye.color;
    }

    private void ResetSpotParent()
    {
        Vector2[] positions =
        {
            new Vector2(22f, 6f),
            new Vector2(-56f, -8f),
            new Vector2(72f, -12f),
            new Vector2(-12f, -26f)
        };

        for (int i = 0; i < spots.Length; i++)
        {
            spots[i].transform.SetParent(shell.transform, false);
            spots[i].rectTransform.anchoredPosition = positions[i];
        }
    }

    private Image CreateShape(Transform parent, string name, Vector2 position, Vector2 size, Color color, float rotation)
    {
        var shape = new GameObject(name);
        shape.transform.SetParent(parent, false);
        var image = shape.AddComponent<Image>();
        image.sprite = circleSprite;
        image.color = color;
        image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        image.rectTransform.anchoredPosition = position;
        image.rectTransform.sizeDelta = size;
        image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
        return image;
    }

    private static Text CreateText(Transform parent, string name, TextAnchor alignment, int fontSize, Vector2 position, Vector2 size)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = PuniFonts.Default;
        text.alignment = alignment;
        text.fontSize = fontSize;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(0.18f, 0.20f, 0.24f);
        text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = position;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private static Sprite CreateCircleSprite()
    {
        const int size = 128;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.43f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(radius - distance + 1f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
