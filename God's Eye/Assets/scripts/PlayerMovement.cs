using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement speed - you can tweak this in Unity Inspector later
    public float moveSpeed = 5f;
    
    // Mouse sensitivity for looking around
    public float lookSensitivity = 2f;
    
    // Reference to the camera (we'll assign this automatically)
    private Transform cameraTransform;
    private float xRotation = 0f; // Tracks up/down camera rotation
    
    // Freeze state - when true, player cannot move or look around
    private bool isFrozen = false;
    
    void Start()
    {
        // Find the camera that's a child of this player
        cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        
        // Lock and hide the cursor (so mouse moves camera, not cursor on screen)
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // If frozen, don't process any input
        if (isFrozen)
        {
            return;
        }
        
        // MOVEMENT - WASD keys
        // Input.GetAxis returns -1 to 1 based on key presses
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows
        
        // Calculate movement direction relative to where player is facing
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        // Actually move the player (Time.deltaTime makes it frame-rate independent)
        transform.position += move * moveSpeed * Time.deltaTime;
        
        
        // MOUSE LOOK - Move mouse to look around
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        
        // Rotate player left/right (Y axis rotation)
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera up/down (X axis rotation, clamped so you can't flip upside down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit to -90 to +90 degrees
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    /// <summary>
    /// Freezes the player - disables movement and camera controls.
    /// </summary>
    public void Freeze()
    {
        isFrozen = true;
        Debug.Log("PlayerMovement: Player FROZEN - controls disabled");
    }
    
    /// <summary>
    /// Unfreezes the player - re-enables movement and camera controls.
    /// </summary>
    public void Unfreeze()
    {
        isFrozen = false;
        Debug.Log("PlayerMovement: Player UNFROZEN - controls enabled");
    }
    
    /// <summary>
    /// Checks if the player is currently frozen.
    /// </summary>
    public bool IsFrozen()
    {
        return isFrozen;
    }
}