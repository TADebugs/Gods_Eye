using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Client for communicating with Claude API (Anthropic) to send screenshots and receive AI responses.
/// Uses UnityWebRequest for HTTP communication in Unity.
/// </summary>
public class ClaudeAPIClient : MonoBehaviour
{
    // API endpoint for Claude messages
    private const string API_ENDPOINT = "https://api.anthropic.com/v1/messages";
    
    // Claude model to use
    private const string MODEL_NAME = "claude-sonnet-4-20250514";
    
    // API version header value
    private const string ANTHROPIC_VERSION = "2023-06-01";
    
    // API key - TODO: Set this from environment variable or secure storage
    [Header("API Configuration")]
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE";
    
    /// <summary>
    /// Sends a screenshot image to Claude API along with a user prompt and returns the AI's text response.
    /// </summary>
    /// <param name="imageBytes">The screenshot image as byte array</param>
    /// <param name="userPrompt">The text prompt/question to send to Claude</param>
    /// <returns>The text response from Claude, or error message if request fails</returns>
    public async System.Threading.Tasks.Task<string> SendScreenshot(byte[] imageBytes, string userPrompt)
    {
        try
        {
            // Convert image bytes to base64 string for JSON encoding
            string base64Image = Convert.ToBase64String(imageBytes);
            
            // Determine image media type based on file signature (simplified - assumes PNG)
            // In production, you might want to detect actual image type
            string mediaType = "image/png";
            
            // Build the JSON request body according to Claude API format
            // Structure: { model, max_tokens, messages: [{ role, content: [{ type, source: { type, media_type, data } }] }] }
            string jsonBody = BuildRequestJson(base64Image, mediaType, userPrompt);
            
            // Create UnityWebRequest for POST to Claude API
            UnityWebRequest request = new UnityWebRequest(API_ENDPOINT, "POST");
            
            // Convert JSON string to byte array for request body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            
            // Set request upload handler with JSON body
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            
            // Set request download handler to receive response
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // Set required headers for Claude API
            request.SetRequestHeader("x-api-key", apiKey);
            request.SetRequestHeader("anthropic-version", ANTHROPIC_VERSION);
            request.SetRequestHeader("content-type", "application/json");
            
            // Send the request asynchronously
            var operation = request.SendWebRequest();
            
            // Wait for the request to complete
            while (!operation.isDone)
            {
                await System.Threading.Tasks.Task.Yield();
            }
            
            // Check for errors in the response
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Claude API Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Body: {request.downloadHandler.text}");
                return $"Error: {request.error}";
            }
            
            // Parse the JSON response to extract Claude's text response
            string responseText = request.downloadHandler.text;
            string claudeResponse = ParseClaudeResponse(responseText);
            
            // Clean up the request
            request.Dispose();
            
            return claudeResponse;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in SendScreenshot: {e.Message}");
            Debug.LogError($"Stack Trace: {e.StackTrace}");
            return $"Exception: {e.Message}";
        }
    }
    
    /// <summary>
    /// Builds the JSON request body for Claude API with image and text prompt.
    /// </summary>
    /// <param name="base64Image">Base64 encoded image string</param>
    /// <param name="mediaType">MIME type of the image (e.g., "image/png")</param>
    /// <param name="userPrompt">The user's text prompt/question</param>
    /// <returns>JSON string formatted for Claude API</returns>
    private string BuildRequestJson(string base64Image, string mediaType, string userPrompt)
    {
        // Claude API request structure:
        // {
        //   "model": "claude-sonnet-4-20250514",
        //   "max_tokens": 1024,
        //   "messages": [
        //     {
        //       "role": "user",
        //       "content": [
        //         {
        //           "type": "image",
        //           "source": {
        //             "type": "base64",
        //             "media_type": "image/png",
        //             "data": "base64_string_here"
        //           }
        //         },
        //         {
        //           "type": "text",
        //           "text": "user prompt here"
        //         }
        //       ]
        //     }
        //   ]
        // }
        
        // Build JSON manually (could use JsonUtility or Newtonsoft.Json, but manual is more explicit)
        StringBuilder json = new StringBuilder();
        json.Append("{");
        json.Append($"\"model\": \"{MODEL_NAME}\",");
        json.Append("\"max_tokens\": 4096,"); // Increased for longer responses
        json.Append("\"messages\": [{");
        json.Append("\"role\": \"user\",");
        json.Append("\"content\": [");
        
        // Add image content block
        json.Append("{");
        json.Append("\"type\": \"image\",");
        json.Append("\"source\": {");
        json.Append("\"type\": \"base64\",");
        json.Append($"\"media_type\": \"{mediaType}\",");
        json.Append($"\"data\": \"{base64Image}\"");
        json.Append("}");
        json.Append("},");
        
        // Add text content block
        json.Append("{");
        json.Append("\"type\": \"text\",");
        json.Append($"\"text\": \"{EscapeJsonString(userPrompt)}\"");
        json.Append("}");
        
        json.Append("]");
        json.Append("}]");
        json.Append("}");
        
        return json.ToString();
    }
    
    /// <summary>
    /// Escapes special characters in a string for JSON encoding.
    /// </summary>
    private string EscapeJsonString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        
        return input
            .Replace("\\", "\\\\")  // Backslash
            .Replace("\"", "\\\"")   // Double quote
            .Replace("\n", "\\n")    // Newline
            .Replace("\r", "\\r")    // Carriage return
            .Replace("\t", "\\t");    // Tab
    }
    
    /// <summary>
    /// Parses the JSON response from Claude API to extract the text content.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON response string from Claude API</param>
    /// <returns>The extracted text content from Claude's response</returns>
    private string ParseClaudeResponse(string jsonResponse)
    {
        try
        {
            // Log raw API response for debugging
            Debug.Log($"ClaudeAPIClient: Raw API Response:\n{jsonResponse}");
            
            // Claude API response structure:
            // {
            //   "id": "...",
            //   "type": "message",
            //   "role": "assistant",
            //   "content": [
            //     {
            //       "type": "text",
            //       "text": "Claude's response text here"
            //     }
            //   ],
            //   ...
            // }
            
            // Parse JSON using Unity's JsonUtility
            ClaudeAPIResponse response = JsonUtility.FromJson<ClaudeAPIResponse>(jsonResponse);
            
            if (response == null)
            {
                Debug.LogError("ClaudeAPIClient: Failed to parse JSON response - response is null");
                return jsonResponse; // Return raw response if parsing fails
            }
            
            // Check if content array exists and has at least one element
            if (response.content == null || response.content.Length == 0)
            {
                Debug.LogError("ClaudeAPIClient: Content array is null or empty");
                Debug.LogError($"ClaudeAPIClient: Raw response was: {jsonResponse}");
                return jsonResponse; // Return raw response if content is missing
            }
            
            // Extract text from first content block
            ClaudeContentBlock firstContent = response.content[0];
            
            if (firstContent == null)
            {
                Debug.LogError("ClaudeAPIClient: First content block is null");
                return jsonResponse;
            }
            
            if (string.IsNullOrEmpty(firstContent.text))
            {
                Debug.LogWarning("ClaudeAPIClient: Text field is null or empty in content block");
                Debug.LogWarning($"ClaudeAPIClient: Content type: {firstContent.type}");
                return jsonResponse; // Return raw response if text is missing
            }
            
            string extractedText = firstContent.text;
            
            // Log extracted text for debugging
            Debug.Log($"ClaudeAPIClient: Extracted Text:\n{extractedText}");
            
            return extractedText;
        }
        catch (Exception e)
        {
            Debug.LogError($"ClaudeAPIClient: Error parsing Claude response: {e.Message}");
            Debug.LogError($"ClaudeAPIClient: Stack Trace: {e.StackTrace}");
            Debug.LogError($"ClaudeAPIClient: Raw response was: {jsonResponse}");
            return jsonResponse; // Return raw response if parsing fails
        }
    }
}

/// <summary>
/// Serializable class for Claude API response structure
/// </summary>
[System.Serializable]
public class ClaudeAPIResponse
{
    public string id;
    public string type;
    public string role;
    public ClaudeContentBlock[] content;
    // Other fields may exist but we only need content
}

/// <summary>
/// Serializable class for content blocks in Claude API response
/// </summary>
[System.Serializable]
public class ClaudeContentBlock
{
    public string type;
    public string text;
}

