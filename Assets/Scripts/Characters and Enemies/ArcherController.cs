using UnityEngine;

// 1. [REQUERIDO] Añadimos esto. Para que el enemigo haga daño por
//    contacto a la columna (como definimos en ColumnHealthBar.cs).
[RequireComponent(typeof(DamageDealer))]
public class ShooterController : MonoBehaviour, IDamageable // 2. [REQUERIDO] Implementamos la interfaz
{
    [Header("Blink Targets")]
    public GameObject[] blinkObjects;
    private int lastBlinkIndex = -1;

    [Header("Configuración del Arquero")]
    // 3. [BUENA PRÁCTICA] Cambiado a private con SerializeField.
    //    Ya nada externo necesita acceder a 'health' directamente.
    [SerializeField] private int health = 1;
    public float fireRate = 1.5f;
    public float blinkInterval = 3f;

    [Header("Proyectil")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f; // Esta variable ya no se usa aquí,
                                        // pero la dejamos por si la usa otro script.

    private float nextFireTime = 0f;
    private float blinkTimer = 0f;

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        BlinkToNewPosition();
    }

    private void Update()
    {
        // 4. [BUENA PRÁCTICA] Comprobación de vida al inicio del Update.
        //    (Ya lo tenías, ¡excelente!)
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

    private void BlinkToNewPosition()
    {
        if (blinkObjects == null || blinkObjects.Length == 0) return;

        int newIndex = Random.Range(0, blinkObjects.Length);
        while (newIndex == lastBlinkIndex && blinkObjects.Length > 1)
            newIndex = Random.Range(0, blinkObjects.Length);

        transform.position = blinkObjects[newIndex].transform.position;
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        lastBlinkIndex = newIndex;

        Debug.Log($"Archer blinked to object {newIndex}.");
    }

    private void ShootProjectile()
    {
        Vector3 spawnPosition = transform.position + new Vector3(1f, 0f, 0f);
        
        // 5. [CORRECCIÓN] Solo instanciamos. No controlamos su Rigidbody.
        //    El script "EnemyProjectile.cs" ya se encarga de su propio
        //    movimiento usando transform.Translate.
        Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(60f, 0f, 0f));
    }

    // 6. [ELIMINADO] Este método ya no es necesario.
    /*
    private void OnTriggerEnter(Collider other)
    {
        // El script "Projectile.cs" (del jugador) ahora
        // es el responsable de detectar al "Enemy" y
        // llamar a "TakeDamage"
    }
    */

    // 7. [REQUERIDO] Este método ahora implementa la interfaz
    //    Debe ser 'public' y el parámetro debe coincidir.
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"Archer took {damageAmount} damage. Remaining HP: {health}");

        if (health <= 0)
        {
            Debug.Log("Archer destroyed");
            DropShard();
            Destroy(gameObject);
        }
    }

    private void DropShard()
    {
        Inventory.Instance.CollectBig(Inventory.ItemType.Ray);
        Debug.Log("Fragmento grande de rayo añadido al inventario.");
    }
}