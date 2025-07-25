using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CanvasFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    void Start()
    {
        canvasGroup = transform.GetComponent<CanvasGroup>();

        // Puedes iniciar con un fade in si lo deseas
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // Asegurar que termine en 1

        yield return new WaitForSeconds(1f);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public IEnumerator FadeOut()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f; // Asegurar que termine en 0
    }
}
