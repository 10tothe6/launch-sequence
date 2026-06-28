using UnityEngine.UI;
using UnityEngine;

// HAS TO BE AXIS ALIGNED, FOR NOW AT LEAST

public class ui_boundingbox : MonoBehaviour
{
    [Header("CONSOLE")]
    public bool drawVertices;
    public float lineThickness;
    public Color col;
    public bool updatePeriodic;
    [Header("DATA")]
    public rectbounds bounds;

    // shortcut references to the edges
    [HideInInspector]
    public RectTransform leftEdge;
    [HideInInspector]
    public RectTransform rightEdge;
    [HideInInspector]
    public RectTransform topEdge;
    [HideInInspector]
    public RectTransform bottomEdge;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        // creating gameObjects for the 4 edges
        for (int i = 0; i < 4; i++)
        {
            GameObject g_newEdge = new GameObject();
            g_newEdge.transform.SetParent(transform);

            g_newEdge.AddComponent<Image>();
            // keep the sprite as none
        }

        topEdge = transform.GetChild(0).GetComponent<RectTransform>();
        bottomEdge = transform.GetChild(1).GetComponent<RectTransform>();
        leftEdge = transform.GetChild(2).GetComponent<RectTransform>();
        rightEdge = transform.GetChild(3).GetComponent<RectTransform>();
    }   

    void Update()
    {
        if (updatePeriodic)
        {
            UpdateFromBounds();
        }
    }

    public void SetBounds(rectbounds newBounds)
    {
        bounds = newBounds;
        UpdateFromBounds();
    }

    public void UpdateFromBounds()
    {
        // we're using the x and y extents here, z is ignored
        Vector3 upperPos = bounds.center + Vector3.up * bounds.extents.y;
        Vector3 lowerPos = bounds.center - Vector3.up * bounds.extents.y;

        Vector3 rightPos = bounds.center + Vector3.right * bounds.extents.x;
        Vector3 leftPos = bounds.center - Vector3.right * bounds.extents.x;

        float horizontalSize = Vector3.Distance(leftPos, rightPos);
        float verticalSize = Vector3.Distance(upperPos, lowerPos);

        topEdge.position = upperPos;
        topEdge.sizeDelta = new Vector2(horizontalSize, lineThickness);
        topEdge.GetComponent<Image>().color = col;

        bottomEdge.position = lowerPos;
        bottomEdge.sizeDelta = new Vector2(horizontalSize, lineThickness);
        bottomEdge.GetComponent<Image>().color = col;

        rightEdge.position = rightPos;
        rightEdge.sizeDelta = new Vector2(lineThickness, verticalSize);
        rightEdge.GetComponent<Image>().color = col;


        leftEdge.position = leftPos;
        leftEdge.sizeDelta = new Vector2(lineThickness, verticalSize);
        leftEdge.GetComponent<Image>().color = col;
    }
}
