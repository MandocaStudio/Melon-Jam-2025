using UnityEngine;

public class BasicEnemySpawner : MonoBehaviour
{
    [Header("Prefab del enemigo (uno solo)")]
    public GameObject enemyPrefab;

    [Header("Puntos de spawneo en columna 2 (Tile_2_0 a Tile_2_4)")]
    public Transform[] spawnPoints;

    [Header("Configuración de spawneo")]
    public float spawnInterval = 4f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            Debug.LogWarning("Faltan spawn points o prefab asignado.");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnLocation = spawnPoints[index];

        Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
    }
}
