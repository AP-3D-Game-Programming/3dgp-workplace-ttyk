using UnityEngine;

public class Movement : MonoBehaviour
{
    //beweeg vorklift
    private float speed = 5.0f;
    private float turnSpeed = 50;
    private float horizontalInput;
    private float forwardInput;

    //beweeg lift
    private Transform lift;
    private Rigidbody vehicleRb;

    private float maxLiftHeight = 6;
    private float minLiftHeight = -0.8f;
    private float liftSpeed = 3;

    private float initialLiftY;

    private Vector3 currentMovementForce;
    private Vector3 lastPosition;
    private Rigidbody attachedPallet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lift = transform.Find("Lift");
        vehicleRb = GetComponent<Rigidbody>();
        initialLiftY = lift.localPosition.y;

        lastPosition = vehicleRb.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        //move the vehicle forward
        Vector3 movement = transform.forward * speed * forwardInput;
        Vector3 newPosition = vehicleRb.position + movement * Time.fixedDeltaTime;

        // Bereken de actual movement force (verschil in positie)
        currentMovementForce = (newPosition - lastPosition) / Time.fixedDeltaTime;
        lastPosition = newPosition;

        vehicleRb.MovePosition(newPosition);

        //rotate met Rigidbody
        float rotation = turnSpeed * horizontalInput * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        vehicleRb.MoveRotation(vehicleRb.rotation * turnRotation);

        //add force to attached pallet
        if (attachedPallet != null)
        {
            attachedPallet.MovePosition(attachedPallet.position + movement * Time.fixedDeltaTime);
        }

        //move lift up or down
        Vector3 localPos = lift.localPosition;

        if (Input.GetKey(KeyCode.Q) && localPos.y < initialLiftY + maxLiftHeight)
        {
            lift.localPosition += Vector3.up * liftSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E) && localPos.y > initialLiftY + minLiftHeight)
        {
            lift.localPosition += Vector3.down * liftSpeed * Time.deltaTime;
        }
    }

    public void AttachPallet(Rigidbody pallet)
    {
        attachedPallet = pallet;
    }
    public void DetachPallet()
    {
        attachedPallet = null;
    }
}
