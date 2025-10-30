using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Necesario para el texto del temporizador

public class GameManager : MonoBehaviour
{
    // --- Contador de Enemigos (Paso 3) ---
    // Todos los enemigos se reportan a este contador
    public static int activeEnemies = 0; 

    [Header("Referencias")]
    public LightingController lightingController; 
    public WaveSpawner waveSpawner;             
    public TextMeshProUGUI timerText;         

    // --- [NUEVO] Lógica de Victoria/Derrota ---
    [Header("Lógica de Juego")]
    [Tooltip("El prefab del sprite/animación que quieres mostrar cuando el mago es derrotado")]
    public GameObject defeatMageSpritePrefab; 
    [Tooltip("Cuánto tiempo (seg) se muestra el sprite antes de cambiar de escena")]
    public float defeatAnimTime = 3f; 

    [Header("Configuración de Fases")]
    [Tooltip("Arrastra aquí tus 9 assets de SubPhaseData, en orden")]
    public List<SubPhaseData> allSubPhases;

    // --- Variables de Estado Internas ---
    private int currentSubPhaseIndex = -1;
    private float phaseTimer = 0f;
    private float totalGameTime = 0f;
    private bool allPhasesComplete = false; // ¿Terminaron las 9 oleadas?
    private bool gameIsOver = false;        // ¿Ya perdimos o ganamos?

    private void Start()
    {
        // 1. Reseteamos los contadores al iniciar el juego
        activeEnemies = 0;
        allPhasesComplete = false;
        gameIsOver = false;

        // 2. Verificamos que todo esté conectado
        if (lightingController == null || waveSpawner == null)
        {
            Debug.LogError("¡Faltan referencias (LightingController o WaveSpawner) en el GameManager!");
            return;
        }

        // 3. [NUEVO] Nos suscribimos a la "señal de radio" de la columna
        //    Cuando la columna dispare 'OnPlayerDefeated', llamará a nuestro método 'HandlePlayerDefeat'
        ColumnHealthBar.OnPlayerDefeated += HandlePlayerDefeat;

        // 4. Empezamos el juego
        StartNextSubPhase();
    }

    private void Update()
    {
        // Si el juego ya terminó (ganamos o perdimos), detenemos toda la lógica
        if (gameIsOver) return;

        // --- Lógica de Fases ---
        if (!allPhasesComplete)
        {
            phaseTimer -= Time.deltaTime;

            if (phaseTimer <= 0)
            {
                StartNextSubPhase();
            }
        }

        // --- Lógica del Temporizador ---
        totalGameTime += Time.deltaTime;
        UpdateTimerUI(totalGameTime);

        // --- [NUEVO] Lógica de Victoria ---
        // ¿Se completaron todas las fases? Y ¿Ya no quedan enemigos?
        if (allPhasesComplete && activeEnemies <= 0)
        {
            HandlePlayerVictory();
        }
    }

    void StartNextSubPhase()
    {
        currentSubPhaseIndex++;

        // ¿Hemos completado las 9 fases?
        if (currentSubPhaseIndex >= allSubPhases.Count)
        {
            Debug.Log("¡TODAS LAS OLEADAS COMPLETADAS! Esperando enemigos restantes...");
            allPhasesComplete = true; // Marcamos que las fases terminaron
            phaseTimer = 0;           // Detenemos el temporizador de fases
            return; // Salimos (el Update() ahora solo vigilará la condición de victoria)
        }

        // Si aún quedan fases, cargamos la siguiente
        SubPhaseData data = allSubPhases[currentSubPhaseIndex];
        phaseTimer = data.duration;
        Debug.Log($"Iniciando Sub-Fase {currentSubPhaseIndex + 1} de {allSubPhases.Count}");

        // Damos las órdenes
        waveSpawner.StartSpawning(data);
        lightingController.TransitionTo(data.directionalLightColor, 
                                        data.ambientLightIntensity, 
                                        data.lightingTransitionTime);
    }

    // --- [NUEVO] Manejadores de Victoria y Derrota ---

    private void HandlePlayerVictory()
    {
        if (gameIsOver) return; // Evita doble ejecución
        gameIsOver = true;
        
        Debug.Log("¡VICTORIA!");
        
        // (Opcional: puedes añadir un sonido de victoria aquí)
        
        // Llamamos al SceneLoader
        SceneLoader.LoadVictoryScene();
    }

    private void HandlePlayerDefeat()
    {
        if (gameIsOver) return; // Evita doble ejecución
        gameIsOver = true;

        Debug.Log("¡DERROTA!");

        // Iniciamos la secuencia de derrota (animación -> cambio de escena)
        StartCoroutine(DefeatSequence());
    }

    private IEnumerator DefeatSequence()
    {
        // 1. Instancia tu sprite de derrota
        if (defeatMageSpritePrefab != null)
        {
            // Busca la columna para spawnear el sprite en su posición
            GameObject column = FindFirstObjectByType<ColumnHealthBar>()?.gameObject;
            Vector3 spawnPos = (column != null) ? column.transform.position : Vector3.zero;
            
            Instantiate(defeatMageSpritePrefab, spawnPos, Quaternion.identity);
        }

        // 2. Espera el tiempo de la animación
       yield return new WaitForSecondsRealtime(defeatAnimTime);

        // 3. Llama al SceneLoader
        SceneLoader.LoadDefeatScene();
    }

    // --- [NUEVO] Limpieza ---
    private void OnDestroy()
    {
        // Es MUY importante desuscribirse de los eventos al destruir el objeto
        // para evitar errores de memoria.
        ColumnHealthBar.OnPlayerDefeated -= HandlePlayerDefeat;
    }

    // --- Lógica del Temporizador (Sin cambios) ---
    private void UpdateTimerUI(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}