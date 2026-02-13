using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private ScoreUI _scoreUI;
    private GameObject _player;
    private PlayerContext _playerContext;
    
    private void Start()
    {
        _player = GameObject.Find("Player");
        _playerContext = _player.GetComponent<PlayerContext>();
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _pauseUI.SetActive(true);
        Time.timeScale = 0;

        _playerContext.enabled = false;
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _pauseUI.SetActive(false);
        Time.timeScale = 1;

        _playerContext.enabled = true;
    }

    public void ResetSave()
    {
        Vector3 originalPosition = new Vector3(2.02f, 1.99f, 3.58291f);
        Quaternion originalRotation = Quaternion.identity;

        SaveSystem.Instance._ballsCollected.Clear();
        SaveSystem.Instance.Save(originalPosition, originalRotation, 0);
        CallLoad();
    }

    public void CallLoad()
    {
        SaveSystem.Instance.Load();
        _scoreUI.ResetScore();
    }
}
