using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class InputSchemeTMPSwitcher : MonoBehaviour
{
    public TMP_SpriteAsset keyboardSpriteAsset;
    public TMP_SpriteAsset gamepadSpriteAsset;
    public TMP_SpriteAsset dualshockSpriteAsset;

    private List<TMP_Text> allTMPs = new List<TMP_Text>();
    [SerializeField] private PlayerInput playerInput;

    private string currentControlScheme;

    private void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;

        // Forzar actualización al inicio
        if (playerInput != null)
        {
            currentControlScheme = playerInput.currentControlScheme;
            UpdateAllTMPs();
        }
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
    }

    private void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            if (playerInput == null) return;

            string scheme = playerInput.currentControlScheme;

            if (scheme != currentControlScheme)
            {
                currentControlScheme = scheme;
                Debug.Log($"Input Scheme changed to: {currentControlScheme}");
                UpdateAllTMPs();
            }
        }
    }

    public void UpdateAllTMPs()
    {
        allTMPs.Clear();
        allTMPs.AddRange(Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None));

        TMP_SpriteAsset assetToUse = keyboardSpriteAsset;

        switch (currentControlScheme)
        {
            case "Gamepad":
                assetToUse = gamepadSpriteAsset;
                break;
            case "DualShockGamepad":
                assetToUse = dualshockSpriteAsset;
                break;
        }

        foreach (var tmp in allTMPs)
        {
            tmp.spriteAsset = assetToUse;
            tmp.text = tmp.text; // Forzar refresco
        }
    }
}
