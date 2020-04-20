/* This class contains all of the functionality related to the neural network.
 *
 * Code inspirded by: https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef
*/

using System;
using System.Collections.Generic;


public class NeuralNetwork
{
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights;

    // Defines the architecture of this neural network as a list.
    // [4, 7, 4] = Input layer of 4, hidden layer of 7, output layer of 4
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    #region Initialization functions
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers.Length; j++)
            {
                bias[j] = 0;
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = 0;
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }
    #endregion

    // Activation function that uses Tanh
    public float activate(float value)
    {
        return (float)Math.Tanh(value);
    }

    // Feeds the information forward through the neural network and returns the
    // index of the output node with the most stimulus
    public int FeedForward(float[] inputs)
    {
        // Initialize the input layer values
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Feed input layers forwards across the whole Neural Net
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = activate(value + biases[i][j]);
            }
        }

        // Argmax: Get the index of the highest value in the list
        return GetMaxIndex(neurons[neurons.Length - 1]);
    }

    // Configures the weights and biases of the neural network using a 1D list
    public void ConfigureNeuralNetwork(List<float> genome)
    {
        int index = 0;
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = genome[index];
                index++;
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = genome[index];
                    index++;
                }
            }
        }
    }

    // Argmax function
    public int GetMaxIndex(float[] anArray)
    {
        int index = 0;
        float highestValue = float.NegativeInfinity;
        for (int i = 0; i < anArray.Length; i++)
        {
            if (anArray[i] > highestValue)
            {
                highestValue = anArray[i];
                index = i;
            }
        }

        return index;
    }
}