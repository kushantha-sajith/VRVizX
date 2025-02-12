using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script gets values from CSVReader script
// It instantiates points and particles according to values read

public class PointRenderer : MonoBehaviour
{

    private int column1;
    private int column2;
    private int column3;
    private int numberOfClusters;
    private string outliersBool;
    private string dataPointColor;
    private string outlierColorVal;

    // Method to set axis columns from UI
    public void SetAxisColumns(int xAxis, int yAxis, int zAxis, string outlier, int cluster, string color, string outlierColorValue, float dataPointScale)
    {
        column1 = xAxis;
        column2 = yAxis;
        column3 = zAxis;

        numberOfClusters = cluster;
        outliersBool = outlier;

        dataPointColor = color;
        outlierColorVal = outlierColorValue;
        pointScale = dataPointScale;

        Debug.Log("Axis columns are " + column1 + " " + column2 + " " + column3 + ", outlier: " + outliersBool + ", cluster: " + numberOfClusters + ", color: " + dataPointColor + ", outlierColor: " + outlierColorVal + ", point scale: " + pointScale);
    }

    //********Public Variables********

    // Bools for editor options
    public bool renderPointPrefabs = true;
    public bool renderParticles = true;
    public bool renderPrefabsWithColor = true;

    // Name of the input file, no extension
    public string inputfile;

    // Indices for columns to be assigned
    //public int column1 = 0;
    //public int column2 = 1;
    //public int column3 = 2;

    // Full column names from CSV (as Dictionary Keys)
    public string xColumnName;
    public string yColumnName;
    public string zColumnName;

    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
    private float plotScale = 10;

    // Scale of the prefab particlePoints
    [Range(0.0f, 0.5f)]
    public float pointScale = 0.25f;

    // Changes size of particles generated
    [Range(0.0f, 2.0f)]
    public float particleScale = 5.0f;

    // The prefab for the data particlePoints that will be instantiated
    public GameObject PointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    // Color for the glow around the particlePoints
    private Color GlowColor;

    // Cluster colors
    private List<Color> clusterColors;

    // To store cluster assignments of each point
    private int[] clusterAssignments;

    //********Private Variables********
    // Minimum and maximum values of columns
    private float xMin;
    private float yMin;
    private float zMin;

    private float xMax;
    private float yMax;
    private float zMax;

    // Number of rows
    private int rowCount;

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;

    // Particle system for holding point particles
    private ParticleSystem.Particle[] particlePoints;

    public Color outlierColor = Color.black;

    private List<int> outlierIndices = new List<int>();

    //********Methods********

    void Awake()
    {
        //Run CSV Reader
        pointList = CSVReader.Read(inputfile);
    }

    private void ClearScatterplot()
    {
        // Destroy all child objects of PointHolder
        foreach (Transform child in PointHolder.transform)
        {
            Destroy(child.gameObject);
        }

        // Clear particle system
        if (renderParticles)
        {
            GetComponent<ParticleSystem>().Clear();
        }
    }

