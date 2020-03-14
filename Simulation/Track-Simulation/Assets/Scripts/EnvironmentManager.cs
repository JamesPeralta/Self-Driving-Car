using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public int populationSize;
    public GameObject prefab; // Holds the Prefab of our Audi
    private List<Car2DController> cars;
    public float MutationChance = 0.01f;
    public float MutationStrength = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //If the size is not even we can't mutate properly
        if (populationSize % 2 != 0)
        {
            populationSize = 50;
        }

        cars = new List<Car2DController>();
        for (int i = 0; i < populationSize; i++)
        {
            Car2DController car = (Instantiate(prefab, new Vector3(0, 0, -1), this.transform.rotation)).GetComponent<Car2DController>();
            cars.Add(car);
        }

        InvokeRepeating("RespawnPopulation", 1.0f, 5.0f);
    }

    public void RespawnPopulation()
    {
        bool stillGoing = false;
        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].hitWall == false)
            {
                stillGoing = true;
                break;
            }
        }

        if (stillGoing == false)
        {
            MutateCars();

            for (int i = 0; i < cars.Count; i++)
            {
                cars[i].ResetCar();
            }
        }
    }

    public void MutateCars()
    {
        cars.Sort();
        Debug.Log(cars[cars.Count - 1].fitness);
        for (int i = 0; i < cars.Count / 2; i++)
        {
            cars[i].myNN = cars[i + (populationSize / 2)].myNN.copy(new NeuralNetwork(new int[] { 3, 5, 3 }));
            cars[i].myNN.Mutate((int)(1/MutationChance), MutationStrength);
        }
    }
}
