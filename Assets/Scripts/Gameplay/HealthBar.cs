using UnityEngine;

public class ColumnHealthBar : MonoBehaviour
{
    public int maxHealth = 3;  // Salud máxima de la columna
    private int currentHealth;  // Salud actual de la columna

    void Start()
    {
        currentHealth = maxHealth;  // Inicializa la salud de la columna
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Restar salud cuando se recibe daño
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Limitar la salud entre 0 y maxHealth

        // Mostrar la salud actual en la consola para probar
        Debug.Log("Columna Salud: " + currentHealth + "/" + maxHealth);

        if (currentHealth == 0)
        {
            Die();  // Si la salud llega a 0, la columna muere
        }
    }

    void Die()
    {
        Debug.Log("¡La columna ha caído!");
        gameObject.SetActive(false);  // Desactiva la columna (el juego termina)
    }
}
