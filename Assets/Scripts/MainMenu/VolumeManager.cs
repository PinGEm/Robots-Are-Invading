using UnityEngine;
using UnityEngine.UI;

public class SimpleVolume : MonoBehaviour
{
    [Header("UI Setup")]
    public Slider volumeSlider;

    [Header("Audio Setup")]
    public AudioSource testSoundPlayer;

    [Tooltip("How many seconds to wait before the sound can play again while dragging")]
    public float soundDelay = 0.2f; // Adjust this in the Inspector!

    private float nextSoundTime = 0f; // A hidden timer to track the cooldown

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
    }

    public void SetVolume(float newVolume)
    {
        // 1. Change the game's global volume to match the slider
        AudioListener.volume = newVolume;
        PlayerPrefs.SetFloat("Volume", newVolume);

        // 2. The Audio Logic with a Cooldown Timer
        if (testSoundPlayer != null)
        {
            // Check if the current time in the game has passed our cooldown timer
            if (Time.time >= nextSoundTime)
            {
                testSoundPlayer.Play(); // Play the sound

                // Set the timer to the current time PLUS your delay
                nextSoundTime = Time.time + soundDelay;
            }
        }
    }
}