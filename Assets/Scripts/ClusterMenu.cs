using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Required for IEnumerator

public class ClusterMenu : MonoBehaviour
{
    // Dropdowns
    public TMP_Dropdown xColumnDropdown;
    public TMP_Dropdown yColumnDropdown;
    public TMP_Dropdown zColumnDropdown;
    public TMP_Dropdown kValueDropdown;
    public Canvas canvas;
    public TextMeshProUGUI warningText;

    // Scrollbar for data point scale
    public Scrollbar dataPointScaleScrollbar;

    // Buttons
    public Button generateChartButton;
    public Button backButton;

    // Reference to the main menu panel
    public GameObject mainMenuPanel;

    // Reference to the current chart menu panel
    public GameObject chartMenuPanel;

    public GameObject scatterPlot;
    public GameObject plotter;

    // Sound effects
    public AudioClip buttonClickSound;  // Sound for button clicks
    public AudioClip dropdownClickSound; // Sound for dropdown clicks

    void Start()
    {
        // Ensure the main menu is visible and the chart menu is hidden initially
        mainMenuPanel.SetActive(true);
        chartMenuPanel.SetActive(false);
        scatterPlot.SetActive(false);

        // Add listeners to buttons
        generateChartButton.onClick.AddListener(OnGenerateChartClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);

        // Add listeners to dropdowns
        xColumnDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        yColumnDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        zColumnDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        kValueDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnGenerateChartClicked()
    {
        // Play button click sound using AudioManager
        if (AudioManager.Instance != null && buttonClickSound != null)
        {
            AudioManager.Instance.PlaySound(buttonClickSound);
        }
        else
        {
            Debug.LogWarning("AudioManager instance or button click sound is missing!");
        }

        // Check if any of the x, y, z columns are not selected (index 0)
        if (xColumnDropdown.value == 0)
        {
            ShowWarning("Please select a column for X axis.");
            return;
        } else if (yColumnDropdown.value == 0)
        {
            ShowWarning("Please select a column for Y axis.");
            return;
        } else if (zColumnDropdown.value == 0)
        {
            ShowWarning("Please select a column for Z axis.");
            return;
        }

        // Check if x, y, z columns have the same index (2 or more)
        if (xColumnDropdown.value == yColumnDropdown.value || xColumnDropdown.value == zColumnDropdown.value || yColumnDropdown.value == zColumnDropdown.value)
        {
            ShowWarning("Please select different columns for X, Y, and Z axes.");
            return;
        }

        // Check if k value is more than 1
        if (kValueDropdown.value + 1 <= 1)
        {
            ShowWarning("Please select a K value greater than 1.");
            return;
        }

        // Start a coroutine to delay panel deactivation
        StartCoroutine(DelayChartGeneration());
    }

    void ShowWarning(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);

        // Hide the warning after 3 seconds
        StartCoroutine(HideWarningAfterDelay(3f));
    }

    IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningText.gameObject.SetActive(false);
    }

    void OnBackButtonClicked()
    {
        // Play button click sound using AudioManager
        if (AudioManager.Instance != null && buttonClickSound != null)
        {
            AudioManager.Instance.PlaySound(buttonClickSound);
        }
        else
        {
            Debug.LogWarning("AudioManager instance or button click sound is missing!");
        }

        // Start a coroutine to delay panel deactivation
        StartCoroutine(DelayPanelDeactivation());
    }

    IEnumerator DelayPanelDeactivation()
    {
        // Wait for the duration of the sound clip
        yield return new WaitForSeconds(buttonClickSound.length);

        // Hide the chart menu panel and show the main menu panel
        chartMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    IEnumerator DelayChartGeneration()
    {
        // Wait for the duration of the sound clip
        yield return new WaitForSeconds(buttonClickSound.length);

        // Perform chart generation logic
        int xColumnIndex = xColumnDropdown.value - 1;
        int yColumnIndex = yColumnDropdown.value - 1;
        int zColumnIndex = zColumnDropdown.value - 1;
        int kValue = kValueDropdown.value + 1;
        float dataPointScale = dataPointScaleScrollbar.value / 2f;

        PointRenderer pointRenderer = plotter.GetComponent<PointRenderer>();
        pointRenderer.SetAxisColumns(xColumnIndex, yColumnIndex, zColumnIndex, "No", kValue, "Default", "Default", dataPointScale);

        // Hide the chart menu panel and show the scatter plot
        chartMenuPanel.SetActive(false);
        canvas.gameObject.SetActive(false);
        scatterPlot.SetActive(true);
    }

    void OnDropdownValueChanged(int value)
    {
        // Play dropdown click sound using AudioManager
        if (AudioManager.Instance != null && dropdownClickSound != null)
        {
            AudioManager.Instance.PlaySound(dropdownClickSound);
        }
        else
        {
            Debug.LogWarning("AudioManager instance or dropdown click sound is missing!");
        }
    }
}