using UnityEngine;

public class LightingController : MonoBehaviour
{
    [Header("Configuración de Iluminación")]
    public float normalLightIntensity = 1f;   // Intensidad de luz normal
    public float phase1LightIntensity = 0.8f; // Intensidad de luz durante la fase 1
    public float phase2LightIntensity = 0.6f; // Intensidad de luz durante la fase 2
    public float phase3LightIntensity = 0.4f; // Intensidad de luz durante la fase 3
    public float bossLightIntensity = 0.2f;   // Intensidad de luz cuando aparece un Boss

    private void Start()
    {
        // Establecer la luz ambiental al inicio
        SetLightingIntensity(normalLightIntensity);
    }

    // Cambiar la intensidad de la luz ambiental
    public void SetLightingIntensity(float intensity)
    {
        // Asegurarse de que la intensidad esté en el rango correcto
        intensity = Mathf.Clamp(intensity, 0f, 1f); // Limitar la intensidad entre 0 y 1
        RenderSettings.ambientLight = new Color(intensity, intensity, intensity); // Ajusta la intensidad de la luz
        Debug.Log("Nueva Intensidad de la luz: " + intensity);
    }

    // Cambiar la luz según la fase
    public void SetLightingForPhase(int phase)
    {
        switch (phase)
        {
            case 1:
                SetLightingIntensity(phase1LightIntensity); // Fase 1
                break;

            case 2:
                SetLightingIntensity(phase2LightIntensity); // Fase 2
                break;

            case 3:
                SetLightingIntensity(phase3LightIntensity); // Fase 3
                break;

            case 4:
                SetLightingIntensity(bossLightIntensity);   // Fase cuando aparece un Boss
                break;

            default:
                SetLightingIntensity(normalLightIntensity);  // Luz normal
                break;
        }
    }
}
