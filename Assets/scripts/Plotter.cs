using UnityEngine;
using UnityEngine.TextCore.Text;

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

    public bool adjustWidth;
    public Transform focusPoint;

    public bool colorGradient;

    void Awake()
    {
        if (lr == null && GetComponent<LineRenderer>() != null)
        {
            lr = GetComponent<LineRenderer>();
        }
    }

    void Update()
    {
        if (adjustWidth)
        {
            lr.startWidth = lineWidth * Vector3.Distance(CameraController.t_cam.position, focusPoint.position);
            lr.endWidth = lineWidth * Vector3.Distance(CameraController.t_cam.position, focusPoint.position);
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
            if (colorGradient)
            {
                Gradient g = new Gradient();
                g.SetColorKeys(new GradientColorKey[]{new GradientColorKey(lineColor, 0f),new GradientColorKey(Color.black, 1f)});
                lr.colorGradient = g;
            } else
            {
                Gradient g = new Gradient();
                g.SetColorKeys(new GradientColorKey[]{new GradientColorKey(lineColor, 0f),new GradientColorKey(Color.black, 1f)});
                lr.colorGradient = g;
            }
        }
    }
}
