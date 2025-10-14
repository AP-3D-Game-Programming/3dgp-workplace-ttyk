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
    private float maxLiftHeight = 9;
    private float minLiftHeight = 2.2f;
    private float liftSpeed = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lift = transform.Find("Lift");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        //move the vehicle forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

        //move lift up or down
        if (Input.GetKey(KeyCode.Q) == true && lift.transform.position.y < maxLiftHeight)
        {
            lift.transform.Translate(Vector3.up * liftSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E) == true && lift.transform.position.y > minLiftHeight)
        {
            lift.transform.Translate(Vector3.down * liftSpeed * Time.deltaTime);
        }

    }
}
