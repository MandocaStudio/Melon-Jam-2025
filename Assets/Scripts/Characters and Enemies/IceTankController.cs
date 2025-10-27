using UnityEngine;

// 1. Añadimos ISlowable a la lista de interfaces
[RequireComponent(typeof(DamageDealer))]
public class IceTankController : MonoBehaviour, IDamageable, ISlowable
{
    [Header("Configuración del Tanque")]
    [SerializeField] private int health = 4;
    [SerializeField] private int shieldHits = 3; 

    public float moveSpeed = 1f; // Esta es la velocidad base

    // --- Variables para ISlowable ---
    private float originalSpeed; // <-- NUEVO: Para guardar la velocidad original
    private float currentSpeed;  // <-- NUEVO: La velocidad que se usa en Update

    public int rowIndex;
    private bool isShieldActive = true;

    public Material blueFullMaterial;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        ActivateShieldAura();
        Debug.Log("Tanque en fila: " + rowIndex);

        // --- Inicialización de ISlowable ---
        originalSpeed = moveSpeed; // <-- NUEVO: Guardamos la velocidad base
        currentSpeed = originalSpeed;  // <-- NUEVO: Seteamos la velocidad actual
    }

    private void Update()
    {
        // 2. Usamos currentSpeed en lugar de moveSpeed
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime); // <-- MODIFICADO
    }

    private void ActivateShieldAura()
    {
        Debug.Log($"Escudo activado en la fila {rowIndex}, bloqueando {shieldHits} impactos.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // (Usando el tag "PlayerColumn" que tenías en tus scripts anteriores)
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
        // --- INICIO DE LA LÓGICA DEL ESCUDO ---
        if (isShieldActive)
        {
            shieldHits--;
            Debug.Log($"Escudo absorbió un impacto. Restan {shieldHits}");

            if (shieldHits <= 0)
            {
                isShieldActive = false;
                Debug.Log("Escudo destruido");
            }
            
            // ¡Importante! Salimos del método aquí
            // para que el daño no se aplique a la vida.
            return; 
        }
        // --- FIN DE LA LÓGICA DEL ESCUDO ---

        health -= damageAmount;
        Debug.Log($"Tanque recibió {damageAmount} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Tanque destruido");
            DropShard();
            Destroy(gameObject);
        }
    }

    // --- Método de Drop (Sin cambios) ---
    private void DropShard()
    {
        Inventory.Instance.CollectBig(Inventory.ItemType.Ice);
        Debug.Log("Fragmento grande de hielo añadido al inventario.");
    }
    
    // --- 3. MÉTODOS REQUERIDOS POR ISlowable ---

    public void ApplySpeedMultiplier(float multiplier) // <-- NUEVO
    {
        // Aplicamos el multiplicador a la velocidad original
        currentSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeed() // <-- NUEVO
    {
        // Restauramos la velocidad a su valor original
        currentSpeed = originalSpeed;
    }
}