/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Linq;

public class CSVProcessor : MonoBehaviour
{
    public List<string> headers = new List<string>();
    public List<float[]> numericalData = new List<float[]>();

    public void ProcessCSV(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("CSV is empty or null.");
            return;
        }

        headers.Clear();
        numericalData.Clear();

        string[] rawHeaders = lines[0].Split(',');
        int columnCount = rawHeaders.Length;

        bool[] isNumericColumn = new bool[columnCount];
        List<List<float>> tempColumnData = new List<List<float>>();

        // Assume all columns are numeric to begin with
        for (int col = 0; col < columnCount; col++)
        {
            isNumericColumn[col] = true;
            tempColumnData.Add(new List<float>());
        }

        //TODO : Rewrite numerical column filtering
        // First pass: detect numeric columns and collect values
        for (int row = 1; row < lines.Length; row++)
        {
            string[] cells = lines[row].Split(',');

            for (int col = 0; col < columnCount; col++)
            {
                if (col < cells.Length)
                {
                    string cell = cells[col].Trim();

                    if (float.TryParse(cell, NumberStyles.Any, CultureInfo.InvariantCulture, out float val))
                    {
                        tempColumnData[col].Add(val);
                    }
                    else
                    {
                        isNumericColumn[col] = false; // If anything fails, mark column as non-numeric
                    }
                }
                else
                {
                    isNumericColumn[col] = false;
                }
            }
        }

        // Store only numeric column indices and headers
        List<int> numericColIndices = new List<int>();
        for (int col = 0; col < columnCount; col++)
        {
            if (isNumericColumn[col])
            {
                numericColIndices.Add(col);
                headers.Add(rawHeaders[col]);
            }
        }

        // Calculate column-wise means
        Dictionary<int, float> columnMeans = new Dictionary<int, float>();
        foreach (int col in numericColIndices)
        {
            if (tempColumnData[col].Count > 0)
            {
                columnMeans[col] = tempColumnData[col].Average();
            }
            else
            {
                columnMeans[col] = 0f; // fallback mean
            }
        }

        // Second pass: build final cleaned dataset with mean imputation
        for (int row = 1; row < lines.Length; row++)
        {
            string[] cells = lines[row].Split(',');
            List<float> rowData = new List<float>();

            foreach (int col in numericColIndices)
            {
                if (col < cells.Length &&
                    float.TryParse(cells[col].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out float val))
                {
                    rowData.Add(val);
                }
                else
                {
                    // Fill missing/invalid values with column mean
                    rowData.Add(columnMeans[col]);
                    Debug.LogWarning($"Row {row}, Column {col} was invalid. Filling with mean: {columnMeans[col]}");
                }
            }

            numericalData.Add(rowData.ToArray());
        }

        Debug.Log($"✅ Processed {numericalData.Count} rows using mean imputation.");
        Debug.Log($"🧪 Columns: {string.Join(", ", headers)}");
    }
}*/



using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVProcessor : MonoBehaviour
{
    public enum MissingValueStrategy { Zero, Mean }

    public MissingValueStrategy missingValueStrategy = MissingValueStrategy.Mean;

    public void ProcessCSV(string filePath)
    {
        var pointList = CSVFileReader.Read(filePath);
        if (pointList == null || pointList.Count == 0)
        {
            Debug.LogError("CSV is empty or failed to load.");
            return;
        }

        List<string> allHeaders = new List<string>(pointList[0].Keys);
        List<string> numericHeaders = new List<string>();

        // Identify quantitative (numerical) headers
        foreach (string header in allHeaders)
        {
            if (pointList[0][header] is float || pointList[0][header] is int)
            {
                numericHeaders.Add(header);
            }
        }

        int columnCount = numericHeaders.Count;
        List<float[]> dataRows = new List<float[]>();
        float[] columnSums = new float[columnCount];
        int[] validCounts = new int[columnCount];

        // Extract numerical values and compute column means
        foreach (var row in pointList)
        {
            float[] rowData = new float[columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                string key = numericHeaders[i];
                object rawValue = row[key];
                bool valid = false;

                if (rawValue != null)
                {
                    try
                    {
                        float val = Convert.ToSingle(rawValue);
                        rowData[i] = val;
                        columnSums[i] += val;
                        validCounts[i]++;
                        valid = true;
                    }
                    catch { /* leave as 0 */ }
                }

                if (!valid && missingValueStrategy == MissingValueStrategy.Zero)
                {
                    rowData[i] = 0f;
                }
            }

            dataRows.Add(rowData);
        }

        // Handle missing values using mean if selected
        if (missingValueStrategy == MissingValueStrategy.Mean)
        {
            float[] columnMeans = new float[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                columnMeans[i] = validCounts[i] > 0 ? columnSums[i] / validCounts[i] : 0f;
            }

            for (int r = 0; r < dataRows.Count; r++)
            {
                for (int c = 0; c < columnCount; c++)
                {
                    if (validCounts[c] == 0) continue; // skip if no valid data

                    if (dataRows[r][c] == 0f && !pointList[r][numericHeaders[c]].ToString().Equals("0"))
                    {
                        dataRows[r][c] = columnMeans[c];
                    }
                }
            }
        }

        string fileName = Path.GetFileNameWithoutExtension(filePath);

        // Pass to DataManager
        DataManager.Instance.SetData(numericHeaders, dataRows, fileName);
        Debug.Log("CSV processed and data stored in DataManager.");

        DataManager.Instance.PrintData();
    }
}