using System.Collections;
using UnityEngine;

/// <summary>
/// Executes God's action plan by performing actions (spawning entities, teleporting player) based on AI decisions.
/// Spawns enemy prefabs at specified spawn points and tracks active enemy counts.
/// </summary>
public class GodPlanExecutor : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("Player Transform for teleportation")]
    [SerializeField] private Transform playerTransform;
    
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject whiteAngelPrefab;
    [SerializeField] private GameObject redAngelPrefab;
    [SerializeField] private GameObject foxPrefab;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Track active enemy counts for constraint checking
    private int activeWhiteAngels = 0;
    private int activeRedAngels = 0;
    
    // Maximum allowed active enemies
    private const int MAX_WHITE_ANGELS = 3;
    private const int MAX_RED_ANGELS = 1;
    
    // Spawn point positions (hardcoded based on dungeon layout)
    // Dungeon floor goes from (0,0) to (9,9). Walls are at the edges.
    // Spawn positions are 1 unit away from walls (safely inside the dungeon).
    
    // Wall centers are at Y=1 (above floor level), 1 unit away from walls
    private readonly Vector3 northWallCenter = new Vector3(4.5f, 1f, 8f);  // Center X, near north wall
    private readonly Vector3 southWallCenter = new Vector3(4.5f, 1f, 1f);  // Center X, near south wall
    private readonly Vector3 eastWallCenter = new Vector3(8f, 1f, 4.5f);   // Near east wall, center Z
    private readonly Vector3 westWallCenter = new Vector3(1f, 1f, 4.5f);   // Near west wall, center Z
    
    // Corners are at Y=0.5 (slightly above floor), 1 unit away from corner walls
    private readonly Vector3 northeastCorner = new Vector3(8f, 0.5f, 8f);
    private readonly Vector3 northwestCorner = new Vector3(1f, 0.5f, 8f);
    private readonly Vector3 southeastCorner = new Vector3(8f, 0.5f, 1f);
    private readonly Vector3 southwestCorner = new Vector3(1f, 0.5f, 1f);
    
    /// <summary>
    /// Public method called by DualAIController to execute God's action plan.
    /// Each action will be executed after its specified delay.
    /// </summary>
    /// <param name="actions">Array of GodAction objects containing action type, delay, and spawn point</param>
    public void ExecutePlan(GodAction[] actions)
    {
        if (actions == null || actions.Length == 0)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("GodPlanExecutor: Plan is empty or null - nothing to execute");
            }
            return;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"GodPlanExecutor: Received plan with {actions.Length} actions");
        }
        
        // Start coroutine to execute actions with proper delays
        StartCoroutine(ExecutePlanCoroutine(actions));
    }
    
    /// <summary>
    /// Coroutine that executes each action in the plan with its specified delay.
    /// Actions are executed sequentially, waiting for each delay before proceeding.
    /// </summary>
    /// <param name="actions">Array of actions to execute</param>
    private IEnumerator ExecutePlanCoroutine(GodAction[] actions)
    {
        foreach (GodAction action in actions)
        {
            // Wait for the delay before executing this action
            if (action.delay_seconds > 0)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"GodPlanExecutor: Waiting {action.delay_seconds} seconds before executing {action.action}");
                }
                yield return new WaitForSeconds(action.delay_seconds);
            }
            
            // Execute the action
            ExecuteAction(action);
            
            // Small delay between actions to prevent frame spikes
            yield return null;
        }
        
        if (showDebugLogs)
        {
            Debug.Log("GodPlanExecutor: Plan execution complete");
        }
    }
    
    /// <summary>
    /// Executes a single God action based on its type.
    /// Routes to appropriate handler method for each action type.
    /// </summary>
    /// <param name="action">The GodAction to execute</param>
    private void ExecuteAction(GodAction action)
    {
        if (string.IsNullOrEmpty(action.action))
        {
            Debug.LogWarning("GodPlanExecutor: Action type is null or empty - skipping");
            return;
        }
        
        // Route to appropriate handler based on action type
        switch (action.action.ToLower())
        {
            case "spawn_white_angel":
                SpawnWhiteAngel(action.spawn_point);
                break;
                
            case "spawn_red_angel":
                SpawnRedAngel(action.spawn_point);
                break;
                
            case "spawn_fox":
                SpawnFox(action.spawn_point);
                break;
                
            case "teleport_player":
                TeleportPlayer(action.spawn_point);
                break;
                
            default:
                Debug.LogWarning($"GodPlanExecutor: Unknown action type '{action.action}' - skipping");
                break;
        }
    }
    
    /// <summary>
    /// Handles spawning a white angel at the specified spawn point.
    /// Checks constraint: maximum 3 white angels can exist at once.
    /// Instantiates the white angel prefab at the spawn position.
    /// </summary>
    /// <param name="spawnPoint">Name of the spawn point (e.g., "north_wall_center")</param>
    private void SpawnWhiteAngel(string spawnPoint)
    {
        // Check constraint: max 3 white angels
        if (activeWhiteAngels >= MAX_WHITE_ANGELS)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"GodPlanExecutor: Cannot spawn white angel - max limit ({MAX_WHITE_ANGELS}) reached. Active: {activeWhiteAngels}");
            }
            return;
        }
        
        // Get spawn position
        Vector3 spawnPosition = GetSpawnPosition(spawnPoint);
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogError($"GodPlanExecutor: Invalid spawn point '{spawnPoint}' for white angel");
            return;
        }
        
        // Check if prefab is assigned
        if (whiteAngelPrefab == null)
        {
            Debug.LogError("GodPlanExecutor: Cannot spawn white angel - whiteAngelPrefab not assigned!");
            return;
        }
        
        // Log the action
        Debug.Log($"GOD ACTS: Spawning white_angel at {spawnPoint} position {spawnPosition}");
        
        // Instantiate white angel prefab at spawn position
        GameObject angel = Instantiate(whiteAngelPrefab, spawnPosition, Quaternion.identity);
        
        // Increment active count (will be decremented when enemy is destroyed)
        activeWhiteAngels++;
        
        if (showDebugLogs)
        {
            Debug.Log($"GodPlanExecutor: White angel count: {activeWhiteAngels}/{MAX_WHITE_ANGELS}");
        }
    }
    
    /// <summary>
    /// Handles spawning a red angel at the specified spawn point.
    /// Checks constraint: maximum 1 red angel can exist at once.
    /// Instantiates the red angel prefab at the spawn position.
    /// </summary>
    /// <param name="spawnPoint">Name of the spawn point (e.g., "south_wall_center")</param>
    private void SpawnRedAngel(string spawnPoint)
    {
        // Check constraint: max 1 red angel
        if (activeRedAngels >= MAX_RED_ANGELS)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"GodPlanExecutor: Cannot spawn red angel - max limit ({MAX_RED_ANGELS}) reached. Active: {activeRedAngels}");
            }
            return;
        }
        
        // Get spawn position
        Vector3 spawnPosition = GetSpawnPosition(spawnPoint);
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogError($"GodPlanExecutor: Invalid spawn point '{spawnPoint}' for red angel");
            return;
        }
        
        // Check if prefab is assigned
        if (redAngelPrefab == null)
        {
            Debug.LogError("GodPlanExecutor: Cannot spawn red angel - redAngelPrefab not assigned!");
            return;
        }
        
        // Log the action
        Debug.Log($"GOD ACTS: Spawning red_angel at {spawnPoint} position {spawnPosition}");
        
        // Instantiate red angel prefab at spawn position
        GameObject angel = Instantiate(redAngelPrefab, spawnPosition, Quaternion.identity);
        
        // Increment active count (will be decremented when enemy is destroyed)
        activeRedAngels++;
        
        if (showDebugLogs)
        {
            Debug.Log($"GodPlanExecutor: Red angel count: {activeRedAngels}/{MAX_RED_ANGELS}");
        }
    }
    
    /// <summary>
    /// Handles spawning a fox at the specified spawn point.
    /// No limit on foxes. Instantiates the fox prefab at the spawn position.
    /// </summary>
    /// <param name="spawnPoint">Name of the spawn point (e.g., "northeast_corner")</param>
    private void SpawnFox(string spawnPoint)
    {
        // Get spawn position
        Vector3 spawnPosition = GetSpawnPosition(spawnPoint);
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogError($"GodPlanExecutor: Invalid spawn point '{spawnPoint}' for fox");
            return;
        }
        
        // Check if prefab is assigned
        if (foxPrefab == null)
        {
            Debug.LogError("GodPlanExecutor: Cannot spawn fox - foxPrefab not assigned!");
            return;
        }
        
        // Log the action
        Debug.Log($"GOD ACTS: Spawning fox at {spawnPoint} position {spawnPosition}");
        
        // Instantiate fox prefab at spawn position
        GameObject fox = Instantiate(foxPrefab, spawnPosition, Quaternion.identity);
    }
    
    /// <summary>
    /// Teleports the player to a random location (spawn point is ignored for teleport).
    /// For now, just logs the action. Actual teleportation will be implemented later.
    /// </summary>
    /// <param name="spawnPoint">Spawn point name (not used for teleport, but included for consistency)</param>
    private void TeleportPlayer(string spawnPoint)
    {
        if (playerTransform == null)
        {
            Debug.LogError("GodPlanExecutor: Cannot teleport player - playerTransform not assigned!");
            return;
        }
        
        // For teleport, we'll use a random spawn point position
        // Get a random spawn point position
        Vector3[] allSpawnPoints = new Vector3[]
        {
            northWallCenter, southWallCenter, eastWallCenter, westWallCenter,
            northeastCorner, northwestCorner, southeastCorner, southwestCorner
        };
        
        Vector3 teleportPosition = allSpawnPoints[Random.Range(0, allSpawnPoints.Length)];
        
        // For now, just log the action (actual teleportation will be added later)
        Debug.Log($"GOD ACTS: Teleporting player to random location {teleportPosition}");
        
        // TODO: Actually teleport the player
        // playerTransform.position = teleportPosition;
    }
    
    /// <summary>
    /// Gets the world position Vector3 for a spawn point name.
    /// Returns Vector3.zero if spawn point name is invalid.
    /// </summary>
    /// <param name="spawnPoint">Name of the spawn point (e.g., "north_wall_center")</param>
    /// <returns>Vector3 position of the spawn point, or Vector3.zero if invalid</returns>
    private Vector3 GetSpawnPosition(string spawnPoint)
    {
        if (string.IsNullOrEmpty(spawnPoint))
        {
            Debug.LogWarning("GodPlanExecutor: Spawn point name is null or empty");
            return Vector3.zero;
        }
        
        // Match spawn point name (case-insensitive) and return corresponding position
        switch (spawnPoint.ToLower())
        {
            case "north_wall_center":
                return northWallCenter;
            case "south_wall_center":
                return southWallCenter;
            case "east_wall_center":
                return eastWallCenter;
            case "west_wall_center":
                return westWallCenter;
            case "northeast_corner":
                return northeastCorner;
            case "northwest_corner":
                return northwestCorner;
            case "southeast_corner":
                return southeastCorner;
            case "southwest_corner":
                return southwestCorner;
            default:
                Debug.LogWarning($"GodPlanExecutor: Unknown spawn point name: '{spawnPoint}'");
                return Vector3.zero;
        }
    }
    
    /// <summary>
    /// Public method to decrement white angel count when an angel is destroyed.
    /// Call this from enemy death scripts.
    /// </summary>
    public void OnWhiteAngelDestroyed()
    {
        activeWhiteAngels = Mathf.Max(0, activeWhiteAngels - 1);
        if (showDebugLogs)
        {
            Debug.Log($"GodPlanExecutor: White angel destroyed. Active: {activeWhiteAngels}/{MAX_WHITE_ANGELS}");
        }
    }
    
    /// <summary>
    /// Public method to decrement red angel count when an angel is destroyed.
    /// Call this from enemy death scripts.
    /// </summary>
    public void OnRedAngelDestroyed()
    {
        activeRedAngels = Mathf.Max(0, activeRedAngels - 1);
        if (showDebugLogs)
        {
            Debug.Log($"GodPlanExecutor: Red angel destroyed. Active: {activeRedAngels}/{MAX_RED_ANGELS}");
        }
    }
}


