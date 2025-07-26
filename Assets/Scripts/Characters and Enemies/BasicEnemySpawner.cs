using UnityEngine;

public class BasicEnemySpawner : MonoBehaviour
{
    [Header("Prefab del enemigo (uno solo)")]
    public GameObject iceTankPrefab;  // Tanque de Hielo
    public GameObject speedsterPrefab;  // Velocista
    public GameObject shooterPrefab;  // Tirador
    public GameObject basicEnemyPrefab;  // Enemigos básicos

    [Header("Puntos de spawneo en columna 2 (Tile_2_0 a Tile_2_4)")]
    public Transform[] spawnPoints;

    [Header("Configuración de spawneo")]
    public float spawnInterval = 4f;
    private float timer = 0f;

    // Contadores de tanques de hielo, velocistas y tiradores
    public static int activeIceTanks = 0;
    public static int activeSpeedsters = 0;
    public static int activeShooters = 0;  // Solo 1 tirador

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
        if (spawnPoints.Length == 0 || (iceTankPrefab == null && speedsterPrefab == null && shooterPrefab == null && basicEnemyPrefab == null))
        {
            Debug.LogWarning("Faltan spawn points o prefabs asignados.");
            return;
        }

        // Escoge un punto de spawn aleatorio
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnLocation = spawnPoints[index];

        // Si no hay un tirador activo, crea un tirador
        if (activeShooters < 1)
        {
            Instantiate(shooterPrefab, spawnLocation.position, Quaternion.identity);
            activeShooters++;
        }
        else if (activeSpeedsters < 2)
        {
            Instantiate(speedsterPrefab, spawnLocation.position, Quaternion.identity);
            activeSpeedsters++;
        }
        else if (activeIceTanks < 2)
        {
            Instantiate(iceTankPrefab, spawnLocation.position, Quaternion.identity);
            activeIceTanks++;
        }
        else
        {
            Instantiate(basicEnemyPrefab, spawnLocation.position, Quaternion.identity);
        }
    }
}
