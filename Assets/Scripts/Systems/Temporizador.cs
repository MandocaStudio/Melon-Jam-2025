using UnityEngine;
using TMPro;

public class GamePhaseTimer : MonoBehaviour
{
    [Header("UI de Temporizador")]
    public TextMeshProUGUI timerText; // Referencia al componente TextMeshProUGUI en la UI

    [Header("Duraciones de las Fases (en minutos)")]
    public float phase1Duration = 2f; // Duración de la Fase 1
    public float phase2Duration = 2f; // Duración de la Fase 2
    public float phase3Duration = 3f; // Duración de la Fase 3

    private float spawnTimer; // Temporizador de la fase actual
    private int currentPhase = 1; // Fase actual (inicia en Fase 1)

    private void Start()
    {
        spawnTimer = 0f; // Inicializamos el temporizador
        UpdateTimerUI(); // Actualizamos la UI con el tiempo inicial
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime; // Incrementamos el temporizador cada frame

        // Controlamos el avance de las fases según el tiempo transcurrido
        switch (currentPhase)
        {
            case 1:
                HandlePhase(phase1Duration, 2);
                break;
            case 2:
                HandlePhase(phase2Duration, 3);
                break;
            case 3:
                HandlePhase(phase3Duration, 4);
                break;
        }

        UpdateTimerUI(); // Actualizamos la UI con el tiempo restante
    }

    // Método para manejar el avance de cada fase
    private void HandlePhase(float phaseDuration, int nextPhase)
    {
        if (spawnTimer >= phaseDuration * 60f) // Comprobamos si hemos llegado al tiempo de la fase
        {
            currentPhase = nextPhase; // Pasamos a la siguiente fase
            spawnTimer = 0f; // Reiniciamos el temporizador para la siguiente fase
        }
    }

    // Método para actualizar la UI con el tiempo restante
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(spawnTimer / 60); // Calculamos los minutos
        int seconds = Mathf.FloorToInt(spawnTimer % 60); // Calculamos los segundos
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Mostramos el tiempo en formato MM:SS
    }
}
