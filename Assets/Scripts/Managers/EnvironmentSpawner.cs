using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Area")]
    public Vector2 areaSize = new Vector2(50, 50);   // mundo a rellenar

    [Header("Spawn Settings")]
    public int spawnCount = 300;                     // cantidad total de objetos
    public float minDistance = 1.2f;                 // evita solapamientos
    public float noiseScale = 0.1f;                  // qué tan “natural” se ve la distribución

    [Header("Safe Zone Around Player")]
    public Transform player;
    public float safeRadius = 5f;

    [Header("Prefabs")]
    public GameObject[] environmentPrefabs;

    private void Start()
    {
        SpawnEnvironment();
    }

    private void SpawnEnvironment()
    {
        if (environmentPrefabs == null || environmentPrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned.");
            return;
        }

        int attempts = spawnCount * 10;
        int spawned = 0;

        for (int i = 0; i < attempts && spawned < spawnCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );

            // Zona segura cerca del jugador
            if (player != null)
            {
                float distToPlayer = Vector2.Distance(player.position, pos);
                if (distToPlayer < safeRadius)
                    continue;
            }

            // Filtro de ruido (para evitar spawn en zonas sin vegetación)
            float noise = Mathf.PerlinNoise(pos.x * noiseScale, pos.y * noiseScale);
            if (noise < 0.45f) 
                continue; // menos ruido = zona "vacía"

            // Evitar solapamientos
            if (Physics2D.OverlapCircle(pos, minDistance) != null)
                continue;

            // Crear objeto aleatorio
            GameObject prefab = environmentPrefabs[Random.Range(0, environmentPrefabs.Length)];

            Instantiate(prefab, pos, Quaternion.identity);
            spawned++;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Solo para visualizar área
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0));
    }
}