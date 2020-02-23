using System;
using System.Collections.Generic;
using UnityEngine;


public class NeuralNetwork
{
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights;
    private int[] activations;

    public float fitness = 0;

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
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        // Start off at the first layer in
        for (int i = 1; i < layers.Length; i++)
        {
           List<float[]> layerWeightsList = new List<float[]>();
           int neuronsInPreviousLayer = layers[i - 1];
           for (int j = 0; j < neurons[i].Length; j++)
           {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float activate(float value)
    {
        return (float) Math.Tanh(value);
    }

    public float[] FeedForward(float[] inputs)
    {
        // Initialize the input layer values
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Feed input layers forwards across the whole Neural Net
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
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
        return neurons[neurons.Length - 1];
    }

    // ________ Debuging Functions ________________

    public void printAttributes()
    {
        foreach (var item in weights)
        {
            String neuron_string = "";
            foreach (var item1 in item)
            {
                neuron_string = neuron_string + " " + item1;
            }
            Debug.Log(neuron_string);
       }
    }
}
