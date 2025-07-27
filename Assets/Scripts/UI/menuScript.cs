using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;



public class menuScript : MonoBehaviour
{

    public GameObject optionsMenu;
    public GameObject firstButtonOptionMenu;
    public GameObject mainMenu;
    public GameObject firstButtonMainMenu;

    private string currentControlScheme;

    [SerializeField] private AudioMixer audioMixer;

    string scheme;
    void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
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

    public void openOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        StartCoroutine(FocusNextButton(firstButtonOptionMenu));


    }

    public void openMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);

        StartCoroutine(FocusNextButton(firstButtonMainMenu));

    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void playGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void fullScream(bool fullScream)
    {
        Screen.fullScreen = fullScream;
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

    IEnumerator FocusNextButton(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button);
    }

    private void Update()
    {
        // Solo si no hay ningún objeto seleccionado
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detectar entrada de teclado o gamepad
            Debug.Log("entro");

            if (optionsMenu.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonOptionMenu);

            }
            else if (mainMenu.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);
            }

        }
    }
}
