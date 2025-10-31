using UnityEngine;
using System.Collections; 

public class ThunderBeamSpell : MonoBehaviour
{
    [Header("Forma del Rayo (Caja)")]
    [SerializeField] private float beamLength = 20f;
    [SerializeField] private float beamWidth = 1f;
    [SerializeField] private float beamHeight = 2f;

    [Header("Daño")]
    [SerializeField] private int damageAmount = 100;
    [Tooltip("Tiempo que dura el VFX antes de desactivarse")]
    [SerializeField] private float lifetime = 1.5f; 
    
    // 1. [ELIMINADO] Ya no necesitamos la variable 'enemyLayer'.
    // [SerializeField] private LayerMask enemyLayer;

    [Header("Visuales")]
    [SerializeField] private GameObject beamVFX; 

    private void OnEnable()
    {
        ApplyAreaDamage();
        if (beamVFX != null) beamVFX.SetActive(true);
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        if (beamVFX != null) beamVFX.SetActive(false);
        gameObject.SetActive(false);
    }

    // --- [MODIFICADO] Este método ahora filtra por Tag ---
    private void ApplyAreaDamage()
    {
        Vector3 center = transform.position;
        Vector3 halfExtents = new Vector3(beamWidth / 2, beamHeight / 2, beamLength / 2);
        Quaternion orientation = transform.rotation;

        // 2. [MODIFICADO] Hacemos el escaneo SIN filtro de capa.
        //    Esto escaneará TODO (suelo, paredes, enemigos, etc.)
        Collider[] hits = Physics.OverlapBox(center, halfExtents, orientation);

        // 3. [NUEVO] Filtramos la lista manualmente usando CompareTag
        foreach (Collider hit in hits)
        {
            // Solo continuamos si el objeto tiene el tag "Enemy"
            if (hit.CompareTag("Enemy"))
            {
                // Aplicamos daño usando la interfaz
                IDamageable damageableObject = hit.GetComponentInParent<IDamageable>();
                if (damageableObject != null)
                {
                    damageableObject.TakeDamage(damageAmount);
                }
            }
        }
    }

    // (El Gizmo no cambia)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.5f); 
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(beamWidth, beamHeight, beamLength));
    }
}