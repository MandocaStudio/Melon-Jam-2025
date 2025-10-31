using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; 

public class menuScript : MonoBehaviour
{
    [Header("Menu Canvases")]
    public GameObject optionsMenu;
    public GameObject firstButtonOptionMenu;
    public GameObject mainMenu;
    public GameObject firstButtonMainMenu;
    public GameObject creditSection;
    public GameObject firstButtoncreditSection;
    public GameObject imageTitle;

    [Header("Audio Variables (Volumen)")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Audio Playback")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip menuMusicClip;
    [SerializeField] private AudioClip buttonClickSound;

    private string currentControlScheme;
    private string scheme;
    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    
    void OnEnable()
    {
        if (inputActions == null) inputActions = new PlayerControls();
        inputActions.Enable();
        InputSystem.onActionChange += HandleDeviceChange;
    }

    void OnDisable()
    {
        if (inputActions != null) inputActions.Disable();
        InputSystem.onActionChange -= HandleDeviceChange;
    }
    
    void Start()
    {
        // 1. [NUEVO] Inicia la corrutina de inicialización de volumen
        StartCoroutine(ApplyInitialVolume());
        
        // Descongela el juego
        Time.timeScale = 1f; 
        
        // Inicia la música del menú
        if (musicAudioSource != null && menuMusicClip != null)
        {
            musicAudioSource.clip = menuMusicClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        // Forzamos el foco del gamepad/teclado al inicio
        ForceFocus();
    }

    // --- [NUEVO] Corrutina de Inicialización de Volumen ---
    private IEnumerator ApplyInitialVolume()
    {
        // Espera un frame para asegurar que el AudioMixer esté completamente inicializado
        yield return new WaitForEndOfFrame();
        
        Debug.Log("Aplicando volumen inicial desde PlayerPrefs...");
        
        // Aplica los valores guardados al Mixer y a los Sliders
        RestoreVolume("volume", masterSlider);
        RestoreVolume("music", musicSlider);
        RestoreVolume("sfx", sfxSlider);
    }
    
    // --- Lógica de Restauración de Volumen ---
    private void RestoreVolume(string key, Slider slider)
    {
        if (slider == null) return;

        // 1. Carga el valor guardado
        float savedVolume = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : 0.5f;

        // 2. Aplica al Slider
        slider.value = savedVolume;
        
        // 3. Aplica al Mixer
        audioMixer.SetFloat(key, ConvertToDecibels(savedVolume));
    }


    // --- Método público para los botones ---
    public void PlayButtonClickSound()
    {
        if (sfxAudioSource != null && buttonClickSound != null)
        {
            sfxAudioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    // --- Lógica de Navegación de Menú ---
    public void openOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        imageTitle.SetActive(false);
        StartCoroutine(FocusNextButton(firstButtonOptionMenu));
    }

    public void openMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        if (creditSection.activeSelf)
        {
            creditSection.SetActive(false);
        }
        imageTitle.SetActive(true);
        StartCoroutine(FocusNextButton(firstButtonMainMenu));
    }

    public void openCreditsSection()
    {
        mainMenu.SetActive(false);
        creditSection.SetActive(true);
        imageTitle.SetActive(false);
        StartCoroutine(FocusNextButton(firstButtoncreditSection));
    }

    // --- Lógica de Carga de Escena ---
    
    public void quitGame()
    {
        Application.Quit();
    }

    public void playGame()
    {
        // Llama a nuestro SceneLoader
        SceneLoader.LoadGameScene();
    }

    public void goToTutorial()
    {
        // Ahora 'SceneManager' es reconocido
        SceneManager.LoadScene("tutorial"); 
    }
    
    // --- Lógica de Opciones ---

    public void fullScream(bool fullScream)
    {
        Screen.fullScreen = fullScream;
    }
    
    // --- Lógica de Volumen ---

    public void changeGeneralVolume(float volume)
    {
        SetVolume("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void changeSFX(float volume)
    {
        SetVolume("sfx", volume);
        PlayerPrefs.SetFloat("sfx", volume);
    }

    public void changeMusic(float volume)
    {
        SetVolume("music", volume);
        PlayerPrefs.SetFloat("music", volume);
    }

    public void changeAmbient(float volume)
    {
        SetVolume("ambient", volume);
        PlayerPrefs.SetFloat("ambient", volume);
    }
    
    private void SetVolume(string key, float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(key, ConvertToDecibels(volume));
        }
    }

    private float ConvertToDecibels(float volume)
    {
        return Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
    }
    
    // --- Lógica de Foco del Gamepad ---

    void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            if (PlayerInput.all.Count > 0)
            {
                scheme = PlayerInput.all[0].currentControlScheme;
                if (scheme != currentControlScheme)
                {
                    currentControlScheme = scheme;
                    ForceFocus(); 
                }
            }
        }
    }
    
    IEnumerator FocusNextButton(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(button);
        }
    }

    private void ForceFocus()
    {
         if (PlayerInput.all.Count > 0)
        {
            currentControlScheme = PlayerInput.all[0].currentControlScheme;
        }

        if (currentControlScheme == "Gamepad" || currentControlScheme == "DualShockGamepad" || currentControlScheme == "KeyBoard")
        {
            if (mainMenu.activeSelf && firstButtonMainMenu != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);
            }
            else if (optionsMenu.activeSelf && firstButtonOptionMenu != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButtonOptionMenu);
            }
            else if (creditSection.activeSelf && firstButtoncreditSection != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButtoncreditSection);
            }
        }
    }
    
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (optionsMenu.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonOptionMenu);
            }
            else if (mainMenu.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);
            }
            else if (creditSection.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtoncreditSection);
            }
        }
    }
}