using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;  // El sistema de controles
    private Vector2 moveInput;        // Entrada del jugador para el movimiento
    public GameObject projectilePrefab;  // Prefab del proyectil
    public float moveSpeed = 5f;         // Velocidad de movimiento
    private PlayerHealth playerHealth;   // Referencia al script de salud del jugador

    private void Awake()
    {
        controls = new PlayerControls();  // Inicializa los controles
        playerHealth = GetComponent<PlayerHealth>();  // Obtener la referencia al script PlayerHealth
    }

    private void OnEnable()
    {
        controls.Enable();  // Activa el sistema de entradas
    }

    private void OnDisable()
    {
        controls.Disable();  // Desactiva el sistema de entradas
    }

    private void Update()
    {
        // Leer la entrada de movimiento vertical (teclado o joystick)
        moveInput = controls.Player.Move.ReadValue<Vector2>();  // Lee la dirección de movimiento
        MovePlayer(moveInput);  // Llama a la función para mover al jugador

        // Comprobar la acción de disparo
        if (controls.Player.Attack.triggered)
        {
            Shoot();
        }
    }

    // Función para mover al jugador
    private void MovePlayer(Vector2 direction)
    {
        Vector3 move = new Vector3(0, direction.y, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(move);  // Aplica el movimiento
    }

    // Función para disparar un proyectil
    private void Shoot()
    {
        Debug.Log("Disparando proyectil");
        Vector3 spawnPosition = transform.position + new Vector3(1f, 0, 0); // Posición de disparo ajustada
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);  // Lanzamos el proyectil
    }

    // Función para manejar las colisiones con los proyectiles enemigos y enemigos
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto con el que colisionamos es un proyectil enemigo o un enemigo
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
        {
            // Llamar a la función TakeDamage del script PlayerHealth
            playerHealth.TakeDamage(1);  // Reducir salud del jugador en 1

            // Destruir el proyectil o el enemigo (opcional)
            Destroy(other.gameObject);
        }
    }
}
