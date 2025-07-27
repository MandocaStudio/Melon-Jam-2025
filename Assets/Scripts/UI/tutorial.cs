using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;



public class tutorial : MonoBehaviour
{
    [SerializeField] private PlayerController player;


    public GameObject optionSection;

    public GameObject firstTutorialButton;



    private PlayerControls inputActions;


    private void Awake()
    {
        inputActions = new PlayerControls();
        Time.timeScale = 0;
    }



    string scheme;

    private string currentControlScheme;


    void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;

        // inputActions.Player.exitButton.performed += openpauseMenu;
        // inputActions.UI.Cancel.performed += BacktoMainMenu;
        // inputActions.UI.exitButton.performed += BacktoMainMenu;
        inputActions.Enable();
    }


    void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;

        // inputActions.Player.exitButton.performed -= openpauseMenu;
        // inputActions.UI.Cancel.performed -= BacktoMainMenu;
        // inputActions.UI.exitButton.performed -= BacktoMainMenu;

        inputActions.Disable();
    }

    public void playGame()
    {
        optionSection.SetActive(false);

        player.allowInput = true;

        Time.timeScale = 1;

        StartCoroutine(FocusNextButton(firstTutorialButton));

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


            if (optionSection.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstTutorialButton);

            }


        }
    }



}
