using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetBGMVolume(float volume)
    {
        _audioMixer.SetFloat("bgmVolume", Mathf.Log10(volume) * 20f);
    }
}
