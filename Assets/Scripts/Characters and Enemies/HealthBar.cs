using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;  
    private int currentHealth; 

    void Start()
    {
        currentHealth = maxHealth;  
    }

    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Restar salud cuando se recibe daño
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Limitar la salud entre 0 y maxHealth

        // Mostrar la salud actual en la consola para probar
        Debug.Log("Player Health: " + currentHealth + "/" + maxHealth);

        if (currentHealth == 0)
        {
            Die();  // Si la salud llega a 0, el jugador muere
        }
    }

    // Función para manejar la muerte del jugador
    void Die()
    {
        Debug.Log("Player died!");
       
        gameObject.SetActive(false);  
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
