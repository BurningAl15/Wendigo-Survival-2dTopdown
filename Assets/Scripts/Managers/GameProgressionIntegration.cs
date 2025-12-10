using UnityEngine;

public class GameProgressionIntegration : MonoBehaviour
{
    [SerializeField] private DaytimeManager daytimeManager;
    [SerializeField] private GameProgressionManager progressionManager;

    private DaytimePhase lastPhase;

    private void Start()
    {
        if (daytimeManager != null)
        {
            lastPhase = daytimeManager.CurrentPhase;
        }
    }

    private void Update()
    {
        if (daytimeManager == null || progressionManager == null)
            return;

        if (lastPhase == DaytimePhase.Night && daytimeManager.CurrentPhase == DaytimePhase.Dawn)
        {
            progressionManager.AdvanceDay();
        }

        lastPhase = daytimeManager.CurrentPhase;
    }
}