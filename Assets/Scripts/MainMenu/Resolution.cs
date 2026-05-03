using UnityEngine;
using TMPro; // <-- THIS IS THE MAGIC WORD!

public class SuperSimpleResolution : MonoBehaviour
{
    // Notice it now says TMP_Dropdown instead of just Dropdown!
    public TMP_Dropdown resolutionDropdown;

    void Start()
    {
        // 1. Ask the computer what the player chose last time.
        int savedChoice = PlayerPrefs.GetInt("ResSettings", 0);

        // 2. If we are in the Main Menu, make the dropdown match their saved choice.
        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = savedChoice;
        }

        // 3. Actually change the screen size right when the level loads.
        SetResolution(savedChoice);
    }

    // Your Dropdown will trigger this exact list when clicked!
    public void SetResolution(int choice)
    {
        // ADD THIS LINE RIGHT HERE:
        Debug.Log("The player just clicked option number: " + choice);

        PlayerPrefs.SetInt("ResSettings", choice);
        // ... the rest of your if statements stay the same

        // Write their new choice on our "sticky note"
        PlayerPrefs.SetInt("ResSettings", choice);


        // Read down the list to apply the resolution
        if (choice == 0)
        {
            // 1920x1080
            Screen.SetResolution(1920, 1080, true);
        }
        else if (choice == 1)
        {
            // 1366x768
            Screen.SetResolution(1366, 768, true);
        }
        else if (choice == 2)
        {
            // 2560x1440
            Screen.SetResolution(2560, 1440, true);
        }
        else if (choice == 3)
        {
            // 3840x2160
            Screen.SetResolution(3840, 2160, true);
        }
    }
}