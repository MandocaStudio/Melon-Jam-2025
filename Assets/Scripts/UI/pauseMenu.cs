using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;


public class pauseMenu : MonoBehaviour
{


    public GameObject PauseMenu;

    public GameObject optionSection;

    public GameObject firstOptionsButton;

    public GameObject basicSection;

    public GameObject firstBasicButton;


    private PlayerControls inputActions;

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

            basicSection.SetActive(true);
            optionSection.SetActive(false);
        }
        else if (!PauseMenu.activeSelf)
        {
            Time.timeScale = 0;

            PauseMenu.SetActive(true);

            basicSection.SetActive(true);
            optionSection.SetActive(false);
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
        audioMixer.SetFloat("volume", volume);
    }

    public void changeSFX(float volume)
    {
        audioMixer.SetFloat("sfx", volume);
    }

    public void changeMusic(float volume)
    {
        audioMixer.SetFloat("music", volume);
    }

    public void changeAmbient(float volume)
    {
        audioMixer.SetFloat("ambient", volume);
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

            Debug.Log(scheme);
            if (scheme != currentControlScheme)
            {

                currentControlScheme = scheme;

            }
        }
    }


    private void Update()
    {

        // Solo si no hay ningún objeto seleccionado
        if (EventSystem.current.currentSelectedGameObject == null)
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
}
