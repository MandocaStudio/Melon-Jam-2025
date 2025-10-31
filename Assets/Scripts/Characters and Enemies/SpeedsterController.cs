using UnityEngine;

[RequireComponent(typeof(DamageDealer), typeof(EnemyFeedback))]
public class SpeedsterController : MonoBehaviour, IDamageable, ISlowable
{
    [Header("Stats")]
    [SerializeField] private int health = 2;
    public float moveSpeed = 2f;

    [Header("Loot")]
    [Tooltip("El prefab 2D de la esquirla que soltará")]
    [SerializeField] private GameObject shardDropPrefab;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    [Header("Referencias")]
    public int rowIndex;
    public Material FullMaterial;

    // Referencias internas
    private EnemyFeedback enemyFeedback;
    private float originalSpeed;
    private float currentSpeed;

    private void Start()
    {
        // --- Inicialización ---
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);

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
        health -= damageAmount;
        Debug.Log($"Velocista recibió {damageAmount} de daño. Salud restante: {health}");

        // 3. Comprobar muerte
        if (health <= 0)
        {
            // --- [NUEVO] Paso 2: Reportarse al GameManager al morir ---
            GameManager.activeEnemies--;
            Debug.Log($"Velocista destruido. Enemigos restantes: {GameManager.activeEnemies}");
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

        // 2. Inicializa la esquirla (le dice que es Grande y de Viento)
        shardInstance.GetComponent<ShardDrop2D>().Initialize(
            Inventory.ItemType.Wind,
            ShardDrop2D.ShardSize.Big
        );

        Debug.Log("Fragmento grande de viento soltado.");
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
            // (Chocar con la columna también es una "muerte")
            GameManager.activeEnemies--;
            Debug.Log($"Velocista chocó con columna. Enemigos restantes: {GameManager.activeEnemies}");
            Destroy(gameObject);
        }
    }
}