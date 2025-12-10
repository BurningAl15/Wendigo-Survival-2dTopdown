using UnityEngine;

[CreateAssetMenu(fileName = "BiomeConfig", menuName = "Wendigo/Biome Configuration")]
public class BiomeConfiguration : ScriptableObject
{
    [Header("Biome Settings")]
    public string biomeName = "Forest";
    
    [Header("Trees")]
    public GameObject[] treePrefabs;
    public int treeCount = 100;
    public Vector2 treeScaleRange = new Vector2(0.8f, 1.5f);
    public float treeMinSpacing = 2f;
    
    [Header("Rocks")]
    public GameObject[] rockPrefabs;
    public int rockCount = 50;
    public Vector2 rockScaleRange = new Vector2(0.7f, 1.2f);
    public float rockMinSpacing = 1.5f;
    
    [Header("Grass/Details")]
    public GameObject[] grassPrefabs;
    public int grassCount = 200;
    public Vector2 grassScaleRange = new Vector2(0.8f, 1.3f);
    public float grassMinSpacing = 0.5f;
    
    [Header("Noise Settings")]
    public float noiseScale = 0.1f;
    public float noiseThreshold = 0.45f;
}
