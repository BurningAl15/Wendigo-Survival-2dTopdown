using UnityEngine;

public class GameProgressionIntegration : MonoBehaviour
{
    [SerializeField] private DaytimeManager daytimeManager;
    [SerializeField] private GameProgressionManager progressionManager;

    private DaytimePhase lastPhase;

    private void Start()
    {
        if (daytimeManager == null)
        {
            Debug.LogError("GameProgressionIntegration: DaytimeManager not assigned!");
            enabled = false;
            return;
        }

        if (progressionManager == null)
        {
            Debug.LogError("GameProgressionIntegration: GameProgressionManager not assigned!");
            enabled = false;
            return;
        }

        lastPhase = daytimeManager.CurrentPhase;
    }

    private void Update()
    {
        if (daytimeManager == null || progressionManager == null)
            return;

        if (lastPhase == DaytimePhase.Night && daytimeManager.CurrentPhase == DaytimePhase.Dawn)
        {
            progressionManager.AdvanceDay();
            Debug.Log($"New day started! Day {progressionManager.CurrentDay}");
        }

        lastPhase = daytimeManager.CurrentPhase;
    }
}