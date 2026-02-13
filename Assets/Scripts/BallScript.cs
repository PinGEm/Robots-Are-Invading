using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private int _ballID;
    [SerializeField] private int _ballPoints = 1;
    private ScoreUI _scoreUI;
    bool _dead = false;

    private void Start()
    {
        _scoreUI = GameObject.FindWithTag("UI Manager").GetComponent<ScoreUI>();

        // Check if this game object should be removed
        if (SaveSystem.Instance._ballsCollected.Contains(_ballID))
        {
            _dead = true;
            this.transform.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!SaveSystem.Instance._ballsCollected.Contains(_ballID) && _dead)
        {
            Debug.Log("Respawning Object");
            _dead = false;
            this.transform.localScale = Vector3.one;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_dead)
        {

            SaveSystem.Instance._ballsCollected.Add(_ballID);
            _scoreUI.ScoreAdd(_ballPoints);
            this.transform.localScale = Vector3.zero;
            _dead = true;
        }
    }
}
