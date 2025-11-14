using System.Collections;
using UnityEngine;

/// <summary>
/// Captures screenshots from the camera view and sends them to Claude API for analysis.
/// Attach this script to the Player GameObject or Camera GameObject.
/// </summary>
public class ScreenshotCapture : MonoBehaviour
{
    [Header("Claude API Client Reference")]
    [Tooltip("Drag the GameObject with ClaudeAPIClient component here")]
    [SerializeField] private ClaudeAPIClient claudeAPIClient;
    
    [Header("Screenshot Settings")]
    [Tooltip("Camera to capture from (if null, uses main camera)")]
    [SerializeField] private Camera targetCamera;
    
    [Tooltip("Prompt to send to Claude with the screenshot")]
    [SerializeField] private string defaultPrompt = "What do you see in this dungeon? Give me a hint about what to do next.";
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Flag to prevent multiple simultaneous captures
    private bool isCapturing = false;
    
    void Start()
    {
        // If no camera is assigned, try to get the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            
            if (targetCamera == null)
            {
                // If no main camera, try to get camera component from this GameObject
                targetCamera = GetComponent<Camera>();
                
                if (targetCamera == null && showDebugLogs)
                {
                    Debug.LogWarning("ScreenshotCapture: No camera found. Please assign a camera in the Inspector.");
                }
            }
        }
        
        // If no ClaudeAPIClient is assigned, try to find it in the scene
        if (claudeAPIClient == null)
        {
            claudeAPIClient = FindObjectOfType<ClaudeAPIClient>();
            
            if (claudeAPIClient == null && showDebugLogs)
            {
                Debug.LogWarning("ScreenshotCapture: No ClaudeAPIClient found. Please assign it in the Inspector.");
            }
        }
    }
    
    void Update()
    {
        // Check if T key is pressed
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Only capture if not already capturing
            if (!isCapturing)
            {
                CaptureAndSendScreenshot();
            }
            else if (showDebugLogs)
            {
                Debug.Log("ScreenshotCapture: Already capturing, please wait...");
            }
        }
    }
    
    /// <summary>
    /// Captures a screenshot from the camera and sends it to Claude API.
    /// </summary>
    private void CaptureAndSendScreenshot()
    {
        // Validate components
        if (targetCamera == null)
        {
            Debug.LogError("ScreenshotCapture: No camera assigned!");
            return;
        }
        
        if (claudeAPIClient == null)
        {
            Debug.LogError("ScreenshotCapture: No ClaudeAPIClient assigned!");
            return;
        }
        
        // Start the coroutine to handle async screenshot capture and API call
        StartCoroutine(CaptureAndSendCoroutine());
    }
    
    /// <summary>
    /// Coroutine that handles the async screenshot capture and API communication.
    /// Unity coroutines are needed to properly handle async/await in Unity.
    /// </summary>
    private IEnumerator CaptureAndSendCoroutine()
    {
        isCapturing = true;
        
        if (showDebugLogs)
        {
            Debug.Log("ScreenshotCapture: Starting screenshot capture...");
        }
        
        // Step 1: Capture screenshot as Texture2D
        // Note: ScreenCapture.CaptureScreenshotAsTexture() captures the entire screen,
        // not just the camera view. For camera-specific capture, consider using RenderTexture.
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        
        // Wait one frame to ensure the texture is fully captured
        yield return null;
        
        if (screenshotTexture == null)
        {
            Debug.LogError("ScreenshotCapture: Failed to capture screenshot!");
            isCapturing = false;
            yield break;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"ScreenshotCapture: Screenshot captured! Size: {screenshotTexture.width}x{screenshotTexture.height}");
        }
        
        // Step 2: Convert Texture2D to PNG byte array
        // EncodeToPNG() converts the texture to PNG format and returns the bytes
        byte[] imageBytes = screenshotTexture.EncodeToPNG();
        
        if (imageBytes == null || imageBytes.Length == 0)
        {
            Debug.LogError("ScreenshotCapture: Failed to encode screenshot to PNG!");
            Destroy(screenshotTexture);
            isCapturing = false;
            yield break;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"ScreenshotCapture: Image encoded to PNG. Size: {imageBytes.Length} bytes");
        }
        
        // Step 3: Send screenshot to Claude API
        // Since SendScreenshot is async, we need to wrap it in a coroutine-friendly way
        System.Threading.Tasks.Task<string> apiTask = claudeAPIClient.SendScreenshot(imageBytes, defaultPrompt);
        
        // Wait for the async task to complete
        // This coroutine will yield until the API call finishes
        while (!apiTask.IsCompleted)
        {
            yield return null; // Wait one frame
        }
        
        // Step 4: Get the result and log it
        string claudeResponse = apiTask.Result;
        
        if (showDebugLogs)
        {
            Debug.Log("ScreenshotCapture: Claude API Response:");
        }
        
        Debug.Log($"Claude: {claudeResponse}");
        
        // Clean up the texture
        Destroy(screenshotTexture);
        
        isCapturing = false;
        
        if (showDebugLogs)
        {
            Debug.Log("ScreenshotCapture: Process complete!");
        }
    }
    
    /// <summary>
    /// Public method to capture and send screenshot programmatically.
    /// </summary>
    /// <param name="customPrompt">Optional custom prompt to send with the screenshot</param>
    public void CaptureAndSend(string customPrompt = null)
    {
        if (isCapturing)
        {
            if (showDebugLogs)
            {
                Debug.Log("ScreenshotCapture: Already capturing, please wait...");
            }
            return;
        }
        
        // Use custom prompt if provided, otherwise use default
        if (!string.IsNullOrEmpty(customPrompt))
        {
            string originalPrompt = defaultPrompt;
            defaultPrompt = customPrompt;
            CaptureAndSendScreenshot();
            defaultPrompt = originalPrompt; // Restore original prompt
        }
        else
        {
            CaptureAndSendScreenshot();
        }
    }
}

