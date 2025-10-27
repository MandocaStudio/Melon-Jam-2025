// SlowAOE.cs (Antes Cloud Ice.cs)
using UnityEngine;

public class SlowAOE : MonoBehaviour
{
    [Tooltip("Tiempo total del AOE en escena")]
    public float aoeLifetime = 7f;          
    
    [Tooltip("Multiplicador de velocidad (ej. 0.1 = 10% de velocidad)")]
    public float speedMultiplier = 0.1f;    

    // 1. ELIMINAMOS effectDuration y slowedSpeed
    // 2. ELIMINAMOS la corrutina ApplySlow

    private void Start()
    {
        Destroy(gameObject, aoeLifetime);   // El AOE desaparece automáticamente
    }

    // 3. Lógica cuando un enemigo ENTRA
    private void OnTriggerEnter(Collider other)
    {
        // Buscamos si el objeto puede ser ralentizado
        ISlowable slowableObject = other.GetComponent<ISlowable>();

        if (slowableObject != null)
        {
            // Le decimos que se ralentice
            slowableObject.ApplySpeedMultiplier(speedMultiplier);
        }
    }

    // 4. [NUEVO] Lógica cuando un enemigo SALE
    private void OnTriggerExit(Collider other)
    {
        // Buscamos si el objeto puede ser ralentizado
        ISlowable slowableObject = other.GetComponent<ISlowable>();

        if (slowableObject != null)
        {
            // Le decimos que restaure su velocidad
            slowableObject.ResetSpeed();
        }
    }
}