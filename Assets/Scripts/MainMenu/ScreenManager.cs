using UnityEngine;
using TMPro; // We are using TextMeshPro for the dropdown!

public class FullscreenManager : MonoBehaviour
{
    public TMP_Dropdown modeDropdown;

    void Start()
    {
        // 1. Ask the computer what the player chose last time. Default is 0 (Fullscreen).
        int savedChoice = PlayerPrefs.GetInt("WindowMode", 0);

        // 2. If we are in the Menu, make the dropdown match the saved choice.
        if (modeDropdown != null)
        {
            modeDropdown.value = savedChoice;
        }

        // 3. Actually apply the window mode when the level loads.
        SetWindowMode(savedChoice);
    }

    // Your Dropdown will trigger this when clicked!
    public void SetWindowMode(int choice)
    {
        // Save the choice on our sticky note
        PlayerPrefs.SetInt("WindowMode", choice);

        // Apply the mode based on what they picked
        if (choice == 0)
        {
            // Fullscreen
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (choice == 1)
        {
            // Windowed Mode
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}