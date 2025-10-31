using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Necesario para el texto del temporizador
using UnityEngine.SceneManagement; // Necesario para FindFirstObjectByType

public class GameManager : MonoBehaviour
{
    // --- Contador de Enemigos ---
    public static int activeEnemies = 0; 

    // --- Estado del Juego ---
    // Lo hacemos 'public static' para que PlayerController pueda leerlo
    public static bool gameIsOver = false; 

    [Header("Referencias")]
    public LightingController lightingController; 
    public WaveSpawner waveSpawner; // (O WaveManager, según lo hayas nombrado)
    public TextMeshProUGUI timerText;         
    public PlayerController playerController; // Referencia al jugador

    [Header("Audio")]
    public AudioSource backgroundMusicSource;
    public AudioClip gameMusicClip;
    public float musicStartDelay = 3f;

    [Header("Lógica de Juego")]
    [Tooltip("Tiempo (seg) que se muestra la animación de derrota antes de cambiar de escena")]
    public float defeatAnimTime = 3f; 

    [Header("Configuración de Fases")]
    [Tooltip("Arrastra aquí tus 9 assets de SubPhaseData, en orden")]
    public List<SubPhaseData> allSubPhases;

    // --- Variables de Estado Internas ---
    private int currentSubPhaseIndex = -1;
    private float phaseTimer = 0f;
    private float totalGameTime = 0f;
    private bool allPhasesComplete = false; 

    private void Start()
    {
        // 1. Reseteamos los contadores estáticos
        gameIsOver = false; 
        activeEnemies = 0;
        allPhasesComplete = false;
        
        // 2. ¡MUY IMPORTANTE! Descongelamos el tiempo (por si venimos de 'Rejugar')
        Time.timeScale = 1f;

        // 3. Verificamos referencias
        if (lightingController == null || waveSpawner == null || playerController == null)
        {
            Debug.LogError("¡Faltan referencias (LightingController, WaveSpawner o PlayerController) en el GameManager!");
            enabled = false; // Desactivamos el GameManager
            return;
        }

        // 4. Nos suscribimos al evento de derrota de la columna
        ColumnHealthBar.OnPlayerDefeated += HandlePlayerDefeat;

        // 5. Iniciamos la corrutina de música
        StartCoroutine(PlayMusicWithDelay());

        // 6. Empezamos el juego
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

        // --- Lógica de Victoria ---
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

    // --- Manejadores de Victoria y Derrota ---

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
        
        // --- ¡CONGELA EL JUEGO! ---
        Time.timeScale = 0f; 
        
        gameIsOver = true;
        Debug.Log("¡DERROTA!");

        // Detenemos la música de fondo
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }

        // Iniciamos la secuencia de derrota (animación -> cambio de escena)
        StartCoroutine(DefeatSequence());
    }

    private IEnumerator DefeatSequence()
    {
        // 1. Llama al PlayerController para que muestre el sprite
        if (playerController != null)
        {
            playerController.PlayDefeatSequence();
        }

        // 2. Espera usando tiempo REAL (ignora si Time.timeScale es 0)
        yield return new WaitForSecondsRealtime(defeatAnimTime);

        // 3. Llama al SceneLoader
        SceneLoader.LoadDefeatScene();
    }
    
    // --- Corrutina de Música ---
    private IEnumerator PlayMusicWithDelay()
    {
        // 1. Espera los 3 segundos
        yield return new WaitForSeconds(musicStartDelay);

        // 2. Comprueba si el jugador NO ha perdido en esos 3 segundos
        if (!gameIsOver && backgroundMusicSource != null && gameMusicClip != null)
        {
            backgroundMusicSource.clip = gameMusicClip;
            backgroundMusicSource.loop = true; // Para que se repita
            backgroundMusicSource.Play();
        }
    }

    // --- Limpieza ---
    private void OnDestroy()
    {
        // Es MUY importante desuscribirse de los eventos
        ColumnHealthBar.OnPlayerDefeated -= HandlePlayerDefeat;
    }

    // --- Lógica del Temporizador ---
    private void UpdateTimerUI(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}