using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public DashboardManager dashboard;

    // Update is called once per frame
    void Update()
    {
        // Find the best car and follow it
        EnvironmentManager manager = GameObject.FindObjectOfType<EnvironmentManager>();
        manager.cars.Sort();

        Car2DController bestCar = manager.cars[manager.populationSize - 1];
        dashboard.UpdateMaxFitness(bestCar.fitness);
        transform.position = new Vector3( bestCar.gameObject.transform.position.x, bestCar.gameObject.transform.position.y, -30f);
    }
}
