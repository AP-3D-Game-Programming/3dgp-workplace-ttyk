using UnityEngine;

public class Movement : MonoBehaviour
{
    //forklift movement
    private float speed = 5.0f;
    private float turnSpeed = 50;
    private float horizontalInput;
    private float forwardInput;

    //lift components
    private Transform lift;
    private Rigidbody vehicleRb;

    private float maxLiftHeight = 6;
    private float minLiftHeight = -0.8f;
    private float liftSpeed = 3f;

    private float initialLiftY;
    private float lastLiftY;

    private Vector3 currentMovementVelocity;
    private Vector3 lastPosition;
    private Rigidbody attachedPallet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lift = transform.Find("Lift");
        vehicleRb = GetComponent<Rigidbody>();
        initialLiftY = lift.localPosition.y;
        lastLiftY = lift.localPosition.y;

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

        // Calculate movement velocity
        currentMovementVelocity = (newPosition - lastPosition) / Time.fixedDeltaTime;
        lastPosition = newPosition;

        vehicleRb.MovePosition(newPosition);

        //rotate with Rigidbody
        float rotation = turnSpeed * horizontalInput * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        vehicleRb.MoveRotation(vehicleRb.rotation * turnRotation);

        //move lift up or down
        Vector3 localPos = lift.localPosition;
        float liftDelta = 0f;

        if (Input.GetKey(KeyCode.E) && localPos.y < initialLiftY + maxLiftHeight)
        {
            lift.localPosition += Vector3.up * liftSpeed * Time.fixedDeltaTime;
            liftDelta = liftSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.Q) && localPos.y > initialLiftY + minLiftHeight)
        {
            lift.localPosition += Vector3.down * liftSpeed * Time.fixedDeltaTime;
            liftDelta = -liftSpeed * Time.fixedDeltaTime;
        }

        //add force to attached pallet
        if (attachedPallet != null)
        {
            Vector3 offset = attachedPallet.position - vehicleRb.position;
            Vector3 rotatedOffset = turnRotation * offset;
            Vector3 newPalletPosition = newPosition + rotatedOffset + movement * Time.fixedDeltaTime;

            //update lift movement to pallet position
            if (liftDelta != 0f)
            {
                newPalletPosition += transform.up * liftDelta;
            }

            attachedPallet.MovePosition(newPalletPosition);
            attachedPallet.MoveRotation(attachedPallet.rotation * turnRotation);
        }
        lastLiftY = lift.localPosition.y;
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
