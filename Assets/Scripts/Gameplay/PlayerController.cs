using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;

    [Header("Referencias")]
    public ElementCombiner elementCombiner;
    public Inventory inventory;

    [Header("Ataque Básico")]
    public GameObject projectilePrefab; 

    [Header("Grid Setup")]
    public GameObject[] tileObjects = new GameObject[5];
    public Transform spawnTile;
    private float[] rowZPositions = new float[5];
    private float columnX;
    private int currentRow;

    [Header("Modelos del Jugador")]
    public GameObject idleModel;
    public GameObject attackModel;
    public float attackDuration = 0.5f;

    [Header("Audio")]
    public AudioSource shootAudioSource;
    public AudioSource moveAudioSource;

    [Header("Disparo")]
    public Transform projectileSpawnPoint;
    public float shootCooldown = 1.5f;
    private float shootTimer = 0f;
    private bool isAttacking = false;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {
        // --- Lógica de Grid (Restaurada) ---
        for (int i = 0; i < tileObjects.Length; i++)
        {
            if (tileObjects[i] != null)
                rowZPositions[i] = tileObjects[i].transform.position.z;
            else
                Debug.LogWarning($"Tile faltante en índice {i}");
        }

        if (spawnTile != null)
        {
            columnX = spawnTile.position.x;
            float spawnZ = spawnTile.position.z;
            int closestRow = 0;
            float minDistance = Mathf.Abs(rowZPositions[0] - spawnZ);
            for (int i = 1; i < rowZPositions.Length; i++)
            {
                float distance = Mathf.Abs(rowZPositions[i] - spawnZ);
                if (distance < minDistance)
                {
                    closestRow = i;
                    minDistance = distance;
                }
            }
            currentRow = closestRow;
            MoveToRow(currentRow);
        }
        else
        {
            Debug.LogError("No has asignado el Spawn Tile al Player.");
        }
        
        if (idleModel != null) idleModel.SetActive(true);
        if (attackModel != null) attackModel.SetActive(false);
        
        if (elementCombiner == null)
            Debug.LogError("¡ElementCombiner no está asignado en el PlayerController!");
        if (inventory == null)
            Debug.LogError("¡Inventory no está asignado en el PlayerController!");
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Attack.performed -= OnAttackPerformed;
        controls.Disable();
    }

    // --- LÓGICA DE MOVIMIENTO (RESTAURADA) ---

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input.y > 0.1f) MoveForward();
        else if (input.y < -0.1f) MoveBackward();
    }

    private void MoveForward()
    {
        if (currentRow < rowZPositions.Length - 1)
        {
            currentRow++;
            MoveToRow(currentRow);
            PlayMoveSound();
        }
    }

    private void MoveBackward()
    {
        if (currentRow > 0)
        {
            currentRow--;
            MoveToRow(currentRow);
            PlayMoveSound();
        }
    }

    private void PlayMoveSound()
    {
        if (moveAudioSource != null)
            moveAudioSource.Play();
    }

    private void MoveToRow(int rowIndex)
    {
        float z = rowZPositions[rowIndex];
        float y = tileObjects[rowIndex].transform.position.y;
        Vector3 newPosition = new Vector3(columnX, y, z);
        transform.position = newPosition;
    }

    // --- LÓGICA DE ATAQUE (CON HECHIZOS) ---

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (shootTimer > 0 || isAttacking) return; 

        var shard1 = elementCombiner.firstSelection;
        var shard2 = elementCombiner.secondSelection;

        if (shard1.HasValue && shard2.HasValue)
        {
            // --- Caso 1: Lanzar Hechizo Combinado ---
            elementCombiner.CastCombinedSpell(shard1.Value, shard2.Value, projectileSpawnPoint);
            inventory.CombineObjects(shard1.Value, shard2.Value);
            elementCombiner.ClearSelections();
            
            if (shootAudioSource != null) shootAudioSource.Play();
            StartCoroutine(PlayAttackAnimation());
            shootTimer = shootCooldown;
        }
        else
        {
            // --- Caso 2: Disparo Básico ---
            ShootBasicProjectile();
        }
    }

    private void ShootBasicProjectile()
    {
        if (isAttacking) return;

        Vector3 spawnPosition = projectileSpawnPoint.position;
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        if (shootAudioSource != null)
            shootAudioSource.Play();

        StartCoroutine(PlayAttackAnimation());
        shootTimer = shootCooldown;
    }

    private IEnumerator PlayAttackAnimation()
    {
        isAttacking = true;

        if (idleModel != null) idleModel.SetActive(false);
        if (attackModel != null) attackModel.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        if (attackModel != null) attackModel.SetActive(false);
        if (idleModel != null) idleModel.SetActive(true);

        isAttacking = false;
    }

    private void Update()
    {
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }
}