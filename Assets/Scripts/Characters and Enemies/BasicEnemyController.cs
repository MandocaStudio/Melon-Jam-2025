using UnityEngine;

// Asegúrate de que las interfaces IDamageable e ISlowable están implementadas
[RequireComponent(typeof(DamageDealer), typeof(EnemyFeedback))]
public class BasicEnemyController : MonoBehaviour, IDamageable, ISlowable
{
    [Header("Stats")]
    public float moveSpeed = 1.5f;
    public int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Loot")]
    [Tooltip("El prefab 2D de la esquirla que soltará")]
    [SerializeField] private GameObject shardDropPrefab; 
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);
    
    // Enum para el tipo de loot
    public enum EnemyColor { Purple, Blue, Yellow }
    [HideInInspector] public EnemyColor enemyColor;

    // Referencias internas
    private EnemyFeedback enemyFeedback; 
    private float originalSpeed; 
    private float currentSpeed;  

    private void Start()
    {
        // --- Inicialización ---
        currentHealth = maxHealth;
        enemyColor = GetRandomColor();
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        transform.tag = "Enemy";

        // --- Inicialización de Módulos ---
        originalSpeed = moveSpeed; 
        currentSpeed = originalSpeed;
        enemyFeedback = GetComponent<EnemyFeedback>();

        // --- [NUEVO] Paso 1: Reportarse al GameManager al nacer ---
        GameManager.activeEnemies++;
    }

    private void Update()
    {
        // Se mueve usando la velocidad actual (que puede ser modificada por ISlowable)
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
    }

    // --- Lógica de Daño (IDamageable) ---
    public void TakeDamage(int damageAmount)
    {
        // 1. Mostrar feedback visual
        if (enemyFeedback != null)
        {
            enemyFeedback.PlayHitEffect();
        }

        // 2. Aplicar daño
        currentHealth -= damageAmount;
        Debug.Log($"Básico recibió {damageAmount} daño. Vida: {currentHealth}");

        // 3. Comprobar muerte
        if (currentHealth <= 0)
        {
            // --- [NUEVO] Paso 2: Reportarse al GameManager al morir ---
            GameManager.activeEnemies--;
            Debug.Log($"Básico destruido. Enemigos restantes: {GameManager.activeEnemies}");
            // --- [FIN DE LO NUEVO] ---

            // Soltar loot y destruirse
            DropShard();
            Destroy(gameObject);
        }
    }

    // --- Lógica de Loot ---
    private void DropShard()
    {
        if (shardDropPrefab == null) return;

        // Determinar qué tipo de esquirla soltar
        Inventory.ItemType dropType;
        switch (enemyColor)
        {
            case EnemyColor.Purple: dropType = Inventory.ItemType.Wind; break;
            case EnemyColor.Blue:   dropType = Inventory.ItemType.Ice; break;
            case EnemyColor.Yellow: dropType = Inventory.ItemType.Ray; break;
            default:                dropType = Inventory.ItemType.Wind; break;
        }

        // Instanciar el prefab de la esquirla 2D
        GameObject shardInstance = Instantiate(
            shardDropPrefab, 
            transform.position + dropOffset, 
            Quaternion.identity
        );
        
        // Inicializar la esquirla (decirle qué es)
        shardInstance.GetComponent<ShardDrop2D>().Initialize(dropType, ShardDrop2D.ShardSize.Small);
    }

    // --- Lógica de Ralentización (ISlowable) ---
    public void ApplySpeedMultiplier(float multiplier) 
    {
        currentSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeed() 
    {
        currentSpeed = originalSpeed;
    }

    // --- Otros Métodos ---
    private EnemyColor GetRandomColor()
    {
        int rand = Random.Range(0, 3);
        return (EnemyColor)rand;
    }

    // Lógica de colisión (solo para autodestruirse contra la columna)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerColumn"))
        {
            // Al chocar, el enemigo se destruye
            // (La columna ya maneja recibir el daño por su lado)
            // PERO, debemos reportar la muerte al GameManager
            GameManager.activeEnemies--;
            Debug.Log($"Básico chocó con columna. Enemigos restantes: {GameManager.activeEnemies}");
            Destroy(gameObject);
        }
    }
}