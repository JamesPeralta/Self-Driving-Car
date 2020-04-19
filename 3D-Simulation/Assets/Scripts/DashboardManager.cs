using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardManager : MonoBehaviour
{
    public Simulation simulation;
    public UnityEngine.UI.Text generationValue;
    public UnityEngine.UI.Text populationValue;
    public UnityEngine.UI.Text mutationRateValue;
    public UnityEngine.UI.Text mutationStrengthValue;
    public UnityEngine.UI.Text maxCurrentFitnessValue;
    public UnityEngine.UI.Text maxFitnessValue;
    public UnityEngine.UI.Text playBackSpeedValue;
    public Window_graph graph;
    private List<int> valuesList = new List<int>();
    private int startingGen = 1;
    private int maxFitness;

    public void InitializeDashboard(IDictionary<string, string>  generationData)
    {
        string generationVal;
        generationData.TryGetValue("generationNumber", out generationVal);

        string populationVal;
        generationData.TryGetValue("populationNumber", out populationVal);

        string mutationRateVal;
        generationData.TryGetValue("mutationRate", out mutationRateVal);

        string mutationStrengthVal;
        generationData.TryGetValue("mutationStrength", out mutationStrengthVal);

        string maxFitnessVal;
        generationData.TryGetValue("maxFitness", out maxFitnessVal);

        string playbackSpeedVal;
        generationData.TryGetValue("playBackSpeed", out playbackSpeedVal);

        // Fix UI component
        generationValue.text = generationVal;
        populationValue.text = populationVal;
        mutationRateValue.text = mutationRateVal + "%";
        mutationStrengthValue.text = mutationStrengthVal;
        maxFitnessValue.text = maxFitnessVal;
        playBackSpeedValue.text = playbackSpeedVal + "x";
    }

    public void UpdateMaxFitness(int genMaxFitness)
    {
        if(genMaxFitness > maxFitness) {
            maxFitnessValue.text = genMaxFitness.ToString();
            maxFitness = genMaxFitness;
        }
        
        UpdateChart(genMaxFitness);
    }

    public void UpdateCurrentMaxFitness(int maxFitness) {
        maxCurrentFitnessValue.text = maxFitness.ToString();
    }

    public void UpdatePlaybackSpeed(float playbackSpeed)
    {
        playBackSpeedValue.text = ((int)playbackSpeed).ToString() + "x";
    }

    public void UpdateGeneration(int generation)
    {
        generationValue.text = generation.ToString();
    }

    public void UpdateChart(int fitness)
    {
        valuesList.Add(fitness);
        // Remove the first element if it exceeds the max
        if(valuesList.Count > 10) {
            valuesList.RemoveAt(0);
            startingGen++;
        }
        graph.ShowGraph(valuesList, startingGen);
    }
}
