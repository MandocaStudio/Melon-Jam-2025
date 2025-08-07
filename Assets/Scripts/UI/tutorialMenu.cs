using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class tutorialMenu : MonoBehaviour
{
    public GameObject tutorialMenuCanvas;
    public GameObject[] firstButtontutorialMenu;

    public GameObject imageTitle;

    public int buttonIndex;

    private string currentControlScheme;

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private InputSchemeTMPSwitcher inputSchemeTMPSwitcher;


    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        currentControlScheme = input.currentControlScheme;
        // Debug.Log("Control scheme changed: " + currentControlScheme);
    }

    public void backIndex()
    {
        if (buttonIndex > 0)
        {
            buttonIndex--;
        }
        StartCoroutine(FocusNextButton());
    }

    public void nextIndex()
    {
        if (buttonIndex < firstButtontutorialMenu.Length - 1)
        {
            buttonIndex++;
        }
        StartCoroutine(FocusNextButton());
    }

    public void backToMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator FocusNextButton()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstButtontutorialMenu[buttonIndex]);

        if (inputSchemeTMPSwitcher != null)
        {
            inputSchemeTMPSwitcher.UpdateAllTMPs();
        }
    }


    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (tutorialMenuCanvas.activeSelf &&
                (currentControlScheme == "Gamepad" || currentControlScheme == "DualShockGamepad" || currentControlScheme == "Keyboard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtontutorialMenu[buttonIndex]);
            }
        }
    }
}
