using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;


public class pauseMenu : MonoBehaviour
{

    public InputAction exitButton;

    public GameObject PauseMenu;

    public GameObject firstMenuButton;

    string scheme;

    [SerializeField] string mainMenuSceneName;

    private string currentControlScheme;

    void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;
        exitButton.Enable();
        exitButton.performed += openpauseMenu;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
        exitButton.performed -= openpauseMenu;
        exitButton.Disable();

    }

    public void openpauseMenu(InputAction.CallbackContext context)
    {


        if (PauseMenu.activeSelf)
        {
            Time.timeScale = 1;
            PauseMenu.SetActive(false);
        }
        else if (!PauseMenu.activeSelf)
        {
            Time.timeScale = 0;

            PauseMenu.SetActive(true);
        }

        StartCoroutine(FocusNextButton(firstMenuButton));

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
        if (EventSystem.current.currentSelectedGameObject == null && PauseMenu.activeSelf)
        {
            // Detectar entrada de teclado o gamepad
            Debug.Log("entro");

            if (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard")
            {
                EventSystem.current.SetSelectedGameObject(firstMenuButton);

            }

        }
    }
}
