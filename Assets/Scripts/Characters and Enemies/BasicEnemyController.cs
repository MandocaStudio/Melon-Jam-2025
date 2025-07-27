using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public int maxHealth = 3;

    // Materiales correspondientes a los shards (para ser añadidos al inventario)
    public Material purpleShardMaterial, blueShardMaterial, yellowShardMaterial;  
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private int currentHealth;
    private bool killedByProjectile = false;

    public enum EnemyColor { Purple, Blue, Yellow }
    [HideInInspector] public EnemyColor enemyColor;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyColor = GetRandomColor();

        // Asegúrate de que el enemigo tenga la rotación correcta
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);  // Rotación de 60 grados en el eje X
    }

    private void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);  // Movimiento constante del enemigo
    }

    private EnemyColor GetRandomColor()
    {
        int rand = Random.Range(0, 3);
        return (EnemyColor)rand;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerColumn"))
        {
            ColumnHealthBar playerHealth = other.GetComponentInParent<ColumnHealthBar>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);

            Destroy(gameObject);  // Destruye el enemigo al colisionar con la columna
        }

        if (other.CompareTag("Projectile"))
        {
            Debug.Log("Enemigo recibió impacto de proyectil");
            killedByProjectile = true;
            TakeDamage(1);
            Destroy(other.gameObject);  // Destruye el proyectil
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Daño recibido: " + amount + " | Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Enemigo destruido");
            if (killedByProjectile)
            {
                DropShard();  // Llama al método para añadir el shard al inventario
            }
            Destroy(gameObject);  // Destruye el enemigo
        }
    }

    private void DropShard()
    {
        // Se asigna el material del shard correspondiente al inventario según el color del enemigo
        switch (enemyColor)
        {
            case EnemyColor.Purple:
                Inventory.Instance.CollectSmall(Inventory.ItemType.Wind);  // Asignar material púrpura (shard)
                break;
            case EnemyColor.Blue:
                Inventory.Instance.CollectSmall(Inventory.ItemType.Ice);  // Asignar material azul (shard)
                break;
            case EnemyColor.Yellow:
                Inventory.Instance.CollectSmall(Inventory.ItemType.Ray);  // Asignar material amarillo (shard)
                break;
        }

        Debug.Log("Fragmento de color " + enemyColor + " añadido al inventario.");
    }
}
