using System.Collections;
using UnityEngine;

// (Archivo: Cloud Thunder.cs)
public class ThunderCloudSpell : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float strikeDelay = 0.75f;
    [SerializeField] private float vfxLifetime = 1.0f;

    [Header("Damage Settings")]
    [SerializeField] private float strikeRadius = 2.0f;
    [SerializeField] private int strikeDamage = 50;
    [SerializeField] private LayerMask enemyLayer; 

    [Header("Visuals (Opcional)")]
    [SerializeField] private GameObject lightningVfxPrefab;
    [SerializeField] private GameObject cloudModel;
    
    // --- Nueva variable para guardar el objetivo ---
    private Vector3 strikePosition; 

    private void Start()
    {
        // 1. [NUEVO] El hechizo busca su propio objetivo
        if (!FindTargetPosition())
        {
            // Si no hay enemigos, nos destruimos inútilmente
            Destroy(gameObject);
            return;
        }

        // 2. [NUEVO] Nos movemos a la posición del objetivo
        //    (La nube aparecerá sobre el enemigo aleatorio)
        transform.position = strikePosition;
        
        // 3. Iniciamos la secuencia
        StartCoroutine(StrikeSequence());
        
        // 4. Programamos la autodestrucción
        Destroy(gameObject, strikeDelay + vfxLifetime);
    }
    
    // --- [NUEVO] Método para encontrar un objetivo ---
    private bool FindTargetPosition()
    {
        // Buscamos a todos los enemigos en la escena por su Tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            // No hay objetivos
            return false; 
        }

        // Elegimos un enemigo al azar de la lista
        int randomIndex = Random.Range(0, enemies.Length);
        GameObject targetEnemy = enemies[randomIndex];
        
        // Guardamos su posición
        strikePosition = targetEnemy.transform.position;
        return true;
    }

    private IEnumerator StrikeSequence()
    {
        // --- FASE 1: Telegraph (Aviso) ---
        // La nube es visible sobre el enemigo
        yield return new WaitForSeconds(strikeDelay);

        // --- FASE 2: Golpe ---
        if (cloudModel != null)
        {
            cloudModel.SetActive(false);
        }
        
        // 5. [MODIFICADO] Usamos la posición guardada
        ApplyAreaDamage(strikePosition); 
        SpawnVFX(strikePosition);
    }

    // 6. [MODIFICADO] Ahora acepta la 'center' como parámetro
    private void ApplyAreaDamage(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, strikeRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            IDamageable damageableObject = hit.GetComponentInParent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(strikeDamage);
            }
        }
    }

    // 7. [MODIFICADO] Ahora acepta la 'center' como parámetro
    private void SpawnVFX(Vector3 center)
    {
        if (lightningVfxPrefab != null)
        {
            Instantiate(lightningVfxPrefab, center, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // El Gizmo se mostrará en la posición del prefab
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, strikeRadius);
    }
}