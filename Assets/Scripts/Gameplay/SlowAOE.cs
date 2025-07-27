using System.Collections;
using UnityEngine;

public class SlowAOE : MonoBehaviour
{
    public float effectDuration = 3f;       // Cuánto tiempo se ralentiza cada enemigo
    public float aoeLifetime = 7f;          // Tiempo total del AOE en escena
    public float slowedSpeed = 0.1f;        // Velocidad reducida temporal

    private void Start()
    {
        Destroy(gameObject, aoeLifetime);   // El AOE desaparece automáticamente
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonoBehaviour enemy = other.GetComponent<MonoBehaviour>();

            if (enemy != null && enemy.GetType().GetField("moveSpeed") != null)
            {
                StartCoroutine(ApplySlow(enemy));
            }
        }
    }

    private IEnumerator ApplySlow(MonoBehaviour enemy)
    {
        var type = enemy.GetType();
        var speedField = type.GetField("moveSpeed");
        float originalSpeed = (float)speedField.GetValue(enemy);
        speedField.SetValue(enemy, slowedSpeed);

        yield return new WaitForSeconds(effectDuration);

        if (enemy != null)
        {
            speedField.SetValue(enemy, originalSpeed);
        }
    }
}
