using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;



public class pauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerController player;


    public GameObject PauseMenu;

    public GameObject optionSection;

    public GameObject firstOptionsButton;

    public GameObject basicSection;

    public GameObject firstBasicButton;


    private PlayerControls inputActions;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    //public Slider ambientSlider;


    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    [SerializeField] private AudioMixer audioMixer;

    string scheme;

    [SerializeField] string mainMenuSceneName;

    private string currentControlScheme;


    void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;

        inputActions.Player.exitButton.performed += openpauseMenu;
        inputActions.UI.Cancel.performed += BacktoMainMenu;
        inputActions.UI.exitButton.performed += BacktoMainMenu;
        inputActions.Enable();
    }


    void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;

        inputActions.Player.exitButton.performed -= openpauseMenu;
        inputActions.UI.Cancel.performed -= BacktoMainMenu;
        inputActions.UI.exitButton.performed -= BacktoMainMenu;

        inputActions.Disable();
    }

    public void BacktoMainMenu(InputAction.CallbackContext context)
    {
        basicSection.SetActive(true);
        optionSection.SetActive(false);

        StartCoroutine(FocusNextButton(firstBasicButton));

    }

    public void openpauseMenu(InputAction.CallbackContext context)
    {

        if (PauseMenu.activeSelf)
        {
            Time.timeScale = 1;
            PauseMenu.SetActive(false);

            player.allowInput = true;


            basicSection.SetActive(true);
            optionSection.SetActive(false);

            inputActions.UI.Disable();
            inputActions.Player.Enable();

            //Debug.Log(scheme);

        }
        else if (!PauseMenu.activeSelf)
        {
            Time.timeScale = 0;

            PauseMenu.SetActive(true);

            player.allowInput = false;


            basicSection.SetActive(true);
            optionSection.SetActive(false);

            inputActions.UI.Enable();
            inputActions.Player.Disable();
        }

        StartCoroutine(FocusNextButton(firstBasicButton));

    }

    public void openOptionsMenu()
    {
        basicSection.SetActive(false);
        optionSection.SetActive(true);

        StartCoroutine(FocusNextButton(firstOptionsButton));

    }

    public void openMainMenu()
    {
        basicSection.SetActive(true);
        optionSection.SetActive(false);

        StartCoroutine(FocusNextButton(firstBasicButton));

    }

    public void changeGeneralVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void changeSFX(float volume)
    {
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfx", volume);
    }

    public void changeMusic(float volume)
    {
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("music", volume);

    }

    public void changeAmbient(float volume)
    {
        audioMixer.SetFloat("ambient", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("ambient", volume);

    }

    public void fullScream(bool fullScream)
    {
        Screen.fullScreen = fullScream;
    }

    public void resumeButton()
    {

        Time.timeScale = 1;
        PauseMenu.SetActive(false);

    }

    public void mainMenuButton()
    {

        SceneManager.LoadScene(mainMenuSceneName);


    }


    IEnumerator FocusNextButton(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button);
    }

    void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            scheme = PlayerInput.all[0].currentControlScheme;

            //Debug.Log(scheme);
            if (scheme != currentControlScheme)
            {

                currentControlScheme = scheme;

            }
        }
    }


    private void Update()
    {

        // Solo si no hay ningún objeto seleccionado
        if (EventSystem.current.currentSelectedGameObject == null && inputActions.UI.enabled)
        {
            // Detectar entrada de teclado o gamepad
            //Debug.Log("entro");


            if (basicSection.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstBasicButton);

            }
            else if (optionSection.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstOptionsButton);
            }

        }
    }

    void Start()
    {
        SetSliderAndVolume(masterSlider, "volume");
        SetSliderAndVolume(musicSlider, "music");
        SetSliderAndVolume(sfxSlider, "sfx");
        //SetSliderAndVolume(ambientSlider, "ambient");
    }

    void SetSliderAndVolume(Slider slider, string key)
    {
        float savedVolume = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : 0.5f;

        slider.value = savedVolume;

        float dB = savedVolume > 0.0001f ? Mathf.Log10(savedVolume) * 20f : -80f;
        audioMixer.SetFloat(key, dB);
    }


    public void ChangeVolume(Slider slider, string key)
    {
        float volume = slider.value;

        PlayerPrefs.SetFloat(key, volume);

        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        audioMixer.SetFloat(key, dB);
    }


}
