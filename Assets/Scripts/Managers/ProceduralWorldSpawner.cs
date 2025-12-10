using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnCategory
{
    public string name = "Trees";
    public GameObject[] prefabs;
    public int spawnCount = 100;
    public Vector2 scaleRange = new Vector2(0.8f, 1.3f);
    public float density = 1f;
    public LayerMask collisionMask;
    public float minSpacing = 1.2f;
}

public class ProceduralWorldSpawner : MonoBehaviour
{
    [Header("Configuration Mode")]
    [SerializeField] private bool useBiomeConfig = false;
    [SerializeField] private BiomeConfiguration biomeConfig;

    [Header("World Settings")]
    [SerializeField] private Vector2 worldSize = new Vector2(50, 50);
    [SerializeField] private int seed = 12345;
    [SerializeField] private bool useRandomSeed = true;

    [Header("Player Safe Zone")]
    [SerializeField] private Transform player;
    [SerializeField] private float safeRadius = 5f;

    [Header("Perlin Noise Distribution")]
    [SerializeField] private bool usePerlinNoise = true;
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private float noiseThreshold = 0.45f;

    [Header("Manual Spawn Categories (if not using Biome Config)")]
    [SerializeField] private SpawnCategory[] categories;

    [Header("Performance")]
    [SerializeField] private int maxAttemptsPerObject = 10;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private Transform spawnParent;

    private List<Vector2> spawnedPositions = new List<Vector2>();
    private int totalSpawned = 0;

    private void Start()
    {
        if (spawnOnStart)
            GenerateWorld();
    }

    public void GenerateWorld()
    {
        if (useRandomSeed)
            seed = Random.Range(0, 999999);

        Random.InitState(seed);
        
        spawnedPositions.Clear();
        totalSpawned = 0;

        ClearExistingObjects();

        SpawnCategory[] categoriesToSpawn = GetCategoriesToSpawn();

        if (categoriesToSpawn == null || categoriesToSpawn.Length == 0)
        {
            Debug.LogWarning("ProceduralWorldSpawner: No categories configured!");
            return;
        }

        foreach (var category in categoriesToSpawn)
        {
            if (category.prefabs == null || category.prefabs.Length == 0)
                continue;

            SpawnCategory(category);
        }

        Debug.Log($"ProceduralWorldSpawner: Spawned {totalSpawned} objects total (Seed: {seed})");
    }

    private SpawnCategory[] GetCategoriesToSpawn()
    {
        if (useBiomeConfig && biomeConfig != null)
        {
            return ConvertBiomeToCategories(biomeConfig);
        }
        
        return categories;
    }

    private SpawnCategory[] ConvertBiomeToCategories(BiomeConfiguration biome)
    {
        List<SpawnCategory> result = new List<SpawnCategory>();

        if (biome.treePrefabs != null && biome.treePrefabs.Length > 0)
        {
            result.Add(new SpawnCategory
            {
                name = "Trees",
                prefabs = biome.treePrefabs,
                spawnCount = biome.treeCount,
                scaleRange = biome.treeScaleRange,
                minSpacing = biome.treeMinSpacing,
                collisionMask = LayerMask.GetMask("Default", "LivingEntity")
            });
        }

        if (biome.rockPrefabs != null && biome.rockPrefabs.Length > 0)
        {
            result.Add(new SpawnCategory
            {
                name = "Rocks",
                prefabs = biome.rockPrefabs,
                spawnCount = biome.rockCount,
                scaleRange = biome.rockScaleRange,
                minSpacing = biome.rockMinSpacing,
                collisionMask = LayerMask.GetMask("Default", "LivingEntity")
            });
        }

        if (biome.grassPrefabs != null && biome.grassPrefabs.Length > 0)
        {
            result.Add(new SpawnCategory
            {
                name = "Grass",
                prefabs = biome.grassPrefabs,
                spawnCount = biome.grassCount,
                scaleRange = biome.grassScaleRange,
                minSpacing = biome.grassMinSpacing,
                collisionMask = LayerMask.GetMask("Default")
            });
        }

        if (useBiomeConfig)
        {
            noiseScale = biome.noiseScale;
            noiseThreshold = biome.noiseThreshold;
        }

        return result.ToArray();
    }

    private void SpawnCategory(SpawnCategory category)
    {
        int spawned = 0;
        int attempts = 0;
        int maxAttempts = category.spawnCount * maxAttemptsPerObject;

        while (spawned < category.spawnCount && attempts < maxAttempts)
        {
            attempts++;

            Vector2 position = GetRandomPosition();

            if (!IsValidSpawnPosition(position, category))
                continue;

            if (usePerlinNoise && !PassesNoiseCheck(position))
                continue;

            GameObject prefab = category.prefabs[Random.Range(0, category.prefabs.Length)];
            float scale = Random.Range(category.scaleRange.x, category.scaleRange.y);
            
            if (!CheckCollision(position, prefab, scale, category))
                continue;

            SpawnObject(prefab, position, scale);
            
            spawnedPositions.Add(position);
            spawned++;
            totalSpawned++;
        }

        Debug.Log($"Spawned {spawned}/{category.spawnCount} {category.name}");
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(
            Random.Range(-worldSize.x / 2f, worldSize.x / 2f),
            Random.Range(-worldSize.y / 2f, worldSize.y / 2f)
        );
    }

    private bool IsValidSpawnPosition(Vector2 position, SpawnCategory category)
    {
        if (player != null && Vector2.Distance(player.position, position) < safeRadius)
            return false;

        foreach (var spawnedPos in spawnedPositions)
        {
            if (Vector2.Distance(spawnedPos, position) < category.minSpacing)
                return false;
        }

        return true;
    }

    private bool PassesNoiseCheck(Vector2 position)
    {
        float noise = Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale);
        return noise >= noiseThreshold;
    }

    private bool CheckCollision(Vector2 position, GameObject prefab, float scale, SpawnCategory category)
    {
        float radius = GetPrefabCollisionRadius(prefab) * scale;
        return Physics2D.OverlapCircle(position, radius, category.collisionMask) == null;
    }

    private float GetPrefabCollisionRadius(GameObject prefab)
    {
        var circle = prefab.GetComponent<CircleCollider2D>();
        if (circle != null)
            return circle.radius;

        var box = prefab.GetComponent<BoxCollider2D>();
        if (box != null)
            return Mathf.Max(box.size.x, box.size.y) * 0.5f;

        var capsule = prefab.GetComponent<CapsuleCollider2D>();
        if (capsule != null)
            return Mathf.Max(capsule.size.x, capsule.size.y) * 0.5f;

        return 0.5f;
    }

    private void SpawnObject(GameObject prefab, Vector2 position, float scale)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.transform.localScale = new Vector3(scale, scale, 1f);
        
        if (spawnParent != null)
            instance.transform.SetParent(spawnParent);
    }

    private void ClearExistingObjects()
    {
        if (spawnParent != null)
        {
            foreach (Transform child in spawnParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(worldSize.x, worldSize.y, 0));

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, safeRadius);
        }
    }
}
