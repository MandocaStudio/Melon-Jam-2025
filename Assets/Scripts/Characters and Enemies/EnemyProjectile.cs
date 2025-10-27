using UnityEngine;

// 1. Nos aseguramos de que el componente DamageDealer esté en el prefab
[RequireComponent(typeof(DamageDealer))]
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 10f;
    private float lifeTimer;
    
    // 2. ¡YA NO NECESITAMOS ESTA VARIABLE!
    // public int damageToPlayer = 1; 
    // El daño ahora se lee desde el componente DamageDealer.

    // 3. Referencia a nuestro componente de daño
    private DamageDealer myDamageDealer;

    void Start()
    {
        lifeTimer = lifetime;
        // Obtenemos nuestro componente DamageDealer
        myDamageDealer = GetComponent<DamageDealer>(); 

        // Es mejor asignar el Tag en el Inspector del Prefab, 
        // pero si lo tenías en el script, lo dejamos.
        transform.tag = "EnemyProjectile";
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
            Destroy(gameObject);
    }

    // --- LÓGICA DE TRIGGER ACTUALIZADA Y SIMPLIFICADA ---
    private void OnTriggerEnter(Collider other)
    {
        // 1. ¿Golpeamos la columna? 
        //    (Uso el tag "PlayerColumn" que tenías en tu script original)
        if (other.CompareTag("PlayerColumn"))
        {
            // 2. Buscamos si la columna puede recibir daño (interfaz IDamageable)
            IDamageable damageableObject = other.GetComponentInParent<IDamageable>();
            
            if (damageableObject != null)
            {
                // 3. Aplicamos el daño (leído desde myDamageDealer.damageAmount)
                damageableObject.TakeDamage(myDamageDealer.damageAmount);
            }
            
            // 4. Nos destruimos al impactar
            Destroy(gameObject);
        }
        // 5. ¿Golpeamos a otro enemigo? (Fuego amigo)
        else if (other.CompareTag("Enemy"))
        {
            // 6. Nos destruimos SIN hacer daño
            Destroy(gameObject);
        }

        // Si golpea cualquier otra cosa (como el Proyectil del Jugador),
        // simplemente la ignora y sigue su camino.
    }
}