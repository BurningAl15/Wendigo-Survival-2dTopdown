using UnityEngine;

public class MaskZoneSpawner : MonoBehaviour
{
    [Header("Zona & Máscara")]
    public SpriteMask spriteMask;     // opcional, si quieres leer bounds
    public float logicalRadius = 1f;  // radio base de la zona antes de aplicar escala

    [Header("Spawn Settings")]
    public int spawnCount = 30;
    public int maxAttemptsPerObject = 20;

    public GameObject[] environmentPrefabs;
    public Vector2 scaleRange = new Vector2(0.8f, 1.3f);
    public float extraSpacing = 0.2f; // separacion extra

    [Header("Safe Zone Around Player")]
    public Transform player;
    public float safeRadius = 3f;

    [Header("Collision Layer")]
    public LayerMask environmentLayerMask; // capa donde están árboles/piedras

    private void Start()
    {
        SpawnInsideMask();
    }

    private void SpawnInsideMask()
    {
        if (environmentPrefabs == null || environmentPrefabs.Length == 0)
        {
            Debug.LogWarning("No environmentPrefabs in MaskZoneSpawner.");
            return;
        }

        // Radio real de la zona según escala del objeto
        float radius = logicalRadius * Mathf.Max(transform.localScale.x, transform.localScale.y);

        int spawned = 0;
        int totalAttempts = spawnCount * maxAttemptsPerObject;

        for (int i = 0; i < totalAttempts && spawned < spawnCount; i++)
        {
            // Punto aleatorio dentro del círculo de la zona
            Vector2 offset = Random.insideUnitCircle * radius;
            Vector2 candidatePos = (Vector2)transform.position + offset;

            // 1) Safe zone del player
            if (player != null && Vector2.Distance(player.position, candidatePos) < safeRadius)
                continue;

            // Elegir prefab
            GameObject prefab = environmentPrefabs[Random.Range(0, environmentPrefabs.Length)];

            // 2) Calcular radio de colisión según collider + escala
            float scale = Random.Range(scaleRange.x, scaleRange.y);
            float colliderRadius = GetPrefabCollisionRadius(prefab) * scale + extraSpacing;

            // 3) Evitar solapamientos
            if (Physics2D.OverlapCircle(candidatePos, colliderRadius, environmentLayerMask) != null)
                continue;

            // 4) Instanciar
            GameObject instance = Instantiate(prefab, candidatePos, Quaternion.identity);
            instance.transform.localScale = new Vector3(scale, scale, 1f);

            // IMPORTANTE: para que respeten la máscara,
            // el SpriteRenderer / TilemapRenderer de los prefabs
            // debe tener MaskInteraction = VisibleInsideMask
            // y estar en el sorting correcto.
            
            spawned++;
        }
    }

    private float GetPrefabCollisionRadius(GameObject prefab)
    {
        // Aproximación simple según el collider que tenga el prefab
        var circle = prefab.GetComponent<CircleCollider2D>();
        if (circle != null)
            return circle.radius;

        var box = prefab.GetComponent<BoxCollider2D>();
        if (box != null)
            return Mathf.Max(box.size.x, box.size.y) * 0.5f;

        var capsule = prefab.GetComponent<CapsuleCollider2D>();
        if (capsule != null)
            return Mathf.Max(capsule.size.x, capsule.size.y) * 0.5f;

        // fallback si no hay collider
        return 0.5f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float radius = logicalRadius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}