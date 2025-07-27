using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;  // El sistema de controles
    public GameObject projectilePrefab;

    [Header("Grid Setup")]
    public GameObject[] tileObjects = new GameObject[5];  // Tiles fila 0 a 4
    public Transform spawnTile;                           // Tile_0_2 por defecto
    private float[] rowZPositions = new float[5];         // Almacenaremos las posiciones Z de los tiles

    private float columnX;   // Se toma del spawnTile (posición X del tile de spawn)
    private int currentRow;  // Se determina automáticamente según spawnTile

    // Asegúrate de inicializar controls en Awake, no en Start.
    private void Awake()
    {
        controls = new PlayerControls();  // Inicializa el sistema de controles
    }

    private void Start()
    {
        // Verifica y asigna posiciones Z de cada fila
        for (int i = 0; i < tileObjects.Length; i++)
        {
            if (tileObjects[i] != null)
                rowZPositions[i] = tileObjects[i].transform.position.z;
            else
                Debug.LogWarning($"Tile faltante en índice {i}");
        }

        if (spawnTile != null)
        {
            columnX = spawnTile.position.x;  // Posición X de spawnTile

            // Detecta automáticamente en qué fila está el spawnTile
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
        if (input.y > 0.1f) MoveForward();   // Mover hacia adelante (Eje Z positivo)
        else if (input.y < -0.1f) MoveBackward();  // Mover hacia atrás (Eje Z negativo)
    }

    private void MoveForward()
    {
        // Mover hacia adelante en el eje Z (solo entre los tiles predefinidos)
        if (currentRow < rowZPositions.Length - 1)
        {
            currentRow++;
            MoveToRow(currentRow);
        }
    }

    private void MoveBackward()
    {
        // Mover hacia atrás en el eje Z (solo entre los tiles predefinidos)
        if (currentRow > 0)
        {
            currentRow--;
            MoveToRow(currentRow);
        }
    }

    private void MoveToRow(int rowIndex)
    {
        // Obtener la posición Z del tile en la fila seleccionada
        float z = rowZPositions[rowIndex];
        
        // Obtener la posición Y del tile correspondiente
        float y = tileObjects[rowIndex].transform.position.y;
        
        // La posición X se toma de la columna del tile
        Vector3 newPosition = new Vector3(columnX, y, z);  // Mantener la posición en X, Y y Z del tile

        transform.position = newPosition;  // Actualiza la posición del jugador
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
