using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;

    public float maxSteerAngle = 30;
    public float motorForce = 50;
    public LayerMask raycastMask;//Mask for the sensors
    private float[] input = new float[5];//input to the neural network

    public float probingDistance;

    void Awake()
    {
        raycastMask = LayerMask.GetMask("Barrier");
    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
        GetInputFromProximitySensors();
    }

    #region Functions that manage car movement
    private void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
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

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform) 
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }
    #endregion

    #region Functions that manage collisions
    void OnCollisionEnter(Collision collision)
    {
    }

    void GetInputFromProximitySensors()
    {
        Vector3 forwardSensor = transform.forward;
        Vector3 northeastSensor = transform.forward + transform.right;
        Vector3 northwestSensor = transform.forward - transform.right;
        Vector3[] proximitySensors = new Vector3[] { forwardSensor, northeastSensor, northwestSensor};

        Vector3 offset = new Vector3(0, 1, 0);
        float distance;
        for (int i = 0; i < proximitySensors.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, proximitySensors[i], out hit, probingDistance, raycastMask))
            {
                distance = probingDistance - hit.distance;
                Debug.Log(distance);
                Debug.DrawLine(transform.position + offset, transform.position + (proximitySensors[i] * probingDistance), Color.red);
            }
            else
            {
                distance = 0;
                Debug.DrawLine(transform.position + offset, transform.position + (proximitySensors[i] * probingDistance), Color.green);
            }
        }
    }
    #endregion
}
