using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Find the best car and follow it
        EnvironmentManager manager = GameObject.FindObjectOfType<EnvironmentManager>();
        manager.cars.Sort();
        GameObject bestCar = manager.cars[manager.populationSize - 1].gameObject;
        transform.position = new Vector3( bestCar.transform.position.x, bestCar.transform.position.y, -20f);
    }
}
