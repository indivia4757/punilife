using UnityEngine;
using UnityEngine.UI;

public static class PuniTheme
{
    private static Sprite roundedSprite;
    private static Sprite circleSprite;

    public static readonly Color Ink = new Color(0.25f, 0.20f, 0.16f);
    public static readonly Color SoftPanel = new Color(1f, 0.98f, 0.92f, 0.90f);
    public static readonly Color PanelShadow = new Color(0.42f, 0.31f, 0.20f, 0.14f);
    public static readonly Color CreamButton = new Color(1f, 0.90f, 0.62f);
    public static readonly Color PeachButton = new Color(1f, 0.72f, 0.55f);
    public static readonly Color MintButton = new Color(0.68f, 0.86f, 0.77f);
    public static readonly Color SkyButton = new Color(0.62f, 0.78f, 0.94f);
    public static readonly Color LilacButton = new Color(0.78f, 0.70f, 0.93f);
    public static readonly Color Disabled = new Color(0.76f, 0.74f, 0.70f);
    public static readonly Color GardenBack = new Color(0.86f, 0.95f, 0.88f);
    public static readonly Color GardenMid = new Color(0.73f, 0.88f, 0.74f);
    public static readonly Color Floor = new Color(0.98f, 0.90f, 0.74f);

    public static Sprite RoundedSprite
    {
        get
        {
            if (roundedSprite == null)
            {
                roundedSprite = CreateRoundedSprite();
            }

            return roundedSprite;
        }
    }

    public static Sprite CircleSprite
    {
        get
        {
            if (circleSprite == null)
            {
                circleSprite = CreateCircleSprite();
            }

            return circleSprite;
        }
    }

    public static void ApplyRounded(Image image, Color color)
    {
        image.sprite = RoundedSprite;
        image.type = Image.Type.Sliced;
        image.color = color;
    }

    public static Image CreateShadow(Transform parent, string name, Vector2 offset)
    {
        var shadowObject = new GameObject(name);
        shadowObject.transform.SetParent(parent, false);
        var shadow = shadowObject.AddComponent<Image>();
        ApplyRounded(shadow, PanelShadow);
        shadow.raycastTarget = false;
        shadow.rectTransform.anchorMin = Vector2.zero;
        shadow.rectTransform.anchorMax = Vector2.one;
        shadow.rectTransform.offsetMin = new Vector2(offset.x, offset.y);
        shadow.rectTransform.offsetMax = new Vector2(offset.x, offset.y);
        shadowObject.transform.SetAsFirstSibling();
        return shadow;
    }

    public static void ApplyCircle(Image image, Color color)
    {
        image.sprite = CircleSprite;
        image.color = color;
    }

    private static Sprite CreateRoundedSprite()
    {
        const int size = 64;
        const int radius = 18;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Max(Mathf.Max(radius - x, 0), x - (size - radius - 1));
                float dy = Mathf.Max(Mathf.Max(radius - y, 0), y - (size - radius - 1));
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(radius - distance + 1f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        var sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect, new Vector4(18f, 18f, 18f, 18f));
        return sprite;
    }

    private static Sprite CreateCircleSprite()
    {
        const int size = 96;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.42f;

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
