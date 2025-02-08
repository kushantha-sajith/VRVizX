using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ScatterplotPointUIHandler : MonoBehaviour
{
    public GameObject labelPrefab; // Reference to the label prefab
    private GameObject labelInstance; // Instance of the label

    private Color initialColor; // Store the initial color of the scatterplot point
    private Renderer pointRenderer; // Renderer component of the scatterplot point

    private XRBaseInteractable interactable; // Reference to the XRBaseInteractable component

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
            pointRenderer.material.color = Color.red; // Change to red (or any other color)
        }

        // Trigger haptic feedback
        TriggerHapticFeedback(args.interactorObject);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        // Reset the color of the scatterplot point when hover ends
        if (pointRenderer != null)
        {
            pointRenderer.material.color = initialColor;
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("ScatterplotPointUIHandler script started.");

        // Get the selected scatterplot point's data            
        ScatterplotPointData pointData = args.interactableObject.transform.GetComponent<ScatterplotPointData>();
        if (pointData != null)
        {
            // Update the UI with the point's x, y, z data
            string labelText = $"X: {pointData.xValue:F2}\nY: {pointData.yValue:F2}\nZ: {pointData.zValue:F2}";
            Debug.Log($"Point Data: {labelText}");

            // Instantiate the label prefab
            //labelInstance = Instantiate(labelPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);

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

            // Destroy the label after 5 seconds
            Destroy(labelInstance, 5f);
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
            controllerInteractor.SendHapticImpulse(0.5f, 0.2f); // Amplitude: 0.5, Duration: 0.2 seconds
        }
    }
}