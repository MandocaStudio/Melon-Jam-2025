using UnityEngine;

[RequireComponent(typeof(DamageDealer), typeof(EnemyFeedback))]
public class IceTankController : MonoBehaviour, IDamageable, ISlowable
{
    [Header("Stats")]
    [SerializeField] private int health = 4;
    [SerializeField] private int shieldHits = 3; 
    public float moveSpeed = 1f;

    [Header("Loot")]
    [Tooltip("El prefab 2D de la esquirla que soltará")]
    [SerializeField] private GameObject shardDropPrefab;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    [Header("Referencias")]
    public int rowIndex;
    public Material blueFullMaterial;

    // Referencias internas
    private bool isShieldActive = true;
    private EnemyFeedback enemyFeedback;
    private float originalSpeed;
    private float currentSpeed;

    private void Start()
    {
        // --- Inicialización ---
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        ActivateShieldAura();

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
        // 1. Mostrar feedback visual (incluso si el escudo lo absorbe)
        if (enemyFeedback != null)
        {
            enemyFeedback.PlayHitEffect();
        }

        // 2. Lógica del Escudo
        if (isShieldActive)
        {
            shieldHits--;
            Debug.Log($"Escudo absorbió un impacto. Restan {shieldHits}");
            if (shieldHits <= 0)
            {
                isShieldActive = false;
                Debug.Log("Escudo destruido");
            }
            return; // El daño no pasa a la vida
        }

        // 3. Aplicar daño si el escudo está roto
        health -= damageAmount;
        Debug.Log($"Tanque recibió {damageAmount} de daño. Salud restante: {health}");

        // 4. Comprobar muerte
        if (health <= 0)
        {
            // --- [NUEVO] Paso 2: Reportarse al GameManager al morir ---
            GameManager.activeEnemies--;
            Debug.Log($"Tanque destruido. Enemigos restantes: {GameManager.activeEnemies}");
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

        // 1. Instancia el prefab
        GameObject shardInstance = Instantiate(
            shardDropPrefab,
            transform.position + dropOffset,
            Quaternion.identity
        );

        // 2. Inicializa la esquirla (le dice que es Grande y de Hielo)
        shardInstance.GetComponent<ShardDrop2D>().Initialize(
            Inventory.ItemType.Ice,
            ShardDrop2D.ShardSize.Big
        );

        Debug.Log("Fragmento grande de hielo soltado.");
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

    // --- Lógica de Colisión (Autodestrucción) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerColumn"))
        {
            // --- [NUEVO] Paso 3: Reportarse al GameManager al chocar ---
            GameManager.activeEnemies--;
            Debug.Log($"Tanque chocó con columna. Enemigos restantes: {GameManager.activeEnemies}");
            Destroy(gameObject);
        }
    }

    // --- Otros Métodos ---
    private void ActivateShieldAura()
    {
        Debug.Log($"Escudo activado en la fila {rowIndex}, bloqueando {shieldHits} impactos.");
    }
}