using UnityEngine;

public class test_polygonarea : MonoBehaviour
{
    public Mesh m;

    void Start()
    {
        Debug.Log(util_polygon.CalculateArea(m));
    }
}
