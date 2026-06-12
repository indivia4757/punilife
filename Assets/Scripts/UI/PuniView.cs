using UnityEngine;
using UnityEngine.UI;

public sealed class PuniView
{
    private readonly RectTransform shadowRect;
    private readonly RectTransform rootRect;
    private readonly Image shadow;
    private readonly Image body;
    private readonly Image bodyHighlight;
    private readonly Image lowerShade;
    private readonly Image shell;
    private readonly Image leftEye;
    private readonly Image rightEye;
    private readonly Image leftEyeHighlight;
    private readonly Image rightEyeHighlight;
    private readonly Image mouth;
    private readonly Image leftCheek;
    private readonly Image rightCheek;
    private readonly Image sproutLeft;
    private readonly Image sproutRight;
    private readonly Image leftWing;
    private readonly Image rightWing;
    private readonly Image[] spots;
    private readonly Text stageText;
    private readonly Text actionMarkText;
    private readonly Sprite circleSprite;
    private readonly Vector2 basePosition = new Vector2(0f, -34f);
    private Vector2 highlightBasePosition = new Vector2(-42f, 58f);
    private readonly Vector2 lowerShadeBasePosition = new Vector2(18f, -58f);
    private PuniStatus currentStatus = new PuniStatus();
    private Vector3 baseScale = Vector3.one;
    private CareActionType reactionAction;
    private float reactionTimer;
    private bool hasReaction;
    private const float ReactionDuration = 1.05f;

    public PuniView(Transform parent)
    {
        circleSprite = CreateCircleSprite();
        var shadowObject = new GameObject("PuniShadow");
        shadowObject.transform.SetParent(parent, false);
        shadowRect = shadowObject.AddComponent<RectTransform>();
        shadowRect.anchorMin = new Vector2(0.5f, 0.5f);
        shadowRect.anchorMax = new Vector2(0.5f, 0.5f);
        shadowRect.pivot = new Vector2(0.5f, 0.5f);
        shadowRect.anchoredPosition = basePosition + new Vector2(0f, -126f);
        shadowRect.sizeDelta = new Vector2(210f, 46f);
        shadow = shadowObject.AddComponent<Image>();
        shadow.sprite = circleSprite;
        shadow.color = new Color(0.35f, 0.30f, 0.22f, 0.18f);
        shadow.raycastTarget = false;

        var root = new GameObject("PuniView");
        root.transform.SetParent(parent, false);
        rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = basePosition;
        rootRect.sizeDelta = new Vector2(250f, 270f);

        body = root.AddComponent<Image>();
        body.sprite = circleSprite;
        body.material = null;
        body.raycastTarget = false;

        lowerShade = CreateShape(root.transform, "LowerShade", new Vector2(18f, -58f), new Vector2(170f, 95f), new Color(0.86f, 0.62f, 0.30f, 0.13f), -4f);
        bodyHighlight = CreateShape(root.transform, "BodyHighlight", new Vector2(-42f, 58f), new Vector2(88f, 56f), new Color(1f, 1f, 1f, 0.33f), -22f);

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
        leftEyeHighlight = CreateShape(leftEye.transform, "LeftEyeHighlight", new Vector2(8f, 12f), new Vector2(11f, 14f), Color.white, 0f);
        rightEyeHighlight = CreateShape(rightEye.transform, "RightEyeHighlight", new Vector2(8f, 12f), new Vector2(11f, 14f), Color.white, 0f);
        mouth = CreateShape(root.transform, "Mouth", new Vector2(0f, -20f), new Vector2(34f, 26f), new Color(0.72f, 0.12f, 0.07f), 0f);
        leftCheek = CreateShape(root.transform, "LeftCheek", new Vector2(-78f, -12f), new Vector2(32f, 22f), new Color(1f, 0.67f, 0.58f, 0.72f), 0f);
        rightCheek = CreateShape(root.transform, "RightCheek", new Vector2(78f, -12f), new Vector2(32f, 22f), new Color(1f, 0.67f, 0.58f, 0.72f), 0f);

        stageText = CreateText(root.transform, "Stage", TextAnchor.MiddleCenter, 22, new Vector2(0f, -150f), new Vector2(320f, 42f));
        stageText.raycastTarget = false;
        actionMarkText = CreateText(root.transform, "ActionMark", TextAnchor.MiddleCenter, 28, new Vector2(0f, 114f), new Vector2(180f, 48f));
        actionMarkText.color = new Color(0.72f, 0.45f, 0.18f);
        actionMarkText.gameObject.SetActive(false);
    }

