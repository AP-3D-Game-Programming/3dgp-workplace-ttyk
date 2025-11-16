using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject vehicle;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float clampAngle = 80f;

    [Header("First Person Settings")]
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0, 1.625f, -0.75f);

    [Header("Third Person Settings")]
    [SerializeField] private float thirdPersonDistance = 3.75f;
    [SerializeField] private float thirdPersonHeight = 0.75f;
    [SerializeField] private float orbitSmoothSpeed = 10f;
    [SerializeField] private float minPitchAngle = -8f;
    [SerializeField] private float maxPitchAngle = 80f;
    [SerializeField] private float minZoomDistance = 2.5f;
    [SerializeField] private float maxZoomDistance = 5f;
    [SerializeField] private float zoomSpeed = 2.5f;

    [Header("Controls")]
    [SerializeField] private KeyCode togglePerspectiveKey = KeyCode.C;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isFirstPerson = true;
    private float thirdPersonYaw = 0f;
    private float thirdPersonPitch = 15f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();

        if (mainCamera == null)
        {
            Debug.LogError("CameraManager: Camera component not found!");
            return;
        }

        if (vehicle == null)
        {
            Debug.LogError("CameraManager: Forklift not found!");
            return;
        }

        yRotation = vehicle.transform.eulerAngles.y;

        // Cursor locked voor gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("CameraManager: Ready (following " + vehicle.name + ")");
    }

    private void Update()
    {
        if (vehicle == null) return;

        HandleInput();
        UpdateCamera();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(togglePerspectiveKey))
        {
            TogglePerspective();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.Confined
                : CursorLockMode.Locked;

            Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
        }
    }

    private void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (isFirstPerson)
        {
            UpdateFirstPersonCamera(mouseX, mouseY);
        }
        else
        {
            UpdateThirdPersonCamera(mouseX, mouseY);
        }
    }

    private void UpdateFirstPersonCamera(float mouseX, float mouseY)
    {
        transform.position = vehicle.transform.position +
            vehicle.transform.TransformDirection(firstPersonOffset);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        yRotation += mouseX;

        float finalYRotation = vehicle.transform.eulerAngles.y + yRotation;
        transform.rotation = Quaternion.Euler(xRotation, finalYRotation, 0f);
    }

    private void UpdateThirdPersonCamera(float mouseX, float mouseY)
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        thirdPersonDistance -= scroll * zoomSpeed;
        thirdPersonDistance = Mathf.Clamp(thirdPersonDistance, minZoomDistance, maxZoomDistance);

        thirdPersonYaw += mouseX;
        thirdPersonPitch -= mouseY;
        thirdPersonPitch = Mathf.Clamp(thirdPersonPitch, minPitchAngle, maxPitchAngle);

        Quaternion rotation = Quaternion.Euler(thirdPersonPitch, thirdPersonYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, thirdPersonHeight, -thirdPersonDistance);

        Vector3 targetPosition = vehicle.transform.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            orbitSmoothSpeed * Time.deltaTime
        );

        transform.LookAt(vehicle.transform.position + Vector3.up * thirdPersonHeight);
    }

    private void TogglePerspective()
    {
        isFirstPerson = !isFirstPerson;

        if (!isFirstPerson)
        {
            thirdPersonYaw = 0f;
            thirdPersonPitch = 15f;
        }

        Debug.Log($"Switched to {(isFirstPerson ? "First" : "Third")} Person View");
    }

    public bool IsFirstPersonMode => isFirstPerson;
}