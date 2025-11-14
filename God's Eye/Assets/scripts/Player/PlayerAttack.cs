using UnityEngine;

/// <summary>
/// Simple click-to-attack system using raycasts.
/// Player attacks enemies by clicking on them within range.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Damage dealt per attack")]
    [SerializeField] private float attackDamage = 50f;
    
    [Tooltip("Maximum range for attacks")]
    [SerializeField] private float attackRange = 10f;
    
    [Tooltip("Time in seconds between attacks")]
    [SerializeField] private float attackCooldown = 0.5f; // Default: 0.5 seconds
    
    [Tooltip("Layer mask for what can be hit (set to enemy layers)")]
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("References")]
    [Tooltip("Player camera for raycast origin (will use Camera.main if null)")]
    [SerializeField] private Camera playerCamera;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Internal state
    private float lastAttackTime = 0f;
    
    void Start()
    {
        // Force cooldown to reasonable value (override any Inspector value that's too high)
        if (attackCooldown > 2f)
        {
            Debug.LogWarning($"PlayerAttack: Cooldown was {attackCooldown}s, forcing to 0.5s");
            attackCooldown = 0.5f;
        }
        
        // Auto-find camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            
            if (playerCamera == null)
            {
                Debug.LogError("PlayerAttack: No camera found! Please assign a camera or ensure Camera.main exists.");
            }
            else if (showDebugLogs)
            {
                Debug.Log("PlayerAttack: Using Camera.main for attacks");
            }
        }
    }
    
    void Update()
    {
        // Check if space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Check if cooldown has passed
            float timeSinceLastAttack = Time.time - lastAttackTime;
            
            if (timeSinceLastAttack >= attackCooldown)
            {
                // Perform attack
                PerformAttack();
                
                // Reset cooldown
                lastAttackTime = Time.time;
            }
            else if (showDebugLogs)
            {
                Debug.Log($"PlayerAttack: Attack on cooldown ({attackCooldown - timeSinceLastAttack:F2}s remaining)");
            }
        }
    }
    
    /// <summary>
    /// Performs a raycast attack from the camera forward.
    /// Hits enemies within range and deals damage.
    /// </summary>
    private void PerformAttack()
    {
        if (playerCamera == null)
        {
            Debug.LogError("PlayerAttack: Cannot attack - no camera assigned!");
            return;
        }
        
        // Create ray from camera center forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        // Hit everything (ignore layer mask for now to make it work)
        int layerMask = ~0; // Everything
        
        if (showDebugLogs)
        {
            Debug.Log($"PlayerAttack: Attacking! Ray from {ray.origin} direction {ray.direction}, range {attackRange}");
        }
        
        // Perform raycast with range - hit everything
        if (Physics.Raycast(ray, out hit, attackRange, layerMask))
        {
            if (showDebugLogs)
            {
                Debug.Log($"PlayerAttack: Raycast hit: {hit.collider.gameObject.name} (tag: '{hit.collider.tag}') at distance {hit.distance:F2}");
            }
            
            // Check if hit object has "Enemy" tag
            if (hit.collider.CompareTag("Enemy"))
            {
                // Hit an enemy!
                Debug.Log($"Hit enemy! {hit.collider.gameObject.name}");
                
                // Destroy the enemy
                Destroy(hit.collider.gameObject);
                
                Debug.Log($"PlayerAttack: Enemy {hit.collider.gameObject.name} destroyed!");
            }
            else
            {
                // Hit something but it's not an enemy
                Debug.LogWarning($"PlayerAttack: Hit {hit.collider.gameObject.name} but tag is '{hit.collider.tag}' (not 'Enemy'). Make sure enemies have 'Enemy' tag!");
            }
        }
        else
        {
            // Nothing hit
            Debug.Log("PlayerAttack: Attack missed - nothing hit within range. Make sure you're aiming at enemies!");
        }
    }
    
    /// <summary>
    /// Draws gizmos in the Unity Editor to visualize attack range.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            // Draw attack range as a line from camera
            Gizmos.color = Color.red;
            Vector3 rayEnd = playerCamera.transform.position + playerCamera.transform.forward * attackRange;
            Gizmos.DrawLine(playerCamera.transform.position, rayEnd);
            
            // Draw a small sphere at max range
            Gizmos.DrawWireSphere(rayEnd, 0.2f);
        }
    }
}

