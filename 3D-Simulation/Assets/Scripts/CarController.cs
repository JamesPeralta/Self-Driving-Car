/* This is the main class that allows a car to interact with the environment
 * This class contains the logic that allows a car to drive, detect collisions
 * with walls, and the logic for the proximity sensors.
 *
 * This class also contains the logic that feeds the input of the environment
 * into the neural network, retrieves these results, and then applys the action
 * that the neural network has recommended. */

using UnityEngine;

public class CarController : MonoBehaviour
{
    // Input used to apply force to a car
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    // Controls the movement of the car
    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    public float maxSteerAngle;
    public float motorForce;

    public LayerMask raycastMask; // Specifies which layers a raycast can hit
    private float[] input = new float[4]; // Input to the neural network
    public float probingDistance; // Distance proximity sensor can probe


    public Rigidbody rb;
    public BoxCollider boxCollider;
    private NeuralNetwork myNN;

    private bool carStarted = false;
    private int fitness;
    public bool hitWall = false;
    private int lastFitness;

    #region Getters/Setters
    public int GetFitness()
    {
        return fitness;
    }

    public void SetNeuralNetwork(NeuralNetwork nn)
    {
        myNN = nn;
        StartCar();
    }
    #endregion

    #region Game Loop functions
    void Awake()
    {
        raycastMask = LayerMask.GetMask("Barrier");
        fitness = 0;
        lastFitness = fitness;
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        IgnoreContactWithOtherCars();
    }

    // On each frame, get the input of the environment and steer the
    // car according to the recommendations placed by the neural network
    private void FixedUpdate()
    {
        if (myNN != null && carStarted && hitWall == false)
        {
            GetInput();
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }
        else
        {
            // If a car has not been started or has hit a wall
            // make sure it is stationary
            rb.velocity = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region Functions that manage car movement
    // Get input from the proximity sensors, run it throught the
    // neural network, and set the vertical and horizontal outputs
    private void GetInput()
    {
        GetInputFromProximitySensors();

        m_horizontalInput = 0;
        m_verticalInput = 0;

        float output = myNN.FeedForward(input);

        if (output == 0)
        {
            m_verticalInput = 1;
        }
        else if (output == 1)
        {
            m_horizontalInput = -1;
        }
        else if (output == 2)
        {
            m_horizontalInput = 1;
        }
        else
        {
            m_verticalInput = -1;
        }
    }

    // Turn the wheels of the car left or right
    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontDriverW.steerAngle = m_steeringAngle;
        frontPassengerW.steerAngle = m_steeringAngle;
    }

    // Apply an acceleration force or brake force
    private void Accelerate()
    {
        frontDriverW.motorTorque = m_verticalInput * motorForce;
        frontPassengerW.motorTorque = m_verticalInput * motorForce;
    }

    // Update the angle of the wheel to give wheel spinning effects
    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    // Update wheel poses helper function
    private void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;

        collider.GetWorldPose(out pos, out quat);

        transform.position = pos;
        transform.rotation = quat;
    }
    #endregion

    #region Functions that manage collisions
    void OnCollisionEnter(Collision collision)
    {
        // If this car crashes into a barrier, set the hitWall flag o
        if (collision.collider.gameObject.tag == "Barrier")
        {
            hitWall = true;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        // Increment the fitness, only if it passes the next checkpoint it hasn't reached
        // yet in incremental fashion. 
        int nextCheckpoint = (fitness % 118 ) + 1;
        if (collision.gameObject.name == ("CheckPoint (" + nextCheckpoint + ")") && hitWall == false)
        {
            fitness += 1;
        }

        // Stop at the last checkpoint
        if (collision.gameObject.name == "CheckPoint (118)")
        {
            hitWall = true;
        }
    }

    // Get's the proximity sensors input using raycasting
    void GetInputFromProximitySensors()
    {
        Vector3 forwardSensor = transform.forward;
        Vector3 northeastSensor = transform.forward + transform.right;
        Vector3 northwestSensor = transform.forward - transform.right;
        Vector3[] proximitySensors = new Vector3[] { forwardSensor, northeastSensor, northwestSensor };

        Vector3 offset = new Vector3(0, 1, 0);
        float distance;
        for (int i = 0; i < proximitySensors.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, proximitySensors[i], out hit, probingDistance, raycastMask))
            {
                distance = probingDistance - hit.distance;
                Debug.DrawLine(transform.position + offset, transform.position + (proximitySensors[i] * probingDistance), Color.red);
            }
            else
            {
                distance = 0;
                Debug.DrawLine(transform.position + offset, transform.position + (proximitySensors[i] * probingDistance), Color.green);
            }

            input[i] = distance;
        }

        // Speed in meters per second
        input[3] = rb.velocity.magnitude;
    }

    // This function allows all cars to navigate the map without crashing into each other
    void IgnoreContactWithOtherCars()
    {
        Object[] allWheels = GameObject.FindObjectsOfType(typeof(WheelCollider));
        Object[] allBoxColliders = GameObject.FindObjectsOfType(typeof(BoxCollider));

        // Make wheels ignore all other wheels and box colliders in the system
        for (int i = 0; i < allWheels.Length; i++)
        {
            // Ignore all other wheels
            for (int j = 0; j < allWheels.Length; j++)
            {
                Physics.IgnoreCollision(allWheels[i] as WheelCollider, allWheels[j] as WheelCollider);
            }

            // Ignore all other box colliders
            for (int j = 0; j < allBoxColliders.Length; j++)
            {
                Physics.IgnoreCollision(allWheels[i] as WheelCollider, allBoxColliders[j] as BoxCollider);
            }
        }

        // Make all box colliders ignore all other wheels or box colliders in the system
        for (int i = 0; i < allBoxColliders.Length; i++)
        {
            // Ignore all other wheels
            for (int j = 0; j < allWheels.Length; j++)
            {
                Physics.IgnoreCollision(allBoxColliders[i] as BoxCollider, allWheels[j] as WheelCollider);
            }

            // Ignore all other box colliders
            for (int j = 0; j < allBoxColliders.Length; j++)
            {
                BoxCollider myBox = allBoxColliders[i] as BoxCollider;
                BoxCollider otherBox = allBoxColliders[j] as BoxCollider;
                if (myBox.gameObject.tag == "CheckPoint" || otherBox.gameObject.tag == "CheckPoint")
                {
                    continue;
                }

                Physics.IgnoreCollision(allBoxColliders[i] as BoxCollider, allBoxColliders[j] as BoxCollider);
            }
        }
    }
    #endregion

    #region Manage car state
    public void StartCar()
    {
        carStarted = true;
        // 10 seconds to progress
        InvokeRepeating("CheckProgression", 10.0f, 10.0f);
    }

    public void StopCar()
    {
        carStarted = false;
    }

    public void CheckProgression()
    {
        // Kill objects that are not progressing
        if (lastFitness >= fitness)
        {
            hitWall = true;
        }
        else
        {
            // Set lastFitness to current Fitness
            lastFitness = fitness;
        }
    }
    #endregion
}
