using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ColumnReader : MonoBehaviour
{
    // The dropdowns for selecting X, Y, Z axis
    public Dropdown xAxisDropdown;
    public Dropdown yAxisDropdown;
    public Dropdown zAxisDropdown;

    void Start()
    {
        string csvPath = "Assets/Resources/winequality-red.csv"; // Specify CSV file path here
        List<string> columnNames = GetColumnNamesFromCSV(csvPath);

        PopulateDropdowns(columnNames);
    }

    // Method to read CSV and get column names (first row)
    List<string> GetColumnNamesFromCSV(string path)
    {
        List<string> columnNames = new List<string>();

        try
        {
            string[] lines = File.ReadAllLines(path);
            if (lines.Length > 0)
            {
                // Split the first line (header row) to get column names
                string[] columns = lines[0].Split(',');
                foreach (var column in columns)
                {
                    columnNames.Add(column.Trim());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading CSV: " + e.Message);
        }

        return columnNames;
    }

    // Method to populate the dropdowns with column names
    void PopulateDropdowns(List<string> columnNames)
    {
        // Clear existing options
        xAxisDropdown.ClearOptions();
        yAxisDropdown.ClearOptions();
        zAxisDropdown.ClearOptions();

        // Add options to the dropdowns
        xAxisDropdown.AddOptions(columnNames);
        yAxisDropdown.AddOptions(columnNames);
        zAxisDropdown.AddOptions(columnNames);
    }
}

