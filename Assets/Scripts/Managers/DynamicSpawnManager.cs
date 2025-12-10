using System.Collections.Generic;
using UnityEngine;

public class DynamicSpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameProgressionManager progressionManager;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float minDistanceFromPlayer = 20f;

    [Header("Prefabs")]
    [SerializeField] private List<GameObject> preyPrefabs = new();
    [SerializeField] private List<GameObject> predatorPrefabs = new();
    [SerializeField] private List<GameObject> humanPrefabs = new();

    [Header("Spawn Counts per Phase")]
    [SerializeField] private SpawnConfig onlyPreyConfig = new SpawnConfig { preyCount = 10 };
    [SerializeField] private SpawnConfig preyAndPredatorsConfig = new SpawnConfig { preyCount = 8, predatorCount = 3 };
    [SerializeField] private SpawnConfig humansAndPreyConfig = new SpawnConfig { preyCount = 5, humanCount = 4 };
    [SerializeField] private SpawnConfig onlyHumansConfig = new SpawnConfig { humanCount = 6 };

    private void OnEnable()
    {
        if (progressionManager != null)
        {
            progressionManager.OnPhaseChanged.AddListener(OnPhaseChanged);
            progressionManager.OnDayChanged.AddListener(OnDayChanged);
        }
    }

    private void OnDisable()
    {
        if (progressionManager != null)
        {
            progressionManager.OnPhaseChanged.RemoveListener(OnPhaseChanged);
            progressionManager.OnDayChanged.RemoveListener(OnDayChanged);
        }
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        Debug.Log($"Spawning entities for phase: {newPhase}");
        ClearAllSpawnedEntities();
        SpawnEntitiesForPhase(newPhase);
    }

    private void OnDayChanged(int day)
    {
        Debug.Log($"New day: {day}");
    }

    private void SpawnEntitiesForPhase(GamePhase phase)
    {
        SpawnConfig config = phase switch
        {
            GamePhase.OnlyPrey => onlyPreyConfig,
            GamePhase.PreyAndPredators => preyAndPredatorsConfig,
            GamePhase.HumansAndPrey => humansAndPreyConfig,
            GamePhase.OnlyHumans => onlyHumansConfig,
            _ => onlyPreyConfig
        };

        SpawnEntities(preyPrefabs, config.preyCount);
        SpawnEntities(predatorPrefabs, config.predatorCount);
        SpawnEntities(humanPrefabs, config.humanCount);
    }

    private void SpawnEntities(List<GameObject> prefabs, int count)
    {
        if (prefabs == null || prefabs.Count == 0 || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            Vector2 spawnPos = GetRandomSpawnPosition();
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 playerPos = player != null ? (Vector2)player.position : Vector2.zero;
        Vector2 randomPos;
        int attempts = 0;

        do
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistanceFromPlayer, spawnRadius);
            randomPos = playerPos + randomDir * distance;
            attempts++;
        }
        while (Vector2.Distance(randomPos, playerPos) < minDistanceFromPlayer && attempts < 10);

        return randomPos;
    }

    private void ClearAllSpawnedEntities()
    {
        GameObject[] preys = GameObject.FindGameObjectsWithTag("Prey");
        GameObject[] predators = GameObject.FindGameObjectsWithTag("Predator");
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");

        foreach (var entity in preys) Destroy(entity);
        foreach (var entity in predators) Destroy(entity);
        foreach (var entity in humans) Destroy(entity);
    }
}

[System.Serializable]
public class SpawnConfig
{
    public int preyCount = 0;
    public int predatorCount = 0;
    public int humanCount = 0;
}
