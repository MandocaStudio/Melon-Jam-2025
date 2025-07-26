using UnityEngine;

public class SpeedsterController : MonoBehaviour
{
    [Header("Configuración del Velocista")]
    public int health = 2;  // Salud del velocista
    public int damageToPlayer = 1;  // Daño que el velocista hace al impactar con la columna (configurable en el Inspector)
    public float moveSpeed = 2f;  // Velocidad de movimiento (más rápida que el tanque)

    public static int maxActiveSpeedsters = 2;  // Máximo de velocistas en toda la escena
    public static int[] activeSpeedstersPerRow = new int[5];  // Conteo de velocistas por fila
    public static int currentSpeedsterCount = 0;  // Conteo total de velocistas activos

    private bool hasReachedPlayer = false;  // Para saber si el velocista ya ha alcanzado la columna
    public int rowIndex; // La fila en la que se encuentra el velocista (asignada al instanciar)

    private void Start()
    {
        // Verifica que el velocista cumpla con las condiciones de spawn (solo 1 por fila y máximo 2 en la escena)
        if (currentSpeedsterCount >= maxActiveSpeedsters || activeSpeedstersPerRow[rowIndex] > 0)
        {
            Debug.LogWarning("No se puede crear más velocistas en esta fila o en la escena.");
            Destroy(gameObject);  // Destruye el velocista si no cumple las condiciones
            return;
        }

        currentSpeedsterCount++;
        activeSpeedstersPerRow[rowIndex]++;
    }

    private void Update()
    {
        if (!hasReachedPlayer)
        {
            // Movimiento del velocista de derecha a izquierda
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

    private void ReachPlayerColumn()
    {
        Debug.Log("Velocista ha alcanzado la columna del jugador.");

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

        // Destruir el velocista al llegar a la columna del jugador
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);

            TakeDamage(1);
        }
    }

    private void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Velocista recibió {dmg} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Velocista destruido");
            Cleanup();
        }
    }

    private void Cleanup()
    {
        currentSpeedsterCount--;
        if (rowIndex >= 0 && rowIndex < activeSpeedstersPerRow.Length)
            activeSpeedstersPerRow[rowIndex]--;
        Destroy(gameObject);
    }

    // Método para acelerar a los enemigos en la fila
    private void AccelerateEnemiesInRow()
    {
        // Obtener todos los enemigos de la misma fila
        GameObject[] enemiesInRow = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemiesInRow)
        {
            IceTankController iceTank = enemy.GetComponent<IceTankController>();
            SpeedsterController speedster = enemy.GetComponent<SpeedsterController>();

            // Acelera a todos los enemigos en la misma fila
            if (iceTank != null && iceTank.rowIndex == rowIndex)
            {
                iceTank.moveSpeed *= 1.5f;  // Aumenta la velocidad del tanque de hielo
            }

            if (speedster != null && speedster.rowIndex == rowIndex)
            {
                speedster.moveSpeed *= 1.5f;  // Aumenta la velocidad del velocista
            }
        }
    }
}
