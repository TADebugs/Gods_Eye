using UnityEngine;

/// <summary>
/// AI controller for white angels that chase and attack the player.
/// Implements three behavior states: Idle, Chase, and Attack based on distance to player.
/// </summary>
public class WhiteAngelAI : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed at which the angel moves toward the player")]
    [SerializeField] private float moveSpeed = 3f;
    
    [Tooltip("Distance at which the angel starts chasing the player")]
    [SerializeField] private float chaseRange = 15f;
    
    [Tooltip("Distance at which the angel can attack the player")]
    [SerializeField] private float attackRange = 2f;
    
    [Header("Combat")]
    [Tooltip("Damage dealt per attack")]
    [SerializeField] private float attackDamage = 1f; // Default: 1 damage per attack
    
    [Tooltip("Time in seconds between attacks")]
    [SerializeField] private float attackCooldown = 1.5f;
    
    [Header("References")]
    [Tooltip("Player Transform - will be found automatically if null")]
    [SerializeField] private Transform player;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = false;
    
    // Internal state tracking
    private float lastAttackTime = 0f;
    private float distanceToPlayer = float.MaxValue;
    
    // Behavior states enum for clarity
    private enum AIState
    {
        Idle,    // Standing still, looking around
        Chase,   // Moving toward player
        Attack   // Attacking player
    }
    
    private AIState currentState = AIState.Idle;
    
    void Start()
    {
        // Force attack damage to 1f (override any Inspector value)
        attackDamage = 1f;
        
        // Auto-find player if reference is null
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                
                if (showDebugLogs)
                {
                    Debug.Log($"WhiteAngelAI: Found player automatically: {player.name}");
                }
            }
            else
            {
                Debug.LogError("WhiteAngelAI: Player not found! Make sure player has 'Player' tag.");
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
    /// Sets the AI state and logs state changes for debugging.
    /// </summary>
    /// <param name="newState">The new state to transition to</param>
    private void SetState(AIState newState)
    {
        if (currentState != newState)
        {
            if (showDebugLogs)
            {
                Debug.Log($"WhiteAngelAI: State changed from {currentState} to {newState} (Distance: {distanceToPlayer:F2})");
            }
            currentState = newState;
        }
    }
    
    /// <summary>
    /// Handles Idle behavior - angel stands still and looks around.
    /// Currently does nothing, but could add idle animations or patrol behavior here.
    /// </summary>
    private void HandleIdle()
    {
        // Idle state - do nothing, just stand still
        // Could add idle animations, looking around, or patrol behavior here
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
            Debug.Log($"WhiteAngelAI: Chasing player (Distance: {distanceToPlayer:F2})");
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
                Debug.Log($"WhiteAngelAI: Waiting for attack cooldown ({attackCooldown - timeSinceLastAttack:F2}s remaining)");
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
            Debug.Log($"WhiteAngelAI: ATTACKING player! Damage: {attackDamage}");
        }
        
        // Deal damage to player health system
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning("WhiteAngelAI: PlayerHealth component not found on player!");
        }
        
        Debug.Log($"GOD'S ENFORCER STRIKES: White Angel deals {attackDamage} damage to player!");
    }
    
    /// <summary>
    /// Draws gizmos in the Unity Editor to visualize chase and attack ranges.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw chase range (yellow wireframe sphere)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // Draw attack range (red wireframe sphere)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

