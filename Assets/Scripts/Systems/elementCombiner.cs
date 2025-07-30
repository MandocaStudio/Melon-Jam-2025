using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ElementCombiner : MonoBehaviour
{
    public Inventory inventory;
    private Inventory.ItemType? firstSelection = null;
    private Inventory.ItemType? secondSelection = null;
    private PlayerControls inputActions;
    public InputAction cancel;
    public WindPushSpell windPushSpellPrefab;  // Prefab para el hechizo de empuje de viento

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.ice.performed += ctx => SelectType(Inventory.ItemType.Ice);
        inputActions.Player.wind.performed += ctx => SelectType(Inventory.ItemType.Wind);
        inputActions.Player.ray.performed += ctx => SelectType(Inventory.ItemType.Ray);
        inputActions.Player.deselect.performed += OnCancelSelection;
    }

    private void OnDisable()
    {
        inputActions.Player.ice.performed -= ctx => SelectType(Inventory.ItemType.Ice);
        inputActions.Player.wind.performed -= ctx => SelectType(Inventory.ItemType.Wind);
        inputActions.Player.ray.performed -= ctx => SelectType(Inventory.ItemType.Ray);
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


            switch (selected)
            {
                case Inventory.ItemType.Ray:
                    firstSlot.sprite = rayImage;
                    break;

                case Inventory.ItemType.Wind:
                    firstSlot.sprite = windImage;
                    break;

                case Inventory.ItemType.Ice:
                    firstSlot.sprite = iceImage;
                    break;
            }


        }
        else if (secondSelection == null)
        {
            secondSelection = selected;
            Debug.Log($"Segunda selección: {selected}");

            switch (selected)
            {
                case Inventory.ItemType.Ray:
                    secondtSlot.sprite = rayImage;
                    break;

                case Inventory.ItemType.Wind:
                    secondtSlot.sprite = windImage;
                    break;

                case Inventory.ItemType.Ice:
                    secondtSlot.sprite = iceImage;
                    break;
            }

            TryCombine();
        }
        else
        {
            Debug.Log("Ya has seleccionado dos elementos. Reiniciando selección.");
            firstSelection = selected;
            secondSelection = null;
        }
    }

    IEnumerator borrarCombinaciones()
    {
        yield return new WaitForSeconds(2f); // Espera 2 segundos

        firstSlot.sprite = voidmage;
        secondtSlot.sprite = voidmage;

    }

    private void OnCancelSelection(InputAction.CallbackContext ctx)
    {
        if (firstSelection != null && secondSelection == null)
        {
            Debug.Log("Primera selección cancelada.");

            firstSlot.sprite = voidmage;
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

            StartCoroutine(borrarCombinaciones());
        }
    }

    private void CombineObjectsLocal(Inventory.ItemType first, Inventory.ItemType second)
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
    // Aquí instanciamos el hechizo de WindPushSpell
    WindPushSpell windPushSpell = Instantiate(windPushSpellPrefab, transform.position, Quaternion.identity);

    // Llamamos al método para ejecutar el hechizo, pasando la posición de la columna del jugador o la posición deseada
    windPushSpell.CastWindPush(transform.position);
}


    private void CastWindThunderCloudSpell()
    {
        // Crear una nube que se mueve horizontalmente hacia la derecha
        // Aplicar daño desde arriba a los enemigos con el tag "Enemy"
    }

   private void CastWindIceSlowSpell()
{
    if (slowAOEPrefab != null && aoeSpawnPoint != null)
    {
        Instantiate(slowAOEPrefab, aoeSpawnPoint.position, Quaternion.identity);
        Debug.Log("AOE de viento + hielo lanzado.");
    }
}

  private void CastThunderSpell()
{
    if (thunderBeamPrefab != null && beamSpawnPoint != null)
    {
        Instantiate(thunderBeamPrefab, beamSpawnPoint.position, Quaternion.identity);
    }
}


    private void CastThunderIceSpell()
    {
        // Crear el hechizo con menor daño pero con congelación al frente del jugador
    }

private void CastIceFreezeSpell()
{
    // Instanciar el hechizo FreezeSpell en la escena (posición 0,0,0 ya que afecta a todos)
    Instantiate(freezeSpellPrefab, Vector3.zero, Quaternion.identity);
}


}
