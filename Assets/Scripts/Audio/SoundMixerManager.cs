using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    private void Start()
    {
        float savedMasterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        SetMasterVolume(savedMasterVolume);
        SetSFXVolume(savedSFXVolume);
        SetBGMVolume(savedMusicVolume);
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("masterVolume", volume);
        _audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);
        _audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
        _audioMixer.SetFloat("bgmVolume", Mathf.Log10(volume) * 20f);
    }
}
