using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Collider))]
public class SlowAOE : MonoBehaviour
{
    [Header("Configuración del AOE")]
    [SerializeField] private float aoeLifetime = 7f;          
    [SerializeField] private float speedMultiplier = 0.1f;    

    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    // OnEnable se llama CADA VEZ que el objeto se activa
    private void OnEnable()
    {
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }
        
        // Programa la auto-desactivación
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(aoeLifetime);
        DisableSpell();
    }

    private void OnTriggerEnter(Collider other)
    {
        ISlowable slowableObject = other.GetComponent<ISlowable>();
        if (slowableObject != null)
        {
            slowableObject.ApplySpeedMultiplier(speedMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ISlowable slowableObject = other.GetComponent<ISlowable>();
        if (slowableObject != null)
        {
            slowableObject.ResetSpeed();
        }
    }
    
    private void DisableSpell()
    {
        // Apaga el collider primero para disparar OnTriggerExit
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // Apaga el GameObject
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Seguridad por si se apaga externamente
        if (myCollider != null && myCollider.enabled)
        {
            myCollider.enabled = false;
        }
    }
}