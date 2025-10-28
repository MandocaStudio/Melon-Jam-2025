// LightingController.cs
using UnityEngine;
using System.Collections;

public class LightingController : MonoBehaviour
{
    [Header("Luces")]
    public Light directionalLight; 

    private Coroutine currentLightTransition;

    public void TransitionTo(Color targetColor, float targetIntensity, float duration)
    {
        if (currentLightTransition != null)
        {
            StopCoroutine(currentLightTransition);
        }
        currentLightTransition = StartCoroutine(LerpLighting(targetColor, targetIntensity, duration));
    }

    private IEnumerator LerpLighting(Color targetColor, float targetIntensity, float duration)
    {
        float timer = 0f;
        Color startColor = directionalLight.color;
        Color startAmbient = RenderSettings.ambientLight;
        Color targetAmbient = new Color(targetIntensity, targetIntensity, targetIntensity);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration; 
            directionalLight.color = Color.Lerp(startColor, targetColor, t);
            RenderSettings.ambientLight = Color.Lerp(startAmbient, targetAmbient, t);
            yield return null;
        }

        directionalLight.color = targetColor;
        RenderSettings.ambientLight = targetAmbient;
    }
}