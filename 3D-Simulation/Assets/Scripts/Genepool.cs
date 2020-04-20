/* This class contains the implementation of the Genepool which contains
 * a pool of all the structures in this generation.
 * 
 *  Class inspired by: Andrew Burton Groeneveldt
 */
using System.Collections.Generic;

public class Genepool
{
    public int MUTATION_RATE;
    public float MUTATION_RADIUS;
    public List<Structure> pool;
    private int poolSize;

    // Instatiates all of the stuctures that will be tested in this generation
    public Genepool(List<Structure> structure, int populationSize, int mutationRate, float mutationRadius, string fileName = null)
    {
        MUTATION_RATE = mutationRate;
        MUTATION_RADIUS = mutationRadius;
        pool = structure;
        poolSize = populationSize;

        // Initializes pool of random structures
        if (pool.Count <= 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                pool.Add(new Structure(new List<float>()));
            }
        }

        // If a weights file was declared, configure this genome with it's contents
        if (fileName != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                pool[i].LoadGenomeFromFile(fileName);
            }
        }

        Test();
    }

    // Spawns each structure onto the track and starts them
    private void Test()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].Evaluate();
        }
    }

    // Returns the best genome of this pool based on fitness
    public Structure GetBestGenome()
    {
        pool.Sort();
        return pool[pool.Count - 1];
    }

    // Returns True if there is atleast one car still driving
    // Returns False if all cars have already crashed
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

    /* This next generation is spawned as follows:
     *   1. Sort the population based on their fitness
     *   2. Select the top 50 percentile
     *   3. The top 50 percentile will create a clone of itself and this clone is mutated
     *   4. The original structures of the top 50 percentile move on without mutation */ 
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

    // Saves the best performing genome into disk
    public void SaveBestPerformingStructure()
    {
        pool.Sort();
        Structure bestGenome = pool[pool.Count - 1];
        bestGenome.SaveGenomeToFile();
    }
}
