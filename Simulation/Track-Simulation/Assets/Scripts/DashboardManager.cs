using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardManager : MonoBehaviour
{
    public EnvironmentManager environment;
    public UnityEngine.UI.Text generationValue;
    public UnityEngine.UI.Text populationValue;
    public UnityEngine.UI.Text mutationRateValue;
    public UnityEngine.UI.Text mutationStrengthValue;
    public UnityEngine.UI.Text maxFitnessValue;
    public UnityEngine.UI.Text playBackSpeedValue;

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

    public void UpdateMaxFitness(int maxFitness)
    {
        maxFitnessValue.text = maxFitness.ToString();
    }

    public void UpdatePlaybackSpeed(float playbackSpeed)
    {
        playBackSpeedValue.text = ((int)playbackSpeed).ToString() + "x";
    }

    public void UpdateGeneration(int generation)
    {
        generationValue.text = generation.ToString();
    }
}
