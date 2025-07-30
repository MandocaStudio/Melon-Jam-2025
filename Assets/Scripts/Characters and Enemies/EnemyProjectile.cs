using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 10f;
    private float lifeTimer;
    public int damageToPlayer = 1;

    void Start()
    {
        lifeTimer = lifetime;
        transform.tag = "EnemyProjectile";
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerColumn"))
        {
            ColumnHealthBar columnHealth = other.GetComponent<ColumnHealthBar>();
            if (columnHealth != null)
                columnHealth.TakeDamage(damageToPlayer);

            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
