using UnityEngine;

public static class PuniLifeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
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

        if (Object.FindAnyObjectByType<GameManager>() == null)
        {
            var gameObject = new GameObject("GameManager");
            gameObject.AddComponent<AdManager>();
            gameObject.AddComponent<GameManager>();
        }
    }
}
