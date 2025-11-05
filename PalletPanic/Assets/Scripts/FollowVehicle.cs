using UnityEngine;

public class FollowVehicle : MonoBehaviour
{
    public GameObject vehicle;

    public float mouseSensitivity = 100f;
    public float clampAngle = 80f;

    // First person settings
    private Vector3 firstPersonOffset = new Vector3(0, 6.5f, -3f);

    // Third person settings
    private float thirdPersonDistance = 15f;
    private float thirdPersonHeight = 3f;
    private float orbitSmoothSpeed = 10f;
    private float minPitchAngle = -8f;
    private float maxPitchAngle = 80f;
    private float minZoomDistance = 10f;
    private float maxZoomDistance = 20f;
    private float zoomSpeed = 10f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool isFirstPerson = true;
    private float thirdPersonYaw = 0f;
    private float thirdPersonPitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yRotation = vehicle.transform.eulerAngles.y;
    }

    void Update()
    {
        // Toggle perspective
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;

            if (!isFirstPerson)
            {
                thirdPersonYaw = 0f;
                thirdPersonPitch = 15f;
            }
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (isFirstPerson)
        {
            UpdateFirstPerson(mouseX, mouseY);
        }
        else
        {
            UpdateThirdPerson(mouseX, mouseY);
        }
    }

    void UpdateFirstPerson(float mouseX, float mouseY)
    {
        transform.position = vehicle.transform.position + vehicle.transform.TransformDirection(firstPersonOffset);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        yRotation += mouseX;

        float finalYRotation = vehicle.transform.eulerAngles.y + yRotation;
        transform.rotation = Quaternion.Euler(xRotation, finalYRotation, 0f);
    }

    void UpdateThirdPerson(float mouseX, float mouseY)
    {
        //update zoom level
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        thirdPersonDistance -= scroll * zoomSpeed;
        thirdPersonDistance = Mathf.Clamp(thirdPersonDistance, minZoomDistance, maxZoomDistance);

        // Update orbit angles
        thirdPersonYaw += mouseX;
        thirdPersonPitch -= mouseY;
        thirdPersonPitch = Mathf.Clamp(thirdPersonPitch, minPitchAngle, maxPitchAngle);

        // Calculate orbit position around the vehicle
        Quaternion rotation = Quaternion.Euler(thirdPersonPitch, thirdPersonYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, thirdPersonHeight, -thirdPersonDistance);

        Vector3 targetPosition = vehicle.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, orbitSmoothSpeed * Time.deltaTime);
        transform.LookAt(vehicle.transform.position + Vector3.up * thirdPersonHeight);
    }
}