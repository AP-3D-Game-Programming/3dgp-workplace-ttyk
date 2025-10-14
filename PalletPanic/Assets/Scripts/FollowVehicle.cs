using UnityEngine;

public class FollowVehicle : MonoBehaviour
{
    public GameObject vehicle;
    private Vector3 offset = new Vector3(0, 15, -10);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = vehicle.transform.position + offset;
        transform.LookAt(vehicle.transform);
    }
}
