using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ElementCombiner : MonoBehaviour
{
    public Inventory inventory;
    private PlayerControls inputActions;

    public Inventory.ItemType? firstSelection { get; private set; } = null;
    public Inventory.ItemType? secondSelection { get; private set; } = null;

    [Header("UI Sprites")]
    [SerializeField] private Sprite rayImage;
    [SerializeField] private Sprite windImage;
    [SerializeField] private Sprite iceImage;
    [SerializeField] private Sprite voidmage;

    [SerializeField] private Image firstSlot;
    [SerializeField] private Image secondtSlot;

    [Header("Hechizos Prefabs")]
    // (Cloud + Cloud)
    [SerializeField] private GameObject windPushPrefab;
    // (Cloud + Ice)
    [SerializeField] private GameObject slowAOEPrefab;
    // (Ice + Ice)
    [SerializeField] private GameObject freezeSpellPrefab;
    // (Thunder + Thunder)
    [SerializeField] private GameObject thunderBeamPrefab;
    
    // 1. [AÑADIDOS] Los dos prefabs que faltaban
    // (Cloud + Thunder)
    [SerializeField] private GameObject thunderCloudPrefab; 
    // (Ice + Thunder)
    [SerializeField] private GameObject iceThunderConePrefab; 

    [Header("Audio Variables")]
    [SerializeField] private AudioClip firstSelectionSound;
    [SerializeField] private AudioSource audiosource;


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
        // ... (Tu lógica de selección está bien) ...
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
    
    public void CastCombinedSpell(Inventory.ItemType first, Inventory.ItemType second, Transform spawnPoint)
    {
        // Aseguramos el orden para que Hielo+Viento sea lo mismo que Viento+Hielo
        if (first > second)
        {
            var temp = first;
            first = second;
            second = temp;
        }

        // --- Lógica de combinación ---
        if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Wind)
        {
            Debug.Log("Combinación: Viento + Viento");
            CastWindPushSpell(spawnPoint);
        }
        else if (first == Inventory.ItemType.Wind && second == Inventory.ItemType.Ray)
        {
            Debug.Log("Combinación: Viento + Trueno");
            CastWindThunderCloudSpell(spawnPoint); // Usa el prefab 'thunderCloudPrefab'
        }
        else if (first == Inventory.ItemType.Ice && second == Inventory.ItemType.Wind) // Ordenado
        {
            Debug.Log("Combinación: Viento + Hielo");
            CastWindIceSlowSpell(spawnPoint);
        }
        else if (first == Inventory.ItemType.Ray && second == Inventory.ItemType.Ray)
        {
            Debug.Log("Combinación: Trueno + Trueno");
            CastThunderSpell(spawnPoint);
        }
        else if (first == Inventory.ItemType.Ice && second == Inventory.ItemType.Ray) // Ordenado
        {
            Debug.Log("Combinación: Trueno + Hielo");
            CastThunderIceSpell(spawnPoint); // Usa el prefab 'iceThunderConePrefab'
        }
        else if (first == Inventory.ItemType.Ice && second == Inventory.ItemType.Ice)
        {
            Debug.Log("Combinación: Hielo + Hielo");
            CastIceFreezeSpell();
        }
    }

    // --- Métodos de Casteo ---

    private void CastWindPushSpell(Transform spawnPoint)
    {
        if (windPushPrefab != null)
        {
            Instantiate(windPushPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    // 2. [ACTUALIZADO] Lógica de casteo para Cloud + Thunder
    private void CastWindThunderCloudSpell(Transform spawnPoint)
    {
        // Este hechizo (el rayo aleatorio) ignora el spawnPoint y se auto-posiciona
        if (thunderCloudPrefab != null)
        {
            Instantiate(thunderCloudPrefab); 
        }
    }

    private void CastWindIceSlowSpell(Transform spawnPoint)
    {
        if (slowAOEPrefab != null)
        {
            // Este hechizo (el AOE de hielo) debe spawnear en el centro de la fila
            // o en la posición del jugador. Usar el spawnPoint está bien.
            Instantiate(slowAOEPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private void CastThunderSpell(Transform spawnPoint)
    {
        if (thunderBeamPrefab != null)
        {
            // El rayo estático debe usar la rotación del jugador
            Instantiate(thunderBeamPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    // 3. [ACTUALIZADO] Lógica de casteo para Ice + Thunder
    private void CastThunderIceSpell(Transform spawnPoint)
    {
        // El cono de hielo/trueno usa la posición Y rotación del jugador
        if (iceThunderConePrefab != null)
        {
            Instantiate(iceThunderConePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private void CastIceFreezeSpell()
    {
        // El hechizo de congelación global no necesita posición
        Instantiate(freezeSpellPrefab, Vector3.zero, Quaternion.identity);
    }
}