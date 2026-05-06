using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collided with Game Object! : {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");

            PlayerContext _player = collision.gameObject.GetComponent<PlayerContext>();

            _player.DamagePlayer(10);

            Debug.Log($"Player HP: {_player.GetCurrentPlayerHP}");

            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(StartDestroying());
        }
    }

    private IEnumerator StartDestroying()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
