using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;  // El sistema de controles
    public GameObject projectilePrefab;

    [Header("Grid Setup")]
    public GameObject[] tileObjects = new GameObject[5];  // Tiles fila 0 a 4
    public Transform spawnTile;                           // Tile_0_2 por defecto
    private float[] rowYPositions = new float[5];

    private float columnX;   // Se toma del spawnTile
    private int currentRow;  // Se determina automáticamente según spawnTile

    // Asegúrate de inicializar controls en Awake, no en Start.
    private void Awake()
    {
        controls = new PlayerControls();  // Inicializa el sistema de controles
    }

    private void Start()
    {
        // Verifica y asigna posiciones Y de cada fila
        for (int i = 0; i < tileObjects.Length; i++)
        {
            if (tileObjects[i] != null)
                rowYPositions[i] = tileObjects[i].transform.position.y;
            else
                Debug.LogWarning($"Tile faltante en índice {i}");
        }

        if (spawnTile != null)
        {
            columnX = spawnTile.position.x;

            // Detecta automáticamente en qué fila está el spawnTile
            float spawnY = spawnTile.position.y;
            int closestRow = 0;
            float minDistance = Mathf.Abs(rowYPositions[0] - spawnY);
            for (int i = 1; i < rowYPositions.Length; i++)
            {
                float distance = Mathf.Abs(rowYPositions[i] - spawnY);
                if (distance < minDistance)
                {
                    closestRow = i;
                    minDistance = distance;
                }
            }

            currentRow = closestRow;
            MoveToRow(currentRow); // Posiciona al jugador correctamente
        }
        else
        {
            Debug.LogError("No has asignado el Spawn Tile al Player.");
        }
    }

    private void OnEnable()
    {
        controls.Enable();  // Activa el sistema de entradas
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Attack.performed -= OnAttackPerformed;
        controls.Disable();  // Desactiva el sistema de entradas
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input.y > 0.1f) MoveDown();      // W o Joystick arriba → baja fila (índice mayor → posición más arriba)
        else if (input.y < -0.1f) MoveUp();  // S o Joystick abajo → sube fila (índice menor → posición más abajo)
    }

    private void MoveUp()
    {
        if (currentRow < rowYPositions.Length - 1)
        {
            currentRow++;
            MoveToRow(currentRow);
        }
    }

    private void MoveDown()
    {
        if (currentRow > 0)
        {
            currentRow--;
            MoveToRow(currentRow);
        }
    }

    private void MoveToRow(int rowIndex)
    {
        float y = rowYPositions[rowIndex];
        transform.position = new Vector3(columnX, y, 0f); // Siempre se alinea con la columna X del spawnTile
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        Shoot();
    }

    private void Shoot()
    {
        Vector3 spawnPosition = transform.position + new Vector3(1f, 0, 0);
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Disparando proyectil");
    }
}
