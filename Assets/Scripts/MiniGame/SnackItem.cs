using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Button))]
public sealed class SnackItem : MonoBehaviour
{
    private SnackTapGameManager manager;

    public void Initialize(SnackTapGameManager owner)
    {
        manager = owner;
        var image = GetComponent<Image>();
        image.sprite = CreateCircleSprite();
        image.color = new Color(1f, 0.62f, 0.32f);
        GetComponent<Button>().onClick.AddListener(() => manager.Collect(this));
    }

    private static Sprite CreateCircleSprite()
    {
        const int size = 64;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.4f;

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
