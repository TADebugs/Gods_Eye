using UnityEngine;

/// <summary>
/// AI controller for red angels - elite enemies with freeze gaze mechanic.
/// When eye contact is made with the player, both freeze for a duration.
/// </summary>
public class RedAngelAI : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed at which the angel moves toward the player")]
    [SerializeField] private float moveSpeed = 2.5f; // Slower than white angels
    
    [Tooltip("Distance at which the angel starts chasing the player")]
    [SerializeField] private float chaseRange = 15f;
    
    [Tooltip("Distance at which the angel can attack the player")]
    [SerializeField] private float attackRange = 2f;
    
    [Header("Combat")]
    [Tooltip("Damage dealt per attack")]
    [SerializeField] private float attackDamage = 20f; // 2x white angel damage
    
    [Tooltip("Time in seconds between attacks")]
    [SerializeField] private float attackCooldown = 2f; // Slower attacks but stronger
    
    [Header("Freeze Gaze")]
    [Tooltip("Cone angle in degrees for eye contact detection (larger = easier to trigger)")]
    [SerializeField] private float gazeDetectionAngle = 45f; // Increased from 30f for easier detection
    
    [Tooltip("Duration in seconds that both player and angel freeze")]
    [SerializeField] private float freezeDuration = 3f;
    
    [Tooltip("Interval in seconds between gaze checks")]
    [SerializeField] private float gazeCheckInterval = 0.1f; // Check more frequently
    
    [Header("References")]
    [Tooltip("Player Transform - will be found automatically if null")]
    [SerializeField] private Transform player;
    
    [Tooltip("Player Camera - will be found automatically if null")]
    [SerializeField] private Camera playerCamera;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true; // Enabled by default for testing
    
    // Internal state tracking
    private float lastAttackTime = 0f;
    private float distanceToPlayer = float.MaxValue;
    private float lastGazeCheckTime = 0f;
    private bool isFrozen = false;
    private float freezeEndTime = 0f;
    private float freezeCooldownEndTime = 0f; // Cooldown after unfreezing to prevent spam
    private const float FREEZE_COOLDOWN = 2f; // 2 seconds cooldown after unfreezing
    
    // Behavior states enum
    private enum AIState
    {
        Idle,    // Standing still, looking around
        Chase,   // Moving toward player
        Attack   // Attacking player
    }
    
    private AIState currentState = AIState.Idle;
    
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
                    Debug.Log($"RedAngelAI: Found player automatically: {player.name}");
                }
            }
            else
            {
                Debug.LogError("RedAngelAI: Player not found! Make sure player has 'Player' tag.");
            }
        }
        
        // Auto-find player camera if reference is null
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            
            if (playerCamera == null && player != null)
            {
                // Try to get camera from player GameObject or its children
                playerCamera = player.GetComponentInChildren<Camera>();
                
                // If still not found, try finding any camera in scene
                if (playerCamera == null)
                {
                    Camera[] allCameras = FindObjectsOfType<Camera>();
                    if (allCameras.Length > 0)
                    {
                        playerCamera = allCameras[0];
                        Debug.LogWarning($"RedAngelAI: Using first camera found in scene: {playerCamera.name}");
                    }
                }
            }
            
            if (playerCamera == null)
            {
                Debug.LogError("RedAngelAI: Player camera not found! Eye contact detection will not work. Make sure there's a camera in the scene.");
            }
            else if (showDebugLogs)
            {
                Debug.Log($"RedAngelAI: Found player camera: {playerCamera.name}");
            }
        }
        
        // Initialize last attack time to allow immediate first attack
        lastAttackTime = -attackCooldown;
    }
    
    void Update()
    {
        // If no player reference, can't do anything
        if (player == null)
        {
            return;
        }
        
        // Check if we're frozen
        if (isFrozen)
        {
            // Check if freeze duration has passed
            if (Time.time >= freezeEndTime)
            {
                Unfreeze();
            }
            // Don't do anything else while frozen
            return;
        }
        
        // Check for eye contact periodically (only if not in cooldown)
        if (Time.time >= freezeCooldownEndTime && Time.time - lastGazeCheckTime >= gazeCheckInterval)
        {
            lastGazeCheckTime = Time.time;
            CheckEyeContact();
        }
        
        // Calculate distance to player
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Determine behavior state based on distance
        if (distanceToPlayer > chaseRange)
        {
            // Player is too far away - Idle state
            SetState(AIState.Idle);
        }
        else if (distanceToPlayer > attackRange)
        {
            // Player is in chase range but not close enough to attack - Chase state
            SetState(AIState.Chase);
        }
        else
        {
            // Player is close enough to attack - Attack state
            SetState(AIState.Attack);
        }
        
        // Execute behavior based on current state
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdle();
                break;
                
            case AIState.Chase:
                HandleChase();
                break;
                
            case AIState.Attack:
                HandleAttack();
                break;
        }
    }
    
    /// <summary>
    /// Checks if the player is looking at the red angel (eye contact).
    /// Uses Vector3.Dot to check if red angel is within the player's view cone.
    /// Only triggers if player is not already frozen.
    /// </summary>
    private void CheckEyeContact()
    {
        if (playerCamera == null || player == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("RedAngelAI: Cannot check eye contact - playerCamera or player is null");
            }
            return;
        }
        
        // Check if player is already frozen - don't trigger again if they are
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.IsFrozen())
        {
            return; // Player already frozen, don't check again
        }
        
        // Get direction from player camera to red angel
        Vector3 directionToAngel = (transform.position - playerCamera.transform.position).normalized;
        
        // Get player camera's forward direction
        Vector3 cameraForward = playerCamera.transform.forward;
        
        // Use Vector3.Angle for simpler and more reliable angle calculation
        float angle = Vector3.Angle(cameraForward, directionToAngel);
        
        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, playerCamera.transform.position);
        
        if (showDebugLogs)
        {
            Debug.Log($"RedAngelAI: Eye contact check - Angle: {angle:F2}°, Detection: {gazeDetectionAngle}°, Distance: {distance:F2}");
        }
        
        // Check if angle is within detection range (player must be looking directly at red angel)
        if (angle <= gazeDetectionAngle)
        {
            // Eye contact detected!
            if (showDebugLogs)
            {
                Debug.Log($"RedAngelAI: EYE CONTACT DETECTED! Angle: {angle:F2}°");
            }
            TriggerFreeze();
        }
    }
    
    /// <summary>
    /// Triggers the mutual freeze when eye contact is detected.
    /// Freezes both the red angel and player for freezeDuration seconds.
    /// </summary>
    private void TriggerFreeze()
    {
        if (isFrozen)
        {
            return; // Already frozen
        }
        
        isFrozen = true;
        freezeEndTime = Time.time + freezeDuration;
        
        Debug.Log("MUTUAL FREEZE! Red Angel's gaze locks with player!");
        
        if (showDebugLogs)
        {
            Debug.Log($"RedAngelAI: Frozen for {freezeDuration} seconds");
        }
        
        // Freeze player controls
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Freeze();
            }
            else
            {
                Debug.LogWarning("RedAngelAI: PlayerMovement component not found - cannot freeze player!");
            }
        }
    }
    
    /// <summary>
    /// Unfreezes the red angel after the freeze duration.
    /// Also unfreezes the player and starts a cooldown period.
    /// </summary>
    private void Unfreeze()
    {
        isFrozen = false;
        
        // Set cooldown to prevent immediate re-freezing
        freezeCooldownEndTime = Time.time + FREEZE_COOLDOWN;
        
        // Unfreeze player controls
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Unfreeze();
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"RedAngelAI: Unfrozen - resuming normal behavior. Cooldown for {FREEZE_COOLDOWN} seconds.");
        }
    }
    
    /// <summary>
    /// Sets the AI state and logs state changes for debugging.
    /// </summary>
    /// <param name="newState">The new state to transition to</param>
    private void SetState(AIState newState)
    {
        if (currentState != newState)
        {
            if (showDebugLogs)
            {
                Debug.Log($"RedAngelAI: State changed from {currentState} to {newState} (Distance: {distanceToPlayer:F2})");
            }
            currentState = newState;
        }
    }
    
    /// <summary>
    /// Handles Idle behavior - angel stands still and looks around.
    /// </summary>
    private void HandleIdle()
    {
        // Idle state - do nothing, just stand still
    }
    
    /// <summary>
    /// Handles Chase behavior - angel moves toward the player.
    /// Uses MoveTowards for smooth movement and LookAt to face the player.
    /// </summary>
    private void HandleChase()
    {
        // Move toward player using MoveTowards for smooth movement
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
        
        // Face the player while chasing
        transform.LookAt(player.position);
        
        if (showDebugLogs)
        {
            Debug.Log($"RedAngelAI: Chasing player (Distance: {distanceToPlayer:F2})");
        }
    }
    
    /// <summary>
    /// Handles Attack behavior - angel attacks the player when in range.
    /// Checks cooldown before attacking and deals damage.
    /// </summary>
    private void HandleAttack()
    {
        // Face the player while attacking
        transform.LookAt(player.position);
        
        // Check if attack cooldown has passed
        float timeSinceLastAttack = Time.time - lastAttackTime;
        
        if (timeSinceLastAttack >= attackCooldown)
        {
            // Attack the player
            PerformAttack();
            
            // Reset cooldown timer
            lastAttackTime = Time.time;
        }
        else
        {
            // Still in cooldown, just wait
            if (showDebugLogs)
            {
                Debug.Log($"RedAngelAI: Waiting for attack cooldown ({attackCooldown - timeSinceLastAttack:F2}s remaining)");
            }
        }
    }
    
    /// <summary>
    /// Performs an attack on the player.
    /// Deals damage to the player's health system.
    /// </summary>
    private void PerformAttack()
    {
        if (showDebugLogs)
        {
            Debug.Log($"RedAngelAI: ATTACKING player! Damage: {attackDamage}");
        }
        
        // Deal damage to player health system
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning("RedAngelAI: PlayerHealth component not found on player!");
        }
        
        Debug.Log($"GOD'S ELITE ENFORCER STRIKES: Red Angel deals {attackDamage} damage to player!");
    }
    
    /// <summary>
    /// Draws gizmos in the Unity Editor to visualize chase, attack ranges, and gaze detection angle.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw chase range (yellow wireframe sphere)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // Draw attack range (red wireframe sphere)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw gaze detection cone if player camera exists
        if (playerCamera != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 directionToAngel = (transform.position - playerCamera.transform.position).normalized;
            Vector3 cameraForward = playerCamera.transform.forward;
            
            // Draw line from camera to angel
            Gizmos.DrawLine(playerCamera.transform.position, transform.position);
            
            // Draw cone visualization (simplified - just show the angle)
            float angle = Vector3.Angle(cameraForward, directionToAngel);
            if (angle <= gazeDetectionAngle)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, 0.3f); // Highlight when in gaze range
            }
        }
    }
}

