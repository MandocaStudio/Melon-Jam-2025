using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections; // Necesario para la corrutina

// [NUEVO] Le decimos a Unity que este script REQUIERE un AudioSource.
// Si no tienes uno, Unity lo añadirá automáticamente.
[RequireComponent(typeof(AudioSource))]
public class EndScreenMenu : MonoBehaviour
{
    [Header("Gamepad Navigation")]
    [Tooltip("El botón por defecto que se seleccionará (ej. 'Jugar de Nuevo')")]
    public GameObject firstSelectedButton;

    // --- [NUEVO] Sección de Audio ---
    [Header("Scene Audio")]
    [Tooltip("El clip de audio que se reproducirá UNA VEZ al cargar (ej. música de derrota/victoria)")]
    public AudioClip sceneAudioClip;
    
    private AudioSource audioSource;
    // --- [FIN DE LO NUEVO] ---

    private string currentControlScheme;

    // Awake se llama antes que Start
    private void Awake()
    {
        // [NUEVO] Obtenemos la referencia al AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    // Start se llama cuando la escena carga
    private void Start()
    {
        // [NUEVO] Llamamos a nuestro método para reproducir el audio
        PlaySceneAudio();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;
        StartCoroutine(FocusButtonAfterFrame());
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
    }

    // --- [NUEVO] Método para reproducir el audio ---
    private void PlaySceneAudio()
    {
        if (audioSource != null && sceneAudioClip != null)
        {
            audioSource.loop = false; // Nos aseguramos de que no se repita
            audioSource.clip = sceneAudioClip;
            audioSource.Play();
        }
    }
    // --- [FIN DE LO NUEVO] ---

    // --- (El resto de tu código de Gamepad, Update y Botones no cambia) ---

    void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            if (PlayerInput.all.Count > 0)
            {
                string scheme = PlayerInput.all[0].currentControlScheme;
                if (scheme != currentControlScheme)
                {
                    currentControlScheme = scheme;
                    ForceFocus(); 
                }
            }
        }
    }
    
    private void Update()
    {
        if (currentControlScheme == "Gamepad" || currentControlScheme == "DualShockGamepad" || currentControlScheme == "KeyBoard")
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }

    private IEnumerator FocusButtonAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        ForceFocus();
    }

    private void ForceFocus()
    {
         if (PlayerInput.all.Count > 0)
        {
            currentControlScheme = PlayerInput.all[0].currentControlScheme;
        }
        if (currentControlScheme == "Gamepad" || currentControlScheme == "DualShockGamepad" || currentControlScheme == "KeyBoard")
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void GoToMainMenu()
    {
        SceneLoader.LoadMainMenuScene();
    }

    public void ReplayGame()
    {
        SceneLoader.LoadGameScene();
    }
}