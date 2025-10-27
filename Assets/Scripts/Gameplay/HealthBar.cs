// HealthBar.cs (Nombre de clase: ColumnHealthBar)
using System;
using UnityEngine;
using UnityEngine.UI;

public class ColumnHealthBar : MonoBehaviour, IDamageable
{
    // --- TUS VARIABLES DE UI ---
    public Image healthBar;
    public Image SecondhealthBar;

    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float delayBeforeLerp = 0.5f;

    public int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private float targetFill;
    private float delayTimer;
    private bool isLerping;

    // --- TU MÉTODO START (RESTAURADO) ---
    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        healthBar.fillAmount = targetFill;
        SecondhealthBar.fillAmount = targetFill;
    }

    // --- TU MÉTODO UPDATE (RESTAURADO) ---
    void Update()
    {
        // Actualizar la barra principal instantáneamente
        float desiredFill = (float)currentHealth / maxHealth;
        if (desiredFill != targetFill)
        {
            targetFill = desiredFill;
            delayTimer = delayBeforeLerp;
            isLerping = false;
        }

        healthBar.fillAmount = targetFill;

        // Temporizador para retrasar el comienzo del Lerp
        if (!isLerping)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                isLerping = true;
            }
        }

        // Lerp de la barra secundaria
        if (isLerping && SecondhealthBar.fillAmount > targetFill)
        {
            SecondhealthBar.fillAmount = Mathf.MoveTowards(SecondhealthBar.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
        }
    }

    // --- TU MÉTODO TAKEDAMAGE (RESTAURADO) ---
    // Este método implementa la interfaz IDamageable
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Columna Salud: " + currentHealth + "/" + maxHealth); // <-- Revisa tu consola para ver este mensaje

        if (currentHealth == 0)
        {
            Die();
        }
    }

    // --- TU MÉTODO DIE (RESTAURADO) ---
    void Die()
    {
        Debug.Log("¡La columna ha caído!");
        gameObject.SetActive(false);
    }

    // --- NUESTRO ONTRIGGERENTER CORREGIDO ---
    private void OnTriggerEnter(Collider other)
    {
        // La columna solo se preocupa si un "Enemy" (físico) la toca.
        if (other.CompareTag("Enemy"))
        {
            // (Asegúrate de que tus prefabs de Enemigos también tengan
            // un DamageDealer para el daño por contacto)
            if (other.TryGetComponent<DamageDealer>(out DamageDealer dealer))
            {
                TakeDamage(dealer.damageAmount);
            }
        }
        
        // Ya NO necesita "if (other.CompareTag("EnemyProjectile"))",
        // porque el proyectil ahora maneja su propio impacto.
    }
}