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
    public int MUTATION_RATE;
    public float MUTATION_RADIUS;
    public List<Structure> pool;
    private int poolSize;

    public Genepool(List<Structure> structure, int populationSize, int mutationRate, float mutationRadius)
    {
        MUTATION_RATE = mutationRate;
        MUTATION_RADIUS = mutationRadius;
        pool = structure;
        poolSize = populationSize;

        // Initialize pool of random structures
        if (pool.Count <= 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                pool.Add(new Structure(new List<float>()));
            }
        }

        Test();
    }

    private void Test()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].Evaluate();
        }
    }

    public Structure GetBestGenome()
    {
        pool.Sort();
        return pool[pool.Count - 1];
    }

    public bool PoolStillAlive()
    {
        int crashedCount = 0;
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].IsAlive() == false)
            {
                crashedCount++;
            }
        }

        if (crashedCount == poolSize)
        {
            return false;
        }

        return true;
    }

    public void NextGeneration()
    {
        List<Structure> newPool = new List<Structure>();

        // The bottom half are replaced by mutated versions of the top half
        for (int i = 0; i < pool.Count / 2; i++)
        {
            Structure newGenome = new Structure(pool[i + (pool.Count / 2)].deepCopyGenome());
            newGenome.Mutate(MUTATION_RATE, MUTATION_RADIUS);
            newPool.Add(newGenome);
        }

        // Top half stay the same
        for (int i = pool.Count / 2; i < pool.Count; i++)
        {
            newPool.Add(new Structure(pool[i].deepCopyGenome()));
        }

        pool = newPool;

        Test();
    }
}
