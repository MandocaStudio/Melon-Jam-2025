using UnityEngine;

// 1. Añadimos ISlowable a la lista de interfaces
[RequireComponent(typeof(DamageDealer))]
public class BasicEnemyController : MonoBehaviour, IDamageable, ISlowable
{
    public float moveSpeed = 1.5f; // Esta es la velocidad base
    public int maxHealth = 3;

    // --- Variables para ISlowable ---
    private float originalSpeed; // <-- NUEVO: Para guardar la velocidad original
    private float currentSpeed;  // <-- NUEVO: La velocidad que se usa en Update

    // Materiales correspondientes a los shards (para ser añadidos al inventario)
    public Material purpleShardMaterial, blueShardMaterial, yellowShardMaterial;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    [SerializeField] private int currentHealth;

    public enum EnemyColor { Purple, Blue, Yellow }
    [HideInInspector] public EnemyColor enemyColor;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyColor = GetRandomColor();
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        transform.tag = "Enemy";

        // --- Inicialización de ISlowable ---
        originalSpeed = moveSpeed; // <-- NUEVO: Guardamos la velocidad base
        currentSpeed = originalSpeed;  // <-- NUEVO: Seteamos la velocidad actual
    }

    private void Update()
    {
        // 2. Usamos currentSpeed en lugar de moveSpeed
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime); // <-- MODIFICADO
    }

    private EnemyColor GetRandomColor()
    {
        int rand = Random.Range(0, 3);
        return (EnemyColor)rand;
    }

    private void OnTriggerEnter(Collider other)
    {
        // (Usando el tag "PlayerColumn" que tenías en tu script)
        if (other.CompareTag("PlayerColumn"))
        {
            // El enemigo solo se destruye.
            // La columna (en ColumnHealthBar.cs) ya detecta
            // el tag "Enemy" y toma el daño de nuestro DamageDealer.
            Destroy(gameObject);
        }
    }

    // --- Método de IDamageable (Sin cambios) ---
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Daño recibido: " + damageAmount + " | Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Enemigo destruido");
            DropShard();
            Destroy(gameObject);
        }
    }

    // --- Método de Drop (Sin cambios) ---
    private void DropShard()
    {
        switch (enemyColor)
        {
            case EnemyColor.Purple:
                Inventory.Instance.CollectSmall(Inventory.ItemType.Wind);
                break;
            case EnemyColor.Blue:
                Inventory.Instance.CollectSmall(Inventory.ItemType.Ice);
                break;
            case EnemyColor.Yellow:
                Inventory.Instance.CollectSmall(Inventory.ItemType.Ray);
                break;
        }
        Debug.Log("Fragmento de color " + enemyColor + " añadido al inventario.");
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