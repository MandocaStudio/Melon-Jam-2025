using UnityEngine;
using UnityEngine.InputSystem;

public class elementCombiner : MonoBehaviour
{
    public Inventory inventory;

    private Inventory.ItemType? firstSelection = null;
    private Inventory.ItemType? secondSelection = null;
    private PlayerControls inputActions;
    public InputAction cancel;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    //Parallel depurar
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.ice.performed += ctx => SelectType(Inventory.ItemType.Ice);
        inputActions.Player.ray.performed += ctx => SelectType(Inventory.ItemType.Ray);
        inputActions.Player.wind.performed += ctx => SelectType(Inventory.ItemType.Wind);
        inputActions.Player.deselect.performed += OnCancelSelection;

    }

    private void OnDisable()
    {
        inputActions.Player.ice.performed -= ctx => SelectType(Inventory.ItemType.Ice);
        inputActions.Player.ray.performed -= ctx => SelectType(Inventory.ItemType.Ray);
        inputActions.Player.wind.performed -= ctx => SelectType(Inventory.ItemType.Wind);
        inputActions.Player.deselect.performed += OnCancelSelection;

        inputActions.Disable();
    }

    private void SelectType(Inventory.ItemType selected)
    {
        int bigCount = inventory.inventory[(int)selected].bigCount;

        // Si ya seleccionaste este tipo una vez, evita seleccionarlo de nuevo si solo hay 1
        if ((firstSelection == selected || secondSelection == selected) && bigCount <= 1)
        {
            Debug.Log($"No puedes seleccionar dos veces el mismo tipo {selected} si solo tienes uno.");
            return;
        }
        // Checar si hay suficientes objetos grandes
        if (inventory.inventory[(int)selected].bigCount <= 0)
        {
            Debug.Log($"No tienes objetos grandes del tipo {selected}.");
            return;
        }

        if (firstSelection == null)
        {
            firstSelection = selected;
            Debug.Log($"Primera selección: {selected}");
        }
        else if (secondSelection == null)
        {
            secondSelection = selected;
            Debug.Log($"Segunda selección: {selected}");

            // Ya tenemos dos elementos, intentar combinación
            TryCombine();
        }
        else
        {
            Debug.Log("Ya has seleccionado dos elementos. Reiniciando selección.");
            firstSelection = selected;
            secondSelection = null;
        }
    }

    private void OnCancelSelection(InputAction.CallbackContext ctx)
    {
        if (firstSelection != null && secondSelection == null)
        {
            Debug.Log("Primera selección cancelada.");
            firstSelection = null;
        }

    }

    private void TryCombine()
    {
        if (firstSelection.HasValue && secondSelection.HasValue)
        {
            inventory.CombineObjects(firstSelection.Value, secondSelection.Value);
            firstSelection = null;
            secondSelection = null;
        }
    }

}
