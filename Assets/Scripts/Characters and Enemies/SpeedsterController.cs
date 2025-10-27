using UnityEngine;

// 1. Añadimos ISlowable a la lista de interfaces
[RequireComponent(typeof(DamageDealer))]
public class SpeedsterController : MonoBehaviour, IDamageable, ISlowable
{
    [Header("Configuración del Velocista")]
    [SerializeField] private int health = 2;
    
    public float moveSpeed = 2f; // Esta es la velocidad base

    // --- Variables para ISlowable ---
    private float originalSpeed; // <-- NUEVO: Para guardar la velocidad original
    private float currentSpeed;  // <-- NUEVO: La velocidad que se usa en Update

    public int rowIndex;
    public Material FullMaterial;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);

        // --- Inicialización de ISlowable ---
        originalSpeed = moveSpeed; // <-- NUEVO: Guardamos la velocidad base
        currentSpeed = originalSpeed;  // <-- NUEVO: Seteamos la velocidad actual
    }

    private void Update()
    {
        // 2. Usamos currentSpeed en lugar de moveSpeed
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime); // <-- MODIFICADO
    }

    private void OnTriggerEnter(Collider other)
    {
        // (Usando el tag "PlayerColumn" que tenías en tu script EnemyProjectile)
        if (other.CompareTag("PlayerColumn"))
        {
            // El enemigo se destruye al chocar.
            // La columna (en su propio script) ya se habrá
            // encargado de tomar el daño de nuestro DamageDealer.
            Destroy(gameObject);
        }
    }

    // --- Método de IDamageable (Sin cambios) ---
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"Velocista recibió {damageAmount} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Velocista destruido");
            DropShard();
            Destroy(gameObject);
        }
    }

    // --- Método de Drop (Sin cambios) ---
    private void DropShard()
    {
        Inventory.Instance.CollectBig(Inventory.ItemType.Wind);
        Debug.Log("Fragmento grande de viento añadido al inventario.");
    }

    // --- 3. MÉTODOS REQUERIDOS POR ISlowable ---

    public void ApplySpeedMultiplier(float multiplier) // <-- NUEVO
    {
        // Aplicamos el multiplicador a la velocidad original (no a la actual)
        // para evitar que los slows se "acumulen" (multipliquen entre sí).
        currentSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeed() // <-- NUEVO
    {
        // Restauramos la velocidad a su valor original
        currentSpeed = originalSpeed;
    }
}