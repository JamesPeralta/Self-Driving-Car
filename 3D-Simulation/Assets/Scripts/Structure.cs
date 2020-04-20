/*  Implementation of the potential solutions that are tested in the Environment.
 *  When instantiated, objects of this class type will contain a genome, a car,
 *  and a Neural Network.
 *
 *  Class inspired by: Andrew Burton Groeneveldt
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Structure: MonoBehaviour, IComparable<Structure>
{
    private int[] LAYER_SIZES = new int[] { 4, 7, 4 };
    private int GENOME_LENGTH = 71;
    private const string WEIGHTS_PATH = "Assets/Scripts/NN-Weights/";
    private string SAVE_PATH = WEIGHTS_PATH + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + ".txt";

    private List<float> genome;
    private CarController car;
    private NeuralNetwork neuralNetwork;

    //Has access to car prefab and spawns it
    public Structure(List<float> _genome)
    {
        genome = _genome;

        // Instantiate the car at the starting line
        GameObject instance = Resources.Load("HatchBack") as GameObject;
        car = (Instantiate(instance)).GetComponent<CarController>();
        GameObject startingLine = GameObject.Find("Starting Line");
        car.transform.position = startingLine.gameObject.transform.position;

        // Instantiate the Neural Network object
        neuralNetwork = new NeuralNetwork(LAYER_SIZES);

        // If the genome is invalid, create a new random genome
        if (genome.Count != GENOME_LENGTH)
        {
            for (int i = 0; i < GENOME_LENGTH; i++)
            {
                genome.Add(UnityEngine.Random.Range(-0.5f, 0.5f));
            }
        }
    }

    #region Getters/Setters
    public CarController GetCar()
    {
        return car;
    }

    public bool IsAlive()
    {
        if (car.hitWall)
        {
            return false;
        }

        return true;
    }

    public int GetFitness()
    {
        return car.GetFitness();
    }
    #endregion

    // Configures the neural network with the genome of this structure
    // and starts the car on the track
    public void Evaluate()
    {
        neuralNetwork.ConfigureNeuralNetwork(genome);
        car.SetNeuralNetwork(neuralNetwork);
    }

    // This is used by the IComparable<Structure> interface to enable the sort() function
    // for a list of these classes 
    public int CompareTo(Structure other)
    {
        if (car.GetFitness() > other.car.GetFitness())
        {
            return 1;
        }
        else if (car.GetFitness() < other.car.GetFitness())
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    /* Mutation function.
     *
     * mutationRate: as a percentage
    */
    public void Mutate(int mutationRate, float mutationRadius)
    {
        for (int i = 0; i < genome.Count; i++)
        {
            genome[i] = (UnityEngine.Random.Range(0, 100) <= mutationRate) ? genome[i] += UnityEngine.Random.Range(-mutationRadius, mutationRadius) : genome[i];
        }
    }

    // Returns a deep copied array of this geneome 
    public List<float> deepCopyGenome()
    {
        List<float> genomeCopy = new List<float>();
        for (int i = 0; i < genome.Count; i++)
        {
            genomeCopy.Add(genome[i]);
        }

        return genomeCopy;
    }

    #region Load/Save genomes from a file
    // Loads the biases and weights from within a file into the neural network.
    public void LoadGenomeFromFile(string fileName)
    {
        string filePath = WEIGHTS_PATH + fileName;

        TextReader tr = new StreamReader(filePath);
        int NumberOfLines = (int)new FileInfo(filePath).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();

        genome = new List<float>();
        if (new FileInfo(filePath).Length > 0)
        {
            for  (int i = 0; i < GENOME_LENGTH; i++)
            {
                genome.Add(float.Parse(ListLines[index]));
                index++;
            }
        }
    }

    // Saves the biases and weights within the network to a file.
    public void SaveGenomeToFile()
    {
        File.Create(SAVE_PATH).Close();
        StreamWriter writer = new StreamWriter(SAVE_PATH, true);

        for (int i = 0; i < GENOME_LENGTH; i++)
        {
            writer.WriteLine(genome[i]);
        }
        
        writer.Close();
    }
    #endregion
}
