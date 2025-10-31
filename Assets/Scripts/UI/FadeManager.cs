using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }
    
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f; // Ajustado a un valor más rápido

    // [NUEVO] Esta bandera evita el fade-in inicial en la primera escena
    private bool isFirstSceneLoad = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // [NUEVO] Forzamos el alfa a 0 en la primera carga
        // para que la primera escena (MainMenu) sea visible al instante.
        canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // [MODIFICADO] Solo hacemos FadeIn si NO es la primera escena
        if (isFirstSceneLoad)
        {
            isFirstSceneLoad = false;
            canvasGroup.alpha = 0f; // Asegurarse de que esté transparente
        }
        else
        {
            // Si venimos de otra escena, sí hacemos el FadeIn
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string sceneName)
    {
        // 1. De Transparente a Negro
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f; // Asegurar que termine negro
        
        // 2. Cargar Escena
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        // 1. De Negro a Transparente
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f; // Asegurar que termine transparente
    }
}