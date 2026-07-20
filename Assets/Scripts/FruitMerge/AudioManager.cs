using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource gameplaySource;
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX")]
    [SerializeField] private AudioClip soundMerge;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip clickSound;

    private float lastClickTime;

    // ===== Volume =====
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolume();
        ApplyVolume();
    }

    // =======================
    // VOLUME
    // =======================

    private void LoadVolume()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    private void ApplyVolume()
    {
        if (bgmSource != null)
            bgmSource.volume = musicVolume;

        if (uiSource != null)
            uiSource.volume = sfxVolume;

        if (gameplaySource != null)
            gameplaySource.volume = sfxVolume;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);

        if (bgmSource != null)
            bgmSource.volume = musicVolume;

        PlayerPrefs.SetFloat(MUSIC_KEY, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);

        if (uiSource != null)
            uiSource.volume = sfxVolume;

        if (gameplaySource != null)
            gameplaySource.volume = sfxVolume;

        PlayerPrefs.SetFloat(SFX_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    // =======================
    // GENERIC
    // =======================

    private void PlayUISound(AudioClip clip)
    {
        if (clip == null || uiSource == null) return;

        uiSource.PlayOneShot(clip);
    }

    private void PlayGameplaySound(AudioClip clip)
    {
        if (clip == null || gameplaySource == null) return;

        gameplaySource.PlayOneShot(clip);
    }

    // =======================
    // MERGE
    // =======================

    public void PlayMergeSound()
    {
        PlayGameplaySound(soundMerge);
    }

    // =======================
    // WIN / LOSE
    // =======================

    public void PlayWinSound()
    {
        PlayUISound(winSound);
    }

    public void PlayLoseSound()
    {
        PlayUISound(loseSound);
    }

    // =======================
    // CLICK
    // =======================

    public void PlayClickSound()
    {
        if (Time.unscaledTime - lastClickTime < 0.05f) return;

        lastClickTime = Time.unscaledTime;

        PlayUISound(clickSound);
    }
}