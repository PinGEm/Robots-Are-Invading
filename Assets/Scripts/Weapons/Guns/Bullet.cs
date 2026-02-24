using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with Game Object!");

        if (collision.gameObject.tag != "Player")
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
