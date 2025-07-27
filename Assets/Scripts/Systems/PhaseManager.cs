using UnityEngine;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    [Header("Duración de Subfases (segundos)")]
    public float[] subPhaseDurations = { 30f, 30f, 30f };

    [Header("Cantidad de enemigos por subfase")]
    public int[] basicEnemiesPerSubPhase = { 5, 5, 5 };
    public int[] mediumEnemiesPerSubPhase = { 1, 2, 4 };

    [Header("Spawners y Prefabs")]
    public GameObject[] basicEnemies;
    public GameObject[] mediumEnemies; // IceTank, ThunderArcher, CloudSpeedster
    public Transform[] spawnTiles;

    [Header("Intervalos entre spawns")]
    public float basicSpawnInterval = 1.0f;
    public float mediumSpawnInterval = 2.0f;

    [Header("Iluminación")]
    public Light directionalLight;
    public Color[] lightingColors;

    private int currentSubPhase = 0;
    private float subPhaseTimer = 0f;

    void Start()
    {
        StartSubPhase(currentSubPhase);
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
                enabled = false;
            }
        }
    }

    void StartSubPhase(int index)
    {
        subPhaseTimer = 0f;

        Debug.Log($"Iniciando Subfase {index + 1}");

        if (directionalLight != null && lightingColors.Length > index)
        {
            directionalLight.color = lightingColors[index];
        }

        StartCoroutine(SpawnEnemiesOverTime(index));
    }

    IEnumerator SpawnEnemiesOverTime(int index)
    {
        int basicCount = basicEnemiesPerSubPhase[index];
        int mediumCount = mediumEnemiesPerSubPhase[index];

        StartCoroutine(SpawnBasicEnemies(basicCount));
        StartCoroutine(SpawnMediumEnemies(mediumCount));

        yield break;
    }

    IEnumerator SpawnBasicEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBasicEnemy(i);
            yield return new WaitForSeconds(basicSpawnInterval);
        }
    }

    IEnumerator SpawnMediumEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnMediumEnemy(i);
            yield return new WaitForSeconds(mediumSpawnInterval);
        }
    }

    void SpawnBasicEnemy(int index)
    {
        if (basicEnemies.Length == 0 || spawnTiles.Length == 0) return;

        GameObject prefab = basicEnemies[Random.Range(0, basicEnemies.Length)];
        Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)];

        if (prefab != null && tile != null)
        {
            Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
            Debug.Log($"[BasicEnemy] Spawned basic enemy {index + 1}");
        }
        else
        {
            Debug.LogWarning("[BasicEnemy] Prefab o tile nulo.");
        }
    }

    void SpawnMediumEnemy(int index)
    {
        if (mediumEnemies.Length == 0 || spawnTiles.Length == 0) return;

        GameObject prefab = mediumEnemies[Random.Range(0, mediumEnemies.Length)];
        Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)];

        if (prefab != null && tile != null)
        {
            GameObject spawned = Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
            Debug.Log($"[MediumEnemy] Spawned medium enemy {index + 1} at tile {tile.name}");

            if (spawned == null)
                Debug.LogError($"[MediumEnemy] Instantiate falló en el enemigo {index + 1}");
        }
        else
        {
            Debug.LogWarning("[MediumEnemy] Prefab o tile nulo.");
        }
    }

    void SpawnBossWave()
    {
        Debug.Log("¡Boss de Fase 1 activado!");

        for (int i = 0; i < 5; i++)
        {
            GameObject prefab = mediumEnemies[Random.Range(0, mediumEnemies.Length)];
            Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)];

            if (prefab != null && tile != null)
            {
                Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
                Debug.Log($"[Boss] Spawned boss enemy {i + 1}");
            }
        }
    }
}
