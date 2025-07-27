using UnityEngine;

public class IceTankController : MonoBehaviour
{
    [Header("Configuración del Tanque")]
    public int health = 4;
    public int shieldHits = 3;
    public int damageToPlayer = 1;
    public float moveSpeed = 1f;

    public static int maxActiveTanks = 2;
    public static int[] activeTanksPerRow = new int[5];
    public static int currentTankCount = 0;

    private bool isShieldActive = true;
    private bool hasReachedPlayer = false;

    public int rowIndex;

    public Material blueFullMaterial;  
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private void Start()
    {
        AssignRowIndex();

        if (currentTankCount >= maxActiveTanks || !IsRowIndexValid(rowIndex) || activeTanksPerRow[rowIndex] > 0)
        {
            Debug.LogWarning("No se puede crear más tanques en esta fila o en la escena.");
            Destroy(gameObject);
            return;
        }

        currentTankCount++;
        activeTanksPerRow[rowIndex]++;
        ActivateShieldAura();

        // Rotación X = 60 grados
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
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

    private bool IsRowIndexValid(int rowIndex)
    {
        return rowIndex >= 0 && rowIndex < activeTanksPerRow.Length;
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
            ColumnHealthBar columnHealth = playerColumn.GetComponentInParent<ColumnHealthBar>();
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
            DropShard();  // DROP DE MATERIAL
            Cleanup();
        }
    }

    private void DropShard()
    {
        // Se añade un shard de "viento" al inventario dependiendo de la cantidad de bigCount
        if (Inventory.Instance.inventory[(int)Inventory.ItemType.Wind].bigCount > 0)
        {
            // Añadir un shard grande al inventario si el bigCount lo permite
            Inventory.Instance.CollectBig(Inventory.ItemType.Wind);
            Debug.Log("Fragmento grande de viento añadido al inventario.");
        }
        else
        {
            // De lo contrario, añadir un shard pequeño
            Inventory.Instance.CollectSmall(Inventory.ItemType.Wind);
            Debug.Log("Fragmento pequeño de viento añadido al inventario.");
        }
    }

    private void Cleanup()
    {
        currentTankCount--;
        if (IsRowIndexValid(rowIndex))
            activeTanksPerRow[rowIndex]--;
        Destroy(gameObject);
    }

    private void AssignRowIndex()
    {
        float spawnY = transform.position.y;

        if (spawnY >= 3.0f)
            rowIndex = 4;
        else if (spawnY >= 2.0f)
            rowIndex = 3;
        else if (spawnY >= 1.0f)
            rowIndex = 2;
        else if (spawnY >= 0.0f)
            rowIndex = 1;
        else
            rowIndex = 0;

        Debug.Log("Tanque en fila: " + rowIndex);
    }
}
