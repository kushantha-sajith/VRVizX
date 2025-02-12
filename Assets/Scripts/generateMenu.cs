using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class generateMenu : MonoBehaviour
{
    public GameObject menuUI;
    public Dropdown xDropdown;
    public Dropdown yDropdown;
    public Dropdown zDropdown;
    public Dropdown outliers;
    public Dropdown clusters;
    public Button generateButton;

    public GameObject scatterPlot;
    public GameObject plotter;

    // Start is called before the first frame update
    void Start()
    {
        // Initially hide the scatterplot
        scatterPlot.SetActive(false);

        // Add listener to the button
        generateButton.onClick.AddListener(OnGenerateChart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGenerateChart()
    {
        // Get selected axis columns
        int xAxis = xDropdown.value;
        int yAxis = yDropdown.value;
        int zAxis = zDropdown.value;

        string outlier = outliers.options[outliers.value].text;
        int cluster = clusters.value + 1;

        // Find the scatterplot script and pass the selected values
        PointRenderer pointRenderer = plotter.GetComponent<PointRenderer>();
        pointRenderer.SetAxisColumns(xAxis, yAxis, zAxis, outlier, cluster, "", "", 0.25f);

        Debug.Log("Generating chart with x: " + xAxis + ", y: " + yAxis + ", z: " + zAxis + ", outlier: " + outlier + ", cluster: " + cluster);

        menuUI.SetActive(false);

        // Show the scatterplot
        scatterPlot.SetActive(true);

        // Generate the chart
        //pointRenderer.InitializeScatterplot();
        //pointRenderer.Invoke("Start", 0f);
    }
}
