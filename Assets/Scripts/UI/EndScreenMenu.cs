using UnityEngine;
using UnityEngine.EventSystems; // Requerido para la navegación de UI
using UnityEngine.InputSystem;   // Requerido para detectar el dispositivo

public class EndScreenMenu : MonoBehaviour
{
    [Header("Gamepad Navigation")]
    [Tooltip("El botón por defecto que se seleccionará (ej. 'Jugar de Nuevo')")]
    public GameObject firstSelectedButton;

    private string currentControlScheme;

    private void OnEnable()
    {
        // Suscribirse al evento de cambio de dispositivo
        InputSystem.onActionChange += HandleDeviceChange;
        
        // Forzar la selección del primer botón (con un pequeño retraso)
        // para asegurar que el EventSystem esté listo.
        StartCoroutine(FocusButtonAfterFrame());
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
    }

    // Esta es tu lógica del script anterior para detectar el dispositivo
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
    
    // Esta es tu lógica del script anterior para mantener el foco
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

    // Corrutina para seleccionar el botón al inicio
    private System.Collections.IEnumerator FocusButtonAfterFrame()
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
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    // --- Métodos de botones (Llaman a nuestro SceneLoader) ---

    public void GoToMainMenu()
    {
        SceneLoader.LoadMainMenuScene();
    }

    public void ReplayGame()
    {
        SceneLoader.LoadGameScene();
    }
}