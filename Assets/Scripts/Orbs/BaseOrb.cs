using UnityEngine;

public abstract class BaseOrb : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject _playerObject = collision.gameObject;

            PlayerContext _player = _playerObject.GetComponent<PlayerContext>();

            Debug.Log("Player has entered orb radius");

            PlayerEnterOrb(_player);
        }
    }

    protected abstract void PlayerEnterOrb(PlayerContext _player);
}
