using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ScatterplotPointUIHandler : MonoBehaviour
{
    public GameObject labelPrefab; // Reference to the label prefab
    private GameObject labelInstance; // Instance of the label

    private Color initialColor; // Store the initial color of the scatterplot point
    private Vector3 initialScale; // Store the initial scale of the scatterplot point
    private Renderer pointRenderer; // Renderer component of the scatterplot point

    private XRBaseInteractable interactable; // Reference to the XRBaseInteractable component

    public AudioClip clickSound;  // Sound for clicks
    public AudioSource audioSource;

    private void Start()
    {
        // Cache the main camera for UI alignment
        Camera mainCamera = Camera.main;

        // Get the Renderer component of the scatterplot point
        pointRenderer = GetComponent<Renderer>();
        if (pointRenderer != null)
        {
            // Store the initial color of the scatterplot point
            initialColor = pointRenderer.material.color;
        }

        // Store the initial scale of the scatterplot point
        initialScale = transform.localScale;

        // Get the XRBaseInteractable component
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            // Subscribe to hover events
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        // Change the color of the scatterplot point when hovered
        if (pointRenderer != null)
        {
            // Create a new material instance to avoid affecting other objects
            pointRenderer.material.SetColor("_Color", Color.red);
        }

        // Increase the size of the scatterplot point when hovered
        transform.localScale = initialScale * 1.5f; // Scale up by 20%

        // Trigger haptic feedback
        TriggerHapticFeedback(args.interactorObject);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        // Reset the color of the scatterplot point when hover ends
        if (pointRenderer != null)
        {
            pointRenderer.material.SetColor("_Color", initialColor);
        }

        // Reset the size of the scatterplot point when hover ends
        transform.localScale = initialScale;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("ScatterplotPointUIHandler script started.");

        // Play a click sound
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogError("Click sound or audio source not set.");
        }

        // Get the selected scatterplot point's data            
        ScatterplotPointData pointData = args.interactableObject.transform.GetComponent<ScatterplotPointData>();
        if (pointData != null)
        {
            // Update the UI with the point's x, y, z data
            string labelText = $"X: {pointData.xValue:F2}\nY: {pointData.yValue:F2}\nZ: {pointData.zValue:F2}";
            Debug.Log($"Point Data: {labelText}");

            // Calculate a position in front of the user's camera
            Vector3 labelPosition = CalculateLabelPositionInFrontOfUser();

            // Instantiate the label prefab at the calculated position
            labelInstance = Instantiate(labelPrefab, labelPosition, Quaternion.identity);

            // Set the label text
            TextMeshProUGUI textComponent = labelInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = labelText;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the label prefab.");
            }

            // Make the label face the user
            if (labelInstance != null)
            {
                labelInstance.transform.LookAt(Camera.main.transform);
                labelInstance.transform.rotation = Quaternion.LookRotation(labelInstance.transform.position - Camera.main.transform.position);
            }

            // Destroy the label after 3 seconds
            Destroy(labelInstance, 3f);
        }

        // Trigger haptic feedback on selection
        TriggerHapticFeedback(args.interactorObject);
    }

    private Vector3 CalculateLabelPositionInFrontOfUser()
    {
        // Get the camera's position and forward direction
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;

        // Define a distance in front of the camera (e.g., 2 units)
        float distanceInFrontOfUser = 0.5f;

        // Calculate the label's position
        Vector3 labelPosition = cameraPosition + cameraForward * distanceInFrontOfUser;

        return labelPosition;
    }

    private void TriggerHapticFeedback(IXRInteractor interactor)
    {
        // Check if the interactor supports haptic feedback
        if (interactor is XRBaseControllerInteractor controllerInteractor)
        {
            // Trigger haptic feedback
            controllerInteractor.SendHapticImpulse(0.3f, 0.1f); // Amplitude: 0.5, Duration: 0.2 seconds
        }
    }
}