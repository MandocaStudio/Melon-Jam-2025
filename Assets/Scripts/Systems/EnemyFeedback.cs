using UnityEngine;
using System.Collections;

// Asegúrate de que el nombre de la clase coincida con el nombre del archivo
public class EnemyFeedback : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El SpriteRenderer del enemigo que cambiará de color")]
    public SpriteRenderer enemySpriteRenderer;
    
    [Tooltip("El Transform del objeto visual que se escalará (usualmente el mismo que el SpriteRenderer)")]
    public Transform visualTransform;

    [Header("Configuración del Efecto")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float effectDuration = 0.2f; // Duración total del 'hit'

    // Definimos las escalas de la animación
    private Vector3 squashScale = new Vector3(1.2f, 0.8f, 1f); // "Achata" (más ancho, más bajo)
    private Vector3 stretchScale = new Vector3(0.8f, 1.2f, 1f); // "Alargándose" (más flaco, más alto)

    private Color originalColor;
    private Vector3 originalScale;
    private Coroutine hitEffectCoroutine;

    private void Start()
    {
        // Guardamos los valores originales para poder restaurarlos
        if (enemySpriteRenderer != null)
        {
            originalColor = enemySpriteRenderer.color;
        }
        if (visualTransform != null)
        {
            originalScale = visualTransform.localScale;
        }
    }

    // Este es el método público que llamará el script del enemigo
    public void PlayHitEffect()
    {
        // Si ya hay un efecto en marcha, lo paramos primero
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
            ResetVisuals(); // Lo resetea al estado original
        }
        
        // Iniciamos la nueva corrutina del efecto
        hitEffectCoroutine = StartCoroutine(HitEffectCoroutine());
    }
    
    private IEnumerator HitEffectCoroutine()
    {
        // 1. Flash y "Achata" (Squash)
        if(enemySpriteRenderer) enemySpriteRenderer.color = flashColor;
        if(visualTransform) visualTransform.localScale = squashScale;
        
        // Esperamos la mitad del efecto
        yield return new WaitForSeconds(effectDuration / 2);

        // 2. "Alargándose" (Stretch)
        if(visualTransform) visualTransform.localScale = stretchScale;
        
        // Esperamos la otra mitad
        yield return new WaitForSeconds(effectDuration / 2);

        // 3. Volver a la normalidad
        ResetVisuals();
        hitEffectCoroutine = null; // Marcamos la corrutina como terminada
    }

    // Método para restaurar los valores originales
    private void ResetVisuals()
    {
        if (enemySpriteRenderer) enemySpriteRenderer.color = originalColor;
        if (visualTransform) visualTransform.localScale = originalScale;
    }

    // (Opcional) Si el enemigo se desactiva/muere mientras el efecto está activo, lo resetea
    private void OnDisable()
    {
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
            hitEffectCoroutine = null;
        }
        ResetVisuals();
    }
}