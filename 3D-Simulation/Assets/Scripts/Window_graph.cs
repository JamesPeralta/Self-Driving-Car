/// Citation
/// 
/// Title: Unity Tutorial - Create a Graph
/// Author: Code Monkey
/// Date: June 22, 2018
/// Code version: 1.0
/// Avaliability: https://unitycodemonkey.com/video.php?v=CmU5-v-v1Qo
/// 
/// This class was modified from the Unity tutorial above

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_graph : MonoBehaviour
{
    // This is the dashboard that will control the graphs data
    public DashboardManager dashboard;

    // This is the sprite that will show for each point on the graph
    [SerializeField] private Sprite circleSprite;

    // This is the container that the graph will be displayed in
    private RectTransform graphContainer;

    // This is the x label template
    private RectTransform labelTemplateX;

    // This is the y label template
    private RectTransform labelTemplateY;

    // This is the data that is contained in the previous graph that we need to removed
    private List<GameObject> previousGraph;

    // These are the labels of the previous graph
    private List<RectTransform> previousAxis;

    /// This function will be run when the Unity project is first run, it will find the components it needs
    /// as well as show the current graph
    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();


        List<int> valuesList = new List<int>();
        previousGraph = new List<GameObject>();
        previousAxis = new List<RectTransform>();

        ShowGraph(valuesList, 0);
    }

    /// This is the function to create a circle on the graph at the
    /// coordinated specified by the Vector2 anchoredPosition parameter
    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        // Initialize a new GameObject
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);

        // Set the image to the circle sprite
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        // Set the position and size of the point
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;
    }

    /// This function takes a list of values to display on the graph
    /// The startingGen parameter represents the first generation number
    /// of the valueList
    public void ShowGraph(List<int> valueList, int startingGen)
    {
        // Destroy the links of the previous graph
        foreach (GameObject g in previousGraph)
        {
            Destroy(g);
        }
        previousGraph.Clear();

        // Remove the previous axis labels so we can add new ones
        foreach (RectTransform a in previousAxis)
        {
            a.gameObject.SetActive(false);
        }
        previousAxis.Clear();

        // Set the sizes of the graph
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = 118;
        float xSize = 20.5f;

        // For every point we will create a new circle and then we will
        // create a link that connects it to the previously placed point
        GameObject prevCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            // Calculate the x and y positions
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMax) * graphHeight;

            // Create a new circle at the x and y positions
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));

            // If a previous circle exists, then create a link to it
            if (prevCircleGameObject != null)
            {
                CreateLink(prevCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }

            // Set the previous circle to the current circle since we will use it in the next iteration
            prevCircleGameObject = circleGameObject;

            // Create the x label for the point that we have just created
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -4f);
            labelX.GetComponent<Text>().text = startingGen.ToString();

            // Track the graph using the lists so that we can remove it when we need to
            previousGraph.Add(circleGameObject);
            previousAxis.Add(labelX);

            startingGen++;
        }

        int separatorCount = 10; // This is the number of seprations that the y axis should have for its values

        // This loop creates the y axis labels
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);

            float normalizedValue = i * 1f / separatorCount;

            // Place the y axis label
            labelY.anchoredPosition = new Vector2(-20f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMax).ToString();

            // Track the Y axis label to place
            previousAxis.Add(labelY);
        }
    }

    /// This method will create a link(line) between the two coordinates that are specified by the parameters
    private void CreateLink(Vector2 dotPosition1, Vector2 dotPosition2)
    {
        // Create the line game object
        GameObject gameObject = new GameObject("dotLink", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        // Get the direction of the the line as well as the distance
        Vector2 dir = (dotPosition2 - dotPosition1).normalized;
        float distance = Vector2.Distance(dotPosition1, dotPosition2);

        // Place the line
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.anchoredPosition = dotPosition1 + dir * distance * .5f;

        // Calculate the angle the line needs to be on the graph
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        // Track the line so that we can remove it after
        previousGraph.Add(gameObject);
    }
}
