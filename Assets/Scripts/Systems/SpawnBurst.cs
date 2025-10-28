// SpawnBurst.cs
using UnityEngine;
using System.Collections.Generic; // ¡Necesario para la lista!

[System.Serializable] 
public class SpawnBurst
{
    [Header("Timing")]
    [Tooltip("A qué hora (en segundos) debe empezar esta ráfaga")]
    public float startTime = 0.0f;

    [Header("Spawning")]
    [Tooltip("Cuántos enemigos saldrán en esta ráfaga")]
    public int count = 3;

    [Tooltip("Intervalo (en segundos) entre cada enemigo de ESTA ráfaga")]
    public float interval = 1.0f;

    // ESTA ES LA LÍNEA QUE FALTA EN TU PROYECTO
    [Tooltip("Los prefabs que pueden spawnear en esta ráfaga. Si hay más de 1, elegirá uno al azar.")]
    public List<GameObject> prefabsToSpawn; 
}