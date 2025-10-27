using UnityEngine;

// Este script congela permanentemente a cualquier enemigo
// que implemente la interfaz ISlowable.
public class FreezeSpell : MonoBehaviour
{
    [Tooltip("Tiempo total del AOE en escena")]
    [SerializeField] private float aoeLifetime = 7f;

    // 1. El multiplicador de velocidad para congelar es 0.
    private float speedMultiplier = 0f;

    // 2. [ELIMINADA] Toda la lógica de DisableAOE, Invoke y Collider.
    // 3. [ELIMINADO] El método OnTriggerExit().

    private void Start()
    {
        // 4. Volvemos al simple Destroy().
        //    El AOE desaparece, pero el efecto en el enemigo perdura.
        Destroy(gameObject, aoeLifetime);
    }

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

    // Al no tener OnTriggerExit, la llamada a slowableObject.ResetSpeed()
    // nunca ocurre, y el enemigo se queda congelado permanentemente.
}