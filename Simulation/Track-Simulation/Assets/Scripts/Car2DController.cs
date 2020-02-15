using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car2DController : MonoBehaviour
{
    float speedForce = 10f;
    float torqueForce = 1f;
    float distanceTravelled;
    Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.up * speedForce);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(1 * torqueForce);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(-1 * torqueForce);
        }

        //Calculate and update the distance travelled
        // Not optimal because it calculates more distance when you just spin the car
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        Debug.Log(distanceTravelled);
    }
}
