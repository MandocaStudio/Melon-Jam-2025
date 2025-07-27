using UnityEngine;

public class SpeedsterController : MonoBehaviour
{
    [Header("Configuración del Velocista")]
    public int health = 2;
    public int damageToPlayer = 1;
    public float moveSpeed = 2f;

    public int rowIndex; // Asignado externamente al spawnear
    public Material FullMaterial;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private bool hasReachedPlayer = false;

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f); // Mantiene la rotación estándar
    }

    private void Update()
    {
        if (!hasReachedPlayer)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            if (transform.position.x <= 0) // Ajusta según tu columna del jugador
            {
                hasReachedPlayer = true;
                ReachPlayerColumn();
            }
        }
    }

    private void ReachPlayerColumn()
    {
        Debug.Log("Velocista ha alcanzado la columna del jugador.");

        GameObject playerColumn = GameObject.Find("PlayerColumn");
        if (playerColumn != null)
        {
            ColumnHealthBar columnHealth = playerColumn.GetComponent<ColumnHealthBar>();
            if (columnHealth != null)
            {
                columnHealth.TakeDamage(damageToPlayer);
            }
        }

        Destroy(gameObject);
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
        Debug.Log($"Velocista recibió {dmg} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Velocista destruido");
            DropShard();
            Destroy(gameObject);
        }
    }

    private void DropShard()
    {
        if (Inventory.Instance.inventory[(int)Inventory.ItemType.Wind].bigCount > 0)
        {
            Inventory.Instance.CollectBig(Inventory.ItemType.Wind);
            Debug.Log("Fragmento grande de viento añadido al inventario.");
        }
        else
        {
            Inventory.Instance.CollectSmall(Inventory.ItemType.Wind);
            Debug.Log("Fragmento pequeño de viento añadido al inventario.");
        }
    }
}
