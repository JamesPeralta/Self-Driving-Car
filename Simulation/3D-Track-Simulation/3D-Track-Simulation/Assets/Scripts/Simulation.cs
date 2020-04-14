using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Initialize dashboard
//TODO: Create a Genepool
//TODO: Loop that runs the genepool for N generations
//TODO: Updates the dashboard
//TODO: Has playback speed to make the simulation go faster

public class Simulation : MonoBehaviour
{
    public float MUTATION_RATE;
    public int MUTATION_RADIUS;
    public int N_GENERATIONS;
    public int POPULATION_SIZE;

    public GameObject prefab; // Holds the Prefab of our car

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Simulation begun");
        new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS);
    }
}
