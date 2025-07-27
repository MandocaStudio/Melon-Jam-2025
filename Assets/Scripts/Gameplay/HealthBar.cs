using UnityEngine;
using UnityEngine.UI;


public class ColumnHealthBar : MonoBehaviour
{

    public Image healthBar;
    public int maxHealth = 3;  // Salud máxima de la columna
    [SerializeField] private int currentHealth;  // Salud actual de la columna

    void Start()
    {
        currentHealth = maxHealth;  // Inicializa la salud de la columna
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Restar salud cuando se recibe daño
                                  //currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Limitar la salud entre 0 y maxHealth

        healthBar.fillAmount = (float)currentHealth / maxHealth;

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

    // Detectar la colisión con un proyectil enemigo
    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con un proyectil enemigo
        if (other.CompareTag("EnemyProjectile"))
        {
            TakeDamage(1);  // Recibe daño del proyectil
            Destroy(other.gameObject);  // Destruye el proyectil
        }

        // Si colisiona con un enemigo
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);  // Recibe daño del enemigo
        }
    }
}