    public void Refresh(SaveData data)
    {
        currentStatus = data.status;
        bool isEgg = data.stage == PuniStage.Egg;
        body.color = GetBodyColor(data);
        body.rectTransform.sizeDelta = isEgg ? new Vector2(150f, 205f) : new Vector2(214f, 225f);
        bodyHighlight.rectTransform.sizeDelta = isEgg ? new Vector2(58f, 48f) : new Vector2(88f, 56f);
        highlightBasePosition = isEgg ? new Vector2(-24f, 44f) : new Vector2(-42f, 58f);
        bodyHighlight.rectTransform.anchoredPosition = highlightBasePosition;
        lowerShade.gameObject.SetActive(!isEgg);
        shell.gameObject.SetActive(!isEgg);
        leftWing.gameObject.SetActive(!isEgg);
        rightWing.gameObject.SetActive(!isEgg && data.stage != PuniStage.Baby);
        sproutLeft.gameObject.SetActive(!isEgg);
        sproutRight.gameObject.SetActive(!isEgg);
        leftEye.gameObject.SetActive(!isEgg);
        rightEye.gameObject.SetActive(!isEgg);
        leftEyeHighlight.gameObject.SetActive(!isEgg);
        rightEyeHighlight.gameObject.SetActive(!isEgg);
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
        baseScale = GetScale(data.stage);
        stageText.text = data.stage == PuniStage.Evolved ? PuniText.EvolutionName(data.evolutionType) : PuniText.StageName(data.stage);
    }

