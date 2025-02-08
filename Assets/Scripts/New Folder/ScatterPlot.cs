using UnityEngine;

public class ScatterPlot : MonoBehaviour
{
    public GameObject pointPrefab;
    public int numPoints = 100;
    public float scale = 1f;

    void Start()
    {
        for (int i = 0; i < numPoints; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * scale;
            GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
            point.transform.parent = this.transform;
        }
    }
}