using UnityEngine;
using UnityEngine.EventSystems; // Requerido para la navegación de UI
using UnityEngine.InputSystem;   // Requerido para detectar el dispositivo
using System.Collections;       // Requerido para la corrutina

public class MainMenu : MonoBehaviour
{
    [Header("Gamepad Navigation")]
    [Tooltip("El primer botón que debe seleccionarse (ej. 'Jugar')")]
    public GameObject firstSelectedButton;

    // Variable para rastrear el dispositivo de control actual
    private string currentControlScheme;

    private void Start()
    {
        // --- [¡MUY IMPORTANTE!] ---
        // Resetea la escala de tiempo a 1 (normal) cada vez que
        // cargamos el Menú Principal. Esto descongela el juego
        // si el jugador perdió en la partida anterior.
        Time.timeScale = 1f; 
        // --- [FIN DE LA LÍNEA IMPORTANTE] ---

        // Forzamos el foco del gamepad/teclado al inicio
        ForceFocus();
    }

    private void OnEnable()
    {
        // Se suscribe a los eventos de cambio de dispositivo y de acción
        InputSystem.onActionChange += HandleDeviceChange;
        StartCoroutine(FocusButtonAfterFrame());
    }

    private void OnDisable()
    {
        // Se desuscribe de los eventos
        InputSystem.onActionChange -= HandleDeviceChange;
    }
    
    // --- Lógica de Botones ---

    /// <summary>
    /// Este método lo llamarás desde tu botón "Jugar".
    /// </summary>
    public void StartGame()
    {
        // Llama a nuestro SceneLoader.
        // El SceneLoader (sin fade) cargará la escena de juego.
        SceneLoader.LoadGameScene();
    }

    /// <summary>
    /// (Opcional) Para un botón de "Salir".
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
    
    
    // --- Lógica de Foco del Gamepad (copiada de EndScreenMenu) ---

    private void Update()
    {
        // Si estamos usando un gamepad o teclado
        if (currentControlScheme == "Gamepad" || currentControlScheme == "DualShockGamepad" || currentControlScheme == "KeyBoard")
        {
            // y si el sistema de eventos ha perdido el foco (no hay nada seleccionado)...
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                // ...forzamos la selección de vuelta al botón por defecto.
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }
    
    void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            // Asegurarse de que hay un PlayerInput en la escena
            if (PlayerInput.all.Count > 0)
            {
                string scheme = PlayerInput.all[0].currentControlScheme;
                if (scheme != currentControlScheme)
                {
                    currentControlScheme = scheme;
                    // Forzar el foco si el dispositivo cambia a gamepad/teclado
                    ForceFocus(); 
                }
            }
        }
    }
    
    // Corrutina para seleccionar el botón al inicio
    private IEnumerator FocusButtonAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        ForceFocus();
    }

    // Método de ayuda para forzar el foco
    private void ForceFocus()
    {
         if (PlayerInput.all.Count > 0)
        {
            currentControlScheme = PlayerInput.all[0].currentControlScheme;
        }

        // Si el control es gamepad/teclado, selecciona el botón
        if (currentControlScheme == "Gamepad" || currentControlScheme == "DualShockGamepad" || currentControlScheme == "KeyBoard")
        {
            if (firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }
}