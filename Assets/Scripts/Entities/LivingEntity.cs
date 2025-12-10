using UnityEngine;

public abstract class LivingEntity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float maxStamina = 100f;

    [Header("Stamina")]
    [SerializeField] protected float staminaRegenPerSecond = 15f;
    [SerializeField] protected float staminaSprintCostPerSecond = 25f;

    public float CurrentHealth { get; protected set; }
    public float CurrentStamina { get; protected set; }
    public bool IsDead => CurrentHealth <= 0f;

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public void UpdateStamina(bool isSprinting, float deltaTime)
    {
        if (isSprinting)
        {
            CurrentStamina -= staminaSprintCostPerSecond * deltaTime;
            if (CurrentStamina < 0f)
                CurrentStamina = 0f;
        }
        else
        {
            CurrentStamina += staminaRegenPerSecond * deltaTime;
            if (CurrentStamina > maxStamina)
                CurrentStamina = maxStamina;
        }
    }
}