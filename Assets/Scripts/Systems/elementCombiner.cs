using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ElementCombiner : MonoBehaviour
{
    public Inventory inventory;
    private PlayerControls inputActions;

    [Header("UI Sprites")]
    [SerializeField] private Sprite rayImage;
    [SerializeField] private Sprite windImage;
    [SerializeField] private Sprite iceImage;
    [SerializeField] private Sprite voidmage;

    [SerializeField] private Image firstSlot;
    [SerializeField] private Image secondtSlot;

    [Header("Audio Variables")]
    [SerializeField] private AudioClip firstSelectionSound;
    [SerializeField] private AudioSource audiosource;

    [Header("Hechizos Prefabs (Dinámicos)")]
    [SerializeField] private GameObject windPushPrefab;
    [SerializeField] private GameObject thunderCloudPrefab; 
    
    [Header("Hechizos Estáticos (En la Jerarquía)")]
    [SerializeField] private GameObject slowAOE_Object; 
    [SerializeField] private GameObject freezeSpell_Object;
    [SerializeField] private GameObject thunderBeam_Object;
    [SerializeField] private GameObject iceThunderCone_Object;

    public Inventory.ItemType? firstSelection { get; private set; } = null;
    public Inventory.ItemType? secondSelection { get; private set; } = null;


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
        // ... (Tu lógica de selección de UI está bien) ...
        int bigCount = inventory.inventory[(int)selected].bigCount;
        if ((firstSelection == selected || secondSelection == selected) && bigCount <= 1) return;
        if (inventory.inventory[(int)selected].bigCount <= 0) return;

        if (firstSelection == null)
        {
            firstSelection = selected;
            audiosource.PlayOneShot(firstSelectionSound);
            switch (selected)
            {
                case Inventory.ItemType.Ray: firstSlot.sprite = rayImage; break;
                case Inventory.ItemType.Wind: firstSlot.sprite = windImage; break;
                case Inventory.ItemType.Ice: firstSlot.sprite = iceImage; break;
            }
        }
        else if (secondSelection == null)
        {
            secondSelection = selected;
            switch (selected)
            {
                case Inventory.ItemType.Ray: secondtSlot.sprite = rayImage; break;
                case Inventory.ItemType.Wind: secondtSlot.sprite = windImage; break;
                case Inventory.ItemType.Ice: secondtSlot.sprite = iceImage; break;
            }
        }
        else
        {
            firstSelection = selected;
            secondSelection = null;
            switch (selected)
            {
                case Inventory.ItemType.Ray: firstSlot.sprite = rayImage; break;
                case Inventory.ItemType.Wind: firstSlot.sprite = windImage; break;
                case Inventory.ItemType.Ice: firstSlot.sprite = iceImage; break;
            }
            secondtSlot.sprite = voidmage;
        }
    }
    
    public void ClearSelections()
    {
        firstSelection = null;
        secondSelection = null;
        firstSlot.sprite = voidmage;
        secondtSlot.sprite = voidmage;
    }

    private void OnCancelSelection(InputAction.CallbackContext ctx)
    {
        Debug.Log("Selección cancelada.");
        ClearSelections();
    }

    // --- LÓGICA DE CASTEO CORREGIDA ---
    public void CastCombinedSpell(Inventory.ItemType first, Inventory.ItemType second, Transform spawnPoint)
    {
        // Asumiendo el orden del enum: Ray (0), Wind (1), Ice (2)
        // Ordenamos para que 'first' sea siempre el valor más bajo.
        if (first > second)
        {
            var temp = first;
            first = second;
            second = temp;
        }

        // --- Lógica de Combinación (AHORA CORREGIDA Y ORDENADA) ---
        
        // Ray (0) + Ray (0)
        if (first == Inventory.ItemType.Ray && second == Inventory.ItemType.Ray)
        {
            Debug.Log("Combinación: Trueno + Trueno");
            CastThunderSpell(spawnPoint); // Estático (SetActive)
        }
        // Ray (0) + Wind (1)
        else if (first == Inventory.ItemType.Ray && second == Inventory.ItemType.Wind)
        {
            Debug.Log("Combinación: Trueno + Viento");
            CastWindThunderCloudSpell(spawnPoint); // Dinámico (Instantiate)
        }
        // Ray (0) + Ice (2)
        else if (first == Inventory.ItemType.Ray && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Trueno + Hielo");
            CastThunderIceSpell(spawnPoint); // Estático (SetActive)
        }
        // Wind (1) + Wind (1)
        else if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Wind)
        {
            Debug.Log("Combinación: Viento + Viento");
            CastWindPushSpell(spawnPoint); // Dinámico (Instantiate)
        }
        // Wind (1) + Ice (2)
        else if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Viento + Hielo");
            CastWindIceSlowSpell(spawnPoint); // Estático (SetActive)
        }
        // Ice (2) + Ice (2)
        else if (first == Inventory.ItemType.Ice && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Hielo + Hielo");
            CastIceFreezeSpell(); // Estático (SetActive)
        }
    }

    // --- Métodos de Casteo Dinámicos (Instantiate) ---
    private void CastWindPushSpell(Transform spawnPoint)
    {
        if (windPushPrefab != null)
        {
            Instantiate(windPushPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private void CastWindThunderCloudSpell(Transform spawnPoint)
    {
        if (thunderCloudPrefab != null)
        {
            Instantiate(thunderCloudPrefab); 
        }
    }

    // --- Métodos de Casteo Estáticos (Reciclaje) ---
    private void CastWindIceSlowSpell(Transform spawnPoint)
    {
        if (slowAOE_Object != null)
        {
            slowAOE_Object.SetActive(true);
        }
    }

    private void CastThunderSpell(Transform spawnPoint)
    {
        if (thunderBeam_Object != null)
        {
            thunderBeam_Object.SetActive(true);
        }
    }

    private void CastThunderIceSpell(Transform spawnPoint)
    {
        if (iceThunderCone_Object != null)
        {
            iceThunderCone_Object.SetActive(true);
        }
    }

    private void CastIceFreezeSpell()
    {
        if (freezeSpell_Object != null)
        {
            freezeSpell_Object.SetActive(true);
        }
    }
}