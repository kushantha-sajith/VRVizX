using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutlierMenu : MonoBehaviour
{
    // Dropdowns
    public TMP_Dropdown xColumnDropdown;       // Dropdown for X column selection
    public TMP_Dropdown yColumnDropdown;       // Dropdown for Y column selection
    public TMP_Dropdown zColumnDropdown;       // Dropdown for Z column selection
    public TMP_Dropdown colorDropdown;         // Dropdown for data point color selection
    public TMP_Dropdown outlierColorDropdown; // Dropdown for outlier color selection
    public Canvas canvas;

    // Scrollbar for data point scale
    public Scrollbar dataPointScaleScrollbar;

    // Buttons
    public Button generateChartButton;         // Button to generate the chart
    public Button backButton;                  // Button to go back to the main menu

    // Reference to the main menu panel
    public GameObject mainMenuPanel;

    // Reference to the current chart menu panel
    public GameObject chartMenuPanel;

    public GameObject scatterPlot;
    public GameObject plotter;

    void Start()
    {
        // Ensure the main menu is visible and the chart menu is hidden initially
        mainMenuPanel.SetActive(true);
        chartMenuPanel.SetActive(false);
        scatterPlot.SetActive(false);

        // Add listeners to buttons
        generateChartButton.onClick.AddListener(OnGenerateChartClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void OnGenerateChartClicked()
    {
        // Get the selected indices for X, Y, Z columns
        int xColumnIndex = xColumnDropdown.value;
        int yColumnIndex = yColumnDropdown.value;
        int zColumnIndex = zColumnDropdown.value;

        // Get the selected color value (e.g., as a string or index)
        string selectedColor = colorDropdown.options[colorDropdown.value].text;

        // Get the selected category column value
        string outlierColor = outlierColorDropdown.options[outlierColorDropdown.value].text;

        // Get the data point scale value from the scrollbar and divide it by 2
        float dataPointScale = dataPointScaleScrollbar.value / 2f;

        // Pass the selected values to the ChartGenerator script
        PointRenderer pointRenderer = plotter.GetComponent<PointRenderer>();
        pointRenderer.SetAxisColumns(xColumnIndex, yColumnIndex, zColumnIndex, "Yes", 1, selectedColor, outlierColor, dataPointScale);

        //debug log
        Debug.Log("X: " + xColumnIndex + " Y: " + yColumnIndex + " Z: " + zColumnIndex + " Color: " + selectedColor + " Outlier Color: " + outlierColor + " Scale: " + dataPointScale);
        chartMenuPanel.SetActive(false);
        canvas.gameObject.SetActive(false);
        scatterPlot.SetActive(true);
    }

    void OnBackButtonClicked()
    {
        // Hide the chart menu panel and show the main menu panel
        chartMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}