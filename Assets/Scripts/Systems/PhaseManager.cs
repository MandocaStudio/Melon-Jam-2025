using System.Collections;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [Header("Duración de Subfases")]
    public float[] subPhaseDurations = { 30f, 30f, 30f }; // Subfases

    [Header("Prefabs de Enemigos")]
    public GameObject[] basicEnemies;
    public GameObject[] mediumEnemies; // IceTank, ThunderArcher, CloudSpeedster

    [Header("Spawn")]
    public Transform[] spawnTiles; // 5 posiciones
    public float basicSpawnInterval = 2f;
    public float mediumSpawnInterval = 3f;

    [Header("Iluminación")]
    public Light directionalLight;
    public Color[] lightingColors;

    [Header("Cantidad de Enemigos por Subfase")]
    public int[] mediumEnemiesPerSubPhase = { 1, 2, 4 };
    public int[] basicEnemiesPerSubPhase = { 4, 4, 4 };

    private int currentSubPhase = 0;
    private float subPhaseTimer = 0f;
    private Coroutine enemySpawnRoutine;

    void Start()
    {
        StartSubPhase(0);
    }

    void Update()
    {
        subPhaseTimer += Time.deltaTime;

        if (subPhaseTimer >= subPhaseDurations[currentSubPhase])
        {
            currentSubPhase++;
            if (currentSubPhase < subPhaseDurations.Length)
            {
                StartSubPhase(currentSubPhase);
            }
            else
            {
                SpawnBossWave();
                if (enemySpawnRoutine != null) StopCoroutine(enemySpawnRoutine);
                enabled = false; // Desactiva este PhaseManager
            }
        }
    }

    void StartSubPhase(int index)
    {
        subPhaseTimer = 0f;

        // Iluminación
        if (directionalLight != null && lightingColors.Length > index)
            directionalLight.color = lightingColors[index];

        if (enemySpawnRoutine != null) StopCoroutine(enemySpawnRoutine);
        enemySpawnRoutine = StartCoroutine(SpawnEnemiesOverTime(index));
    }

    IEnumerator SpawnEnemiesOverTime(int index)
    {
        int basicCount = basicEnemiesPerSubPhase[index];
        int mediumCount = mediumEnemiesPerSubPhase[index];

        int basicSpawned = 0;
        int mediumSpawned = 0;

        while (basicSpawned < basicCount || mediumSpawned < mediumCount)
        {
            if (basicSpawned < basicCount)
            {
                SpawnBasicEnemy();
                basicSpawned++;
                yield return new WaitForSeconds(basicSpawnInterval);
            }

            if (mediumSpawned < mediumCount)
            {
                SpawnMediumEnemy();
                mediumSpawned++;
                yield return new WaitForSeconds(mediumSpawnInterval);
            }
        }
    }

    void SpawnBasicEnemy()
    {
        if (basicEnemies.Length == 0) return;

        GameObject prefab = basicEnemies[Random.Range(0, basicEnemies.Length)];
        if (prefab == null) return;

        Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)];
        Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
    }

    void SpawnMediumEnemy()
    {
        if (mediumEnemies.Length == 0) return;

        GameObject prefab = mediumEnemies[Random.Range(0, mediumEnemies.Length)];
        if (prefab == null) return;

        Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)];
        Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
    }

    void SpawnBossWave()
    {
        Debug.Log("¡Spawn de Boss de Fase 1!");

        for (int i = 0; i < 5; i++)
        {
            Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)];
            GameObject boss = mediumEnemies[Random.Range(0, mediumEnemies.Length)];
            if (boss != null)
                Instantiate(boss, tile.position, Quaternion.Euler(60f, 0f, 0f));
        }
    }
}
