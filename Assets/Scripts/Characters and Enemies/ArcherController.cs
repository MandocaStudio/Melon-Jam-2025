using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public GameObject[] tileObjects = new GameObject[5];  // Arreglo de prefabs de los tiles
    private int rowIndex = 0;  // Fila actual, empieza en 0 (la primera fila)

    [Header("Configuración del Tirador")]
    public int health = 1;  // Salud del tirador (1 por defecto)
    public int damageToPlayer = 2;  // Daño que el proyectil hace al jugador (2)
    public float fireRate = 2f;  // Tasa de disparo (cada cuánto tiempo dispara)

    [Header("Proyectil")]
    public GameObject projectilePrefab;  // Prefab del proyectil (flecha)
    public float projectileSpeed = 5f;  // Velocidad del proyectil

    private static bool isShooterActive = false;  // Verifica si hay un tirador activo en la escena
    private float nextFireTime = 0f;  // Controla el tiempo entre disparos

    private float blinkTimer = 0f;  // Temporizador para el blink
    public float blinkInterval = 3f;  // Intervalo entre los blinks (en segundos)
    public float blinkProbability = 0.2f;  // Probabilidad de blink (20%)

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
    }

    private void Update()
    {
        // Si el tirador está vivo, realiza un blink y dispara
        if (health > 0)
        {
            // Control de la tasa de disparo
            if (Time.time >= nextFireTime)
            {
                ShootProjectile();
                nextFireTime = Time.time + fireRate;  // Establece el próximo disparo según la tasa de disparo
            }

            // Actualiza el temporizador para el blink
            blinkTimer += Time.deltaTime;

            // Verifica si ha pasado el tiempo necesario para realizar un "blink"
            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;  // Resetear el temporizador
                TryBlink();  // Intentar hacer un "blink" (teletransportarse)
            }
        }
        else
        {
            Cleanup();  // El tirador muere, elimina el objeto
        }
    }

    private void TryBlink()
    {
        // 20% de probabilidad de hacer un "blink"
        if (Random.value < blinkProbability)
        {
            // Escoge una fila aleatoria dentro de los índices posibles (0-4)
            int newRowIndex = Random.Range(0, tileObjects.Length);

            // Asegúrate de que el archer se mantenga sobre el tile correspondiente
            Vector3 newPosition = tileObjects[newRowIndex].transform.position;
            newPosition.z = transform.position.z;  // Mantén el valor de Z (si es necesario)

            transform.position = newPosition;  // Teletransportarse a esa fila
            rowIndex = newRowIndex;  // Actualiza el índice de la fila
            Debug.Log($"¡Blink! Tirador teletransportado a la fila {newRowIndex}");
        }
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
}