    public void Tick(float time, float deltaTime)
    {
        float idleY = Mathf.Sin(time * 2.1f) * 9f;
        float idleX = Mathf.Sin(time * 1.25f) * 3.5f;
        float reactionProgress = 0f;
        Vector2 reactionOffset = Vector2.zero;
        Vector3 reactionScale = Vector3.one;
        float rotation = Mathf.Sin(time * 1.8f) * 2.6f;
        Vector3 breathScale = new Vector3(1f + Mathf.Sin(time * 2.0f) * 0.025f, 1f + Mathf.Cos(time * 2.0f) * 0.018f, 1f);

        if (hasReaction)
        {
            reactionTimer = Mathf.Max(0f, reactionTimer - deltaTime);
            reactionProgress = 1f - reactionTimer / ReactionDuration;
            ApplyReaction(reactionProgress, ref reactionOffset, ref reactionScale, ref rotation);
            if (reactionTimer <= 0f)
            {
                hasReaction = false;
                actionMarkText.gameObject.SetActive(false);
            }
        }

        rootRect.anchoredPosition = basePosition + new Vector2(idleX, idleY) + reactionOffset;
        rootRect.localScale = Vector3.Scale(baseScale, Vector3.Scale(reactionScale, breathScale));
        rootRect.localRotation = Quaternion.Euler(0f, 0f, rotation);
        shadowRect.anchoredPosition = basePosition + new Vector2(idleX * 0.35f, -126f);
        float shadowSquash = Mathf.Clamp01(1f - (idleY + reactionOffset.y) / 120f);
        shadowRect.localScale = new Vector3(baseScale.x * (0.95f + shadowSquash * 0.12f), baseScale.y * (0.86f - shadowSquash * 0.08f), 1f);
        shadow.color = new Color(0.35f, 0.30f, 0.22f, 0.14f + shadowSquash * 0.07f);

        float wingFlap = Mathf.Sin(time * 6.5f) * 13f;
        leftWing.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 28f + wingFlap);
        rightWing.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -28f - wingFlap);
        sproutLeft.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -32f + Mathf.Sin(time * 3.2f) * 8f);
        sproutRight.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 38f + Mathf.Sin(time * 3.1f + 0.8f) * 8f);
        bodyHighlight.rectTransform.anchoredPosition = highlightBasePosition + new Vector2(Mathf.Sin(time * 1.4f) * 2.8f, Mathf.Sin(time * 1.8f) * 2.2f);
        lowerShade.rectTransform.anchoredPosition = lowerShadeBasePosition + new Vector2(Mathf.Sin(time * 1.1f) * 2.4f, 0f);

        if (hasReaction && leftEye.gameObject.activeSelf)
        {
            ApplyReactionFace(reactionProgress);
        }
        else
        {
            actionMarkText.gameObject.SetActive(false);
            ApplyMood(currentStatus);
            ApplyBlink(time);
        }
    }

    public void PlayReaction(CareActionType action)
    {
        reactionAction = action;
        reactionTimer = ReactionDuration;
        hasReaction = true;
    }

    public void PlayCelebrate()
    {
        reactionAction = CareActionType.Play;
        reactionTimer = ReactionDuration;
        hasReaction = true;
    }

    private void ApplyReaction(float progress, ref Vector2 offset, ref Vector3 scale, ref float rotation)
    {
        float pulse = Mathf.Sin(progress * Mathf.PI);
        float fastPulse = Mathf.Sin(progress * Mathf.PI * 4f);

        switch (reactionAction)
        {
            case CareActionType.Feed:
                offset.y += pulse * 14f;
                offset.x += Mathf.Sin(progress * Mathf.PI * 6f) * 5f * pulse;
                scale = new Vector3(1f + pulse * 0.20f, 1f - pulse * 0.12f, 1f);
                break;
            case CareActionType.Play:
                offset.y += Mathf.Abs(fastPulse) * 52f;
                offset.x += Mathf.Sin(progress * Mathf.PI * 4f) * 18f * pulse;
                rotation += fastPulse * 15f;
                scale = Vector3.one * (1f + pulse * 0.12f);
                break;
            case CareActionType.Clean:
                offset.x += Mathf.Sin(progress * Mathf.PI * 10f) * 18f * pulse;
                offset.y += Mathf.Abs(Mathf.Sin(progress * Mathf.PI * 5f)) * 10f * pulse;
                rotation += Mathf.Sin(progress * Mathf.PI * 8f) * 10f * pulse;
                scale = Vector3.one * (1f + pulse * 0.08f);
                break;
            case CareActionType.Sleep:
                offset.y -= pulse * 22f;
                rotation -= pulse * 12f;
                scale = new Vector3(1f + pulse * 0.12f, 1f - pulse * 0.13f, 1f);
                break;
            case CareActionType.Study:
                offset.y += Mathf.Sin(progress * Mathf.PI * 3f) * 8f * pulse;
                rotation += Mathf.Sin(progress * Mathf.PI * 5f) * 9f * pulse;
                break;
            case CareActionType.Train:
                offset.y += Mathf.Abs(fastPulse) * 22f;
                offset.x += Mathf.Sin(progress * Mathf.PI * 8f) * 14f * pulse;
                scale = new Vector3(1f + pulse * 0.22f, 1f + pulse * 0.10f, 1f);
                rotation += Mathf.Sin(progress * Mathf.PI * 6f) * 10f * pulse;
                break;
        }
    }

    private void ApplyReactionFace(float progress)
    {
        float pulse = Mathf.Sin(progress * Mathf.PI);
        actionMarkText.gameObject.SetActive(true);
        actionMarkText.rectTransform.anchoredPosition = new Vector2(0f, 116f + pulse * 18f);
        actionMarkText.color = new Color(0.72f, 0.45f, 0.18f, 0.25f + pulse * 0.75f);
        actionMarkText.rectTransform.localScale = Vector3.one * (1f + pulse * 0.18f);

        leftEye.color = new Color(0.12f, 0.08f, 0.06f);
        rightEye.color = leftEye.color;
        leftCheek.color = new Color(1f, 0.67f, 0.58f, 0.72f + pulse * 0.18f);
        rightCheek.color = leftCheek.color;
        leftCheek.rectTransform.sizeDelta = new Vector2(32f + pulse * 10f, 22f + pulse * 6f);
        rightCheek.rectTransform.sizeDelta = leftCheek.rectTransform.sizeDelta;

        switch (reactionAction)
        {
            case CareActionType.Feed:
                actionMarkText.text = "냠냠";
                SetEyes(new Vector2(28f, 12f), new Vector2(-46f, 28f), new Vector2(46f, 28f), -8f, 8f);
                SetMouth(new Vector2(42f + pulse * 10f, 30f + pulse * 8f), new Vector2(0f, -22f), new Color(0.78f, 0.16f, 0.08f));
                break;
            case CareActionType.Play:
                actionMarkText.text = "♪";
                SetEyes(new Vector2(38f + pulse * 6f, 54f + pulse * 5f), new Vector2(-48f, 24f), new Vector2(48f, 24f), 0f, 0f);
                SetMouth(new Vector2(48f + pulse * 12f, 38f + pulse * 8f), new Vector2(0f, -24f), new Color(0.78f, 0.12f, 0.07f));
                break;
            case CareActionType.Clean:
                actionMarkText.text = "반짝";
                SetEyes(new Vector2(34f, 42f), new Vector2(-46f, 24f), new Vector2(46f, 24f), -3f, 3f);
                SetMouth(new Vector2(34f, 16f), new Vector2(0f, -22f), new Color(0.78f, 0.16f, 0.08f));
                break;
            case CareActionType.Sleep:
                actionMarkText.text = "Zz";
                SetEyes(new Vector2(34f, 7f), new Vector2(-46f, 27f), new Vector2(46f, 27f), 0f, 0f);
                SetMouth(new Vector2(20f + pulse * 6f, 18f), new Vector2(0f, -20f), new Color(0.18f, 0.20f, 0.24f));
                break;
            case CareActionType.Study:
                actionMarkText.text = "?";
                SetEyes(new Vector2(24f, 38f), new Vector2(-42f, 24f), new Vector2(42f, 24f), 0f, 0f);
                SetMouth(new Vector2(24f, 8f), new Vector2(0f, -20f), new Color(0.18f, 0.20f, 0.24f));
                break;
            case CareActionType.Train:
                actionMarkText.text = "!";
                SetEyes(new Vector2(38f, 18f), new Vector2(-46f, 28f), new Vector2(46f, 28f), 14f, -14f);
                SetMouth(new Vector2(34f, 10f), new Vector2(0f, -20f), new Color(0.18f, 0.20f, 0.24f));
                break;
        }
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
        if (status == null)
        {
            status = currentStatus ?? new PuniStatus();
        }

        ResetFace();
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

    private void ResetFace()
    {
        SetEyes(new Vector2(34f, 48f), new Vector2(-46f, 22f), new Vector2(46f, 22f), 0f, 0f);
        SetMouth(new Vector2(34f, 26f), new Vector2(0f, -20f), new Color(0.72f, 0.12f, 0.07f));
        leftCheek.rectTransform.sizeDelta = new Vector2(32f, 22f);
        rightCheek.rectTransform.sizeDelta = new Vector2(32f, 22f);
        leftCheek.color = new Color(1f, 0.67f, 0.58f, 0.72f);
        rightCheek.color = leftCheek.color;
    }

    private void SetEyes(Vector2 size, Vector2 leftPosition, Vector2 rightPosition, float leftRotation, float rightRotation)
    {
        leftEye.rectTransform.sizeDelta = size;
        rightEye.rectTransform.sizeDelta = size;
        leftEye.rectTransform.anchoredPosition = leftPosition;
        rightEye.rectTransform.anchoredPosition = rightPosition;
        leftEye.rectTransform.localRotation = Quaternion.Euler(0f, 0f, leftRotation);
        rightEye.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rightRotation);
        bool showHighlights = size.y > 20f;
        leftEyeHighlight.gameObject.SetActive(leftEye.gameObject.activeSelf && showHighlights);
        rightEyeHighlight.gameObject.SetActive(rightEye.gameObject.activeSelf && showHighlights);
    }

    private void SetMouth(Vector2 size, Vector2 position, Color color)
    {
        mouth.rectTransform.sizeDelta = size;
        mouth.rectTransform.anchoredPosition = position;
        mouth.rectTransform.localRotation = Quaternion.identity;
        mouth.color = color;
    }

    private void ApplyBlink(float time)
    {
        if (!leftEye.gameObject.activeSelf || !rightEye.gameObject.activeSelf)
        {
            return;
        }

        float blinkCycle = Mathf.Repeat(time, 4.2f);
        bool blinking = blinkCycle > 3.95f;
        Vector2 eyeSize = blinking ? new Vector2(34f, 8f) : new Vector2(34f, 48f);
        leftEye.rectTransform.sizeDelta = eyeSize;
        rightEye.rectTransform.sizeDelta = eyeSize;
        leftEyeHighlight.gameObject.SetActive(!blinking);
        rightEyeHighlight.gameObject.SetActive(!blinking);
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
        image.raycastTarget = false;
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
        text.raycastTarget = false;
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