    // Use this for initialization
    void Start()
    {
        ClearScatterplot();

        if(outliersBool == "Yes")
        {
            List<string> columnList = new List<string>(pointList[1].Keys);

            Debug.Log("There are " + columnList.Count + " columns in the CSV");

            foreach (string key in columnList)
                Debug.Log("Column name is " + key);

            xColumnName = columnList[column1];
            yColumnName = columnList[column2];
            zColumnName = columnList[column3];

            Debug.Log("Axis names are " + xColumnName + " " + yColumnName + " " + zColumnName);

            xMax = FindMaxValue(xColumnName);
            yMax = FindMaxValue(yColumnName);
            zMax = FindMaxValue(zColumnName);

            xMin = FindMinValue(xColumnName);
            yMin = FindMinValue(yColumnName);
            zMin = FindMinValue(zColumnName);

            // Detect outliers using IQR method
            DetectOutliers();

            AssignLabels();

            if (renderPointPrefabs == true)
            {
                PlacePrefabPointsOutliers();
            }

            if (renderParticles == true)
            {
                CreateParticlesOutliers();
                GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
            }
        } 
        else
        {
            if (numberOfClusters > 1)
            {
                List<string> columnList = new List<string>(pointList[1].Keys);

                xColumnName = columnList[column1];
                yColumnName = columnList[column2];
                zColumnName = columnList[column3];

                Debug.Log("Column names are " + xColumnName + " " + yColumnName + " " + zColumnName);

                xMax = FindMaxValue(xColumnName);
                yMax = FindMaxValue(yColumnName);
                zMax = FindMaxValue(zColumnName);

                xMin = FindMinValue(xColumnName);
                yMin = FindMinValue(yColumnName);
                zMin = FindMinValue(zColumnName);

                AssignLabels();

                // Perform k-means clustering
                clusterAssignments = PerformKMeansClustering(numberOfClusters);
                InitializeClusterColors(numberOfClusters);

                if (renderPointPrefabs)
                    PlacePrefabPointsCluster();

                if (renderParticles)
                {
                    CreateParticles();
                    GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
                }
            }
            else
            {
                // Store dictionary keys (column names in CSV) in a list
                List<string> columnList = new List<string>(pointList[1].Keys);

                Debug.Log("There are " + columnList.Count + " columns in the CSV");

                foreach (string key in columnList)
                    Debug.Log("Column name is " + key);

                // Assign column names according to index indicated in columnList
                xColumnName = columnList[column1];
                yColumnName = columnList[column2];
                zColumnName = columnList[column3];

                Debug.Log("Axis names are " + xColumnName + " " + yColumnName + " " + zColumnName);

                // Get maxes of each axis, using FindMaxValue method defined below
                xMax = FindMaxValue(xColumnName);
                yMax = FindMaxValue(yColumnName);
                zMax = FindMaxValue(zColumnName);

                // Get minimums of each axis, using FindMinValue method defined below
                xMin = FindMinValue(xColumnName);
                yMin = FindMinValue(yColumnName);
                zMin = FindMinValue(zColumnName);

                // Debug.Log(xMin + " " + yMin + " " + zMin); // Write to console

                AssignLabels();

                if (renderPointPrefabs == true)
                {
                    // Call PlacePoint methods defined below
                    PlacePrefabPoints();
                }

                // If statement to turn particles on and off
                if (renderParticles == true)
                {
                    // Call CreateParticles() for particle system
                    CreateParticles();

                    // Set particle system, for point glow- depends on CreateParticles()
                    GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Activate Particle System
        //GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
    }

    // Places the prefabs according to values read in
    private void PlacePrefabPoints()
    {
        // Get count (number of rows in table)
        rowCount = pointList.Count;

        for (var i = 0; i < pointList.Count; i++)
        {
            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);

            // Create vector 3 for positioning particlePoints
            Vector3 position = new Vector3(x, y, z) * plotScale;

            //instantiate as gameobject variable so that it can be manipulated within loop
            GameObject dataPoint = Instantiate(PointPrefab, Vector3.zero, Quaternion.identity);
            ScatterplotPointData pointData = dataPoint.GetComponent<ScatterplotPointData>();
            if (pointData != null)
            {
                float a = Convert.ToSingle(pointList[i][xColumnName]);
                float b = Convert.ToSingle(pointList[i][yColumnName]);
                float c = Convert.ToSingle(pointList[i][zColumnName]);
                pointData.SetPointData(a, b, c);
            }


            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position point at relative to parent
            dataPoint.transform.localPosition = position;

            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            // Converts index to string to name the point the index number
            string dataPointName = i.ToString();

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;

            if (renderPrefabsWithColor == true)
            {
                // Sets color according to x/y/z value
                dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);

                // Activate emission color keyword so we can modify emission color
                dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

                dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));

            }
        }
    }

    // creates particlePoints in the Particle System game object
    private void CreateParticles()
    {
        //pointList = CSVReader.Read(inputfile);

        rowCount = pointList.Count;
        // Debug.Log("Row Count is " + rowCount);

        particlePoints = new ParticleSystem.Particle[rowCount];

        for (int i = 0; i < pointList.Count; i++)
        {
            // Convert object from list into float
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);

            // Debug.Log("Position is " + x + y + z);

            // Set point location
            particlePoints[i].position = new Vector3(x, y, z) * plotScale;

            //GlowColor = 
            // Set point color
            particlePoints[i].startColor = new Color(x, y, z, 1.0f);
            particlePoints[i].startSize = particleScale;
        }

    }

    // Finds labels named in scene, assigns values to their text meshes
    // WARNING: game objects need to be named within scene
    private void AssignLabels()
    {
        // Update point counter
        GameObject.Find("Point_Count").GetComponent<TextMesh>().text = pointList.Count.ToString("0");

        // Update title according to inputfile name
        GameObject.Find("Dataset_Label").GetComponent<TextMesh>().text = inputfile;

        // Update axis titles to ColumnNames
        GameObject.Find("X_Title").GetComponent<TextMesh>().text = xColumnName;
        GameObject.Find("Y_Title").GetComponent<TextMesh>().text = yColumnName;
        GameObject.Find("Z_Title").GetComponent<TextMesh>().text = zColumnName;

        // Set x Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("X_Min_Lab").GetComponent<TextMesh>().text = xMin.ToString("0.0");
        GameObject.Find("X_Mid_Lab").GetComponent<TextMesh>().text = (xMin + (xMax - xMin) / 2f).ToString("0.0");
        GameObject.Find("X_Max_Lab").GetComponent<TextMesh>().text = xMax.ToString("0.0");

        // Set y Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("Y_Min_Lab").GetComponent<TextMesh>().text = yMin.ToString("0.0");
        GameObject.Find("Y_Mid_Lab").GetComponent<TextMesh>().text = (yMin + (yMax - yMin) / 2f).ToString("0.0");
        GameObject.Find("Y_Max_Lab").GetComponent<TextMesh>().text = yMax.ToString("0.0");

        // Set z Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("Z_Min_Lab").GetComponent<TextMesh>().text = zMin.ToString("0.0");
        GameObject.Find("Z_Mid_Lab").GetComponent<TextMesh>().text = (zMin + (zMax - zMin) / 2f).ToString("0.0");
        GameObject.Find("Z_Max_Lab").GetComponent<TextMesh>().text = zMax.ToString("0.0");

    }

    //Method for finding max value, assumes PointList is generated
    private float FindMaxValue(string columnName)
    {
        //set initial value to first value
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        //Spit out the max value
        return maxValue;
    }

    //Method for finding minimum value, assumes PointList is generated
    private float FindMinValue(string columnName)
    {
        //set initial value to first value
        float minValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }

    //********K-Means Clustering********
    // Initializes unique colors for each cluster
    private void InitializeClusterColors(int clusters)
    {
        clusterColors = new List<Color>();
        for (int i = 0; i < clusters; i++)
        {
            clusterColors.Add(new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f));
        }
    }

    private int[] PerformKMeansClustering(int k)
    {
        int[] assignments = new int[pointList.Count];
        Vector3[] centroids = new Vector3[k];

        // Randomly initialize centroids
        for (int i = 0; i < k; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pointList.Count);
            float x = (Convert.ToSingle(pointList[randomIndex][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[randomIndex][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[randomIndex][zColumnName]) - zMin) / (zMax - zMin);
            centroids[i] = new Vector3(x, y, z);
        }

        bool changed;
        do
        {
            changed = false;

            // Assign each point to the nearest centroid
            for (int i = 0; i < pointList.Count; i++)
            {
                float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
                float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
                float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);
                Vector3 point = new Vector3(x, y, z);

                int nearestCentroid = 0;
                float minDistance = Vector3.Distance(point, centroids[0]);

                for (int j = 1; j < k; j++)
                {
                    float distance = Vector3.Distance(point, centroids[j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestCentroid = j;
                    }
                }

                if (assignments[i] != nearestCentroid)
                {
                    changed = true;
                    assignments[i] = nearestCentroid;
                }
            }

            // Recalculate centroids
            for (int j = 0; j < k; j++)
            {
                Vector3 sum = Vector3.zero;
                int count = 0;
                for (int i = 0; i < pointList.Count; i++)
                {
                    if (assignments[i] == j)
                    {
                        float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
                        float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
                        float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);
                        sum += new Vector3(x, y, z);
                        count++;
                    }
                }
                if (count > 0)
                    centroids[j] = sum / count;
            }
        } while (changed);

        return assignments;
    }

    private void PlacePrefabPointsCluster()
    {
        rowCount = pointList.Count;

        for (int i = 0; i < pointList.Count; i++)
        {
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);

            Vector3 position = new Vector3(x, y, z) * plotScale;

            GameObject dataPoint = Instantiate(PointPrefab, Vector3.zero, Quaternion.identity);
            ScatterplotPointData pointData = dataPoint.GetComponent<ScatterplotPointData>();
            if (pointData != null)
            {
                float a = Convert.ToSingle(pointList[i][xColumnName]);
                float b = Convert.ToSingle(pointList[i][yColumnName]);
                float c = Convert.ToSingle(pointList[i][zColumnName]);
                pointData.SetPointData(a, b, c);
            }

            dataPoint.transform.parent = PointHolder.transform;
            dataPoint.transform.localPosition = position;
            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            // Color based on cluster assignment
            int cluster = clusterAssignments[i];
            dataPoint.GetComponent<Renderer>().material.color = clusterColors[cluster];
        }
    }

    //***********Outliers***********
    private void DetectOutliers()
    {
        outlierIndices.Clear();

        // Calculate IQR for each axis
        float xIQR = CalculateIQR(xColumnName);
        float yIQR = CalculateIQR(yColumnName);
        float zIQR = CalculateIQR(zColumnName);

        // Define outlier bounds
        float xLowerBound = FindQ1(xColumnName) - 1.5f * xIQR;
        float xUpperBound = FindQ3(xColumnName) + 1.5f * xIQR;
        float yLowerBound = FindQ1(yColumnName) - 1.5f * yIQR;
        float yUpperBound = FindQ3(yColumnName) + 1.5f * yIQR;
        float zLowerBound = FindQ1(zColumnName) - 1.5f * zIQR;
        float zUpperBound = FindQ3(zColumnName) + 1.5f * zIQR;

        // Identify outliers
        for (int i = 0; i < pointList.Count; i++)
        {
            float x = Convert.ToSingle(pointList[i][xColumnName]);
            float y = Convert.ToSingle(pointList[i][yColumnName]);
            float z = Convert.ToSingle(pointList[i][zColumnName]);

            if (x < xLowerBound || x > xUpperBound || y < yLowerBound || y > yUpperBound || z < zLowerBound || z > zUpperBound)
            {
                outlierIndices.Add(i);
            }
        }

        Debug.Log("Number of outliers detected: " + outlierIndices.Count);
    }

    // Helper method to calculate IQR
    private float CalculateIQR(string columnName)
    {
        float q1 = FindQ1(columnName);
        float q3 = FindQ3(columnName);
        return q3 - q1;
    }

    // Helper method to find Q1 (25th percentile)
    private float FindQ1(string columnName)
    {
        List<float> values = new List<float>();
        for (int i = 0; i < pointList.Count; i++)
        {
            values.Add(Convert.ToSingle(pointList[i][columnName]));
        }
        values.Sort();
        int index = (int)Mathf.Ceil(0.25f * values.Count) - 1;
        return values[index];
    }

    // Helper method to find Q3 (75th percentile)
    private float FindQ3(string columnName)
    {
        List<float> values = new List<float>();
        for (int i = 0; i < pointList.Count; i++)
        {
            values.Add(Convert.ToSingle(pointList[i][columnName]));
        }
        values.Sort();
        int index = (int)Mathf.Ceil(0.75f * values.Count) - 1;
        return values[index];
    }

    private void PlacePrefabPointsOutliers()
    {
        rowCount = pointList.Count;

        for (var i = 0; i < pointList.Count; i++)
        {
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);

            Vector3 position = new Vector3(x, y, z) * plotScale;

            GameObject dataPoint = Instantiate(PointPrefab, Vector3.zero, Quaternion.identity);
            ScatterplotPointData pointData = dataPoint.GetComponent<ScatterplotPointData>();
            if (pointData != null)
            {
                float a = Convert.ToSingle(pointList[i][xColumnName]);
                float b = Convert.ToSingle(pointList[i][yColumnName]);
                float c = Convert.ToSingle(pointList[i][zColumnName]);
                pointData.SetPointData(a, b, c);
            }

            dataPoint.transform.parent = PointHolder.transform;
            dataPoint.transform.localPosition = position;
            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            string dataPointName = i.ToString();
            dataPoint.transform.name = dataPointName;

            if (renderPrefabsWithColor == true)
            {
                // Check if the point is an outlier
                if (outlierIndices.Contains(i))
                {
                    dataPoint.GetComponent<Renderer>().material.color = outlierColor;
                    dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", outlierColor);
                }
                else
                {
                    dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);
                    dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));
                }
            }
        }
    }

    private void CreateParticlesOutliers()
    {
        rowCount = pointList.Count;
        particlePoints = new ParticleSystem.Particle[rowCount];

        for (int i = 0; i < pointList.Count; i++)
        {
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);

            particlePoints[i].position = new Vector3(x, y, z) * plotScale;

            // Check if the point is an outlier
            if (outlierIndices.Contains(i))
            {
                particlePoints[i].startColor = outlierColor;
            }
            else
            {
                particlePoints[i].startColor = new Color(x, y, z, 1.0f);
            }

            particlePoints[i].startSize = particleScale;
        }
    }
}