using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;

public class victory_defeat_Menu : MonoBehaviour
{

    public GameObject VictoryCanvas;

    public GameObject DefeatCanvas;

    public GameObject firstButtonMenu;

    private string currentControlScheme;

    string scheme;

    private void Start()
    {
        if (GameData.wasVictory)
        {
            DefeatCanvas.SetActive(false);
            VictoryCanvas.SetActive(true);
        }
        else
        {
            DefeatCanvas.SetActive(true);
            VictoryCanvas.SetActive(false);
        }
    }
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


    public void backToMainMenu()
    {

        SceneManager.LoadScene(1);
    }

    public void ReplayGame()
    {

        SceneManager.LoadScene(2);
    }


    IEnumerator FocusNextButton()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstButtonMenu);
    }

    private void Update()
    {
        // Solo si no hay ningún objeto seleccionado
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detectar entrada de teclado o gamepad
            //Debug.Log("entro");

            if (VictoryCanvas.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonMenu);

            }
            else if (DefeatCanvas.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonMenu);

            }

        }
    }

}

public static class GameData
{
    public static bool wasVictory = false;
}