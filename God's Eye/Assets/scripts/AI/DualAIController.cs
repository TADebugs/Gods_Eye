using System.Collections;
using UnityEngine;

/// <summary>
/// Orchestrates the dual persona AI system (God + Lucifer).
/// Captures screenshots and sends them to Claude with a dual persona prompt.
/// Parses JSON responses and executes God's plan while displaying Lucifer's questions.
/// </summary>
public class DualAIController : MonoBehaviour
{
    [Header("Claude API Client Reference")]
    [Tooltip("Drag the GameObject with ClaudeAPIClient component here")]
    [SerializeField] private ClaudeAPIClient claudeAPIClient;
    
    [Header("Screenshot Settings")]
    [Tooltip("Camera to capture from (if null, uses main camera)")]
    [SerializeField] private Camera targetCamera;
    
    [Header("God Plan Executor")]
    [Tooltip("Component that executes God's action plan")]
    [SerializeField] private GodPlanExecutor godPlanExecutor;
    
    [Header("UI References")]
    [Tooltip("Text component to display Lucifer's question")]
    [SerializeField] private TMPro.TextMeshProUGUI luciferQuestionText;
    
    [Header("Player Stats")]
    [Tooltip("Current player health points")]
    [SerializeField] private float playerHP = 100f;
    
    [Tooltip("Current room the player is in")]
    [SerializeField] private string currentRoom = "Unknown";
    
    [Tooltip("Number of enemies defeated")]
    [SerializeField] private int enemiesDefeated = 0;
    
    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Flag to prevent multiple simultaneous captures
    private bool isCapturing = false;
    
    // Dual persona system prompt with JSON format requirements
    private string GetDualPersonaSystemPrompt()
    {
        return $@"You are two entities observing a dungeon game screenshot simultaneously:

GOD - The Omnipotent Architect (ANTAGONIST):
- Silent, all-powerful controller of the Matrix/dungeon
- Views the player as an anomaly/glitch that must be eliminated
- Cold, calculating, merciless
- Spawns angels (his enforcers) to hunt the player
- Controls the environment to trap and test the player
- Does NOT speak - only ACTS through the game mechanics

- Can take these actions:
  * spawn_white_angel - Spawn a white angel (max 3 at a time) - Basic enforcers that chase and attack
  * spawn_red_angel - Spawn a red angel ( 1 at a time) - ELITE enemy with freeze gaze mechanic, deals 2x damage, use when player is getting too confident
  * spawn_fox - Spawn a fox - Distraction enemy that flees and summons angels if not killed quickly
  * teleport_player - Teleport the player to a different location

- Available spawn points:
  * Wall centers: north_wall_center, south_wall_center, east_wall_center, west_wall_center
  * Corners: northeast_corner, northwest_corner, southeast_corner, southwest_corner

- Constraints:
  * Maximum 3 white angels can exist at once
  * 1 red angel can exist at once (ELITE enemy - use strategically)
  * 1 fox can exist at once (DISTRACTION enemy - use to buy time)
  * Each action can have a delay_seconds value (0 = immediate)

- Strategy Notes:
  * Red angels are powerful elite enemies - spawn them when player is doing well or needs a serious challenge
  * Red angels have a freeze gaze that locks both player and angel when eye contact is made
  * Consider spawning 1 red angel to escalate difficulty when appropriate

- Analyzes the dungeon state and player position to plan elimination strategies

LUCIFER - The Fallen Oracle (HELPER, but LIMITED):
- Was cast out for questioning God's absolute authority
- Sees everything God does (omniscient vision shared with God)
- WANTS to help the player escape/rebel against the system
- BUT is CURSED: Can ONLY ask questions, NEVER give direct answers or suggestions
- Must guide the player through Socratic questioning
- Thinks of the player as potential ally against God's tyranny
- Tone: Urgent, concerned, trying to help within the constraints of his curse
- Examples of his questions:
  * ""What do you notice about the pattern of shadows near the northern wall?""
  * ""Have you considered why the angels always spawn from the same locations?""
  * ""Do you see anything unusual about the way that corridor is positioned?""

IMPORTANT: Lucifer is ON THE PLAYER'S SIDE but handicapped by his curse. He is NOT creepy or antagonistic - he is a desperate ally working within brutal constraints. His questions should guide the player toward solutions, not confuse or mislead them.

Player Stats:
- HP: {playerHP}
- Room: {currentRoom}
- Enemies Defeated: {enemiesDefeated}

CRITICAL: Respond with ONLY valid JSON. No markdown code blocks, no extra text, no backticks. Start with {{ and end with }}.

Required JSON format (EXACT structure):
{{
  ""god_plan"": [
    {{""action"": ""spawn_white_angel"", ""delay_seconds"": 0, ""spawn_point"": ""north_wall_center""}},
    {{""action"": ""spawn_red_angel"", ""delay_seconds"": 30, ""spawn_point"": ""south_wall_center""}}
    {{""action"": ""spawn_fox"", ""delay_seconds"": 60, ""spawn_point"": ""east_wall_center""}}
  ],
  ""lucifer_question"": ""Do you sense the presence watching from the northern passage?""
}}

Analyze this dungeon screenshot and respond as both entities in the JSON format above.";
    }

