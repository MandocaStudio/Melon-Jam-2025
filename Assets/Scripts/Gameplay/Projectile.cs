using UnityEngine;

[RequireComponent(typeof(DamageDealer))]
public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 10f; 
    private float lifeTimer;
    
    private DamageDealer myDamageDealer;

    [Header("Efectos Visuales")]
    [Tooltip("El prefab del VFX de impacto que se spawnea al chocar")]
    public GameObject impactVfxPrefab; 
    
    // --- [NUEVO] ---
    [Header("Audio")]
    [Tooltip("El clip de sonido que se reproduce al chocar")]
    public AudioClip impactSound;
    // --- [FIN DE LO NUEVO] ---

    void Start()
    {
        lifeTimer = lifetime;
        myDamageDealer = GetComponent<DamageDealer>();
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. PRIMER FILTRO: ¿Es un enemigo?
        if (other.CompareTag("Enemy"))
        {
            // 2. SI ES ENEMIGO: Buscamos si puede recibir daño
            IDamageable damageableObject = other.GetComponentInParent<IDamageable>();

            if (damageableObject != null)
            {
                // 3. SI PUEDE: Le hacemos daño
                damageableObject.TakeDamage(myDamageDealer.damageAmount);

                // --- [NUEVO] ---
                // 4. Spawneamos el VFX de impacto
                if (impactVfxPrefab != null)
                {
                    Instantiate(impactVfxPrefab, transform.position, Quaternion.identity);
                }
                
                // 5. Spawneamos el SONIDO de impacto
                // PlayClipAtPoint es la forma ideal de hacer esto,
                // ya que crea un objeto temporal que se autodestruye.
                if (impactSound != null)
                {
                    AudioSource.PlayClipAtPoint(impactSound, transform.position);
                }
                // --- [FIN DE LO NUEVO] ---

                // 6. Nos destruimos
                Destroy(gameObject);
            }
        }
    }
}