using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Initialize dashboard
//TODO: Updates the dashboard

public class Simulation : MonoBehaviour
{
    private int MUTATION_RATE = 50; // As a %
    private float MUTATION_RADIUS = 0.5f;
    public int POPULATION_SIZE;
    private string WEIGHTS_FILE = null;
    private Genepool genePool;
    public DashboardManager dashboard;
    int generationNumber;

    // Start is called before the first frame update
    void Start()
    {
        if (WEIGHTS_FILE != null)
        {
            genePool = new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS, WEIGHTS_FILE);
        }
        else
        {
            genePool = new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS);
        }

        // If user has choosen to load weights
        InvokeRepeating("CheckOnGeneration", 5.0f, 5.0f);

        // Initialize the dashboard
        generationNumber = 1;
        dashboard.InitializeDashboard(this.GetGenerationData());
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
            dashboard.UpdateMaxFitness(bestGenome.GetFitness());

            // Destory all old cars
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < allPlayers.Length; i++)
            {
                Destroy(allPlayers[i]);
            }

            generationNumber++;
            dashboard.UpdateGeneration(generationNumber);
            genePool.NextGeneration();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale += 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale -= 1.0f;
        }

        dashboard.UpdatePlaybackSpeed(Time.timeScale);

        // Save the genome of the best performer
        if (Input.GetKeyDown(KeyCode.W))
        {
            genePool.SaveBestPerformingStructure();
        }
    }

    public Structure GetBestCar()
    {
        return genePool.GetBestGenome();
    }

    public IDictionary<string, string> GetGenerationData()
    {
        IDictionary<string, string> generationData = new Dictionary<string, string>();
        generationData.Add("generationNumber", generationNumber.ToString());
        generationData.Add("populationNumber", POPULATION_SIZE.ToString());
        generationData.Add("mutationRate", (MUTATION_RATE * 100).ToString());
        generationData.Add("mutationStrength", ((int)Math.Round(MUTATION_RADIUS)).ToString());
        //generationData.Add("maxFitness", "0");
        generationData.Add("playBackSpeed", ((int)Time.timeScale).ToString());

        return generationData;
    }
}
