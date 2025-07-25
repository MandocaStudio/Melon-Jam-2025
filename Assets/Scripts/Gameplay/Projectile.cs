using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 10f;
    private float lifeTimer;

    void Start()
    {
        lifeTimer = lifetime;
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detectado con: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Impacto con enemigo. Destruyendo proyectil...");
            Destroy(gameObject);
        }
    }
}
