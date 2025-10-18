using UnityEngine;

public class FollowVehicle : MonoBehaviour
{
    public GameObject vehicle;

    public float mouseSensitivity = 100f;
    public float clampAngle = 80f;
    private Vector3 offset = new Vector3(0, 6.5f, -3f);

    private float xRotation = 0f;
    private float yRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        yRotation = vehicle.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = vehicle.transform.position + vehicle.transform.TransformDirection(offset);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);

        yRotation += mouseX;

        float finalYRotation = vehicle.transform.eulerAngles.y + yRotation;

        transform.rotation = Quaternion.Euler(xRotation, finalYRotation, 0f);
    }
}
