/* This class is the driver whose main job is to initalize all of the components
 * required for the GA and run generations endlessly until stopped by a user. The
 * components that it directly instantiates are the Dashboard and the Genepool.
 * Afterwards it will check on the generation every 5 seconds and will spawn the next
 * generation when all cars have crashed.
 * 
 * This script is a component of the Terrain game object.
 *
 */
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    // Can be initalized through Hierarchy/Terrain -> Inspector
    public int MUTATION_RATE; // As a percentage
    public float MUTATION_RADIUS;
    public int POPULATION_SIZE; // Must be even and non-negative
    public string WEIGHTS_FILE; // .txt file that is located in the \Assests\Scripts\NN-Weights folder

    private Genepool genePool;
    public DashboardManager dashboard;
    private int generationNumber;


    /* When this script is started:
     *   1. Instantiate the genepool
     *   2. Start function that will check on gene pool periodically
     *   3. Initialize dashboard 
     */
    void Start()
    {
        // Need an even and positive population size
        if (POPULATION_SIZE % 2 != 0 || POPULATION_SIZE < 2)
        {
            POPULATION_SIZE = 50;
        }

        if (WEIGHTS_FILE != "")
        {
            try
            {
                genePool = new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS, WEIGHTS_FILE);
            }
            catch
            {
                Debug.Log("Weights file doesn't exist");
                genePool = new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS);
            }
        }
        else
        {
            Debug.Log("Weights file not set");
            genePool = new Genepool(new List<Structure>(), POPULATION_SIZE, MUTATION_RATE, MUTATION_RADIUS);
        }

        // If user has choosen to load weights
        InvokeRepeating("CheckOnGeneration", 5.0f, 5.0f);

        // Initialize the dashboard
        generationNumber = 1;
        dashboard.InitializeDashboard(this.GetGenerationData());
    }

    /* Check if this generation has crashed yet.
     * If it has, inform the genepool so that it can perform the pre-defined genetic operators
     * and spawn the next generation. */
    void CheckOnGeneration()
    {
        // If this whole generation has crashed
        if (genePool.PoolStillAlive() == false)
        {
            genePool.pool.Sort();

            // Sort the population based on fitness and report the best one
            Structure bestGenome = genePool.GetBestGenome();
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

    // Wait for user input from specific pre-defined keyboard presses
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

        dashboard.UpdateCurrentMaxFitness(GetBestCar().GetFitness());

        // Save the genome of the best performer
        if (Input.GetKeyDown(KeyCode.W))
        {
            genePool.SaveBestPerformingStructure();
        }
    }

    // Retrieves the best car based on fitness.
    public Structure GetBestCar()
    {
        return genePool.GetBestGenome();
    }

    // Returns a dictionary containing all of the information
    // regarding the genetic algorithm
    public IDictionary<string, string> GetGenerationData()
    {
        IDictionary<string, string> generationData = new Dictionary<string, string>();
        generationData.Add("generationNumber", generationNumber.ToString());
        generationData.Add("populationNumber", POPULATION_SIZE.ToString());
        generationData.Add("mutationRate", (MUTATION_RATE).ToString());
        generationData.Add("mutationStrength", (MUTATION_RADIUS).ToString());
        generationData.Add("playBackSpeed", ((int)Time.timeScale).ToString());

        return generationData;
    }
}
