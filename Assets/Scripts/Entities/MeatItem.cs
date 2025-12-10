using UnityEngine;

public class MeatItem : MonoBehaviour
{
    [SerializeField] private FoodType foodType = FoodType.Prey;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        HungerSystem hunger = other.GetComponent<HungerSystem>();
        if (hunger != null)
        {
            hunger.Eat(foodType);
            Destroy(gameObject);
        }
    }
}