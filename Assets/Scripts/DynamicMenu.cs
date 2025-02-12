using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicMenu : MonoBehaviour
{
    public TMP_Dropdown mainDropdown; // Reference to the main dropdown
    public GameObject submenu1;   // Reference to the first submenu panel
    public GameObject submenu2;   // Reference to the second submenu panel
    public GameObject submenu3;   // Reference to the third submenu panel
    public Button confirmButton;  // Reference to the confirmation button
    public TextMeshProUGUI warningText;      // Reference to the warning message text
    public GameObject mainPanel;  // Reference to the main panel

    private int selectedIndex = -1; // Store the selected dropdown index

    void Start()
    {
        // Ensure all submenus are disabled initially
        submenu1.SetActive(false);
        submenu2.SetActive(false);
        submenu3.SetActive(false);

        // Hide the warning message initially
        warningText.gameObject.SetActive(false);

        // Add listener to the dropdown
        mainDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Add listener to the confirmation button
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    void OnDropdownValueChanged(int index)
    {
        // Store the selected index
        selectedIndex = index - 1;

        // Hide the warning message when a new option is selected
        warningText.gameObject.SetActive(false);
    }

    void OnConfirmButtonClicked()
    {
        // Disable all submenus first
        submenu1.SetActive(false);
        submenu2.SetActive(false);
        submenu3.SetActive(false);

        // Enable the selected submenu based on the stored index
        switch (selectedIndex)
        {
            case 0:
                submenu1.SetActive(true);
                mainPanel.SetActive(false);
                break;
            case 1:
                submenu2.SetActive(true);
                mainPanel.SetActive(false);
                break;
            case 2:
                submenu3.SetActive(true);
                mainPanel.SetActive(false);
                break;
            default:
                // Show warning message if no valid option is selected
                warningText.text = "Please select a chart from the menu.";
                warningText.gameObject.SetActive(true);
                break;
        }
    }
}