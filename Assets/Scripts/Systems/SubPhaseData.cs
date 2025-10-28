// SubPhaseData.cs
using UnityEngine;
using System.Collections.Generic; // ¡Necesario para la Lista!

[CreateAssetMenu(fileName = "SubPhase_01", menuName = "Game/Sub-Phase Data")]
public class SubPhaseData : ScriptableObject
{
    [Header("Timing")]
    [Tooltip("Duración TOTAL de esta subfase en SEGUNDOS")]
    public float duration = 46.6f; 

    [Header("Lighting")]
    public Color directionalLightColor = Color.white;
    public float ambientLightIntensity = 1.0f;
    public float lightingTransitionTime = 5.0f;

    [Header("Burst Events (Ráfagas)")]
    [Tooltip("Lista de ráfagas específicas en momentos clave")]
    public List<SpawnBurst> spawnBursts;

    // -----------------------------------------------------------------
    // [NUEVO] Sección para los enemigos "normales" (flujo constante)
    // -----------------------------------------------------------------
    [Header("Streamed Spawns (Flujo)")]
    [Tooltip("Cuántos enemigos básicos saldrán distribuidos durante TODA la fase")]
    public int basicStreamCount = 0;
    [Tooltip("Qué prefabs usar para el flujo de básicos (elige uno al azar)")]
    public List<GameObject> basicStreamPrefabs;

    [Tooltip("Cuántos enemigos medios saldrán distribuidos durante TODA la fase")]
    public int mediumStreamCount = 0;
    [Tooltip("Qué prefabs usar para el flujo de medios (elige uno al azar)")]
    public List<GameObject> mediumStreamPrefabs;
}