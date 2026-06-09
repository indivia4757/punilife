using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioClip bgmClip;
    private AudioClip buttonClip;
    private AudioClip careClip;
    private AudioClip errorClip;
    private AudioClip evolutionClip;
    private AudioClip rewardClip;
    private AudioClip snackClip;

    private void Awake()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.14f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 0.55f;

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
        if (bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.Play();
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

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private static AudioClip CreateBgmClip()
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

        var clip = AudioClip.Create("PUNI_Code_BGM", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateArpeggioClip(string name, float[] frequencies, float duration, float volume)
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

        var clip = AudioClip.Create($"PUNI_{name}", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateToneClip(string name, float frequency, float duration, float volume, WaveType waveType)
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

        var clip = AudioClip.Create($"PUNI_{name}", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private enum WaveType
    {
        Sine,
        Triangle
    }
}
