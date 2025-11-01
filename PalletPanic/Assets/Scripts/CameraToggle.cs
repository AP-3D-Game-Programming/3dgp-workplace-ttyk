using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    // Reference to the camera GameObject that has FollowVehicle on it
    public GameObject firstPersonCameraObject;
    
    // Reference to the third person camera
    public Camera thirdPersonCamera;
    
    // Reference to the FollowVehicle script
    private FollowVehicle followVehicleScript;
    
    // Track which camera is active
    private bool isFirstPerson = true;
    
    void Start()
    {
        // Get the FollowVehicle script from the first person camera object
        followVehicleScript = firstPersonCameraObject.GetComponent<FollowVehicle>();
        
        // Start with first person active
        followVehicleScript.enabled = true;
        thirdPersonCamera.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Check if the C key is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCamera();
        }
    }
    
    void ToggleCamera()
    {
        // Switch between cameras
        isFirstPerson = !isFirstPerson;
        
        // Disable FollowVehicle script (camera stops moving)
        followVehicleScript.enabled = isFirstPerson;
        
        // Turn on/off third person camera
        thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
    }
}
