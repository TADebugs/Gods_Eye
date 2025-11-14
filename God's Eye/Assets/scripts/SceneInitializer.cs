using UnityEngine;

/// <summary>
/// Master controller script that serves as the entry point for scene setup.
/// This script coordinates the initialization of all scene managers (DungeonManager, LightingSetup, etc.)
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    // Public references to the manager scripts
    // These can be assigned in the Unity Inspector
    [Header("Scene Managers")]
    public DungeonManager dungeonManager;
    public LightingSetup lightingSetup;
    
    /// <summary>
    /// Entry point for scene setup - called when the scene starts
    /// </summary>
    void Start()
    {
        // Initialize the dungeon (floor and walls)
        if (dungeonManager != null)
        {
            dungeonManager.Initialize();
        }
        else
        {
            Debug.LogWarning("DungeonManager reference is missing! Please assign it in the Inspector.");
        }
        
        // Initialize the lighting setup (ambient, point lights, directional light)
        if (lightingSetup != null)
        {
            lightingSetup.Initialize();
        }
        else
        {
            Debug.LogWarning("LightingSetup reference is missing! Please assign it in the Inspector.");
        }
    }
}

