using UnityEngine;

public class ScatterplotPointData : MonoBehaviour
{
    public float xValue;
    public float yValue;
    public float zValue;

    // Function to set the data dynamically if needed
    public void SetPointData(float x, float y, float z)
    {
        xValue = x;
        yValue = y;
        zValue = z;
    }
}