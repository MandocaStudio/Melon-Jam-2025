using UnityEngine;

public class ThunderBeamSpell : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damageAmount = 100;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonoBehaviour enemy = other.GetComponent<MonoBehaviour>();
            if (enemy != null)
            {
                var healthField = enemy.GetType().GetField("health");
                if (healthField != null)
                {
                    int currentHealth = (int)healthField.GetValue(enemy);
                    currentHealth -= damageAmount;
                    healthField.SetValue(enemy, currentHealth);

                    if (currentHealth <= 0)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
            }
        }
    }
}
