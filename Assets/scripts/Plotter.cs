using UnityEngine;

// mostly used to draw orbit lines in the map menu, but really could be used for anything

public class Plotter : MonoBehaviour
{
    public LineRenderer lr;

    [Header("CONFIG")]
    public bool isShowing;
    public Material m_line;

    public bool useColor;
    public Color lineColor;
    
    public float lineWidth;

    void Awake()
    {
        if (lr == null && GetComponent<LineRenderer>() != null)
        {
            lr = GetComponent<LineRenderer>();
        }
    }

    public void Plot(Vector3[] points)
    {
        lr.positionCount = points.Length;
        lr.SetPositions(points);

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        

        lr.material = m_line;

        if (useColor)
        {
            m_line.color = lineColor;
        }
    }
}
