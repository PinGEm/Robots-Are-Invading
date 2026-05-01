using UnityEngine;
using UnityEngine.UI; // Because your hierarchy showed you are using the Legacy Dropdown

public class SimpleGraphics : MonoBehaviour
{
    [Header("UI Setup")]
    public Dropdown graphicsDropdown; // Leave empty if in gameplay scene!

    void Start()
    {
        // 1. Load the saved graphics level. 
        // We ask Unity what the current default quality is just in case it hasn't been saved yet.
        int defaultQuality = QualitySettings.GetQualityLevel();
        int savedGraphics = PlayerPrefs.GetInt("Graphics", defaultQuality);

        // 2. Apply that quality to the game immediately
        QualitySettings.SetQualityLevel(savedGraphics);

        // 3. If the dropdown exists in this scene, make it match the saved setting
        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = savedGraphics;
        }
    }

    // 4. Connect this to your Dropdown's OnValueChanged event!
    public void SetGraphicsQuality(int qualityIndex)
    {
        // Change Unity's global graphics quality
        QualitySettings.SetQualityLevel(qualityIndex);

        // Save the player's choice to the "sticky note"
        PlayerPrefs.SetInt("Graphics", qualityIndex);
        PlayerPrefs.Save();
    }
}