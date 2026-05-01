using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnDelay = 2f;
    public int maxEnemies = 5;

    [Header("Path Settings")]
    public EnemyPath path;

    private int currentEnemies;

    private void Start()
    {
        // Find path if not assigned
        if (path == null)
        {
            path = FindAnyObjectByType<EnemyPath>(); // Initiates 'SinglePath' value
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyObj = ObjectPoolManager.SpawnObject(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetPath(path);
        }

        currentEnemies++;
    }
}

