using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Object components
    public Rigidbody rb;
    public BoxCollider boxCollider;
    private NeuralNetwork myNN;

    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;

    public float maxSteerAngle;
    public float motorForce;
    public LayerMask raycastMask; //Mask for the sensors
    private float[] input = new float[3]; //input to the neural network

    public float probingDistance;

    private bool carStarted = false;

    void Awake()
    {
        raycastMask = LayerMask.GetMask("Barrier");
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        IgnoreContactWithOtherCars();
    }

    private void FixedUpdate()
    {
        if (myNN != null && carStarted)
        {
            GetInput();
            Steer();
            Accelerate();
            UpdateWheelPoses();
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 20);
        }
    }

    #region Functions that manage car movement
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

    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontDriverW.steerAngle = m_steeringAngle;
        frontPassengerW.steerAngle = m_steeringAngle;
    }

    private void Accelerate()
    {
        frontDriverW.motorTorque = m_verticalInput * motorForce;
        frontPassengerW.motorTorque = m_verticalInput * motorForce;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

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
        if (collision.collider.gameObject.tag != "Terrain" && collision.collider.gameObject.tag != "Barrier")
        {
            Physics.IgnoreCollision(collision.collider, this.boxCollider);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        int fitness = -1;
        int nextCheckpoint = fitness % 35;
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == ("CheckPoint (" + nextCheckpoint + ")"))
        {
            fitness += 1;
        }
    }

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
    }

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
                Physics.IgnoreCollision(allBoxColliders[i] as BoxCollider, allBoxColliders[j] as BoxCollider);
            }
        }
    }
    #endregion

    #region Manage car state
    public void StartCar()
    {
        carStarted = true;
    }

    public void StopCar()
    {
        carStarted = false;
    }

    public void SetNeuralNetwork(NeuralNetwork nn)
    {
        myNN = nn;
        StartCar();
    }
    #endregion
}
