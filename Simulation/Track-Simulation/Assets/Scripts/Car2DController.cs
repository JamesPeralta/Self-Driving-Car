using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car2DController : MonoBehaviour
{
    float speedForce = 10f;
    float torqueForce = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
