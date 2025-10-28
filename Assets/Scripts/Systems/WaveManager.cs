// WaveSpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [Header("Referencias de Spawneo")]
    [Tooltip("Arrastra aquí TODOS tus 'spawn tiles'")]
    public Transform[] spawnTiles; 

    public void StartSpawning(SubPhaseData data)
    {
        // 1. Detenemos TODAS las corrutinas de la fase anterior
        StopAllCoroutines(); 
        
        Debug.Log($"WaveSpawner: Iniciando {data.spawnBursts.Count} ráfagas Y un flujo de {data.basicStreamCount} básicos / {data.mediumStreamCount} medios.");

        // 2. Iniciamos las corrutinas de RÁFAGA
        foreach (var burst in data.spawnBursts)
        {
            StartCoroutine(ExecuteBurst(burst));
        }

        // 3. [NUEVO] Iniciamos las corrutinas de FLUJO
        StartCoroutine(ExecuteStream(
            data.basicStreamCount, 
            data.basicStreamPrefabs, 
            data.duration
        ));
        
        StartCoroutine(ExecuteStream(
            data.mediumStreamCount, 
            data.mediumStreamPrefabs, 
            data.duration
        ));
    }

    // --- LÓGICA DE RÁFAGA (Sin cambios) ---
    private IEnumerator ExecuteBurst(SpawnBurst burst)
    {
        if (burst.prefabsToSpawn == null || burst.prefabsToSpawn.Count == 0)
        {
            Debug.LogWarning($"Ráfaga a los {burst.startTime}s no tiene prefabs asignados. Saltando.");
            yield break;
        }
        if (spawnTiles.Length == 0)
        {
            Debug.LogWarning("WaveSpawner: No hay 'spawnTiles' asignados.");
            yield break;
        }

        yield return new WaitForSeconds(burst.startTime);
        
        Debug.Log($"Iniciando ráfaga: {burst.count}x enemigos a los {burst.startTime} seg.");

        for (int i = 0; i < burst.count; i++)
        {
            SpawnOneEnemy(burst.prefabsToSpawn);
            yield return new WaitForSeconds(burst.interval);
        }
    }

    // --- [NUEVA] LÓGICA DE FLUJO (Enemigos "Normales") ---
    private IEnumerator ExecuteStream(int count, List<GameObject> prefabs, float phaseDuration)
    {
        if (count == 0 || prefabs == null || prefabs.Count == 0)
        {
            yield break; // No hay nada que spawnear en este flujo
        }
        if (spawnTiles.Length == 0)
        {
            Debug.LogWarning("WaveSpawner: No hay 'spawnTiles' asignados.");
            yield break;
        }

        // Calculamos el intervalo para distribuir los enemigos
        float interval = phaseDuration / count;

        for (int i = 0; i < count; i++)
        {
            // Esperamos el intervalo
            yield return new WaitForSeconds(interval);
            
            // Y spawneamos un enemigo
            SpawnOneEnemy(prefabs);
        }
    }

    // --- MÉTODO DE AYUDA (Sin cambios) ---
    void SpawnOneEnemy(List<GameObject> prefabList)
    {
        GameObject prefab = prefabList[Random.Range(0, prefabList.Count)];
        Transform tile = spawnTiles[Random.Range(0, spawnTiles.Length)]; 

        if (prefab != null && tile != null)
        {
            Instantiate(prefab, tile.position, Quaternion.Euler(60f, 0f, 0f));
        }
    }
}