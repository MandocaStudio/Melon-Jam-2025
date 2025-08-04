using System;
using UnityEngine;
using UnityEngine.UI;

public class ColumnHealthBar : MonoBehaviour
{
    public Image healthBar;
    public Image SecondhealthBar;

    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float delayBeforeLerp = 0.5f;

    public int maxHealth = 3;
    [SerializeField] private int currentHealth;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}
