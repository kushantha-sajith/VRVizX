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
    [SerializeField] private InputActionProperty rightGripButton;  // Grip button to control rotation and locomotion
    [SerializeField] private InputActionProperty leftGripButton;   // Grip button to control zoom

    [SerializeField] private LocomotionSystem locomotionSystem;
    [SerializeField] private ContinuousMoveProviderBase continuousMoveProvider; // Reference to ContinuousMoveProvider
    [SerializeField] private TeleportationProvider teleportationProvider; // Reference to TeleportationProvider
    [SerializeField] private ActionBasedContinuousTurnProvider continuousTurnProvider; // Reference to ContinuousTurnProvider
    [SerializeField] private ActionBasedSnapTurnProvider snapTurnProvider; // Reference to SnapTurnProvider
    [SerializeField] private InputActionProperty resetButton;

    private Transform scatterplotTransform;

    private float currentZoom = 0.2f;  // Track the current zoom level
    private float previousZoomInput = 0f;  // Store the last zoom input to prevent large jumps

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private void Start()
    {
        scatterplotTransform = this.transform;

        initialPosition = scatterplotTransform.position;
        initialRotation = scatterplotTransform.rotation;
        initialScale = scatterplotTransform.localScale;
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
        HandleLocomotion();
        HandleReset();
    }

    private void HandleRotation()
    {
        // Only allow rotation if the grip button is pressed
        if (rightGripButton.action.ReadValue<float>() > 0.5f)
        {
            // Get rotation input from the right-hand controller's primary 2D axis
            Vector2 primaryAxis = rightHandPrimaryAxis.action.ReadValue<Vector2>();

            float horizontalRotation = primaryAxis.x * rotationSpeed * Time.deltaTime;
            float verticalRotation = -primaryAxis.y * rotationSpeed * Time.deltaTime;

            // Rotate the object around its own center (local space rotation)
            RotateAroundCenter(scatterplotTransform, horizontalRotation, verticalRotation);
        }
    }

    private void RotateAroundCenter(Transform target, float horizontalRotation, float verticalRotation)
    {
        // Rotate around the Y-axis (horizontal rotation)
        target.Rotate(Vector3.up, horizontalRotation, Space.World);

        // Rotate around the X-axis (vertical rotation)
        target.Rotate(Vector3.right, verticalRotation, Space.Self);
    }

    private void HandleZoom()
    {
        if (leftGripButton.action.ReadValue<float>() > 0.5f)
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
    }

    private void HandleLocomotion()
    {
        // Disable teleportation while grip button is pressed
        if (rightGripButton.action.ReadValue<float>() > 0.5f || leftGripButton.action.ReadValue<float>() > 0.5f)
        {
            // Disable teleportation while the grip button is pressed
            if (locomotionSystem != null)
            {
                locomotionSystem.enabled = false;
            }
            if (continuousMoveProvider != null)
            {
                continuousMoveProvider.enabled = false;
            }
            if (teleportationProvider != null)
            {
                teleportationProvider.enabled = false;
            }
            if (continuousTurnProvider != null)
            {
                continuousTurnProvider.enabled = false;
            }
            if (snapTurnProvider != null)
            {
                snapTurnProvider.enabled = false;
            }
        }
        else
        {
            // Enable teleportation when the grip button is not pressed
            if (locomotionSystem != null)
            {
                locomotionSystem.enabled = true;
            }
            if (continuousMoveProvider != null)
            {
                continuousMoveProvider.enabled = true;
            }
            if (teleportationProvider != null)
            {
                teleportationProvider.enabled = true;
            }
            if (continuousTurnProvider != null)
            {
                continuousTurnProvider.enabled = true;
            }
            if (snapTurnProvider != null)
            {
                snapTurnProvider.enabled = true;
            }
        }
    }

    private void HandleReset()
    {
        // Check if the reset button is pressed
        if (resetButton.action.ReadValue<float>() > 0.5f)
        {
            ResetScatterplot();
        }
    }

    private void ResetScatterplot()
    {
        // Reset the scatterplot to its initial state
        scatterplotTransform.position = initialPosition;
        scatterplotTransform.rotation = initialRotation;
        scatterplotTransform.localScale = initialScale;

        // Reset the current zoom level
        currentZoom = initialScale.x; // Assuming uniform scaling
    }
}