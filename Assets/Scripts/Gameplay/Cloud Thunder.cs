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
    // --- Esta es la variable de daño ---
    [SerializeField] private int strikeDamage = 50; 
    
    [Header("Visuals (Opcional)")]
    [SerializeField] private GameObject lightningVfxPrefab;
    [SerializeField] private GameObject cloudModel;
    
    private Vector3 strikePosition; 

    private void Start()
    {
        Debug.Log("[ThunderCloud] Hechizo instanciado. Empezando 'Start()'.");

        if (!FindTargetPosition())
        {
            Debug.LogWarning("[ThunderCloud] 'FindTargetPosition' falló (no hay enemigos con Tag 'Enemy'). Destruyendo hechizo.");
            Destroy(gameObject);
            return;
        }

        transform.position = strikePosition;
        StartCoroutine(StrikeSequence());
        Destroy(gameObject, strikeDelay + vfxLifetime);
    }
    
    private bool FindTargetPosition()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Debug.Log($"[ThunderCloud] 'FindTargetPosition' encontró {enemies.Length} GameObjects con el Tag 'Enemy'.");

        if (enemies.Length == 0)
        {
            return false; 
        }

        int randomIndex = Random.Range(0, enemies.Length);
        GameObject targetEnemy = enemies[randomIndex];
        strikePosition = targetEnemy.transform.position;
        
        Debug.Log($"[ThunderCloud] Objetivo seleccionado: {targetEnemy.name} en la posición {strikePosition}");
        return true;
    }

    private IEnumerator StrikeSequence()
    {
        Debug.Log($"[ThunderCloud] 'StrikeSequence' iniciado. Esperando {strikeDelay} segundos para el golpe.");
        yield return new WaitForSeconds(strikeDelay);

        Debug.Log("[ThunderCloud] ¡GOLPE! Tiempo de espera terminado. Llamando a ApplyAreaDamage y SpawnVFX.");
        
        if (cloudModel != null)
        {
            cloudModel.SetActive(false);
        }
        
        ApplyAreaDamage(strikePosition); 
        SpawnVFX(strikePosition);
    }

    private void ApplyAreaDamage(Vector3 center)
    {
        Debug.Log($"[ThunderCloud] 'ApplyAreaDamage' escaneando en {center} con un radio de {strikeRadius}.");

        Collider[] hits = Physics.OverlapSphere(center, strikeRadius);

        Debug.Log($"[ThunderCloud] 'OverlapSphere' encontró {hits.Length} colliders en total (sin filtrar).");

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                IDamageable damageableObject = hit.GetComponentInParent<IDamageable>();
                if (damageableObject != null)
                {
                    // --- [CORRECCIÓN AQUÍ] ---
                    // Asegúrate de que ambas líneas usen "strikeDamage"
                    Debug.Log($"[ThunderCloud] ¡ÉXITO! Objeto '{hit.gameObject.name}' tiene Tag 'Enemy'. Aplicando {strikeDamage} de daño."); 
                    damageableObject.TakeDamage(strikeDamage);
                }
                else
                {
                    Debug.LogWarning($"[ThunderCloud] El objeto {hit.gameObject.name} tiene Tag 'Enemy' pero no se encontró un script 'IDamageable'.");
                }
            }
        }
    }

    private void SpawnVFX(Vector3 center)
    {
        if (lightningVfxPrefab != null)
        {
            Instantiate(lightningVfxPrefab, center, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, strikeRadius);
    }
}