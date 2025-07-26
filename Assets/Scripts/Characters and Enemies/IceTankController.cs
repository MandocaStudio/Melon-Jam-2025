using UnityEngine;

public class IceTankController : MonoBehaviour
{
    [Header("Configuración del Tanque")]
    public int health = 4;  // Salud del tanque
    public int shieldHits = 3;  // Número de impactos que el escudo puede bloquear
    public int damageToPlayer = 1;  // Daño que el tanque hace al impactar con la columna (configurable en el Inspector)
    public float moveSpeed = 1f;  // Velocidad del movimiento

    public static int maxActiveTanks = 2;  // Máximo de tanques de Hielo en toda la escena
    public static int[] activeTanksPerRow = new int[5];  // Conteo de tanques por fila
    public static int currentTankCount = 0;  // Conteo total de tanques activos

    private bool isShieldActive = true;
    private bool hasReachedPlayer = false;  // Para saber si el tanque ya ha alcanzado la columna

    public int rowIndex; // La fila en la que se encuentra el tanque (calculado automáticamente)

    private void Start()
    {
        // Asignar el rowIndex dinámicamente basado en la posición Y del tanque
        AssignRowIndex();

        // Verifica que el tanque cumpla con las condiciones de spawn (solo 1 por fila y máximo 2 en la escena)
        if (currentTankCount >= maxActiveTanks || !IsRowIndexValid(rowIndex) || activeTanksPerRow[rowIndex] > 0)
        {
            Debug.LogWarning("No se puede crear más tanques en esta fila o en la escena.");
            Destroy(gameObject);  // Destruye el tanque si no cumple las condiciones
            return;
        }

        currentTankCount++;
        activeTanksPerRow[rowIndex]++;
        ActivateShieldAura();
    }

    private void Update()
    {
        if (!hasReachedPlayer)
        {
            // Movimiento del tanque de derecha a izquierda
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // Verificar si alcanzó la columna del jugador (PlayerColumn)
            if (transform.position.x <= 0)  // Ajusta el valor '0' según la posición de la PlayerColumn
            {
                hasReachedPlayer = true;
                // Llamar a la función que maneja el daño a la columna
                ReachPlayerColumn();
            }
        }
    }

    private bool IsRowIndexValid(int rowIndex)
    {
        // Asegurarse de que el índice esté dentro del rango permitido (0 - 4)
        return rowIndex >= 0 && rowIndex < activeTanksPerRow.Length;
    }

    private void ActivateShieldAura()
    {
        Debug.Log($"Escudo activado en la fila {rowIndex}, bloqueando {shieldHits} impactos.");
        // Aquí puedes agregar el efecto visual del escudo (si lo deseas)
    }

    private void ReachPlayerColumn()
    {
        Debug.Log("Tanque ha alcanzado la columna del jugador.");

        // Aquí puedes aplicar el daño a la columna del jugador
        GameObject playerColumn = GameObject.Find("PlayerColumn");
        if (playerColumn != null)
        {
            ColumnHealthBar columnHealth = playerColumn.GetComponent<ColumnHealthBar>();
            if (columnHealth != null)
            {
                columnHealth.TakeDamage(damageToPlayer);  // Restar vida de la columna con el valor configurable
            }
        }

        // Destruir el tanque al llegar a la columna del jugador
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);

            if (isShieldActive)
            {
                shieldHits--;
                Debug.Log($"Escudo absorbió un impacto. Restan {shieldHits}");

                if (shieldHits <= 0)
                {
                    isShieldActive = false;
                    Debug.Log("Escudo destruido");
                    // Puedes quitar el efecto visual del escudo aquí
                }
            }
            else
            {
                TakeDamage(1);
            }
        }
    }

    private void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Tanque recibió {dmg} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Tanque destruido");
            Cleanup();
        }
    }

    private void Cleanup()
    {
        currentTankCount--;
        if (rowIndex >= 0 && rowIndex < activeTanksPerRow.Length)
            activeTanksPerRow[rowIndex]--;
        Destroy(gameObject);
    }

    private void AssignRowIndex()
    {
        // Asignar rowIndex según la posición Y del punto de spawn
        float spawnY = transform.position.y;

        // Asumiendo que tus filas están distribuidas en el rango de Y (ajusta los valores según tu escena)
        if (spawnY >= 3.0f)
            rowIndex = 4;
        else if (spawnY >= 2.0f)
            rowIndex = 3;
        else if (spawnY >= 1.0f)
            rowIndex = 2;
        else if (spawnY >= 0.0f)
            rowIndex = 1;
        else
            rowIndex = 0;

        Debug.Log("Tanque en fila: " + rowIndex);
    }
}
