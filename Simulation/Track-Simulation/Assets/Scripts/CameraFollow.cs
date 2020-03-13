using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GameObject[] allCars = GameObject.FindGameObjectsWithTag("Player");
        GameObject randCar = allCars[0];

        // find a car and follow it
        transform.position = new Vector3( randCar.transform.position.x, randCar.transform.position.y, -20f);
    }
}
