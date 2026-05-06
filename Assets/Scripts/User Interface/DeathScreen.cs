using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void RestartButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
