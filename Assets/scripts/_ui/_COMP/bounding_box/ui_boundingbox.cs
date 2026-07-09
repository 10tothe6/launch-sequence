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

    public Sprite borderSprite;

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

        int existingChildCount = transform.childCount-4;

        topEdge = transform.GetChild(0+existingChildCount).GetComponent<RectTransform>();
        bottomEdge = transform.GetChild(1+existingChildCount).GetComponent<RectTransform>();
        leftEdge = transform.GetChild(2+existingChildCount).GetComponent<RectTransform>();
        rightEdge = transform.GetChild(3+existingChildCount).GetComponent<RectTransform>();
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
        topEdge.sizeDelta = new Vector2(lineThickness, horizontalSize);
        topEdge.eulerAngles = new Vector3(topEdge.eulerAngles.x, topEdge.eulerAngles.y, 90);
        topEdge.GetComponent<Image>().color = col;
        topEdge.GetComponent<Image>().sprite = borderSprite;

        bottomEdge.position = lowerPos;
        bottomEdge.sizeDelta = new Vector2(lineThickness, horizontalSize);
        bottomEdge.eulerAngles = new Vector3(bottomEdge.eulerAngles.x, bottomEdge.eulerAngles.y, 90);
        bottomEdge.GetComponent<Image>().color = col;
        bottomEdge.GetComponent<Image>().sprite = borderSprite;

        rightEdge.position = rightPos;
        rightEdge.sizeDelta = new Vector2(lineThickness, verticalSize);
        rightEdge.eulerAngles = new Vector3(rightEdge.eulerAngles.x, rightEdge.eulerAngles.y, 0);
        rightEdge.GetComponent<Image>().color = col;
        rightEdge.GetComponent<Image>().sprite = borderSprite;


        leftEdge.position = leftPos;
        leftEdge.sizeDelta = new Vector2(lineThickness, verticalSize);
        leftEdge.eulerAngles = new Vector3(leftEdge.eulerAngles.x, leftEdge.eulerAngles.y, 0);
        leftEdge.GetComponent<Image>().color = col;
        leftEdge.GetComponent<Image>().sprite = borderSprite;
    }
}
