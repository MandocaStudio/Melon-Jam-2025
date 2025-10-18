using System.Collections;
using UnityEngine;

public class FreezeSpell : MonoBehaviour
{
    [SerializeField] private float effectDuration = 5f;   // Tiempo que dura la congelación
    [SerializeField] private float aoeLifetime = 7f;      // Tiempo que dura el hechizo en la escena
    [SerializeField] private float slowedSpeed = 0f;      // Velocidad de movimiento reducida a 0

    private void Start()
    {
        Destroy(gameObject, aoeLifetime);   // Destruye el AOE después de su duración
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonoBehaviour enemy = other.GetComponent<MonoBehaviour>();

            if (enemy != null && enemy.GetType().GetField("moveSpeed") != null)
            {
                StartCoroutine(ApplyFreeze(enemy));
            }
        }
    }

    private IEnumerator ApplyFreeze(MonoBehaviour enemy)
    {
        var type = enemy.GetType();
        var speedField = type.GetField("moveSpeed");

        float originalSpeed = (float)speedField.GetValue(enemy);
        speedField.SetValue(enemy, slowedSpeed);  // Velocidad a 0

        yield return new WaitForSeconds(effectDuration);

        if (enemy != null)
        {
            speedField.SetValue(enemy, originalSpeed);  // Restaurar velocidad original
        }
    }
}
