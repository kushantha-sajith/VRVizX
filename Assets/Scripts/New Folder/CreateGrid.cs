using UnityEngine;
using TMPro;

public class CreateGrid : MonoBehaviour
{
    public int gridSize = 10; // Number of grid lines on each side of the origin
    public float spacing = 1f; // Distance between grid lines
    public GameObject linePrefab; // Prefab with a LineRenderer component
    public GameObject axisLabelPrefab; // Prefab for axis labels
    public Vector3 dataBoundsMin = new Vector3(-10, -10, -10); // Minimum bounds of data points
    public Vector3 dataBoundsMax = new Vector3(10, 10, 10); // Maximum bounds of data points

    void Start()
    {
        // Generate grid lines within the bounds of the data points
        CreateGridLines();
        // Add axis labels
        AddAxisLabels();
    }

    void CreateGridLines()
    {
        // Calculate the start and end positions for the grid lines based on data bounds
        float startX = Mathf.Floor(dataBoundsMin.x / spacing) * spacing;
        float endX = Mathf.Ceil(dataBoundsMax.x / spacing) * spacing;
        float startZ = Mathf.Floor(dataBoundsMin.z / spacing) * spacing;
        float endZ = Mathf.Ceil(dataBoundsMax.z / spacing) * spacing;

        // Generate X-axis lines
        for (float x = startX; x <= endX; x += spacing)
        {
            CreateLine(new Vector3(x, dataBoundsMin.y, startZ), new Vector3(x, dataBoundsMin.y, endZ));
        }

        // Generate Z-axis lines
        for (float z = startZ; z <= endZ; z += spacing)
        {
            CreateLine(new Vector3(startX, dataBoundsMin.y, z), new Vector3(endX, dataBoundsMin.y, z));
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        // Instantiate the line prefab
        GameObject line = Instantiate(linePrefab);
        // Get the LineRenderer component
        LineRenderer lr = line.GetComponent<LineRenderer>();
        // Set the start and end positions of the line
        lr.SetPositions(new Vector3[] { start, end });
    }

    void AddAxisLabels()
    {
        // Add X-axis label
        GameObject xLabel = Instantiate(axisLabelPrefab, new Vector3(dataBoundsMax.x + 1f, dataBoundsMin.y, dataBoundsMin.z), Quaternion.identity);
        xLabel.GetComponent<TextMeshPro>().text = "X";

        // Add Y-axis label
        GameObject yLabel = Instantiate(axisLabelPrefab, new Vector3(dataBoundsMin.x, dataBoundsMax.y + 1f, dataBoundsMin.z), Quaternion.identity);
        yLabel.GetComponent<TextMeshPro>().text = "Y";

        // Add Z-axis label
        GameObject zLabel = Instantiate(axisLabelPrefab, new Vector3(dataBoundsMin.x, dataBoundsMin.y, dataBoundsMax.z + 1f), Quaternion.identity);
        zLabel.GetComponent<TextMeshPro>().text = "Z";
    }
}