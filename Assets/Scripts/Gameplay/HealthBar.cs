using UnityEngine;

public class ColumnHealthBar : MonoBehaviour
{
    public int maxHealth = 3;  // Salud máxima de la columna
    private int currentHealth; // Salud actual

    void Start()
    {
        currentHealth = maxHealth;  // Iniciar la salud con el valor máximo
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Restar vida
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Limitar la salud entre 0 y maxHealth

        // Mostrar la salud actual en consola (para pruebas)
        Debug.Log("Column Health: " + currentHealth + "/" + maxHealth);

        if (currentHealth == 0)
        {
            Die();  // Si la salud llega a 0, la columna "muere"
        }
    }

    // Método que se ejecuta cuando la columna se destruye
    void Die()
    {
        Debug.Log("La columna ha caído. ¡Game Over!");
        gameObject.SetActive(false);  // Desactiva la columna al llegar a 0 de salud
    }

    // Detectar colisiones con el Trigger (enemigos o proyectiles)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
        {
            // Al recibir daño, la columna pierde 1 de salud
            TakeDamage(1);
            // Destruir proyectiles al impactar con la columna
            if (other.CompareTag("EnemyProjectile"))
            {
                Destroy(other.gameObject);  // Destruye el proyectil
            }
        }
    }

    // Método para incrementar la salud (útil para pruebas manuales)
    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Limitar la salud entre 0 y maxHealth
        Debug.Log("Health increased! Current Health: " + currentHealth);
    }

    // Método para decrementar la salud manualmente (útil para efectos o pruebas)
    public void DecreaseHealth(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Health decreased! Current Health: " + currentHealth);
    }
}
