using UnityEngine;

public class LightingSetup : MonoBehaviour
{
    // Store references to the 4 corner point lights for flickering effect
    private Light[] cornerLights;
    
    // Base intensity for corner lights (will flicker around this value)
    private const float BASE_INTENSITY = 1.5f;
    
    // Flicker range for corner lights (more dramatic)
    private const float FLICKER_MIN = 1.5f;  // Much dimmer for dramatic effect
    private const float FLICKER_MAX = 4.5f;  // Brighter peaks
    
    // Ambient light color range for pulsing
    private readonly Color ambientMin = new Color(0.15f, 0.15f, 0.2f);
    private readonly Color ambientMax = new Color(0.25f, 0.25f, 0.3f);
    
    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        // Flicker the corner lights independently
        FlickerCornerLights();
        
        // Pulse the ambient light
        PulseAmbientLight();
    }
    
    /// <summary>
    /// Public initialization method that can be called by SceneInitializer
    /// </summary>
    public void Initialize()
    {
        // Set up atmospheric lighting for the dungeon
        SetupAmbientLight();
        SetupPointLights();
        SetupDirectionalLight();
        SetupFog();
    }
    
    /// <summary>
    /// Sets the ambient light to a very dark color for dungeon atmosphere
    /// </summary>
    private void SetupAmbientLight()
    {
        // Very dark ambient light (RGB: 0.05, 0.05, 0.08) - will pulse in Update()
        Color veryDark = new Color(0.05f, 0.05f, 0.08f);
        
        // Set the ambient light for the entire scene
        RenderSettings.ambientLight = veryDark;
    }
    
    /// <summary>
    /// Pulses the ambient light smoothly between min and max values for atmospheric effect
    /// </summary>
    private void PulseAmbientLight()
    {
        // Use PingPong to create a smooth back-and-forth value between 0 and 1
        // Time.time * 0.5f controls the speed of the pulse (slower = 0.5f multiplier)
        float pingPongValue = Mathf.PingPong(Time.time * 0.5f, 1f);
        
        // Lerp between min and max ambient colors based on ping pong value
        Color currentAmbient = Color.Lerp(ambientMin, ambientMax, pingPongValue);
        
        // Apply the pulsing ambient light
        RenderSettings.ambientLight = currentAmbient;
    }
    
    /// <summary>
    /// Creates 4 point lights positioned at the corners of the 10x10 dungeon grid
    /// </summary>
    private void SetupPointLights()
    {
        // Orange color for torch-like lighting (RGB: 1.0, 0.6, 0.3)
        Color orangeColor = new Color(1f, 0.6f, 0.3f);
        
        // Light properties for corner lights (reduced intensity)
        float intensity = 3.5f;  // Reduced for less brightness
        float range = 15f;     // Increased from 8f
        
        // Corner positions for the 4 point lights (for 10x10 grid from 0-9)
        Vector3[] cornerPositions = new Vector3[]
        {
            new Vector3(0f, 2f, 0f),   // Bottom-left corner
            new Vector3(9f, 2f, 0f),   // Bottom-right corner
            new Vector3(0f, 2f, 9f),   // Top-left corner
            new Vector3(9f, 2f, 9f)    // Top-right corner
        };
        
        // Initialize array to store light references for flickering
        cornerLights = new Light[4];
        
        // Create a point light at each corner
        for (int i = 0; i < cornerPositions.Length; i++)
        {
            // Create a new GameObject for the light
            GameObject lightObject = new GameObject($"CornerLight_{i + 1}");
            
            // Add the Light component to the GameObject
            Light pointLight = lightObject.AddComponent<Light>();
            
            // Store reference for flickering effect
            cornerLights[i] = pointLight;
            
            // Set the light type to point light
            pointLight.type = LightType.Point;
            
            // Set the position of the light
            lightObject.transform.position = cornerPositions[i];
            
            // Configure the light properties
            pointLight.color = orangeColor;
            pointLight.intensity = intensity;
            pointLight.range = range;
            pointLight.shadows = LightShadows.Soft; // Soft Shadows
            
            // Debug log to verify light creation
            Debug.Log($"Created CornerLight_{i + 1} at position {cornerPositions[i]} with intensity {intensity}");
        }
        
        // Create a center light for better room illumination
        GameObject centerLightObject = new GameObject("CenterLight");
        Light centerLight = centerLightObject.AddComponent<Light>();
        centerLight.type = LightType.Point;
        centerLightObject.transform.position = new Vector3(4.5f, 3f, 4.5f); // Center of 10x10 grid
        centerLight.color = orangeColor;
        centerLight.intensity = 2.5f;  // Reduced from 3f
        centerLight.range = 12f;
        centerLight.shadows = LightShadows.Soft;
        
        Debug.Log($"Created CenterLight at position (4.5, 3, 4.5) with intensity 2.5f");
    }
    
    /// <summary>
    /// Flickers the corner lights independently to simulate torch flames
    /// Each light flickers at a slightly different rate for natural variation
    /// </summary>
    private void FlickerCornerLights()
    {
        if (cornerLights == null)
        {
            Debug.Log("FlickerCornerLights: cornerLights array is null!");
            return;
        }
        
        Debug.Log($"Flickering {cornerLights.Length} lights");
        
        // Flicker each light independently using Perlin noise for smooth variation
        for (int i = 0; i < cornerLights.Length; i++)
        {
            if (cornerLights[i] != null)
            {
                // Use Perlin noise for smooth, natural flickering
                // Each light gets a different offset (i * 100) for independent flickering
                float noise = Mathf.PerlinNoise(Time.time * 5f, i * 100f); // Faster flicker for more dramatic effect
                
                // Map noise (0-1) to flicker intensity range
                float intensity = Mathf.Lerp(FLICKER_MIN, FLICKER_MAX, noise);
                
                // Apply the flickering intensity
                cornerLights[i].intensity = intensity;
            }
        }
    }
    
    /// <summary>
    /// Creates a directional light from above for subtle fill lighting
    /// </summary>
    private void SetupDirectionalLight()
    {
        // Create a new GameObject for the directional light
        GameObject directionalLightObject = new GameObject("DirectionalFillLight");
        
        // Add the Light component to the GameObject
        Light directionalLight = directionalLightObject.AddComponent<Light>();
        
        // Set the light type to directional
        directionalLight.type = LightType.Directional;
        
        // Position the light above the scene and rotate it to point down
        directionalLightObject.transform.position = new Vector3(0f, 10f, 0f);
        directionalLightObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Point straight down
        
        // Set intensity for fill light (increased for better visibility)
        directionalLight.intensity = 0.8f;  // Increased from 0.3f
    }
    
    /// <summary>
    /// Sets up fog for atmospheric depth and visual effect
    /// </summary>
    private void SetupFog()
    {
        // Enable fog in the scene
        RenderSettings.fog = true;
        
        // Set fog mode to Exponential for smooth distance-based fog
        RenderSettings.fogMode = FogMode.Exponential;
        
        // Set fog color to dark blue-gray for dungeon atmosphere
        RenderSettings.fogColor = new Color(0.1f, 0.1f, 0.15f);
        
        // Set fog density (higher = more fog, lower = less fog)
        // Reduced to 0.05f so it doesn't obscure vision
        RenderSettings.fogDensity = 0.05f;  // Reduced from 0.15f
    }
}

