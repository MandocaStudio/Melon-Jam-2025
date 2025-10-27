// IDamageable.cs
public interface IDamageable
{
    // Cualquier objeto que implemente esta interfaz DEBE tener un método TakeDamage.
    void TakeDamage(int damageAmount);
}