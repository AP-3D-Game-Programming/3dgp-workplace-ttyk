using UnityEngine;

public class Movement : MonoBehaviour
{
    //input
    private float horizontalInput;
    private float forwardInput;
    private const float INPUT_THRESHOLD = 0.1f;

    //movement
    [SerializeField] private float speed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 10f;
    private float currentSpeed = 0f;

    //steering
    [SerializeField] private float turnSpeed = 50;
    [SerializeField] private float steeringAdaptSpeed = 3f;
    private float targetSteeringDirection = 1f;
    private float currentSteeringDirection = 1f;

    //lift
    [SerializeField] private Transform lift;
    [SerializeField] private float liftSpeed = 3f;
    private float initialLiftY;
    private float maxLiftHeight = 6;
    private float minLiftHeight = -0.8f;

    //references
    private Rigidbody vehicleRb;
    private Rigidbody attachedPallet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vehicleRb = GetComponent<Rigidbody>();
        initialLiftY = lift.localPosition.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        //move the vehicle forward
        float targetSpeed = speed * forwardInput;
        if (Mathf.Abs(forwardInput) > INPUT_THRESHOLD)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        } 
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        Vector3 movement = transform.forward * currentSpeed;
        Vector3 newPosition = vehicleRb.position + movement * Time.fixedDeltaTime;
        vehicleRb.MovePosition(newPosition);

        //rotate the vehicle
        if (forwardInput > INPUT_THRESHOLD)
        {
            targetSteeringDirection = 1f; //normal
        }
        else if (forwardInput < -INPUT_THRESHOLD)
        {
            targetSteeringDirection = -1f; //inverted
        }
        currentSteeringDirection = Mathf.Lerp(currentSteeringDirection, targetSteeringDirection, steeringAdaptSpeed * Time.fixedDeltaTime);

        float rotation = turnSpeed * horizontalInput * currentSteeringDirection * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        vehicleRb.MoveRotation(vehicleRb.rotation * turnRotation);

        //move lift vertically
        Vector3 localPos = lift.localPosition;
        float liftDelta = 0f;

        if (Input.GetKey(KeyCode.E) && localPos.y < initialLiftY + maxLiftHeight)
        {
            liftDelta = liftSpeed * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.Q) && localPos.y > initialLiftY + minLiftHeight)
        {
            liftDelta = -liftSpeed * Time.fixedDeltaTime;
        }

        if (liftDelta != 0f)
        {
            lift.localPosition += Vector3.up * liftDelta;
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
