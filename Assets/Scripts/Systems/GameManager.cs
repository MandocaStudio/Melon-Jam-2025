// GameManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// --- LA LÍNEA CORREGIDA ---
public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public LightingController lightingController; 
    public WaveSpawner waveSpawner;             
    public TextMeshProUGUI timerText;         

    [Header("Configuración de Fases")]
    [Tooltip("Arrastra aquí tus 9 assets de SubPhaseData, en orden")]
    public List<SubPhaseData> allSubPhases;

    private int currentSubPhaseIndex = -1;
    private float phaseTimer = 0f;
    private float totalGameTime = 0f;

    void Start()
    {
        if (lightingController == null || waveSpawner == null)
        {
            Debug.LogError("¡Faltan referencias en el GameManager!");
            return;
        }
        StartNextSubPhase();
    }

    void Update()
    {
        phaseTimer -= Time.deltaTime;
        totalGameTime += Time.deltaTime;
        UpdateTimerUI(totalGameTime);

        if (phaseTimer <= 0)
        {
            StartNextSubPhase();
        }
    }

    void StartNextSubPhase()
    {
        currentSubPhaseIndex++;
        if (currentSubPhaseIndex >= allSubPhases.Count)
        {
            Debug.Log("¡JUEGO COMPLETADO!");
            enabled = false;
            return;
        }

        SubPhaseData data = allSubPhases[currentSubPhaseIndex];
        phaseTimer = data.duration;
        Debug.Log($"Iniciando Sub-Fase {currentSubPhaseIndex + 1} de {allSubPhases.Count}");

        waveSpawner.StartSpawning(data);
        lightingController.TransitionTo(data.directionalLightColor, 
                                        data.ambientLightIntensity, 
                                        data.lightingTransitionTime);
    }

    private void UpdateTimerUI(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}