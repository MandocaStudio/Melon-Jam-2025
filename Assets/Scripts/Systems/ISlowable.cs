// ISlowable.cs
public interface ISlowable
{
    // Aplica un multiplicador de velocidad (ej. 0.1 para 10% de velocidad)
    void ApplySpeedMultiplier(float multiplier);

    // Restaura la velocidad original
    void ResetSpeed();
}