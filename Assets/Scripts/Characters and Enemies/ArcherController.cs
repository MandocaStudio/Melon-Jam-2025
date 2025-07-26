using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Configuración del Tirador")]
    public int health = 1;  // Salud del tirador (1 por defecto)
    public int damageToPlayer = 2;  // Daño que el proyectil hace al jugador (2)
    public float moveSpeed = 1f;  // Velocidad de movimiento (más rápida que el tanque)
    public float fireRate = 2f;  // Tasa de disparo (cada cuánto tiempo dispara)

    [Header("Proyectil")]
    public GameObject projectilePrefab;  // Prefab del proyectil (flecha)
    public float projectileSpeed = 5f;  // Velocidad del proyectil

    private static bool isShooterActive = false;  // Verifica si hay un tirador activo en la escena
    private float nextFireTime = 0f;  // Controla el tiempo entre disparos

    private int rowIndex;  // Fila en la que se encuentra el tirador (asignada dinámicamente)
    private Vector3[] rowPositions;  // Para manejar las posiciones en Y de los tiles

    private void Start()
    {
        // Verifica si ya existe un tirador en la escena
        if (isShooterActive)
        {
            Debug.LogWarning("Solo puede haber un tirador en la escena al mismo tiempo.");
            Destroy(gameObject);  // Si ya hay un tirador, destruye este
            return;
        }

        isShooterActive = true;  // Marca que el tirador está activo

        // Asignamos la fila en función de la posición Y
        AssignRowIndex();

        // Inicializar las posiciones de los tiles para el movimiento vertical
        rowPositions = new Vector3[5]; // Suponiendo que tienes 5 filas
        for (int i = 0; i < rowPositions.Length; i++)
        {
            rowPositions[i] = new Vector3(transform.position.x, (i * 2) - 4, 0); // Ajusta las posiciones de las filas
        }
    }

    private void Update()
    {
        // Movimiento del tirador entre los tiles (de arriba a abajo)
        MoveVertically();

        // Control de la tasa de disparo
        if (Time.time >= nextFireTime)
        {
            ShootProjectile();
            nextFireTime = Time.time + fireRate;  // Establece el próximo disparo según la tasa de disparo
        }
    }

    private void MoveVertically()
    {
        // Movimiento entre las filas
        int currentRow = Mathf.RoundToInt(transform.position.y / 2f) + 2;  // Calcular fila actual
        currentRow = Mathf.Clamp(currentRow, 0, 4);  // Limita la fila entre 0 y 4

        // Mover de arriba a abajo entre los tiles
        transform.position = rowPositions[currentRow];

        // Verificar si necesita cambiar de fila, alternando entre 0 y 4 (arriba/abajo)
        if (currentRow == 4) moveSpeed = -Mathf.Abs(moveSpeed);  // Si está en la fila más baja, mueve hacia arriba
        else if (currentRow == 0) moveSpeed = Mathf.Abs(moveSpeed);  // Si está en la fila más alta, mueve hacia abajo
    }

    private void ShootProjectile()
    {
        // Disparar un proyectil (flecha) hacia la columna del jugador (izquierda)
        Vector3 spawnPosition = transform.position + new Vector3(1f, 0, 0);  // Ajusta según el lugar del disparo
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Establece la dirección del proyectil
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = Vector3.left * projectileSpeed;  // Mueve el proyectil hacia la izquierda
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Recibe daño por proyectiles (en caso de ser impactado por el proyectil del jugador)
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);  // Destruye el proyectil del jugador

            TakeDamage(1);  // El tirador recibe 1 de daño por cada impacto
        }
    }

    private void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Tirador recibió {dmg} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Tirador destruido");
            Cleanup();
        }
    }

    private void Cleanup()
    {
        isShooterActive = false;  // Marca que el tirador ha sido destruido
        Destroy(gameObject);  // Destruye el tirador
    }

    private void AssignRowIndex()
    {
        // Asignar `rowIndex` según la posición Y del punto de spawn
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

        Debug.Log("Tirador en fila: " + rowIndex);
    }
}
