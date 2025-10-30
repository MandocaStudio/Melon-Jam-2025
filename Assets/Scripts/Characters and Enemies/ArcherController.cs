using UnityEngine;
using System.Collections; // Necesario para la corrutina

[RequireComponent(typeof(DamageDealer), typeof(EnemyFeedback))]
public class ShooterController : MonoBehaviour, IDamageable // Solo implementa IDamageable
{
    [Header("Blink Targets")]
    public GameObject[] blinkObjects; 
    private int lastBlinkIndex = -1;

    [Header("Configuración del Arquero")]
    [SerializeField] private int health = 1; // Encapsulado
    public float fireRate = 1.5f;
    public float blinkInterval = 3f;

    [Header("Proyectil")]
    public GameObject projectilePrefab;

    [Header("Loot")]
    [Tooltip("El prefab 2D de la esquirla que soltará")]
    [SerializeField] private GameObject shardDropPrefab;

    // Referencias internas
    private float nextFireTime = 0f;
    private float blinkTimer = 0f;
    private EnemyFeedback enemyFeedback;

    private void Start()
    {
        // --- Inicialización ---
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        
        // --- Inicialización de Módulos ---
        enemyFeedback = GetComponent<EnemyFeedback>();
        BlinkToNewPosition(); // Primer blink inicial

        // --- [NUEVO] Paso 1: Reportarse al GameManager al nacer ---
        GameManager.activeEnemies++;
    }

    private void Update()
    {
        // Si está muerto, no hace nada
        if (health <= 0) return;

        // Disparo del arquero
        if (Time.time >= nextFireTime)
        {
            ShootProjectile();
            nextFireTime = Time.time + fireRate;
        }

        // Blink a nuevo tile
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            BlinkToNewPosition();
        }
    }

    // --- Lógica de Daño (IDamageable) ---
    public void TakeDamage(int damageAmount)
    {
        // Si ya está muerto, no puede recibir más daño
        if (health <= 0) return;

        // 1. Mostrar feedback visual
        if (enemyFeedback != null)
        {
            enemyFeedback.PlayHitEffect();
        }

        // 2. Aplicar daño
        health -= damageAmount;
        Debug.Log($"Arquero recibió {damageAmount} de daño. Salud restante: {health}");

        // 3. Comprobar muerte
        if (health <= 0)
        {
            // --- [NUEVO] Paso 2: Reportarse al GameManager al morir ---
            GameManager.activeEnemies--;
            Debug.Log($"Arquero destruido. Enemigos restantes: {GameManager.activeEnemies}");
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
            transform.position,
            Quaternion.identity
        );

        // 2. Inicializa la esquirla (le dice que es Grande y de Rayo)
        shardInstance.GetComponent<ShardDrop2D>().Initialize(
            Inventory.ItemType.Ray,
            ShardDrop2D.ShardSize.Big
        );

        Debug.Log("Fragmento grande de rayo soltado.");
    }
    
    // --- Métodos de Habilidad ---
    private void BlinkToNewPosition()
    {
        if (blinkObjects == null || blinkObjects.Length == 0) return;

        int newIndex = Random.Range(0, blinkObjects.Length);
        while (newIndex == lastBlinkIndex && blinkObjects.Length > 1)
            newIndex = Random.Range(0, blinkObjects.Length);

        transform.position = blinkObjects[newIndex].transform.position;
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        lastBlinkIndex = newIndex;
    }

    private void ShootProjectile()
    {
        // El script EnemyProjectile.cs se encarga de su propio movimiento
        Vector3 spawnPosition = transform.position + new Vector3(1f, 0f, 0f);
        Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(60f, 0f, 0f));
    }
}