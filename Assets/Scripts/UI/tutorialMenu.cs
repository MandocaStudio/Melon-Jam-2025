using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;




public class tutorialMenu : MonoBehaviour
{

    public GameObject tutorialMenuCanvas;
    public GameObject[] firstButtontutorialMenu;

    public GameObject imageTitle;

    public int buttonIndex;

    private string currentControlScheme;

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




    public void backIndex()
    {
        if (buttonIndex >= 0)
        {
            buttonIndex--;
        }
        StartCoroutine(FocusNextButton());

    }

    public void nextIndex()
    {
        if (buttonIndex <= 5)
        {
            buttonIndex++;
        }
        StartCoroutine(FocusNextButton());

    }


    public void backToMainMenu()
    {

        SceneManager.LoadScene(1);
    }


    IEnumerator FocusNextButton()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstButtontutorialMenu[buttonIndex]);
    }

    private void Update()
    {
        // Solo si no hay ningún objeto seleccionado
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detectar entrada de teclado o gamepad
            //Debug.Log("entro");

            if (tutorialMenuCanvas.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtontutorialMenu[buttonIndex]);

            }

        }
    }


}
