using UnityEngine;

public class ThirdPersonCameraControl : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float maxVerticalAngle = 60f;
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    
    void Update()
    {
        // Krijg muis input (exact als FollowVehicle)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Up/down kijken (X rotatie)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxVerticalAngle, maxVerticalAngle);
        
        // Left/right kijken (Y rotatie)
        yRotation += mouseX;
        
        // Apply rotation schoon
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
