using UnityEngine;
using UnityEngine.Events;

public class GameProgressionManager : MonoBehaviour
{
    [Header("Day Settings")]
    [SerializeField] private int totalDaysToSurvive = 10;
    [SerializeField] private int daysRequiredEatingHumans = 2;

    [Header("Progression Phases")]
    [SerializeField] private int daysOnlyPrey = 2;
    [SerializeField] private int daysPreyAndPredators = 4;
    [SerializeField] private int dayHumansAppear = 7;

    [Header("Events")]
    public UnityEvent<int> OnDayChanged;
    public UnityEvent<GamePhase> OnPhaseChanged;
    public UnityEvent OnWendigoTransformation;

    private int currentDay = 1;
    private int consecutiveDaysEatingHumans = 0;
    private FoodType lastFoodConsumed = FoodType.None;

    public int CurrentDay => currentDay;
    public GamePhase CurrentPhase { get; private set; }

    private void Start()
    {
        UpdatePhase();
    }

    public void AdvanceDay()
    {
        currentDay++;
        OnDayChanged?.Invoke(currentDay);
        
        UpdatePhase();
        CheckWendigoCondition();
    }

    public void RegisterFoodConsumed(FoodType foodType)
    {
        lastFoodConsumed = foodType;

        if (foodType == FoodType.Human)
        {
            consecutiveDaysEatingHumans++;
        }
        else
        {
            consecutiveDaysEatingHumans = 0;
        }

        Debug.Log($"Food consumed: {foodType}. Consecutive human days: {consecutiveDaysEatingHumans}");
    }

    private void UpdatePhase()
    {
        GamePhase newPhase;

        if (currentDay <= daysOnlyPrey)
        {
            newPhase = GamePhase.OnlyPrey;
        }
        else if (currentDay <= daysOnlyPrey + daysPreyAndPredators)
        {
            newPhase = GamePhase.PreyAndPredators;
        }
        else if (currentDay < dayHumansAppear)
        {
            newPhase = GamePhase.PreyAndPredators;
        }
        else if (currentDay == dayHumansAppear)
        {
            newPhase = GamePhase.HumansAndPrey;
        }
        else
        {
            newPhase = GamePhase.OnlyHumans;
        }

        if (newPhase != CurrentPhase)
        {
            CurrentPhase = newPhase;
            OnPhaseChanged?.Invoke(newPhase);
            Debug.Log($"Phase changed to: {newPhase}");
        }
    }

    private void CheckWendigoCondition()
    {
        if (currentDay >= totalDaysToSurvive && 
            consecutiveDaysEatingHumans >= daysRequiredEatingHumans)
        {
            TriggerWendigoTransformation();
        }
    }

    private void TriggerWendigoTransformation()
    {
        Debug.Log("WENDIGO TRANSFORMATION TRIGGERED!");
        OnWendigoTransformation?.Invoke();
    }
}

public enum GamePhase
{
    OnlyPrey,
    PreyAndPredators,
    HumansAndPrey,
    OnlyHumans
}

public enum FoodType
{
    None,
    Prey,
    Predator,
    Human
}
