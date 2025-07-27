using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Blink Targets")]
    public GameObject[] blinkObjects; // Objetos de la escena que definen los tiles
    private int lastBlinkIndex = -1;

    [Header("Configuración del Arquero")]
    public int health = 1;
    public float fireRate = 1.5f;
    public float blinkInterval = 3f;

    [Header("Proyectil")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    private float nextFireTime = 0f;
    private float blinkTimer = 0f;

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f); // Siempre rotado para vista isométrica
        BlinkToNewPosition(); // Primer blink inicial
    }

    private void Update()
    {
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
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(60f, 0f, 0f));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.left * projectileSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
    }

    private void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Archer took {dmg} damage. Remaining HP: {health}");

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
