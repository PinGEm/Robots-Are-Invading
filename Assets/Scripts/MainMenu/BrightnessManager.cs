using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    [Header("UI Setup")]
    public Image blackOverlay;
    public Slider brightnessSlider; // Will be empty in the gameplay scene

    void Start()
    {
        // 1. Load the saved brightness. Defaults to 1 (full brightness) if never saved before.
        float savedBrightness = PlayerPrefs.GetFloat("GameBrightness", 1f);
        void Start()
        {
            // 1. Load the saved brightness. Defaults to 1 (full brightness) if never saved before.
            float savedBrightness = PlayerPrefs.GetFloat("GameBrightness", 1f);

            Debug.Log("LEVEL LOADED! The saved brightness number is: " + savedBrightness);

            // 2. If we are in the Options menu (meaning a slider is attached), update the slider visually
            if (brightnessSlider != null)
            {
                brightnessSlider.value = savedBrightness;
            }

            // 3. Apply the darkness to the screen right away
            ApplyDarkness(savedBrightness);
        }
        // 2. If we are in the Options menu (meaning a slider is attached), update the slider visually
        if (brightnessSlider != null)
        {
            brightnessSlider.value = savedBrightness;
        }

        // 3. Apply the darkness to the screen right away
        ApplyDarkness(savedBrightness);
    }

    // This runs when you drag the "Brightness Scale" slider
    public void OnSliderMoved(float sliderValue)
    {
        ApplyDarkness(sliderValue);

        // Save the new value so the gameplay scene remembers it!
        PlayerPrefs.SetFloat("GameBrightness", sliderValue);
        PlayerPrefs.Save();
    }

    // The math to change how see-through the black screen is
    private void ApplyDarkness(float brightnessValue)
    {
        if (blackOverlay != null)
        {
            Color color = blackOverlay.color;
            color.a = 1f - brightnessValue;
            blackOverlay.color = color;
        }
    }

}