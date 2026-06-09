using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class SnackTapGameManager : MonoBehaviour
{
    private const float GameDuration = 10f;
    private const float SpawnInterval = 0.7f;

    private GameManager gameManager;
    private UIManager uiManager;
    private int score;
    private int combo;
    private float remainingTime;
    private float nextSpawnAt;
    private bool running;
    private bool rewardClaimed;
    private Transform snackRoot;
    private Text timerText;
    private Text scoreText;
    private Text comboText;
    private GameObject resultPanel;

    public void StartGame(GameManager manager, UIManager ui)
    {
        if (running)
        {
            return;
        }

        gameManager = manager;
        uiManager = ui;
        score = 0;
        combo = 0;
        rewardClaimed = false;
        var overlay = new GameObject("SnackTapOverlay");
        snackRoot = overlay.transform;
        snackRoot.SetParent(uiManager.CanvasTransform, false);
        var overlayRect = overlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        CreateHud(snackRoot);
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        running = true;
        remainingTime = GameDuration;
        nextSpawnAt = 0f;

        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if (Time.time >= nextSpawnAt)
            {
                SpawnSnack();
                nextSpawnAt = Time.time + SpawnInterval;
            }

            RefreshHud();
            yield return null;
        }

        running = false;
        ClearSnacks();
        ShowResult();
    }

    private void SpawnSnack()
    {
        var snack = new GameObject("Snack");
        snack.transform.SetParent(snackRoot, false);
        snack.transform.SetAsLastSibling();
        snack.AddComponent<Image>();
        snack.AddComponent<Button>();
        snack.AddComponent<SnackItem>().Initialize(this);

        var rect = snack.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(76f, 76f);
        rect.anchoredPosition = new Vector2(Random.Range(-260f, 260f), Random.Range(-330f, 230f));
    }

    public void Collect(SnackItem item)
    {
        if (!running)
        {
            return;
        }

        score++;
        combo++;
        RefreshHud();
        Destroy(item.gameObject);
    }

    private void CreateHud(Transform parent)
    {
        var dim = new GameObject("Dim");
        dim.transform.SetParent(parent, false);
        var image = dim.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.18f);
        var dimRect = dim.GetComponent<RectTransform>();
        dimRect.anchorMin = Vector2.zero;
        dimRect.anchorMax = Vector2.one;
        dimRect.offsetMin = Vector2.zero;
        dimRect.offsetMax = Vector2.zero;

        timerText = CreateText(parent, "Timer", new Vector2(-225f, 520f), new Vector2(220f, 46f), 26, TextAnchor.MiddleLeft);
        scoreText = CreateText(parent, "Score", new Vector2(0f, 520f), new Vector2(220f, 46f), 26, TextAnchor.MiddleCenter);
        comboText = CreateText(parent, "Combo", new Vector2(225f, 520f), new Vector2(220f, 46f), 26, TextAnchor.MiddleRight);
        RefreshHud();
    }

    private void RefreshHud()
    {
        if (timerText == null)
        {
            return;
        }

        timerText.text = $"Time {Mathf.CeilToInt(Mathf.Max(0f, remainingTime))}";
        scoreText.text = $"Score {score}";
        comboText.text = combo > 1 ? $"Combo {combo}" : string.Empty;
    }

    private void ShowResult()
    {
        resultPanel = new GameObject("ResultPanel");
        resultPanel.transform.SetParent(snackRoot, false);
        var image = resultPanel.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.96f);

        var rect = resultPanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(500f, 360f);

        var title = CreateText(resultPanel.transform, "Title", new Vector2(0f, 105f), new Vector2(430f, 54f), 30, TextAnchor.MiddleCenter);
        title.text = "Snack Tap";

        var result = CreateText(resultPanel.transform, "Result", new Vector2(0f, 35f), new Vector2(430f, 84f), 24, TextAnchor.MiddleCenter);
        result.text = $"Score {score}\nCoins +{score * 2}";

        CreateButton(resultPanel.transform, "Claim", new Vector2(-115f, -105f), () => ClaimReward(false));
        CreateButton(resultPanel.transform, "2x Ad", new Vector2(115f, -105f), () => ClaimReward(true));
    }

    private void ClaimReward(bool doubled)
    {
        if (rewardClaimed)
        {
            return;
        }

        rewardClaimed = true;
        if (doubled && gameManager.IsRewardedAdReady())
        {
            gameManager.ShowRewardedAd(RewardedAdPlacement.DoubleMiniGameCoins, () => FinishReward(true));
            return;
        }

        FinishReward(false);
    }

    private void FinishReward(bool doubled)
    {
        gameManager.AddMiniGameReward(score, doubled);
        if (snackRoot != null)
        {
            Destroy(snackRoot.gameObject);
        }

        uiManager.Refresh();
    }

    private void ClearSnacks()
    {
        SnackItem[] snacks = snackRoot.GetComponentsInChildren<SnackItem>();
        foreach (SnackItem snack in snacks)
        {
            Destroy(snack.gameObject);
        }
    }

    private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color(0.18f, 0.21f, 0.23f);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
        return text;
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
}
