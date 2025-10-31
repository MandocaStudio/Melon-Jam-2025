using UnityEngine;
using System.Collections; // Necesario para la corrutina

[RequireComponent(typeof(Collider))] // Nos aseguramos de que tenga un collider
public class FreezeSpell : MonoBehaviour
{
    [Header("Configuración del AOE")]
    [Tooltip("Tiempo total que el AOE permanece activo en la escena")]
    [SerializeField] private float aoeLifetime = 7f;
    
    [Tooltip("El multiplicador de velocidad (DEBE ser 0 para congelar)")]
    [SerializeField] private float speedMultiplier = 0f;

    // Referencia interna a nuestro propio collider
    private Collider myCollider;

    // Awake se usa para obtener referencias (solo se llama una vez)
    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    // OnEnable se llama CADA VEZ que el objeto se activa
    private void OnEnable()
    {
        // 1. Aseguramos que el collider esté encendido
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }

        // 2. Programamos la auto-desactivación
        StartCoroutine(DisableAfterTime());
    }

    // Corrutina para auto-desactivarse
    private IEnumerator DisableAfterTime()
    {
        // Espera el tiempo de vida
        yield return new WaitForSeconds(aoeLifetime);
        
        // Llama al método que lo apaga
        DisableSpell();
    }

    // --- Lógica de ISlowable ---

    // Cuando un enemigo ENTRA al área
    private void OnTriggerEnter(Collider other)
    {
        // Buscamos si el objeto puede ser ralentizado/congelado
        ISlowable slowableObject = other.GetComponent<ISlowable>();

        if (slowableObject != null)
        {
            // Le decimos que se congele (aplique multiplicador 0)
            slowableObject.ApplySpeedMultiplier(speedMultiplier);
        }
    }

    // --- NOTA DE CONGELACIÓN DEFINITIVA ---
    // Como quieres que la congelación sea definitiva,
    // hemos eliminado el método "OnTriggerExit".
    // Los enemigos que entren serán congelados y NUNCA
    // recibirán la orden de "ResetSpeed()".
    // ---

    // --- Lógica de Desactivación (Reemplaza a Destroy) ---
    
    private void DisableSpell()
    {
        // 1. Apagamos el GameObject.
        // Ahora está listo para ser "reciclado".
        gameObject.SetActive(false);
    }
    
    // (Opcional) Seguridad por si el objeto se apaga externamente
    private void OnDisable()
    {
        // Asegura que el collider esté apagado
        if (myCollider != null && myCollider.enabled)
        {
            myCollider.enabled = false;
        }
    }
}