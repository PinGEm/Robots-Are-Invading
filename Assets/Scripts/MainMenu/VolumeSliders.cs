using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    [Header("UI Setup")]
    public Slider _masterVolumeSlider;
    public Slider _sfxVolumeSlider;
    public Slider _musicVolumeSlider;



    void Start()
    {
        float savedMasterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        _masterVolumeSlider.value = savedMasterVolume;
        _sfxVolumeSlider.value = savedSFXVolume;
        _musicVolumeSlider.value = savedMusicVolume;
    }
}