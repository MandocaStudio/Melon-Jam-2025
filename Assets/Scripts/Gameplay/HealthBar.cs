using System; // <-- Requerido para 'Action'
using UnityEngine;
using UnityEngine.UI;

// Sigue implementando IDamageable, como ya lo habíamos hecho
public class ColumnHealthBar : MonoBehaviour, IDamageable
{
    // --- [NUEVO] ---
    // Esta es la "señal de radio" (evento) que el GameManager escuchará
    public static event Action OnPlayerDefeated;
    // --- [FIN DE LO NUEVO] ---

    [Header("UI")]
    public Image healthBar;
    public Image SecondhealthBar;

    [Header("Configuración de Vida")]
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float delayBeforeLerp = 0.5f;
    public int maxHealth = 3;
    [SerializeField] private int currentHealth;

    // Variables privadas para la UI
    private float targetFill;
    private float delayTimer;
    private bool isLerping;

    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        healthBar.fillAmount = targetFill;
        SecondhealthBar.fillAmount = targetFill;
    }

    void Update()
    {
        // Lógica de la barra de vida que se "desliza"
        float desiredFill = (float)currentHealth / maxHealth;
        if (desiredFill != targetFill)
        {
            targetFill = desiredFill;
            delayTimer = delayBeforeLerp;
            isLerping = false;
        }

        healthBar.fillAmount = targetFill;

        if (!isLerping)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                isLerping = true;
            }
        }

        if (isLerping && SecondhealthBar.fillAmount > targetFill)
        {
            SecondhealthBar.fillAmount = Mathf.MoveTowards(SecondhealthBar.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
        }
    }

    // Este es el método público de la interfaz IDamageable
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Columna Salud: " + currentHealth + "/" + maxHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("¡La columna ha caído!");
        gameObject.SetActive(false);

        // --- [NUEVO] ---
        // Dispara el evento para avisarle al GameManager que perdimos
        OnPlayerDefeated?.Invoke();
        // --- [FIN DE LO NUEVO] ---
    }

    // Este es tu trigger para el daño por CONTACTO de enemigo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // La columna toma daño si un enemigo (con DamageDealer) la choca
            if (other.TryGetComponent<DamageDealer>(out DamageDealer dealer))
            {
                TakeDamage(dealer.damageAmount);
            }
        }
        // Nota: El daño por "EnemyProjectile" ahora es manejado
        // por el script del propio proyectil, no por la columna.
    }
}