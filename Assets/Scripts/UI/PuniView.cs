using UnityEngine;
using UnityEngine.UI;

public sealed class PuniView
{
    private readonly RectTransform rootRect;
    private readonly Image body;
    private readonly Text face;
    private readonly Text stageText;

    public PuniView(Transform parent)
    {
        var root = new GameObject("PuniView");
        root.transform.SetParent(parent, false);
        rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(0f, 105f);
        rootRect.sizeDelta = new Vector2(250f, 270f);

        body = root.AddComponent<Image>();
        body.sprite = CreateCircleSprite();
        body.color = new Color(0.88f, 0.68f, 0.95f);

        face = CreateText(root.transform, "Face", TextAnchor.MiddleCenter, 64, new Vector2(0f, 15f), new Vector2(210f, 90f));
        stageText = CreateText(root.transform, "Stage", TextAnchor.MiddleCenter, 22, new Vector2(0f, -150f), new Vector2(320f, 42f));
    }

    public void Refresh(SaveData data)
    {
        body.color = GetColor(data);
        face.text = GetFace(data.status);
        rootRect.localScale = GetScale(data.stage);
        stageText.text = data.stage == PuniStage.Evolved ? $"{data.evolutionType} PUNI" : data.stage.ToString();
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

    private static Color GetColor(SaveData data)
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
            _ => new Color(0.88f, 0.68f, 0.95f)
        };
    }

    private static string GetFace(PuniStatus status)
    {
        if (status.isSick)
        {
            return "x_x";
        }

        if (status.isHungry || status.isDirty || status.isSulking)
        {
            return "._.";
        }

        if (status.energy <= Constants.LowStatusThreshold)
        {
            return "-_-";
        }

        return "^_^";
    }

    private static Text CreateText(Transform parent, string name, TextAnchor alignment, int fontSize, Vector2 position, Vector2 size)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
