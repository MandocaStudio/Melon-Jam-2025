using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;        // Velocidad del proyectil
    public float lifetime = 10f;     // Tiempo de vida antes de destruirse
    private float lifeTimer;         // Temporizador para la vida del proyectil

    public int damageToPlayer = 2;   // Daño que el proyectil hace al jugador

    void Start()
    {
        lifeTimer = lifetime;  // Iniciar el temporizador con el tiempo de vida
    }

    void Update()
    {
        // Mueve el proyectil hacia la izquierda (en la dirección de la columna del jugador)
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Reducir el tiempo de vida del proyectil
        lifeTimer -= Time.deltaTime;

        // Destruir el proyectil después de un tiempo
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug para verificar la colisión
        Debug.Log("Trigger detectado con: " + other.name);

        // Si colisiona con la columna del jugador
        if (other.CompareTag("PlayerColumn"))
        {
            Debug.Log("Impacto con la columna del jugador. Aplicando daño.");

            // Obtén el componente de la columna y aplica el daño
            ColumnHealthBar columnHealth = other.GetComponent<ColumnHealthBar>();
            if (columnHealth != null)
            {
                columnHealth.TakeDamage(damageToPlayer);  // Aplica el daño al jugador
            }

            // Destruir el proyectil después de impactar con la columna
            Destroy(gameObject);
        }

        // Si colisiona con un enemigo, destruimos el proyectil
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Impacto con enemigo. Destruyendo proyectil...");
            Destroy(gameObject);
        }
    }
}
