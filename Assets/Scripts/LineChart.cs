using System.Collections.Generic;
using UnityEngine;

public class LineChart : MonoBehaviour
{
    public List<Vector3> dataPoints = new List<Vector3>(); // Points to plot
    public Material lineMaterial; // Material for the chart lines
    public Material axisMaterial; // Material for the axes
    public GameObject textPrefab; // Prefab for axis labels (using 3D Text or TMP)

    public float axisLength = 5f; // Length of each axis
    public int axisDivisions = 5; // Number of ticks on each axis

    void Start()
    {
        // Example data points (replace with your dataset)
        dataPoints.Add(new Vector3(0, 1, 0));
        dataPoints.Add(new Vector3(1, 2, 1));
        dataPoints.Add(new Vector3(2, 3, 2));
        dataPoints.Add(new Vector3(3, 2, 3));
        dataPoints.Add(new Vector3(4, 1, 4));

        DrawAxes();
        DrawLineChart();
    }

    void DrawLineChart()
    {
        for (int i = 0; i < dataPoints.Count - 1; i++)
        {
            DrawLine(dataPoints[i], dataPoints[i + 1], lineMaterial);
        }
    }

    void DrawAxes()
    {
        // Draw X Axis
        DrawLine(Vector3.zero, new Vector3(axisLength, 0, 0), axisMaterial);
        AddAxisLabels(Vector3.zero, Vector3.right, "X");

        // Draw Y Axis
        DrawLine(Vector3.zero, new Vector3(0, axisLength, 0), axisMaterial);
        AddAxisLabels(Vector3.zero, Vector3.up, "Y");

        // Draw Z Axis
        DrawLine(Vector3.zero, new Vector3(0, 0, axisLength), axisMaterial);
        AddAxisLabels(Vector3.zero, Vector3.forward, "Z");
    }

    void DrawLine(Vector3 start, Vector3 end, Material material)
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(this.transform);

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;

        // Optional: Color gradient for axes or lines
        if (material == lineMaterial)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.red, 0.0f),
                    new GradientColorKey(Color.blue, 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 1.0f)
                }
            );
            lineRenderer.colorGradient = gradient;
        }
    }

    void AddAxisLabels(Vector3 origin, Vector3 direction, string axisName)
    {
        for (int i = 1; i <= axisDivisions; i++)
        {
            // Calculate the position for the tick
            Vector3 tickPosition = origin + direction * (i * axisLength / axisDivisions);

            // Draw tick lines
            DrawLine(tickPosition - (Vector3.one * 0.1f), tickPosition + (Vector3.one * 0.1f), axisMaterial);

            // Add text labels
            if (textPrefab != null)
            {
                GameObject textObj = Instantiate(textPrefab, tickPosition, Quaternion.identity, this.transform);
                textObj.GetComponent<TextMesh>().text = (i * axisLength / axisDivisions).ToString("F1") + " " + axisName;
                textObj.transform.localScale = Vector3.one * 0.1f; // Scale down for readability
            }
        }
    }
}
