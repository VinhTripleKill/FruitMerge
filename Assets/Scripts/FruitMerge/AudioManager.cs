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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========= GENERIC =========

    private void PlayUISound(AudioClip clip)
    {
        if (clip == null || uiSource == null)
            return;

        uiSource.PlayOneShot(clip);
    }

    private void PlayGameplaySound(AudioClip clip)
    {
        if (clip == null || gameplaySource == null)
            return;

        gameplaySource.PlayOneShot(clip);
    }
    // ========= MERGE =========

    public void PlayMergeSound()
    {
        PlayGameplaySound(soundMerge);
    }

    // ========= WIN =========

    public void PlayWinSound()
    {
        PlayUISound(winSound);
    }

    public void PlayLoseSound()
    {
        PlayUISound(loseSound);
    }

    // ========= CLICK =========
    public void PlayClickSound()
    {
        if (Time.unscaledTime - lastClickTime < 0.05f)
            return;

        lastClickTime = Time.unscaledTime;

        PlayUISound(clickSound);
    }
}