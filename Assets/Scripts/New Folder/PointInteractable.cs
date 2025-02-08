using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PointInteractable : XRBaseInteractable
{
    public GameObject labelPrefab; // Reference to the label prefab
    private GameObject labelInstance; // Instance of the label
    private Color initialColor; // Stores the initial color of the material

    protected override void Awake()
    {
        base.Awake();
        // Store the initial color of the material
        initialColor = GetComponent<Renderer>().material.color;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        // Change the sphere's color to red
        GetComponent<Renderer>().material.color = Color.red;

        // Get the position of the data point
        Vector3 position = transform.position;

        // Format the position into a string (e.g., "X: 1.23, Y: 4.56, Z: 7.89")
        string labelText = $"X: {position.x:F2}, Y: {position.y:F2}, Z: {position.z:F2}";

        // Instantiate the label prefab
        labelInstance = Instantiate(labelPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);

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
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        // Reset the sphere's color back to the initial color
        GetComponent<Renderer>().material.color = initialColor;

        // Destroy the label when hovering ends
        if (labelInstance != null)
        {
            Destroy(labelInstance);
        }
    }
}