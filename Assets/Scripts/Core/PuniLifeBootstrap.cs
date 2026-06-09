using UnityEngine;
using UnityEngine.EventSystems;

public static class PuniLifeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        Cursor.visible = true;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (Camera.main == null)
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.backgroundColor = new Color(0.93f, 0.96f, 0.90f);
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        if (Object.FindAnyObjectByType<UIManager>() == null)
        {
            new GameObject("UIManager").AddComponent<UIManager>();
        }

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        if (Object.FindAnyObjectByType<GameManager>() == null)
        {
            var gameObject = new GameObject("GameManager");
            gameObject.AddComponent<AudioManager>();
            gameObject.AddComponent<AdManager>();
            gameObject.AddComponent<GameManager>();
        }
    }
}
