using UnityEngine;

/// <summary>
/// Collectible health item that heals the player on contact.
/// Requires: Collider with "Is Trigger" checked, Rigidbody with "Is Kinematic" checked.
/// </summary>
public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Amount of health to restore when collected")]
    [SerializeField] private float healAmount = 25f;
    
    [Tooltip("Whether to destroy this pickup after collection")]
    [SerializeField] private bool destroyOnPickup = true;
    
    [Header("Visual Feedback")]
    [Tooltip("Speed of rotation in degrees per second")]
    [SerializeField] private float rotationSpeed = 50f;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true;
    
    void Start()
    {
        // Verify required components exist
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"HealthPickup: No Collider component found on {gameObject.name}! Add a Collider with 'Is Trigger' checked.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"HealthPickup: Collider on {gameObject.name} is not set as Trigger! Enable 'Is Trigger' for proper pickup detection.");
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"HealthPickup: No Rigidbody component found on {gameObject.name}! Add a Rigidbody with 'Is Kinematic' checked.");
        }
        else if (!rb.isKinematic)
        {
            Debug.LogWarning($"HealthPickup: Rigidbody on {gameObject.name} is not kinematic! Enable 'Is Kinematic' to prevent physics interactions.");
        }
    }
    
    void Update()
    {
        // Rotate the pickup to make it visually obvious and collectible-looking
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Called when another collider enters this trigger.
    /// Checks if it's the player and heals them if so.
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Get PlayerHealth component from the player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth == null)
            {
                // Try getting from parent if not on the collider GameObject itself
                playerHealth = other.GetComponentInParent<PlayerHealth>();
            }
            
            if (playerHealth != null)
            {
                // Heal the player
                playerHealth.Heal(healAmount);
                
                if (showDebugLogs)
                {
                    Debug.Log($"HealthPickup: Player collected health: +{healAmount} HP");
                }
                
                // Destroy the pickup if configured to do so
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"HealthPickup: Player collider detected but PlayerHealth component not found on {other.name}");
            }
        }
    }
}

