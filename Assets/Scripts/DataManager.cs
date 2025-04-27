using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public List<string> headers;
    public List<float[]> numericalData;
    public string fileName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetData(List<string> headers, List<float[]> data, string fileName)
    {
        this.headers = headers;
        this.numericalData = data;
        this.fileName = fileName;
    }

    public void PrintData()
    {
        if (headers == null || numericalData == null)
        {
            Debug.LogWarning("No data to print.");
            return;
        }

        // Print headers
        string headerLine = string.Join(", ", headers);
        Debug.Log("Headers: " + headerLine);

        // Print each row
        for (int i = 0; i < numericalData.Count; i++)
        {
            string row = string.Join(", ", numericalData[i]);
            Debug.Log($"Row {i + 1}: {row}");
        }
    }

}