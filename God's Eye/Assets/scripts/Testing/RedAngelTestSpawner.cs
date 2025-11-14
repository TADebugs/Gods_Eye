using UnityEngine;

/// <summary>
/// Test script to spawn a red angel when the player spawns.
/// Attach this to the Player GameObject or any manager object.
/// </summary>
public class RedAngelTestSpawner : MonoBehaviour
{
    [Header("Red Angel Prefab")]
    [Tooltip("Red angel prefab to spawn for testing")]
    [SerializeField] private GameObject redAngelPrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("World position to spawn red angel (inside dungeon bounds 0-9 on X and Z, Y should be on floor level)")]
    [SerializeField] private Vector3 worldSpawnPosition = new Vector3(7f, 0.5f, 7f); // Near corner, on floor
    
    void Start()
    {
        if (redAngelPrefab == null)
        {
            Debug.LogError("RedAngelTestSpawner: Red angel prefab not assigned! Cannot spawn red angel.");
            return;
        }
        
        // Spawn the red angel at specified position
        GameObject redAngel = Instantiate(redAngelPrefab, worldSpawnPosition, Quaternion.identity);
        
        Debug.Log($"RedAngelTestSpawner: Spawned test red angel at position {worldSpawnPosition}");
    }
}

