using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
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
    public AudioSource moveAudioSource;  // 🎵 Nuevo audio de movimiento

    [Header("Disparo")]
    public Transform projectileSpawnPoint;

    private bool isAttacking = false;

    public bool allowInput = false;


    private void Awake()
    {
        controls = new PlayerControls();
        allowInput = false;

    }

    private void Start()
    {
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

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!allowInput) return;

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
            PlayMoveSound();  // 🎵 Sonido de movimiento
        }
    }

    private void MoveBackward()
    {
        if (currentRow > 0)
        {
            currentRow--;
            MoveToRow(currentRow);
            PlayMoveSound();  // 🎵 Sonido de movimiento
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

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (!allowInput) return;

        Shoot();
    }

    private void Shoot()
    {
        if (isAttacking) return;

        Vector3 spawnPosition = projectileSpawnPoint.position;
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        if (shootAudioSource != null)
            shootAudioSource.Play();

        StartCoroutine(PlayAttackAnimation());
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
}
