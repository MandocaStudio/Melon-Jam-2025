using UnityEngine;

public class IceTankController : MonoBehaviour
{
    [Header("Configuración del Tanque")]
    public int health = 4;
    public int shieldHits = 3;
    public int damageToPlayer = 1;
    public float moveSpeed = 1f;

    public int rowIndex; // Fila asignada por el spawner
    private bool isShieldActive = true;
    private bool hasReachedPlayer = false;

    public Material blueFullMaterial;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private void Start()
    {
        transform.tag = "Enemy";
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        ActivateShieldAura();
        Debug.Log("Tanque en fila: " + rowIndex);
    }

    private void Update()
    {
        if (!hasReachedPlayer)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            if (transform.position.x <= 0)
            {
                hasReachedPlayer = true;
                ReachPlayerColumn();
            }
        }
    }

    private void ActivateShieldAura()
    {
        Debug.Log($"Escudo activado en la fila {rowIndex}, bloqueando {shieldHits} impactos.");
    }

    private void ReachPlayerColumn()
    {
        Debug.Log("Tanque ha alcanzado la columna del jugador.");

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

            if (isShieldActive)
            {
                shieldHits--;
                Debug.Log($"Escudo absorbió un impacto. Restan {shieldHits}");

                if (shieldHits <= 0)
                {
                    isShieldActive = false;
                    Debug.Log("Escudo destruido");
                }
            }
            else
            {
                TakeDamage(1);
            }
        }
    }

    private void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Tanque recibió {dmg} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Debug.Log("Tanque destruido");
            DropShard();
            Destroy(gameObject);
        }
    }

    private void DropShard()
    {
        if (Inventory.Instance.inventory[(int)Inventory.ItemType.Ice].bigCount > 0)
        {
            Inventory.Instance.CollectBig(Inventory.ItemType.Ice);
            Debug.Log("Fragmento grande de hielo añadido al inventario.");
        }
        else
        {
            Inventory.Instance.CollectSmall(Inventory.ItemType.Ice);
            Debug.Log("Fragmento pequeño de hielo añadido al inventario.");
        }
    }
}
