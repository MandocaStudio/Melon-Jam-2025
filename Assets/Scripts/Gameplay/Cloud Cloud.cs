using System.Collections;
using UnityEngine;

public class WindPush : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float lifetime = 10f;

    [Header("Push Logic")]
    [SerializeField] private float pushForce = 1.5f;
    [SerializeField] private float pushDuration = 3f;

    [Header("Collision")]
    [SerializeField] private int maxHits = 3;
    private int currentHits = 0;

    // 1. [NUEVA BANDERA] Para detener el movimiento y los triggers
    private bool isDying = false; 

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // 2. [CAMBIO] Si estamos "muriendo", dejamos de movernos.
        if (isDying) return; 

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 3. [CAMBIO] Si ya estamos muriendo, no procesamos más triggers.
        if (isDying) return; 

        // Lógica de "perforación" (piercing)
        if (other.CompareTag("Enemy") || other.CompareTag("Projectile"))
        {
            currentHits++;
        }

        // Lógica de Empuje (solo a enemigos)
        if (other.CompareTag("Enemy"))
        {
            var enemyScript = other.GetComponent<IDamageable>() as MonoBehaviour;
            
            if (enemyScript != null && enemyScript.enabled)
            {
                StartCoroutine(PushEnemyCoroutine(enemyScript));
            }
        }
        
        // 4. [CAMBIO] Lógica de destrucción
        if (currentHits >= maxHits)
        {
            isDying = true; // Activamos la bandera

            // Desactivamos el collider para no golpear a nadie más
            GetComponent<Collider>().enabled = false; 

            // Programamos la destrucción para DESPUÉS de que la corrutina
            // de empuje haya terminado (pushDuration + un pequeño búfer).
            Destroy(gameObject, pushDuration + 0.1f);
        }
    }

    IEnumerator PushEnemyCoroutine(MonoBehaviour enemyAI)
    {
        // 1. Desactivamos la IA
        enemyAI.enabled = false;

        float timer = 0f;
        
        // 2. Bucle de empuje
        while (timer < pushDuration && enemyAI != null) 
        {
            enemyAI.transform.Translate(Vector3.right * pushForce * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        // 3. Reactivamos la IA (ahora la corrutina sí llegará aquí)
        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }
    }
}