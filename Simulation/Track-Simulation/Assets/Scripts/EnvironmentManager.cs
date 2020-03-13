using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public int populationSize;
    public GameObject prefab; // Holds the Prefab of our Audi
    private List<Car2DController> cars;

    // Start is called before the first frame update
    void Start()
    {
        // If the size is not even we can't mutate properly
        if (populationSize % 2 != 0)
        {
            populationSize = 50;
        }

        GeneratePopulation();
    }

    public void GeneratePopulation()
    {
        cars = new List<Car2DController>();
        for (int i = 0; i < populationSize; i++)
        {
            Car2DController car = (Instantiate(prefab, new Vector3(0, 0, -1), new Quaternion(0, 0, 1, 0))).GetComponent<Car2DController>();
            cars.Add(car);
        }
    }
}
