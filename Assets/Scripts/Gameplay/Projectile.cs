// --- CÓDIGO MODIFICADO (Projectile.cs) ---
using UnityEngine;

// 1. Asegúrate de que el prefab de este proyectil tenga:
//    - Un Collider (marcado como Is Trigger)
//    - Un Rigidbody (marcado como Is Kinematic)
//    - El script DamageDealer.cs (con damageAmount = 10, por ejemplo)

[RequireComponent(typeof(DamageDealer))] // Opcional, pero buena práctica
public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 10f;
    private float lifeTimer;
    
    // Obtenemos la referencia a nuestro propio daño
    private DamageDealer myDamageDealer;

    void Start()
    {
        lifeTimer = lifetime;
        myDamageDealer = GetComponent<DamageDealer>();
        if (myDamageDealer == null)
        {
            Debug.LogError("¡Proyectil no tiene DamageDealer component!");
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
            Destroy(gameObject);
    }

    // --- LÓGICA DE DAÑO MEJORADA ---
    private void OnTriggerEnter(Collider other)
    {
        // Buscamos la interfaz IDamageable en lo que golpeamos (o en sus padres)
        IDamageable damageableObject = other.GetComponentInParent<IDamageable>();

        // Si el objeto que golpeamos PUEDE recibir daño (es un Enemigo, una Columna, etc.)
        if (damageableObject != null)
        {
            // Le aplicamos el daño que tenemos en nuestro componente DamageDealer
            damageableObject.TakeDamage(myDamageDealer.damageAmount);

            // Y nos destruimos
            Destroy(gameObject);
        }

        // (Opcional) Si quieres que el proyectil se destruya
        // contra muros que no son "damageable", añade otra lógica aquí.
        // if (other.CompareTag("Wall")) { Destroy(gameObject); }
    }
}