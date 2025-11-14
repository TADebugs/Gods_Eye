using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages player health, damage, healing, and death.
/// Handles UI updates and scene reload on death.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health the player can have")]
    [SerializeField] private float maxHealth = 100f;
    
    [Tooltip("Current health (will be initialized to maxHealth in Start)")]
    [SerializeField] private float currentHealth;
    
    [Header("UI References")]
    [Tooltip("Text component to display health (e.g., 'HP: 85/100')")]
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Property to access current health (read-only from outside)
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    
    void Start()
    {
        // Initialize health to maximum
        currentHealth = maxHealth;
        
        // Update UI on start
        UpdateHealthUI();
        
        if (showDebugLogs)
        {
            Debug.Log($"PlayerHealth: Initialized with {currentHealth}/{maxHealth} HP");
        }
    }
    
    /// <summary>
    /// Reduces player health by the specified damage amount.
    /// Updates UI and checks for death.
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    public void TakeDamage(float damage)
    {
        if (damage <= 0)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"PlayerHealth: Invalid damage amount ({damage}) - must be positive");
            }
            return;
        }
        
        // Reduce health
        currentHealth -= damage;
        
        // Clamp health to minimum of 0
        currentHealth = Mathf.Max(0f, currentHealth);
        
        if (showDebugLogs)
        {
            Debug.Log($"PlayerHealth: Took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }
        
        // Update UI
        UpdateHealthUI();
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Increases player health by the specified amount.
    /// Health is capped at maxHealth.
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    public void Heal(float amount)
    {
        if (amount <= 0)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"PlayerHealth: Invalid heal amount ({amount}) - must be positive");
            }
            return;
        }
        
        // Increase health
        currentHealth += amount;
        
        // Clamp health to maximum
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        if (showDebugLogs)
        {
            Debug.Log($"PlayerHealth: Healed {amount}. Health: {currentHealth}/{maxHealth}");
        }
        
        // Update UI
        UpdateHealthUI();
    }
    
    /// <summary>
    /// Handles player death.
    /// Logs death message and reloads the scene after 2 seconds.
    /// </summary>
    private void Die()
    {
        if (showDebugLogs)
        {
            Debug.Log("PlayerHealth: Player has died!");
        }
        
        Debug.Log("PLAYER DEFEATED - God's will is absolute");
        
        // Reload the current scene after 2 seconds
        Invoke(nameof(ReloadScene), 2f);
    }
    
    /// <summary>
    /// Reloads the current scene.
    /// Called after death delay.
    /// </summary>
    private void ReloadScene()
    {
        if (showDebugLogs)
        {
            Debug.Log("PlayerHealth: Reloading scene...");
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Updates the health text UI to display current/max health.
    /// Format: "HP: 85/100"
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("PlayerHealth: Health text UI not assigned - cannot update display");
        }
    }
    
    /// <summary>
    /// Public method to check if player is alive.
    /// </summary>
    /// <returns>True if player is alive (health > 0), false otherwise</returns>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    /// <summary>
    /// Public method to get health percentage (0-1).
    /// </summary>
    /// <returns>Health percentage as float between 0 and 1</returns>
    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? currentHealth / maxHealth : 0f;
    }
}

