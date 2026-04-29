using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _optionsMenu;
    
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _controlsMenu;

    public void EnterGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        _mainMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void LeaveOptions()
    {
        _mainMenu.SetActive(true);
        _optionsMenu.SetActive(false);
    }

    public void ControlOptions()
    {
         _controlsMenu.SetActive(true);
        _optionsMenu.SetActive(false);
    }
    
    public void ExitControlOptions()
    {
        _controlsMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
