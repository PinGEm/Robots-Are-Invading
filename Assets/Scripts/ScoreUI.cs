using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _score = 0;

    public int GetScore()
    {
        return _score;
    }

    private void Start()
    {
        ResetScore();
        UpdateScore();
    }

    public void ScoreAdd(int score)
    {
        _score += score;
        UpdateScore();
    }

    private void UpdateScore()
    {
        _scoreText.text = _score.ToString();
    }

    public void ResetScore()
    {
        _score = SaveSystem.Instance._savedScore;
        UpdateScore();
    }
}
