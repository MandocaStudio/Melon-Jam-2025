using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColumnHealthBar : MonoBehaviour, IDamageable
{
    public static event Action OnPlayerDefeated;

    [Header("UI (Barra de Vida)")]
    public Image healthBar;
    public Image SecondhealthBar;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float delayBeforeLerp = 0.5f;

    [Header("Configuración de Vida")]
    public int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Audio (Feedback de Impacto)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] impactSounds;

    [Header("Visuales de Barrera (Shader)")]
    [SerializeField] private Renderer barrierRenderer;
    [SerializeField] private Color fullHealthColor = Color.blue;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float fadeOutTime = 0.5f;

    private Material barrierMaterial; 
    private bool isDead = false; 
    
    private float targetFill;
    private float delayTimer;
    private bool isLerping;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        // --- Configuración de UI ---
        targetFill = 1f;
        healthBar.fillAmount = targetFill;
        SecondhealthBar.fillAmount = targetFill;

        // --- Configuración de Shader ---
        if (barrierRenderer != null)
        {
            barrierMaterial = barrierRenderer.material;
            
            // [CORREGIDO] Obtenemos el color de la propiedad "_Color" (la principal)
            Color mainColor = barrierMaterial.GetColor("_Color");
            // Modificamos su canal 'a' (alpha)
            mainColor.a = 1f;
            // Establecemos el color de vuelta en "_Color"
            barrierMaterial.SetColor("_Color", mainColor);
            
            UpdateBarrierVisuals(); 
        }

        // --- Configuración de Audio ---
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isDead) return;

        // --- Lógica de UI (Tu código original) ---
        // (Tu lógica de UI para el delay y lerp...)
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
        // (Fin de la lógica de UI)

        // --- Lógica de Shader (Sin cambios) ---
        // Esto ya estaba bien, actualiza el color de la barrera
        UpdateBarrierVisuals();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Columna Salud: " + currentHealth + "/" + maxHealth);

        PlayRandomImpactSound();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void PlayRandomImpactSound()
    {
        if (audioSource != null && impactSounds != null && impactSounds.Length > 0)
        {
            AudioClip clip = impactSounds[UnityEngine.Random.Range(0, impactSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    void UpdateBarrierVisuals()
    {
        if (barrierMaterial == null || isDead) return; // No actualices si se está desvaneciendo

        // 1. Calcular el porcentaje de vida
        float healthPercent = (float)currentHealth / maxHealth;

        // 2. Interpolar (Lerp) entre rojo y azul
        Color newColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);

        // 3. [CORREGIDO] Mantenemos el Alpha que ya tiene el material
        //    (para no interferir con el FadeOutBarrier)
        newColor.a = barrierMaterial.GetColor("_Color").a;

        // 4. Asignar el nuevo color al shader
        barrierMaterial.SetColor("_Color", newColor);
    }

    void Die()
    {
        if (isDead) return; 
        isDead = true;

        Debug.Log("¡La columna ha caído!");
        OnPlayerDefeated?.Invoke(); 

        StartCoroutine(FadeOutBarrier());
    }

    IEnumerator FadeOutBarrier()
    {
        float timer = 0f;
        
        // [CORREGIDO] Obtenemos el Color de la propiedad "_Color"
        Color currentColor = barrierMaterial.GetColor("_Color");
        float startAlpha = currentColor.a;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            
            float newAlpha = Mathf.Lerp(startAlpha, 0f, timer / fadeOutTime);
            
            // [CORREGIDO] Modificamos solo el canal 'a'
            currentColor.a = newAlpha;
            
            // [CORREGIDO] Usamos SetColor en "_Color"
            barrierMaterial.SetColor("_Color", currentColor);
            
            yield return null;
        }

        // Aseguramos que al final esté totalmente transparente
        currentColor.a = 0f;
        barrierMaterial.SetColor("_Color", currentColor);
        
        // Y finalmente desactiva el objeto
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<DamageDealer>(out DamageDealer dealer))
            {
                TakeDamage(dealer.damageAmount);
            }
        }
    }
}