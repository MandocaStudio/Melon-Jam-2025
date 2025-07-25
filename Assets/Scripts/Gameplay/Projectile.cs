using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;  // Velocidad del proyectil
    public float lifetime = 10f;  // Tiempo de vida antes de destruirse (en segundos)
    private float lifeTimer;  // Temporizador para la vida del proyectil

    void Start()
    {
        lifeTimer = lifetime;  // Iniciar el temporizador con el tiempo de vida
    }

    void Update()
    {
        // Mover el proyectil horizontalmente
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Reducir el tiempo de vida del proyectil
        lifeTimer -= Time.deltaTime;

        // Destruir el proyectil después de un tiempo
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);  // Destruir el proyectil si ha pasado el tiempo de vida
        }
    }

    // Detectar colisión con los enemigos
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);  // Destruir el proyectil cuando impacta con un enemigo
            Destroy(collision.gameObject);  // Destruir el enemigo (opcional, dependiendo de la lógica de enemigos)
        }
    }
}