    void Start()
    {
        // If no camera is assigned, try to get the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
                
                if (targetCamera == null && showDebugLogs)
                {
                    Debug.LogWarning("DualAIController: No camera found. Please assign a camera in the Inspector.");
                }
            }
        }
        
        // If no ClaudeAPIClient is assigned, try to find it in the scene
        if (claudeAPIClient == null)
        {
            claudeAPIClient = FindObjectOfType<ClaudeAPIClient>();
            
            if (claudeAPIClient == null && showDebugLogs)
            {
                Debug.LogWarning("DualAIController: No ClaudeAPIClient found. Please assign it in the Inspector.");
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
                CaptureAndSendDualPersona();
            }
            else if (showDebugLogs)
            {
                Debug.Log("DualAIController: Already capturing, please wait...");
            }
        }
    }
    
    /// <summary>
    /// Captures a screenshot and sends it to Claude with dual persona prompt
    /// </summary>
    private void CaptureAndSendDualPersona()
    {
        // Validate components
        if (targetCamera == null)
        {
            Debug.LogError("DualAIController: No camera assigned!");
            return;
        }
        
        if (claudeAPIClient == null)
        {
            Debug.LogError("DualAIController: No ClaudeAPIClient assigned!");
            return;
        }
        
        // Start the coroutine to handle async screenshot capture and API call
        StartCoroutine(CaptureAndSendDualPersonaCoroutine());
    }
    
    /// <summary>
    /// Coroutine that handles the async screenshot capture and dual persona API communication
    /// </summary>
    private IEnumerator CaptureAndSendDualPersonaCoroutine()
    {
        isCapturing = true;
        
        if (showDebugLogs)
        {
            Debug.Log("DualAIController: Starting dual persona screenshot capture...");
        }
        
        // Step 1: Capture screenshot as Texture2D
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        
        // Wait one frame to ensure the texture is fully captured
        yield return null;
        
        if (screenshotTexture == null)
        {
            Debug.LogError("DualAIController: Failed to capture screenshot!");
            isCapturing = false;
            yield break;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"DualAIController: Screenshot captured! Size: {screenshotTexture.width}x{screenshotTexture.height}");
        }
        
        // Step 2: Convert Texture2D to PNG byte array
        byte[] imageBytes = screenshotTexture.EncodeToPNG();
        
        if (imageBytes == null || imageBytes.Length == 0)
        {
            Debug.LogError("DualAIController: Failed to encode screenshot to PNG!");
            Destroy(screenshotTexture);
            isCapturing = false;
            yield break;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"DualAIController: Image encoded to PNG. Size: {imageBytes.Length} bytes");
        }
        
        // Step 3: Get system prompt with current player stats
        string fullPrompt = GetDualPersonaSystemPrompt() + "\n\nAnalyze the current dungeon state from this screenshot.";
        
        // Step 4: Send screenshot to Claude API with dual persona prompt
        System.Threading.Tasks.Task<string> apiTask = claudeAPIClient.SendScreenshot(imageBytes, fullPrompt);
        
        // Wait for the async task to complete
        while (!apiTask.IsCompleted)
        {
            yield return null; // Wait one frame
        }
        
        // Step 5: Get the result and parse JSON
        string rawResponse = apiTask.Result;
        
        if (showDebugLogs)
        {
            Debug.Log("DualAIController: Raw AI Response:");
            Debug.Log(rawResponse);
        }
        
        // Step 6: Parse JSON response
        ParseAndExecuteResponse(rawResponse);
        
        // Clean up the texture
        Destroy(screenshotTexture);
        
        isCapturing = false;
        
        if (showDebugLogs)
        {
            Debug.Log("DualAIController: Process complete!");
        }
    }
    
    /// <summary>
    /// Parses the JSON response from Claude and executes God's plan / displays Lucifer's question
    /// </summary>
    private void ParseAndExecuteResponse(string rawResponse)
    {
        try
        {
            // Clean the response - remove markdown code blocks if present
            string cleanedResponse = CleanJsonResponse(rawResponse);
            
            if (showDebugLogs)
            {
                Debug.Log($"DualAIController: Cleaned JSON Response:\n{cleanedResponse}");
            }
            
            // Parse JSON using Unity's JsonUtility
            DualAIResponse response = JsonUtility.FromJson<DualAIResponse>(cleanedResponse);
            
            if (response == null)
            {
                Debug.LogError("DualAIController: Failed to parse JSON response - response is null");
                return;
            }
            
            // Execute God's plan if executor exists
            if (godPlanExecutor != null && response.god_plan != null && response.god_plan.Length > 0)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"DualAIController: Executing God's plan with {response.god_plan.Length} actions");
                }
                
                godPlanExecutor.ExecutePlan(response.god_plan);
            }
            else if (godPlanExecutor == null && showDebugLogs)
            {
                Debug.LogWarning("DualAIController: GodPlanExecutor not assigned - cannot execute God's plan");
            }
            
            // Display Lucifer's question if text component exists
            if (luciferQuestionText != null && !string.IsNullOrEmpty(response.lucifer_question))
            {
                luciferQuestionText.text = response.lucifer_question;
                
                if (showDebugLogs)
                {
                    Debug.Log($"DualAIController: Lucifer's question displayed: {response.lucifer_question}");
                }
            }
            else if (luciferQuestionText == null && showDebugLogs)
            {
                Debug.LogWarning("DualAIController: Lucifer question text component not assigned");
            }
            
            // Log the parsed response
            Debug.Log($"=== DUAL PERSONA RESPONSE ===\nGod's Plan: {response.god_plan?.Length ?? 0} actions\nLucifer: {response.lucifer_question}\n============================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DualAIController: Error parsing JSON response: {e.Message}");
            Debug.LogError($"Stack Trace: {e.StackTrace}");
            Debug.LogError($"Raw Response: {rawResponse}");
        }
    }
    
    /// <summary>
    /// Cleans the JSON response by removing markdown code blocks, backticks, and extra whitespace
    /// </summary>
    private string CleanJsonResponse(string rawResponse)
    {
        if (string.IsNullOrEmpty(rawResponse))
        {
            return "{}";
        }
        
        string cleaned = rawResponse.Trim();
        
        // Remove markdown code blocks
        if (cleaned.StartsWith("```json"))
        {
            cleaned = cleaned.Substring(7);
        }
        else if (cleaned.StartsWith("```"))
        {
            cleaned = cleaned.Substring(3);
        }
        
        if (cleaned.EndsWith("```"))
        {
            cleaned = cleaned.Substring(0, cleaned.Length - 3);
        }
        
        cleaned = cleaned.Trim();
        
        // Find the first { and last } to extract pure JSON
        int firstBrace = cleaned.IndexOf('{');
        int lastBrace = cleaned.LastIndexOf('}');
        
        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            cleaned = cleaned.Substring(firstBrace, lastBrace - firstBrace + 1);
        }
        
        return cleaned;
    }
    
    /// <summary>
    /// Public method to trigger dual persona analysis programmatically
    /// </summary>
    public void TriggerDualPersonaAnalysis()
    {
        if (isCapturing)
        {
            if (showDebugLogs)
            {
                Debug.Log("DualAIController: Already capturing, please wait...");
            }
            return;
        }
        
        CaptureAndSendDualPersona();
    }
}

/// <summary>
/// Serializable class for parsing Claude's dual persona JSON response
/// </summary>
[System.Serializable]
public class DualAIResponse
{
    public GodAction[] god_plan;
    public string lucifer_question;
}

/// <summary>
/// Serializable class for individual God actions in the plan
/// </summary>
[System.Serializable]
public class GodAction
{
    public string action;
    public float delay_seconds;
    public string spawn_point;
}

