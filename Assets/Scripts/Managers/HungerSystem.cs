using UnityEngine;

public class HungerSystem : MonoBehaviour
{
    [Header("Hunger Settings")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hungerDecayPerSecond = 1f;
    [SerializeField] private float hungerThresholdToDie = 0f;

    [Header("Food Values")]
    [SerializeField] private float preyFoodValue = 30f;
    [SerializeField] private float predatorFoodValue = 50f;
    [SerializeField] private float humanFoodValue = 60f;

    [Header("References")]
    [SerializeField] private GameProgressionManager progressionManager;

    public float CurrentHunger { get; private set; }

    private void Start()
    {
        CurrentHunger = maxHunger;
    }

    private void Update()
    {
        CurrentHunger -= hungerDecayPerSecond * Time.deltaTime;
        
        if (CurrentHunger <= hungerThresholdToDie)
        {
            Die();
        }

        CurrentHunger = Mathf.Clamp(CurrentHunger, 0f, maxHunger);
    }

    public void Eat(FoodType foodType)
    {
        float foodValue = foodType switch
        {
            FoodType.Prey => preyFoodValue,
            FoodType.Predator => predatorFoodValue,
            FoodType.Human => humanFoodValue,
            _ => 0f
        };

        CurrentHunger += foodValue;
        CurrentHunger = Mathf.Clamp(CurrentHunger, 0f, maxHunger);

        if (progressionManager != null)
        {
            progressionManager.RegisterFoodConsumed(foodType);
        }

        Debug.Log($"Ate {foodType}. Hunger now: {CurrentHunger}");
    }

    private void Die()
    {
        Debug.Log("Player died of hunger!");
        GetComponent<LivingEntity>()?.TakeDamage(1000f);
    }
}