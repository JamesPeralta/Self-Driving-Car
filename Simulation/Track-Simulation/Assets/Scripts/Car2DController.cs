using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car2DController : MonoBehaviour, IComparable<Car2DController>
{
    public float speedForce;
    public float torqueForce;
    public float driftDrag;
    public float probingDistance;

    private LayerMask raycastMask;
    public NeuralNetwork myNN;
    private float[] input;
    public float fitness;

    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;

    public bool hitWall = false;
    private float lastFitness;
    private int nextCheckpoint;

    void Awake()
    {
        myNN = new NeuralNetwork(new int[] { 3, 5, 3 });
        input = new float[3];
        raycastMask = LayerMask.GetMask("Wall");
        fitness = 0;
        lastFitness = fitness;
        nextCheckpoint = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        // 3 seconds to progress
        InvokeRepeating("CheckProgression", 5.0f,  5.0f);
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

            Vector3 velocity = transform.InverseTransformDirection(rb.velocity);
            forces += (transform.right * -velocity.x * driftDrag);
            rb.AddForce(forces);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.tag == "Player" && collision.collider.gameObject.tag == "Player")//check if the car passes a gate
        {
            Physics2D.IgnoreCollision(collision.collider, this.boxCollider);
        }
        if (collision.collider.gameObject.tag == "Wall")
        {
            hitWall = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == ("CheckPoint (" + nextCheckpoint + ")"))
        {
            fitness += 1;
            nextCheckpoint += 1;
        }
    }

    void GetInputFromProximitySensors()
    {
        Vector3[] proximitySensors = new Vector3[] { transform.up, transform.right, -transform.right };

        for (int i = 0; i < proximitySensors.Length; i++)
        {
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

    public int CompareTo(Car2DController other)
    {
        if (other == null)
            return 1;
        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }

    public void ResetCar()
    {
        CancelInvoke("CheckProgression");
        // 3 seconds to progress
        InvokeRepeating("CheckProgression", 5.0f, 5.0f);

        fitness = 0;
        lastFitness = 0;
        nextCheckpoint = 0;
        hitWall = false;
        transform.position = new Vector3(0, 0, -1);
        transform.rotation = this.transform.rotation;
    }

    public void CheckProgression()
    {
        // Kill objects that are not progressing
        if (lastFitness <= fitness)
        {
            hitWall = true;
        }
        else
        {
            // Set lastFitness to current Fitness
            lastFitness = fitness;
        }
    }
}
