using TMPro;
using UnityEngine;

public class TemporaryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _staminaLabel;
    private PlayerContext _player;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _player = player.GetComponent<PlayerContext>();
    }

    void Update()
    {
        _staminaLabel.text = $"Stamina: {_player.GetStaminaCount}";
    }
}
