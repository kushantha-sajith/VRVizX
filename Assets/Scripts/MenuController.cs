using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputActionProperty menuButton; // Reference to the menu button input action
    [SerializeField] private GameObject menu; // The menu GameObject
    [SerializeField] private GameObject panel; // The panel GameObject
    [SerializeField] private Transform cameraTransform; // Reference to the main camera or player's head
    [SerializeField] private float distanceFromCamera = 1f; // Distance from the camera to spawn the menu

    private bool isMenuVisible = false; // Tracks whether the menu is currently visible

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
    }

    private void OnDisable()
    {
        if (menuButton.action != null)
        {
            menuButton.action.Disable();
            menuButton.action.performed -= OnMenuButtonPressed; // Unsubscribe from the button press event
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

        HideMenu();
    }

    private void OnMenuButtonPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        if (isMenuVisible)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
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