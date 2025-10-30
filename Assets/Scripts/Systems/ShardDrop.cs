using UnityEngine;
using System.Collections;

// 1. Requerimos los componentes 2D
[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collider2D))]
public class ShardDrop2D : MonoBehaviour
{
    // 2. El enum de tamaño sigue igual
    public enum ShardSize { Small, Big }

    // 3. [CAMBIO] Referencias a los assets de SPRITE (imágenes)
    [Header("Sprite Assets")]
    public Sprite smallRaySprite;
    public Sprite smallIceSprite;
    public Sprite smallWindSprite;
    public Sprite bigRaySprite;
    public Sprite bigIceSprite;
    public Sprite bigWindSprite;

    [Header("Configuración de Colección")]
    [Tooltip("Tiempo (seg) que la esquirla está en el suelo")]
    public float collectDelay = 0.8f;
    
    [Tooltip("Fuerza con la que la esquirla 'salta' del enemigo")]
    public float popForce = 4f;

    // 4. [CAMBIO] Referencias a componentes 2D
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        // 5. Obtenemos los componentes 2D
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        
        // Es buena idea asegurarse de que el sprite esté alineado
        // con la cámara del juego. Si es 2.5D, quizá quieras
        // un Billboard script. Si es 2D puro, esto está bien.
    }

    // 6. El enemigo llamará a esto
    public void Initialize(Inventory.ItemType type, ShardSize size)
    {
        // A. Asigna el sprite correcto
        ActivateCorrectSprite(type, size);

        // B. [CAMBIO] Aplica la fuerza "Pop" en 2D
        Vector2 randomForce = new Vector2(Random.Range(-0.7f, 0.7f), 1f); // Fuerza X aleatoria, Y siempre arriba
        rb2d.AddForce(randomForce.normalized * popForce, ForceMode2D.Impulse);

        // C. Inicia la cuenta regresiva para la recolección
        StartCoroutine(CollectSequence(type, size));
    }

    // 7. [CAMBIO] Este método ahora asigna el sprite, no activa GameObjects
    void ActivateCorrectSprite(Inventory.ItemType type, ShardSize size)
    {
        if (size == ShardSize.Small)
        {
            if (type == Inventory.ItemType.Ray) spriteRenderer.sprite = smallRaySprite;
            else if (type == Inventory.ItemType.Ice) spriteRenderer.sprite = smallIceSprite;
            else if (type == Inventory.ItemType.Wind) spriteRenderer.sprite = smallWindSprite;
        }
        else // Es Grande
        {
            if (type == Inventory.ItemType.Ray) spriteRenderer.sprite = bigRaySprite;
            else if (type == Inventory.ItemType.Ice) spriteRenderer.sprite = bigIceSprite;
            else if (type == Inventory.ItemType.Wind) spriteRenderer.sprite = bigWindSprite;
        }
    }

    // 8. La lógica de recolección (casi idéntica)
    IEnumerator CollectSequence(Inventory.ItemType type, ShardSize size)
    {
        yield return new WaitForSeconds(collectDelay);

        // (OPCIONAL: Animación de "vuelo" hacia el jugador)

        if (size == ShardSize.Small)
        {
            Inventory.Instance.CollectSmall(type);
        }
        else
        {
            Inventory.Instance.CollectBig(type);
        }
        
        Destroy(gameObject);
    }
}