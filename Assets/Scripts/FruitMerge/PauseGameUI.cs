using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PauseGameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeCurrentText;
    [SerializeField] private TextMeshProUGUI sfxVolumeCurrentText;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
            sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
    
            UpdateMusicVolumeText(musicVolumeSlider.value);
            UpdateSFXVolumeText(sfxVolumeSlider.value);
        }
    
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    private void UpdateMusicVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        musicVolumeCurrentText.text = $"{percent}%";
    }
    
    private void UpdateSFXVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        sfxVolumeCurrentText.text = $"{percent}%";
    }
    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        UpdateMusicVolumeText(value);
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        UpdateSFXVolumeText(value);
    }

    private void OnDestroy()
    {
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

}