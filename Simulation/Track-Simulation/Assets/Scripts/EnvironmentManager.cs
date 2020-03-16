using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        Time.timeScale = 1.0f;
        //If the size is not even we can't mutate properly
        if (populationSize % 2 != 0)
        {
            populationSize = 50;
        }

        cars = new List<Car2DController>();
        for (int i = 0; i < populationSize; i++)
        {
            Car2DController car = (Instantiate(prefab, new Vector3(0, 0, -1), this.transform.rotation)).GetComponent<Car2DController>();
            // car.myNN.Load("Assets/Scripts/NN-Weights/Weights31452.txt");
            cars.Add(car);
        }

        InvokeRepeating("RespawnPopulation", 3.0f, 3.0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale += 1.0f;
            Debug.Log("Current playback speed: " + Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale -= 1.0f;
            Debug.Log("Current playback speed: " + Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //saves networks weights and biases to file, to preserve network performance
            cars.Sort();
            cars[populationSize - 1].myNN.Save("Assets/Scripts/NN-Weights/Weights" + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + ".txt");
            Debug.Log("Saving Neural Network Config");
        }
    }

    public void RespawnPopulation()
    {
        bool stillGoing = false;
        int carsStillGoing = 0;
        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].hitWall == false)
            {
                stillGoing = true;
                carsStillGoing += 1;
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
        Debug.Log("Max Fitness: " + cars[cars.Count - 1].fitness);
        for (int i = 0; i < cars.Count / 2; i++)
        {
            cars[i].myNN = cars[i + (populationSize / 2)].myNN.copy(new NeuralNetwork(new int[] { 3, 5, 3 }));
            cars[i].myNN.Mutate((int)(1/MutationChance), MutationStrength);
        }
    }
}
