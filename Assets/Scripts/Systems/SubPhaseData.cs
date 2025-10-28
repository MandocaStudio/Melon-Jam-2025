// SubPhaseData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "SubPhase_01", menuName = "Game/Sub-Phase Data")]
public class SubPhaseData : ScriptableObject
{
    [Header("Timing")]
    [Tooltip("Duración de esta subfase en SEGUNDOS")]
    public float duration = 46.6f; 

    [Header("Spawning")]
    public int basicEnemiesToSpawn = 5;
    public float basicSpawnInterval = 1.0f;
    public int mediumEnemiesToSpawn = 1;
    public float mediumSpawnInterval = 2.0f;

    [Header("Lighting")]
    [Tooltip("El color de la luz direccional al INICIAR esta fase")]
    public Color directionalLightColor = Color.white;
    [Tooltip("La intensidad de la luz ambiental al INICIAR esta fase")]
    public float ambientLightIntensity = 1.0f;
    [Tooltip("Tiempo (en segundos) que tardará la luz en transicionar")]
    public float lightingTransitionTime = 5.0f;
}