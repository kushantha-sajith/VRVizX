using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;

public class selectDataset : MonoBehaviour
{
    public GameObject fileSelectionUI;
    public GameObject filePickerUI;

    public void Start()
    {
        filePickerUI.SetActive(false); // Hide the file picker UI at the start
    }

    public void OpenCSVFile()
    {
        // Hide the UI panel
        if (fileSelectionUI != null)
            fileSelectionUI.SetActive(false);
        if (filePickerUI != null)
            filePickerUI.SetActive(true);

        // Set file filter to show only .csv files
        FileBrowser.SetFilters(true, new FileBrowser.Filter("CSV Files", ".csv"));

        // Set default directory (e.g., Desktop)
        string defaultPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // Show file picker
        FileBrowser.ShowLoadDialog(
            onSuccess: (string[] paths) =>
            {
                if (paths.Length > 0)
                {
                    string selectedFilePath = paths[0];
                    // Ensure the file is a CSV by checking the extension
                    if (Path.GetExtension(selectedFilePath).ToLower() == ".csv")
                    {
                        Debug.Log("CSV File Selected: " + selectedFilePath);
                        string[] csvLines = File.ReadAllLines(selectedFilePath);

                        var processor = GetComponent<CSVProcessor>();
                        if (processor != null)
                            processor.ProcessCSV(selectedFilePath);
                    }
                    else
                    {
                        Debug.Log("Selected file is not a CSV file.");
                    }
                }
            },
            onCancel: () =>
            {
                Debug.Log("File picker canceled.");

                // Show UI again if canceled
                if (fileSelectionUI != null)
                    fileSelectionUI.SetActive(true);
            },
            FileBrowser.PickMode.Files,
            false,  // Don't allow directories
            defaultPath, null,
            "Select a CSV File"
        );
    }
}
