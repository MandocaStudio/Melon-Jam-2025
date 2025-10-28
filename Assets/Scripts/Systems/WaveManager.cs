// WaveSpawner.cs (antes PhaseManager.cs)
using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawners y Prefabs")]
    public GameObject[] basicEnemies;
    public GameObject[] mediumEnemies; 
    public Transform[] spawnTiles; // <-- ¡Tu lógica de spawn points se mantiene aquí!

    public void StartSpawning(SubPhaseData data)
    {
        Debug.Log($"WaveSpawner: Spawneando {data.basicEnemiesToSpawn} básicos y {data.mediumEnemiesToSpawn} medios.");
        StartCoroutine(SpawnEnemies(basicEnemies, data.basicEnemiesToSpawn, data.basicSpawnInterval));
        StartCoroutine(SpawnEnemies(mediumEnemies, data.mediumEnemiesToSpawn, data.mediumSpawnInterval));
    }

    IEnumerator SpawnEnemies(GameObject[] prefabArray, int count, float interval)
    {
        if (prefabArray.Length == 0 || spawnTiles.Length == 0)
        {
            Debug.LogWarning("WaveSpawner: Faltan prefabs de enemigos o spawn tiles.");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabArray[Random.Range(0, prefabArray.Length)];
            Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)]; // <-- Se usa aquí

            if (prefab != null && tile != null)
            {
                Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
            }

            yield return new WaitForSeconds(interval);
        }
    }
}