using System.Collections;
using UnityEngine;

/// <summary>
/// AI controller for fox enemy that flees from player, summons angels after a timer, and drops health on death.
/// Fox is a distraction enemy - fast but weak, designed to create chaos.
/// </summary>
public class FoxAI : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed at which the fox flees from the player")]
    [SerializeField] private float moveSpeed = 5f; // Faster than angels
    
    [Tooltip("Distance at which the fox starts fleeing from the player")]
    [SerializeField] private float fleeDistance = 12f;
    
    [Header("Summoning")]
    [Tooltip("Time in seconds before the fox howls and summons an angel")]
    [SerializeField] private float summonTimer = 60f; // Increased for testing
    
    [Tooltip("Disable summoning for testing (fox won't auto-destroy)")]
    [SerializeField] private bool disableSummoning = false; // Set to true for testing
    
    [Tooltip("GodPlanExecutor reference to spawn angels (will be found automatically if null)")]
    [SerializeField] private GodPlanExecutor godExecutor;
    
    [Header("Loot")]
    [Tooltip("Health pickup prefab to drop when the fox is defeated")]
    [SerializeField] private GameObject healthPickupPrefab;
    
    [Header("References")]
    [Tooltip("Player Transform - will be found automatically if null")]
    [SerializeField] private Transform player;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = false;
    
    // Internal state
    private bool hasSummoned = false;
    private bool isFleeing = false;
    private float distanceToPlayer = float.MaxValue;
    
    void Start()
    {
        // Auto-find player if reference is null
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                
                if (showDebugLogs)
                {
                    Debug.Log($"FoxAI: Found player automatically: {player.name}");
                }
            }
            else
            {
                Debug.LogError("FoxAI: Player not found! Make sure player has 'Player' tag.");
            }
        }
        
        // Auto-find GodPlanExecutor if reference is null
        if (godExecutor == null)
        {
            godExecutor = FindObjectOfType<GodPlanExecutor>();
            
            if (godExecutor == null && showDebugLogs)
            {
                Debug.LogWarning("FoxAI: GodPlanExecutor not found in scene. Summoning will not work.");
            }
            else if (showDebugLogs)
            {
                Debug.Log("FoxAI: Found GodPlanExecutor automatically");
            }
        }
        
        // Start the summon timer coroutine only if summoning is enabled
        if (!disableSummoning)
        {
            StartCoroutine(SummonTimerCoroutine());
        }
        else if (showDebugLogs)
        {
            Debug.Log("FoxAI: Summoning disabled for testing - fox will not auto-destroy");
        }
    }
    
    void Update()
    {
        // If no player reference, can't do anything
        if (player == null)
        {
            return;
        }
        
        // Calculate distance to player
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is within flee distance
        if (distanceToPlayer <= fleeDistance)
        {
            // Flee from player
            if (!isFleeing)
            {
                isFleeing = true;
                if (showDebugLogs)
                {
                    Debug.Log($"FoxAI: Player detected! Fleeing! (Distance: {distanceToPlayer:F2})");
                }
            }
            
            FleeFromPlayer();
        }
        else
        {
            // Player is too far away, stop fleeing
            if (isFleeing)
            {
                isFleeing = false;
                if (showDebugLogs)
                {
                    Debug.Log($"FoxAI: Player out of range. Stopping flee. (Distance: {distanceToPlayer:F2})");
                }
            }
        }
    }
    
    /// <summary>
    /// Makes the fox flee away from the player.
    /// Calculates flee direction and moves away, facing the movement direction.
    /// Keeps fox within dungeon bounds (0-9 on X and Z) and on the ground (Y = 0.5).
    /// </summary>
    private void FleeFromPlayer()
    {
        // Calculate direction away from player (flattened to XZ plane - ignore Y)
        Vector3 toPlayer = player.position - transform.position;
        Vector3 fleeDirection = new Vector3(-toPlayer.x, 0f, -toPlayer.z).normalized;
        
        // Calculate new position (only move in XZ plane)
        Vector3 newPosition = transform.position + fleeDirection * moveSpeed * Time.deltaTime;
        
        // Clamp position to dungeon bounds (0-9 on X and Z)
        // Add small margin (0.5) to keep fox away from walls
        newPosition.x = Mathf.Clamp(newPosition.x, 0.5f, 8.5f);
        newPosition.z = Mathf.Clamp(newPosition.z, 0.5f, 8.5f);
        
        // Keep Y position fixed at ground level (0.5)
        newPosition.y = 0.5f;
        
        // Apply clamped position
        transform.position = newPosition;
        
        // Face the direction we're moving (away from player, on XZ plane)
        Vector3 lookDirection = transform.position + fleeDirection;
        lookDirection.y = transform.position.y; // Keep look direction at same height
        transform.LookAt(lookDirection);
        
        if (showDebugLogs)
        {
            Debug.Log($"FoxAI: Fleeing from player at speed {moveSpeed}");
        }
    }
    
    /// <summary>
    /// Coroutine that waits for the summon timer, then makes the fox howl and summon an angel.
    /// After summoning, the fox destroys itself.
    /// </summary>
    private IEnumerator SummonTimerCoroutine()
    {
        // Wait for the summon timer duration
        yield return new WaitForSeconds(summonTimer);
        
        // Check if we've already summoned (in case coroutine runs multiple times)
        if (hasSummoned)
        {
            yield break;
        }
        
        hasSummoned = true;
        
        // Howl and summon
        if (showDebugLogs)
        {
            Debug.Log("FoxAI: FOX HOWLS! Summoning God's enforcer!");
        }
        
        Debug.Log("FOX HOWLS! Summoning God's enforcer!");
        
        // Summon white angel at fox's position
        SummonAngelAtPosition(transform.position);
        
        // Destroy self after summoning
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Summons a white angel at the specified position using GodPlanExecutor.
    /// Finds the closest spawn point to the position and creates a GodAction.
    /// </summary>
    /// <param name="position">World position where the angel should spawn</param>
    private void SummonAngelAtPosition(Vector3 position)
    {
        if (godExecutor == null)
        {
            Debug.LogError("FoxAI: Cannot summon angel - GodPlanExecutor not assigned!");
            return;
        }
        
        // Find the closest spawn point to the fox's position
        string closestSpawnPoint = FindClosestSpawnPoint(position);
        
        if (string.IsNullOrEmpty(closestSpawnPoint))
        {
            Debug.LogWarning("FoxAI: Could not find closest spawn point. Using default.");
            closestSpawnPoint = "north_wall_center"; // Fallback
        }
        
        // Create a GodAction to spawn white angel
        GodAction summonAction = new GodAction
        {
            action = "spawn_white_angel",
            delay_seconds = 0f, // Immediate spawn
            spawn_point = closestSpawnPoint
        };
        
        // Execute the plan with single action
        godExecutor.ExecutePlan(new GodAction[] { summonAction });
        
        if (showDebugLogs)
        {
            Debug.Log($"FoxAI: Summoned white angel at spawn point: {closestSpawnPoint}");
        }
    }
    
    /// <summary>
    /// Finds the closest spawn point to the given position.
    /// Returns the spawn point name as a string.
    /// </summary>
    /// <param name="position">World position to find closest spawn point to</param>
    /// <returns>Name of the closest spawn point</returns>
    private string FindClosestSpawnPoint(Vector3 position)
    {
        // Define all spawn point positions (matching GodPlanExecutor positions)
        Vector3[] spawnPositions = new Vector3[]
        {
            new Vector3(4.5f, 1f, 8f),  // north_wall_center
            new Vector3(4.5f, 1f, 1f),  // south_wall_center
            new Vector3(8f, 1f, 4.5f),  // east_wall_center
            new Vector3(1f, 1f, 4.5f),  // west_wall_center
            new Vector3(8f, 0.5f, 8f),  // northeast_corner
            new Vector3(1f, 0.5f, 8f),  // northwest_corner
            new Vector3(8f, 0.5f, 1f),  // southeast_corner
            new Vector3(1f, 0.5f, 1f)   // southwest_corner
        };
        
        string[] spawnPointNames = new string[]
        {
            "north_wall_center",
            "south_wall_center",
            "east_wall_center",
            "west_wall_center",
            "northeast_corner",
            "northwest_corner",
            "southeast_corner",
            "southwest_corner"
        };
        
        // Find closest spawn point
        float closestDistance = float.MaxValue;
        int closestIndex = 0;
        
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            float distance = Vector3.Distance(position, spawnPositions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        
        return spawnPointNames[closestIndex];
    }
    
    /// <summary>
    /// Called when the fox is destroyed.
    /// Drops a health pickup at the fox's current position if killed by player.
    /// </summary>
    void OnDestroy()
    {
        // Drop health pickup if we have the prefab
        // Only skip dropping if we summoned (fox despawns after summoning)
        if (healthPickupPrefab != null && !hasSummoned)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
            
            if (showDebugLogs)
            {
                Debug.Log("FoxAI: Fox defeated! Dropped health pickup");
            }
            
            Debug.Log("Fox defeated! Dropped health pickup");
        }
    }
}

