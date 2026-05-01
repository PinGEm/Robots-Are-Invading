using System.Collections;
using UnityEngine;

public class DestroyAfterTimer : MonoBehaviour
{
    private const float TIME_TO_DESPAWN = 1f;
    private Coroutine _timerCoroutine;

    private void OnEnable()
    {
        _timerCoroutine = StartCoroutine(ReturnToPoolAfterTime());
    }

    private IEnumerator ReturnToPoolAfterTime()
    {
        float elapsedTime = 0f;
        while(elapsedTime < TIME_TO_DESPAWN)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
