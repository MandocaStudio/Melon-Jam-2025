using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageDealer), typeof(EnemyFeedback), typeof(Animator))] 
public class ArcherController : MonoBehaviour, IDamageable 
{
    [Header("Blink Targets")]
    private GameObject[] blinkObjects; 

    // --- [CORREGIDO] ---
    // La variable 'lastBlinkIndex' solo debe declararse UNA VEZ.
    // La he movido aquí abajo con las otras variables internas.
    // private int lastBlinkIndex = -1; // <-- Esta era la declaración duplicada

    [Header("Configuración del Arquero")]
    [SerializeField] private int health = 1;
    public float fireRate = 5f;
    public float blinkInterval = 5f;

    [Header("Proyectil")]
    public GameObject projectilePrefab;

    [Header("Loot")]
    [SerializeField] private GameObject shardDropPrefab;

    // --- Referencias internas ---
    private int lastBlinkIndex = -1; // <-- Declarada UNA SOLA VEZ aquí
    private float nextFireTime = 0f;
    private float blinkTimer = 0f;
    private EnemyFeedback enemyFeedback;
    private Animator animator; 

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        enemyFeedback = GetComponent<EnemyFeedback>();
        animator = GetComponent<Animator>(); 

        // Busca sus propios puntos de salto
        blinkObjects = GameObject.FindGameObjectsWithTag("BlinkPoint");
        if (blinkObjects.Length == 0)
        {
            Debug.LogError("¡ArcherController no encontró GameObjects con el tag 'BlinkPoint'!");
            enabled = false; 
        }
        
        BlinkToNewPosition(); 
        GameManager.activeEnemies++;
    }

    private void Update()
    {
        if (health <= 0) return;

        // Disparo
        if (Time.time >= nextFireTime)
        {
            ShootProjectile();
            nextFireTime = Time.time + fireRate;
        }

        // Blink
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
        if (health <= 0) return;
        if (enemyFeedback != null) enemyFeedback.PlayHitEffect();
        health -= damageAmount;
        
        Debug.Log($"Arquero recibió {damageAmount} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            GameManager.activeEnemies--;
            Debug.Log($"Arquero destruido. Enemigos restantes: {GameManager.activeEnemies}");
            DropShard();
            Destroy(gameObject);
        }
    }

    // --- Lógica de Loot ---
    private void DropShard()
    {
        if (shardDropPrefab == null) return;
        GameObject shardInstance = Instantiate(shardDropPrefab, transform.position, Quaternion.identity);
        shardInstance.GetComponent<ShardDrop2D>().Initialize(Inventory.ItemType.Ray, ShardDrop2D.ShardSize.Big);
    }
    
    // --- Habilidades ---
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
        // 1. Llama al trigger de la animación
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 2. Spawnea el proyectil
        Vector3 spawnPosition = transform.position + new Vector3(1f, 0f, 0f);
        Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(60f, 0f, 0f));
    }
}