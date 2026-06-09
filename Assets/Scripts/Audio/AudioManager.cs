using System;
using System.Reflection;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    private Type audioClipType;
    private Type audioSourceType;
    private Component bgmSource;
    private Component sfxSource;
    private object bgmClip;
    private object buttonClip;
    private object careClip;
    private object errorClip;
    private object evolutionClip;
    private object rewardClip;
    private object snackClip;
    private MethodInfo playMethod;
    private MethodInfo playOneShotMethod;

    private void Awake()
    {
        if (!TryInitializeAudioTypes())
        {
            return;
        }

        bgmSource = gameObject.AddComponent(audioSourceType);
        sfxSource = gameObject.AddComponent(audioSourceType);
        SetProperty(bgmSource, "loop", true);
        SetProperty(bgmSource, "volume", 0.14f);
        SetProperty(sfxSource, "volume", 0.55f);

        bgmClip = CreateBgmClip();
        buttonClip = CreateToneClip("ButtonClick", 660f, 0.05f, 0.22f, WaveType.Sine);
        careClip = CreateArpeggioClip("Care", new[] { 523f, 659f, 784f }, 0.24f, 0.28f);
        errorClip = CreateArpeggioClip("Error", new[] { 240f, 190f }, 0.24f, 0.24f);
        evolutionClip = CreateArpeggioClip("Evolution", new[] { 523f, 659f, 784f, 1046f }, 0.55f, 0.35f);
        rewardClip = CreateArpeggioClip("Reward", new[] { 784f, 988f, 1175f }, 0.32f, 0.32f);
        snackClip = CreateToneClip("Snack", 880f, 0.08f, 0.28f, WaveType.Triangle);

        PlayBgm();
    }

    public void PlayBgm()
    {
        if (bgmSource == null || bgmClip == null)
        {
            return;
        }

        bool isPlaying = GetProperty<bool>(bgmSource, "isPlaying");
        if (isPlaying)
        {
            return;
        }

        SetProperty(bgmSource, "clip", bgmClip);
        playMethod?.Invoke(bgmSource, null);
    }

    public void PlayButtonClick()
    {
        PlaySfx(buttonClip);
    }

    public void PlayCareSuccess()
    {
        PlaySfx(careClip);
    }

    public void PlayError()
    {
        PlaySfx(errorClip);
    }

    public void PlayEvolution()
    {
        PlaySfx(evolutionClip);
    }

    public void PlayReward()
    {
        PlaySfx(rewardClip);
    }

    public void PlaySnack()
    {
        PlaySfx(snackClip);
    }

    private bool TryInitializeAudioTypes()
    {
        audioClipType = Type.GetType("UnityEngine.AudioClip, UnityEngine.AudioModule");
        audioSourceType = Type.GetType("UnityEngine.AudioSource, UnityEngine.AudioModule");
        if (audioClipType == null || audioSourceType == null)
        {
            Debug.LogWarning("Audio module is disabled. PUNI Life will run without sound until the Audio built-in package is enabled.");
            return false;
        }

        playMethod = audioSourceType.GetMethod("Play", Type.EmptyTypes);
        playOneShotMethod = audioSourceType.GetMethod("PlayOneShot", new[] { audioClipType });
        return playMethod != null && playOneShotMethod != null;
    }

    private void PlaySfx(object clip)
    {
        if (sfxSource != null && clip != null)
        {
            playOneShotMethod?.Invoke(sfxSource, new[] { clip });
        }
    }

    private object CreateBgmClip()
    {
        const int sampleRate = 44100;
        const float duration = 8f;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        var samples = new float[sampleCount];
        float[] notes = { 261.63f, 329.63f, 392f, 523.25f, 392f, 329.63f, 293.66f, 349.23f };

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            int noteIndex = Mathf.FloorToInt(time * 2f) % notes.Length;
            float frequency = notes[noteIndex];
            float melody = Mathf.Sin(Mathf.PI * 2f * frequency * time) * 0.09f;
            float harmony = Mathf.Sin(Mathf.PI * 2f * frequency * 0.5f * time) * 0.045f;
            float pulse = 0.72f + Mathf.Sin(Mathf.PI * 2f * 0.5f * time) * 0.18f;
            samples[i] = (melody + harmony) * pulse;
        }

        return CreateClip("PUNI_Code_BGM", samples, sampleRate);
    }

    private object CreateArpeggioClip(string name, float[] frequencies, float duration, float volume)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        var samples = new float[sampleCount];
        float segment = duration / frequencies.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            int noteIndex = Mathf.Min(frequencies.Length - 1, Mathf.FloorToInt(time / segment));
            float localTime = time - noteIndex * segment;
            float envelope = 1f - Mathf.Clamp01(localTime / segment);
            samples[i] = Mathf.Sin(Mathf.PI * 2f * frequencies[noteIndex] * time) * volume * envelope;
        }

        return CreateClip($"PUNI_{name}", samples, sampleRate);
    }

    private object CreateToneClip(string name, float frequency, float duration, float volume, WaveType waveType)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        var samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float t = frequency * time;
            float wave = waveType == WaveType.Triangle
                ? 2f * Mathf.Abs(2f * (t - Mathf.Floor(t + 0.5f))) - 1f
                : Mathf.Sin(Mathf.PI * 2f * t);
            float envelope = 1f - Mathf.Clamp01(time / duration);
            samples[i] = wave * volume * envelope;
        }

        return CreateClip($"PUNI_{name}", samples, sampleRate);
    }

    private object CreateClip(string clipName, float[] samples, int sampleRate)
    {
        MethodInfo createMethod = audioClipType.GetMethod(
            "Create",
            new[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(bool) });
        object clip = createMethod?.Invoke(null, new object[] { clipName, samples.Length, 1, sampleRate, false });
        MethodInfo setDataMethod = audioClipType.GetMethod("SetData", new[] { typeof(float[]), typeof(int) });
        setDataMethod?.Invoke(clip, new object[] { samples, 0 });
        return clip;
    }

    private static void SetProperty(object target, string propertyName, object value)
    {
        target?.GetType().GetProperty(propertyName)?.SetValue(target, value);
    }

    private static T GetProperty<T>(object target, string propertyName)
    {
        object value = target?.GetType().GetProperty(propertyName)?.GetValue(target);
        return value is T typedValue ? typedValue : default;
    }

    private enum WaveType
    {
        Sine,
        Triangle
    }
}
