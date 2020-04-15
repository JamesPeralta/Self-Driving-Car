using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Initialize dashboard
//TODO: Loop that runs the genepool for N generations
//TODO: Updates the dashboard
//TODO: Has playback speed to make the simulation go faster

public class Simulation : MonoBehaviour
{
    private int MUTATION_RATE = 50; // As a %
    private float MUTATION_RADIUS = 0.5f;
    private int N_GENERATIONS = 100;
    public int POPULATION_SIZE;
    private Genepool genePool;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Simulation begun");
        genePool = new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS);
        InvokeRepeating("CheckOnGeneration", 5.0f, 5.0f);
    }

    void CheckOnGeneration()
    {
        // If this whole generation has crashed
        if (genePool.PoolStillAlive() == false)
        {
            genePool.pool.Sort();

            // Sort the population based on fitness and report the best one
            Structure bestGenome = genePool.GetBestGenome();
            Debug.Log(bestGenome.GetFitness());

            // Destory all old cars
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < allPlayers.Length; i++)
            {
                Destroy(allPlayers[i]);
            }

            // Spawn the next generation
            genePool.NextGeneration();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale += 1.0f;
            Debug.Log(Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale -= 1.0f;
            Debug.Log(Time.timeScale);
        }
    }


    public Structure GetBestCar()
    {
        return genePool.GetBestGenome();
    }
}
