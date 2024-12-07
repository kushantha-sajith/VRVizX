using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ScatterplotController : MonoBehaviour
{
    public float rotationSpeed = 50f;  // Adjust rotation speed
    public float zoomSpeed = 0.1f;     // Adjust zoom speed (this controls how fast zoom happens)
    public float minZoom = 0.1f;       // Minimum scale
    public float maxZoom = 1.5f;       // Maximum scale

    [SerializeField] private InputActionProperty rightHandPrimaryAxis;  // Rotation input
    [SerializeField] private InputActionProperty leftHandPrimaryAxis;   // Zoom input
    [SerializeField] private InputActionProperty gripButton;  // Grip button to control rotation and locomotion

    [SerializeField] private LocomotionSystem locomotionSystem;
    private Transform scatterplotTransform;

    private float currentZoom = 0.2f;  // Track the current zoom level
    private float previousZoomInput = 0f;  // Store the last zoom input to prevent large jumps

    private void Start()
    {
        scatterplotTransform = this.transform;
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
        HandleLocomotion();
    }

    private void HandleRotation()
    {
        // Only allow rotation if the grip button is pressed
        if (gripButton.action.ReadValue<float>() > 0.5f)
        {
            // Get rotation input from the right-hand controller's primary 2D axis
            Vector2 primaryAxis = rightHandPrimaryAxis.action.ReadValue<Vector2>();

            float horizontalRotation = primaryAxis.x * rotationSpeed * Time.deltaTime;
            float verticalRotation = -primaryAxis.y * rotationSpeed * Time.deltaTime;

            // Rotate the object around its own center (local space rotation)
            scatterplotTransform.Rotate(Vector3.up, horizontalRotation, Space.Self);  // Horizontal rotation around own axis (Y-axis)
            scatterplotTransform.Rotate(Vector3.right, verticalRotation, Space.Self); // Vertical rotation around own axis (X-axis)
        }
    }

    private void HandleZoom()
    {
        // Get zoom input from the left-hand controller's primary 2D axis (vertical axis for zoom)
        Vector2 primaryAxis = leftHandPrimaryAxis.action.ReadValue<Vector2>();

        // Use the vertical axis (Y) for zooming (up-down direction)
        float zoomInput = primaryAxis.y;

        // Only zoom if there is a noticeable change in input to prevent jumps
        if (zoomInput != previousZoomInput)
        {
            // Apply gradual zooming based on zoom input and zoom speed
            currentZoom += zoomInput * zoomSpeed;

            // Clamp the zoom to the minimum and maximum zoom limits
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            // Apply the zoom to the object's scale
            scatterplotTransform.localScale = Vector3.one * currentZoom;
        }

        // Store the current zoom input for the next frame to prevent large jumps
        previousZoomInput = zoomInput;
    }

    private void HandleLocomotion()
    {
        // Disable teleportation while grip button is pressed
        if (gripButton.action.ReadValue<float>() > 0.5f)
        {
            // Disable teleportation while the grip button is pressed
            locomotionSystem.enabled = false;
        }
        else
        {
            // Enable teleportation when the grip button is not pressed
            locomotionSystem.enabled = true;
        }
    }
}
