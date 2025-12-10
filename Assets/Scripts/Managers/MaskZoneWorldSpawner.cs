using System.Collections.Generic;
using UnityEngine;

public class MaskZoneWorldSpawner : MonoBehaviour
{
    [Header("Area")]
    public Vector2 areaSize = new Vector2(50, 50);

    [Header("Zones (SpriteMasks)")]
    public GameObject maskZonePrefab;   // prefab con SpriteMask + MaskZoneSpawner
    public int zoneCount = 5;
    public Vector2 zoneScaleRange = new Vector2(5f, 15f); // escala de la máscara (tamaño de la zona)
    public float minDistanceBetweenZones = 5f;            // que no se encimen las zonas

    [Header("Safe Zone Around Player")]
    public Transform player;
    public float safeRadius = 5f;

    private List<Vector2> _zonePositions = new List<Vector2>();

    private void Start()
    {
        SpawnZones();
    }

    private void SpawnZones()
    {
        if (maskZonePrefab == null)
        {
            Debug.LogWarning("No maskZonePrefab assigned.");
            return;
        }

        int spawned = 0;
        int attempts = zoneCount * 20;

        for (int i = 0; i < attempts && spawned < zoneCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            // 1) Zona segura del player
            if (player != null && Vector2.Distance(player.position, pos) < safeRadius)
                continue;

            // 2) No muy pegadas entre sí
            bool tooClose = false;
            foreach (var p in _zonePositions)
            {
                if (Vector2.Distance(p, pos) < minDistanceBetweenZones)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
                continue;

            // Crear zona
            GameObject zone = Instantiate(maskZonePrefab, pos, Quaternion.identity);
            
            // Escala aleatoria de la máscara (define tamaño de la zona)
            float scale = Random.Range(zoneScaleRange.x, zoneScaleRange.y);
            zone.transform.localScale = new Vector3(scale, scale, 1f);

            // Pasar referencia del player al spawner interno de la zona (si existe)
            var zoneSpawner = zone.GetComponent<MaskZoneSpawner>();
            if (zoneSpawner != null)
            {
                zoneSpawner.player = player;
            }

            _zonePositions.Add(pos);
            spawned++;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0));
    }
}