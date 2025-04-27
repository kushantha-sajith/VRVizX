using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVFileReader
{
    public static List<Dictionary<string, object>> Read(string filePath)
    {
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length <= 1)
            {
                Debug.LogError("CSV file does not contain enough lines.");
                return null;
            }

            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue; // Skip empty lines

                string[] values = lines[i].Split(',');

                Dictionary<string, object> entry = new Dictionary<string, object>();
                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    entry[headers[j]] = ParseValue(values[j]);
                }

                dataList.Add(entry);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to read CSV: " + e.Message);
        }

        return dataList;
    }

    private static object ParseValue(string value)
    {
        value = value.Trim();

        if (float.TryParse(value, out float floatVal))
            return floatVal;
        if (int.TryParse(value, out int intVal))
            return intVal;

        return value; // Keep as string if not a number
    }
}
