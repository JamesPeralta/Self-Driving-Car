using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car2DController : MonoBehaviour
{
    public float speedForce;
    public float torqueForce;
    public float driftDrag;
    public float probingDistance;

    private LayerMask raycastMask;
    public NeuralNetwork myNN;
    private float[] input;

    float distanceTravelled;
    Vector3 lastPosition;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    private bool hitWall = false;

    void Awake()
    {
        myNN = new NeuralNetwork(new int[] { 3, 5, 3 });
        input = new float[3];
        raycastMask = LayerMask.GetMask("Wall");
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hitWall == false)
        {
            GetInputFromProximitySensors();

            float output = myNN.FeedForward(input);

            // Collect forces
            Vector3 forces = new Vector3(0, 0, 0);
            if (output == 0)
            {
                forces += (transform.up * speedForce);
            }
            else if (output == 1)
            {
                rb.AddTorque(1 * torqueForce);
            }
            else
            {
                rb.AddTorque(-1 * torqueForce);
            }
            //if (Input.GetKey(KeyCode.W))
            //{
            //    forces += (transform.up * speedForce);
            //}
            //if (Input.GetKey(KeyCode.A))
            //{
            //    rb.AddTorque(1 * torqueForce);
            //}
            //if (Input.GetKey(KeyCode.D))
            //{
            //    rb.AddTorque(-1 * torqueForce);
            //}

            Vector3 velocity = transform.InverseTransformDirection(rb.velocity);
            forces += (transform.right * -velocity.x * driftDrag);
            rb.AddForce(forces);
            lastPosition = transform.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.tag == "Player" && collision.collider.gameObject.tag == "Player")//check if the car passes a gate
        {
            Physics2D.IgnoreCollision(collision.collider, this.boxCollider);
        }

        Debug.Log("Hit Something");
        if (collision.collider.gameObject.tag == "Wall")
        {
            hitWall = true;
        }
    }

    void GetInputFromProximitySensors()
    {
        Vector3[] proximitySensors = new Vector3[] { transform.up, transform.right, -transform.right };

        for (int i = 0; i < proximitySensors.Length; i++) {
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, proximitySensors[i], probingDistance, raycastMask);

            float distance;
            if (hitInfo.collider != null)
            {
                distance = probingDistance - hitInfo.distance;
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            }
            else
            {
                distance = 0;
                Debug.DrawLine(transform.position, transform.position + (proximitySensors[i] * 2), Color.green);
            }

            input[i] = distance;
        }
    }
}
