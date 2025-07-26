using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public int maxHealth = 3;

    public Sprite whiteSprite, purpleSprite, blueSprite, yellowSprite;
    public GameObject purpleShard, blueShard, yellowShard;
    public Vector3 dropOffset = new Vector3(-0.3f, 0, 0);

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private bool killedByProjectile = false;

    public enum EnemyColor { White, Purple, Blue, Yellow }
    [HideInInspector] public EnemyColor enemyColor;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyColor = GetRandomColor();
        AsignarColorYSprite();
    }

    private void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    private EnemyColor GetRandomColor()
    {
        int rand = Random.Range(0, 4);
        return (EnemyColor)rand;
    }

    private void AsignarColorYSprite()
    {
        switch (enemyColor)
        {
            case EnemyColor.White: spriteRenderer.sprite = whiteSprite; break;
            case EnemyColor.Purple: spriteRenderer.sprite = purpleSprite; break;
            case EnemyColor.Blue: spriteRenderer.sprite = blueSprite; break;
            case EnemyColor.Yellow: spriteRenderer.sprite = yellowSprite; break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemigo detectó colisión con: " + other.name);

        if (other.CompareTag("PlayerColumn"))
        {
            ColumnHealthBar playerHealth = other.GetComponentInParent<ColumnHealthBar>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);

            Destroy(gameObject);
        }

        if (other.CompareTag("Projectile"))
        {
            Debug.Log("Enemigo recibió impacto de proyectil");
            killedByProjectile = true;
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Daño recibido: " + amount + " | Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Enemigo destruido");
            if (killedByProjectile) DropShard();
            Destroy(gameObject);
        }
    }

    private void DropShard()
    {
        GameObject shardToDrop = null;

        switch (enemyColor)
        {
            case EnemyColor.Purple: shardToDrop = purpleShard; break;
            case EnemyColor.Blue: shardToDrop = blueShard; break;
            case EnemyColor.Yellow: shardToDrop = yellowShard; break;
        }

        if (shardToDrop != null)
        {
            Instantiate(shardToDrop, transform.position + dropOffset, Quaternion.identity);
            Debug.Log("Shard generado");
        }
    }
}
