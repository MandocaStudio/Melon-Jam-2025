using UnityEngine;
using System.Collections.Generic; // ¡Necesario para la lista!

// (Archivo: Ice Thunder.cs)
public class IceThunderCone : MonoBehaviour
{
    [Header("Cone Settings")]
    [Tooltip("Tiempo que el cono permanece en el mundo")]
    [SerializeField] private float lifetime = 3.0f;
    
    [Header("Ice Effect (Slow)")]
    [Tooltip("Multiplicador de velocidad (ej: 0.3 = 70% más lento)")]
    [SerializeField] private float speedMultiplier = 0.3f; 

    [Header("Thunder Effect (Damage)")]
    [Tooltip("Daño que se aplica UNA VEZ al entrar")]
    [SerializeField] private int damageAmount = 25;
    
    // --- System Internals ---
    private Collider myCollider;
    
    // Esta lista rastrea a los enemigos que ya han sido dañados
    // para evitar que reciban daño varias veces.
    private List<Collider> alreadyDamagedEnemies;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        if (myCollider == null) {
            Debug.LogError("¡IceThunderCone no tiene Collider!");
            return;
        }
        
        // Inicializamos la lista
        alreadyDamagedEnemies = new List<Collider>();

        // Usamos el patrón de 'Invoke' para desactivar de forma segura
        Invoke(nameof(DisableAOE), lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo nos importan los enemigos
        if (!other.CompareTag("Enemy")) return; 

        // --- 1. Aplicar Ralentización (Ice) ---
        ISlowable slowableObject = other.GetComponentInParent<ISlowable>();
        if (slowableObject != null)
        {
            slowableObject.ApplySpeedMultiplier(speedMultiplier);
        }

        // --- 2. Aplicar Daño (Thunder) ---
        // Comprobamos si ya hemos dañado a este enemigo
        if (alreadyDamagedEnemies.Contains(other))
        {
            return; // Si ya está en la lista, no hacemos nada
        }

        // Si no está en la lista, le hacemos daño
        IDamageable damageableObject = other.GetComponentInParent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damageAmount);
            
            // Añadimos al enemigo a la lista para no volver a dañarlo
            alreadyDamagedEnemies.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Solo nos importan los enemigos
        if (!other.CompareTag("Enemy")) return; 

        // --- 3. Quitar Ralentización (Ice) ---
        ISlowable slowableObject = other.GetComponentInParent<ISlowable>();
        if (slowableObject != null)
        {
            // El enemigo salió del cono, restauramos su velocidad
            slowableObject.ResetSpeed();
        }
    }

    private void DisableAOE()
    {
        // Este patrón asegura que OnTriggerExit se llame para todos
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // Destruimos el objeto un segundo después
        Destroy(gameObject, 1f);
    }
}