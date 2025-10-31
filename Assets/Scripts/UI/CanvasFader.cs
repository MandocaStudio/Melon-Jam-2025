using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
    [Tooltip("El CanvasGroup que controla el fade")]
    public CanvasGroup canvasGroup;
    
    // --- ESTA ES LA VARIABLE CLAVE ---
    [Tooltip("Tiempo (en segundos) que tarda en aparecer y desaparecer. AMBOS USAN ESTE VALOR.")]
    public float fadeDuration = 0.5f;

    [Tooltip("Tiempo (en segundos) que el logo permanece visible")]
    public float logoDisplayTime = 2.0f;

    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        // Empezamos la secuencia completa
        StartCoroutine(SplashSequence());
    }

    private IEnumerator SplashSequence()
    {
        // 1. APARECER (Fade In)
        // Usa 'fadeDuration'
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // 2. ESPERAR (Mostrar logo)
        yield return new WaitForSeconds(logoDisplayTime);

        // 3. DESAPARECER (Fade Out)
        // También usa 'fadeDuration'
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // 4. CARGAR ESCENA SIGUIENTE
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    /// <summary>
    /// Corrutina de fundido que usa SmoothStep para un Easing.
    /// </summary>
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            
            // Usa SmoothStep para un fundido suave
            canvasGroup.alpha = Mathf.SmoothStep(startAlpha, endAlpha, time / duration);

            yield return null;
        }
        canvasGroup.alpha = endAlpha; // Asegurar el valor final
    }
}