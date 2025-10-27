using UnityEngine;

// (Archivo: Thunder Thunder.cs)
public class ThunderBeamSpell : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damageAmount = 100;

    private void Start()
    {
        // El hechizo se autodestruye después de su vida útil
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Se mueve hacia la derecha (hacia los enemigos)
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
    }

    // --- LÓGICA DE TRIGGER CORREGIDA ---
    private void OnTriggerEnter(Collider other)
    {
        // 1. Filtramos solo por enemigos
        if (other.CompareTag("Enemy"))
        {
            // 2. Buscamos si el enemigo puede recibir daño (interfaz)
            //    Usamos GetComponentInParent por si el collider está en un objeto hijo.
            IDamageable damageableObject = other.GetComponentInParent<IDamageable>();

            if (damageableObject != null)
            {
                // 3. ¡Adiós Reflection! Aplicamos el daño.
                //    El enemigo (en su propio script TakeDamage)
                //    se encargará de revisar su vida y destruirse.
                damageableObject.TakeDamage(damageAmount);
            }
        }

        // Nota: El rayo atraviesa a los enemigos porque no hay
        // un "Destroy(gameObject);" dentro del if.
        // Seguirá golpeando enemigos hasta que 'lifetime' termine.
    }
}