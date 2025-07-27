using UnityEngine;
using UnityEngine.InputSystem;

public class ElementCombiner : MonoBehaviour
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
        inputActions.Player.deselect.performed -= OnCancelSelection;

        inputActions.Disable();
    }

    private void SelectType(Inventory.ItemType selected)
    {
        int bigCount = inventory.inventory[(int)selected].bigCount;

        if ((firstSelection == selected || secondSelection == selected) && bigCount <= 1)
        {
            Debug.Log($"No puedes seleccionar dos veces el mismo tipo {selected} si solo tienes uno.");
            return;
        }

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
            CombineObjects(firstSelection.Value, secondSelection.Value);
            firstSelection = null;
            secondSelection = null;
        }
    }

    private void CombineObjects(Inventory.ItemType first, Inventory.ItemType second)
    {
        // Lógica de combinación de hechizos
        if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Wind)
        {
            Debug.Log("Combinación: Viento + Viento");
            CastWindPushSpell();
        }
        else if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Ray)
        {
            Debug.Log("Combinación: Viento + Trueno");
            CastWindThunderCloudSpell();
        }
        else if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Viento + Hielo");
            CastWindIceSlowSpell();
        }
        else if (first == Inventory.ItemType.Ray && second == Inventory.ItemType.Ray)
        {
            Debug.Log("Combinación: Trueno + Trueno");
            CastThunderSpell();
        }
        else if (first == Inventory.ItemType.Ray && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Trueno + Hielo");
            CastThunderIceSpell();
        }
        else if (first == Inventory.ItemType.Ice && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Hielo + Hielo");
            CastIceFreezeSpell();
        }
    }

    // Métodos para cada hechizo

    private void CastWindPushSpell()
    {
        // Crear el hechizo de empuje de viento
        // Se crea un viento que empuja a los enemigos hacia la derecha
        // Asegúrate de usar un Collider con el tag "Enemy"
        // Usa un RigidBody o Collider para simular el empuje
    }

    private void CastWindThunderCloudSpell()
    {
        // Crear una nube que se mueve horizontalmente hacia la derecha
        // Aplicar daño desde arriba a los enemigos con el tag "Enemy"
    }

    private void CastWindIceSlowSpell()
    {
        // Crear ventisca que ralentiza a los enemigos en el área de efecto (AOE)
    }

    private void CastThunderSpell()
    {
        // Crear el hechizo de daño horizontal con movimiento
        // El jugador puede moverse mientras lanza este hechizo
    }

    private void CastThunderIceSpell()
    {
        // Crear el hechizo con menor daño pero con congelación al frente del jugador
    }

    private void CastIceFreezeSpell()
    {
        // Crear una congelación absoluta que congela a todos los enemigos dentro del radio
    }
}
