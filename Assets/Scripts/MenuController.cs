using UnityEngine;
using UnityEngine.UI; // Required for Button
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputActionProperty menuButton; // Reference to the menu button input action
    [SerializeField] private GameObject menu; // The menu GameObject
    [SerializeField] private GameObject panel; // The panel GameObject
    [SerializeField] private GameObject confirmationUI; // Confirmation UI with Yes/No options
    [SerializeField] private Transform cameraTransform; // Reference to the main camera or player's head
    [SerializeField] private float distanceFromCamera = 1f; // Distance from the camera to spawn the menu

    [SerializeField] private Button yesButton; // Reference to the "Yes" button
    [SerializeField] private Button noButton; // Reference to the "No" button

    private bool isMenuVisible = false; // Tracks whether the menu is currently visible
    private bool isConfirmationVisible = false; // Tracks whether the confirmation UI is visible

    private void OnEnable()
    {
        if (menuButton.action != null)
        {
            menuButton.action.Enable();
            menuButton.action.performed += OnMenuButtonPressed; // Subscribe to the button press event
        }
        else
        {
            Debug.LogError("Menu button input action is not assigned.");
        }

        // Assign OnClick events to the buttons
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(OnYesButtonPressed);
        }
        else
        {
            Debug.LogError("Yes Button is not assigned.");
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoButtonPressed);
        }
        else
        {
            Debug.LogError("No Button is not assigned.");
        }
    }

    private void OnDisable()
    {
        if (menuButton.action != null)
        {
            menuButton.action.Disable();
            menuButton.action.performed -= OnMenuButtonPressed; // Unsubscribe from the button press event
        }

        // Remove listeners to avoid memory leaks
        if (yesButton != null)
        {
            yesButton.onClick.RemoveListener(OnYesButtonPressed);
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveListener(OnNoButtonPressed);
        }
    }

    private void Start()
    {
        if (menu == null)
        {
            Debug.LogError("Menu GameObject is not assigned.");
        }
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned.");
        }
        if (confirmationUI == null)
        {
            Debug.LogError("Confirmation UI GameObject is not assigned.");
        }

        HideMenu();
        HideConfirmationUI();
    }

    private void OnMenuButtonPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (isMenuVisible)
        {
            // If the menu is visible, hide it
            HideMenu();
        }
        else if (isConfirmationVisible)
        {
            // If the confirmation UI is visible, hide it
            HideConfirmationUI();
        }
        else
        {
            // Otherwise, show the confirmation UI
            ShowConfirmationUI();
        }
    }

    private void ShowConfirmationUI()
    {
        if (cameraTransform != null && confirmationUI != null)
        {
            // Position the confirmation UI 1 meter in front of the camera
            confirmationUI.transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            // Rotate the confirmation UI to face the user
            confirmationUI.transform.rotation = Quaternion.LookRotation(-cameraTransform.forward, cameraTransform.up);
            confirmationUI.SetActive(true);
            isConfirmationVisible = true;
        }
        else
        {
            Debug.LogError("Camera Transform or Confirmation UI GameObject is missing.");
        }
    }

    private void HideConfirmationUI()
    {
        if (confirmationUI != null)
        {
            confirmationUI.SetActive(false);
            isConfirmationVisible = false;
        }
    }

    public void OnYesButtonPressed()
    {
        HideConfirmationUI();
        ShowMenu();
    }

    public void OnNoButtonPressed()
    {
        HideConfirmationUI();
    }

    private void ShowMenu()
    {
        if (cameraTransform != null && menu != null)
        {
            // Position the menu 1 meter in front of the camera
            menu.transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            // Rotate the menu to face the user
            menu.transform.rotation = Quaternion.LookRotation(-cameraTransform.forward, cameraTransform.up);
            menu.SetActive(true);
            panel.SetActive(true);
            isMenuVisible = true;
        }
        else
        {
            Debug.LogError("Camera Transform or Menu GameObject is missing.");
        }
    }

    private void HideMenu()
    {
        if (menu != null)
        {
            menu.SetActive(false);
            isMenuVisible = false;
        }
    }
}