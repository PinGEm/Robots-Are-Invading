using TMPro;
using UnityEngine;

public class TemporaryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _staminaLabel;
    [SerializeField] private TextMeshProUGUI _healthLabel;
    [SerializeField] private TextMeshProUGUI _ammoLabel;
    private PlayerContext _player;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _player = player.GetComponent<PlayerContext>();
    }

    void Update()
    {
        _staminaLabel.text = $"Stamina: {_player.GetStaminaCount}";
        _healthLabel.text = $"Health: {_player.GetCurrentPlayerHP}";
        _ammoLabel.text = $"{_player.GetCurrentWeapon.GetCurrentAmmo} / {_player.GetCurrentWeapon.GetMaxCapacity}";
    }
}
