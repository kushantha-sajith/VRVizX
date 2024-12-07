using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script now clusters data points using k-means and visualizes clusters with different colors
public class PointRendererKMeans : MonoBehaviour
{
    private int column1;
    private int column2;
    private int column3;
    public int numberOfClusters; // Number of clusters for k-means

    // Cluster colors
    private List<Color> clusterColors;

    // To store cluster assignments of each point
    private int[] clusterAssignments;

    // Method to set axis columns from UI
    public void SetAxisColumns(int xAxis, int yAxis, int zAxis)
    {
        column1 = xAxis;
        column2 = yAxis;
        column3 = zAxis;

        Debug.Log("Axis columns are " + column1 + " " + column2 + " " + column3);
    }

    // Public Variables
    public bool renderPointPrefabs = true;
    public bool renderParticles = true;
    public bool renderPrefabsWithColor = true;

    public string inputfile;

    public string xColumnName;
    public string yColumnName;
    public string zColumnName;

    private float plotScale = 10;

    [Range(0.0f, 0.5f)]
    public float pointScale = 0.25f;

    [Range(0.0f, 2.0f)]
    public float particleScale = 5.0f;

    public GameObject PointPrefab;
    public GameObject PointHolder;

    private float xMin, yMin, zMin, xMax, yMax, zMax;
    private int rowCount;
    private List<Dictionary<string, object>> pointList;
    private ParticleSystem.Particle[] particlePoints;

    void Awake()
    {
        // Run CSV Reader
        pointList = CSVReader.Read(inputfile);
    }

    void Start()
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
            PlacePrefabPoints();

        if (renderParticles)
        {
            CreateParticles();
            GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
        }
    }

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

    private void PlacePrefabPoints()
    {
        rowCount = pointList.Count;

        for (int i = 0; i < pointList.Count; i++)
        {
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);

            Vector3 position = new Vector3(x, y, z) * plotScale;

            GameObject dataPoint = Instantiate(PointPrefab, Vector3.zero, Quaternion.identity);
            dataPoint.transform.parent = PointHolder.transform;
            dataPoint.transform.localPosition = position;
            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            // Color based on cluster assignment
            int cluster = clusterAssignments[i];
            dataPoint.GetComponent<Renderer>().material.color = clusterColors[cluster];
        }
    }

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

}

