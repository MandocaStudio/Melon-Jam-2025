using UnityEngine;
using System.Collections;

public class WindPushSpell : MonoBehaviour
{
    public float pushForce = 10f;  // Fuerza de empuje
    public float range = 5f;       // Radio del hechizo
    public AudioClip windSound;    // Clip de sonido de viento
    private AudioSource audioSource;  // Componente AudioSource

    public GameObject windVFXPrefab; // Prefab de las partículas de viento
    private ParticleSystem windVFX;  // Componente ParticleSystem para las partículas de viento

    private void Start()
    {
        // Obtener las referencias al AudioSource y ParticleSystem
        audioSource = GetComponent<AudioSource>();  // Obtén el AudioSource del prefab
        windVFX = GetComponentInChildren<ParticleSystem>(); // Asegúrate de tener el sistema de partículas como hijo
    }

    // Método que lanza el hechizo de empuje de viento
    public void CastWindPush(Vector3 position)
    {
        // Reproducir el sonido de viento solo si está activado
        if (audioSource != null && windSound != null)
        {
            audioSource.PlayOneShot(windSound);  // Reproducir el sonido
        }

        // Detectamos los enemigos dentro del rango del hechizo
        Collider[] enemiesInRange = Physics.OverlapSphere(position, range, LayerMask.GetMask("Enemy"));

        foreach (Collider enemy in enemiesInRange)
        {
            // Aplicar empuje solo a enemigos con Rigidbody
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Aplica la fuerza de empuje hacia la derecha
                Vector3 pushDirection = transform.right; // Dirección de empuje (hacia la derecha)
                enemyRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }

        // Instanciamos el VFX (efecto visual de viento) en la posición del hechizo
        windVFX.Play();  // Activa el sistema de partículas de viento
        Instantiate(windVFXPrefab, position, Quaternion.identity); // Instancia el prefab en la posición

        // Llamamos a una corutina para aplicar los impactos de manera secuencial
        StartCoroutine(ApplyWindPushImpact());
    }

    // Corutina que aplica tres impactos consecutivos de viento
    private IEnumerator ApplyWindPushImpact()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.5f); // Espera medio segundo entre impactos
            Debug.Log("Impacto de viento #" + (i + 1));
        }
    }
}
