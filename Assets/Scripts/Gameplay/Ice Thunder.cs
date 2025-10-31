using UnityEngine;
using System.Collections.Generic; // ¡Necesario para la lista!
using System.Collections;       // ¡Necesario para la corrutina!

[RequireComponent(typeof(Collider))] // Nos aseguramos de que tenga un collider
public class IceThunderCone : MonoBehaviour
{
    [Header("Configuración del Cono")]
    [Tooltip("Tiempo que el cono permanece activo en el mundo")]
    [SerializeField] private float lifetime = 3.0f;
    
    [Header("Efecto Hielo (Slow)")]
    [Tooltip("Multiplicador de velocidad (ej: 0.3 = 70% más lento)")]
    [SerializeField] private float speedMultiplier = 0.3f; 

    [Header("Efecto Trueno (Damage)")]
    [Tooltip("Daño que se aplica UNA VEZ al entrar")]
    [SerializeField] private int damageAmount = 25;
    
    // --- Referencias Internas ---
    private Collider myCollider;
    
    // Esta lista rastrea a los enemigos que ya han sido dañados
    // para evitar que reciban daño varias veces.
    private List<Collider> alreadyDamagedEnemies;

    // Awake se usa para obtener referencias (solo se llama una vez)
    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        // Inicializamos la lista aquí
        alreadyDamagedEnemies = new List<Collider>();
    }

    // OnEnable se llama CADA VEZ que el objeto se activa
    private void OnEnable()
    {
        // 1. Reseteamos la lista de enemigos dañados
        alreadyDamagedEnemies.Clear();

        // 2. Aseguramos que el collider esté encendido
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }

        // 3. Programamos la auto-desactivación
        StartCoroutine(DisableAfterTime());
    }

    // Corrutina para auto-desactivarse
    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        DisableSpell();
    }

    // --- Lógica de Hielo (Slow) y Trueno (Damage) ---

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

    // --- Lógica de Desactivación (Reemplaza a Destroy) ---
    
    private void DisableSpell()
    {
        // 1. [CRÍTICO] Apagamos el collider primero.
        // Esto fuerza a que OnTriggerExit() se llame en todos los
        // enemigos que siguen dentro, reseteando su velocidad.
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // 2. Apagamos el GameObject.
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        // Seguridad por si el objeto se apaga externamente
        if (myCollider != null && myCollider.enabled)
        {
            myCollider.enabled = false;
        }
    }
}