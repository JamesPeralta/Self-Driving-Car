using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

// TODO: Initializes a pool of random structures
// TODO: Spawns each structure onto the map
// TODO: Checks for the status of structures
// TODO:   - If a structure is not making any progress it is killed
// TODO:   - If all structures have been killed or crashed, move onto the next step
// TODO: Sorts each structure by their fitness function
// TODO: Creates the next generation


public class Genepool
{
    public float MUTATION_RATE;
    public int MUTATION_RADIUS;
    public List<Structure> pool;
    private int poolSize;

    public Genepool(List<Structure> structure, int populationSize, float mutationRate, int mutationRadius)
    {
        MUTATION_RATE = mutationRate;
        MUTATION_RADIUS = mutationRadius;
        pool = structure;
        poolSize = populationSize;

        // Initialize pool of random structures
        if (structure.Count <= 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                pool.Add(new Structure(new List<float>()));
            }
        }

        //Test them out
        Test();
    }

    private void Test()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].Evaluate();
        }
    }
}
