using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


// TODO: Contains the genome
// TODO: Contains a neural network
// TODO: Contains a car prefab
// TODO: Contains fitness value
// TODO: Contains all operators
//         - n_point_crossover
//         - mutate
// TODO: Contains evaluate function: When this function is called, a car prefab will be created and will attempt to navigate the track.
// TODO: Save function
// TODO: Load function

public class Structure: MonoBehaviour
{
    private int[] LAYER_SIZES = new int[] { 3, 7, 4 };
    private int GENOME_LENGTH = 63;

    private List<float> genome;
    private CarController car;
    private NeuralNetwork neuralNetwork;

    //Has access to car prefab and spawns it
    public Structure(List<float> _genome)
    {
        genome = _genome;

        // Instantiate the car
        GameObject instance = Resources.Load("HatchBack") as GameObject;
        car = (Instantiate(instance)).GetComponent<CarController>();
        car.transform.position = new Vector3(40, 30, 380);

        // Instantiate the Neural Network
        neuralNetwork = new NeuralNetwork(LAYER_SIZES);

        // If the genome is invalid, create a new random genome
        if (genome.Count != 63)
        {
            for (int i = 0; i < GENOME_LENGTH; i++)
            {
                genome.Add(UnityEngine.Random.Range(-0.5f, 0.5f));
            }
        }
    }

    public void Evaluate()
    {
        // Configure car with Neural Network
        neuralNetwork.ConfigureNeuralNetwork(genome);
        car.SetNeuralNetwork(neuralNetwork);
    }

    //used as a simple mutation function for any genetic implementations.
    public void Mutate(int mutationRate, float mutationRadius)
    {
        for (int i = 0; i < genome.Count; i++)
        {
            genome[i] = (UnityEngine.Random.Range(0, 100) <= mutationRate * 100) ? genome[i] += UnityEngine.Random.Range(-mutationRadius, mutationRadius) : genome[i];
        }
    }

    public List<float> deepCopyGenome()
    {
        List<float> genomeCopy = new List<float>();
        for (int i = 0; i < genome.Count; i++)
        {
            genomeCopy.Add(genome[i]);
        }

        return genomeCopy;
    }

    public void LoadGenomeFromFile(string path)//this loads the biases and weights from within a file into the neural network.
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();

        genome = new List<float>();
        if (new FileInfo(path).Length > 0)
        {
            for  (int i = 0; i < GENOME_LENGTH; i++)
            {
                genome.Add(float.Parse(ListLines[index]));
                index++;
            }
        }
    }

    public void SaveGenomeToFile(string path)//this is used for saving the biases and weights within the network to a file.
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < GENOME_LENGTH; i++)
        {
            writer.WriteLine(genome[i]);
        }

        writer.Close();
    }
}
