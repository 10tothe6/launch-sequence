using Unity.VisualScripting;
using UnityEngine;

// re-purposing the vast majority of this class from the original Drivetrain,
// so that we can have physics-based hanging lights among other things

// TODO: make more bouncy

public class phys_rope : MonoBehaviour
{
    private LineRenderer lr;
    [Header("CONFIG")]
    public bool useColor;
    public Color lineColor;
    public Material lineMaterial;
    
    public float lineWidth;

    [Space(8)]
    public bool generateOnAwake;
    // the prefab that the rope is generated from
    // this CAN INCLUDE the visual part (such as in a chain, with segments), but doesn't have to (actual ropes use line renderers)
    public GameObject p_segment;

    // how far between each segment's SNAPPING POINTS
    public float segmentSpacing;
    public float pointMass;
    public int segmentCount; // self-explanatory
    
    // for more rope-y ropes
    public bool useLineRenderer;

    // the front-end, so to speak
    private Transform[] segmentTransforms;

    // back-end structure of the rope
    public Point[] points;
    public Stick[] sticks;
    // more iterations means a smoother sim, but way laggier
    public int iterationsPerUpdate;

    void Awake() {
        lr = GetComponent<LineRenderer>();

        if(generateOnAwake)
        {
            Initialize(segmentCount);
        }
        
        if (useLineRenderer)
        {
            SetupLine();
        } else {lr.enabled = false;}
    }

    // spawning the rope, called once 
    public void Initialize(int segmentCount) {
        ui_canvasutils.DestroyChildren(gameObject);

        segmentTransforms = new Transform[segmentCount];

        // initialize the physical objects
        for (int i = 0; i < segmentCount; i++) {
            Transform t_segment;
            t_segment = Instantiate(p_segment, Vector3.zero, Quaternion.identity).transform;

            t_segment.SetParent(transform);
            t_segment.forward = Vector3.up;

            if (i == 0)
            {
                t_segment.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }

            // rotating some, only needed for chains so removed for now
            // if (i != 0) {
            //     if (i%2==0 && i < segmentCount-2) {
            //         t_segment.localEulerAngles += new Vector3(0, 90, 0);
            //     }
            // }

            // the first segment spawns such that its UPPER snap point is at the local origin
            if (i == 0) {
                t_segment.localPosition = -(t_segment.GetChild(0).position - t_segment.position);
            }
            else { // all subsequent ones spawn so that their snapping points are connected
                t_segment.position = segmentTransforms[i-1].GetChild(1).position - (t_segment.GetChild(0).position - t_segment.position);
            }
            
            segmentTransforms[i] = t_segment;
        }

        // now that we've created the transform objects, we create the backend structure
        points = new Point[segmentCount];
        for (int i = 0; i < segmentCount; i++) {
            points[i] = new Point(segmentTransforms[i].position, i==0);
            points[i].mass = pointMass;
        }

        // we use the distance as the stick length cuz its easier
        sticks = new Stick[segmentCount-1];
        for (int i = 0; i < segmentCount-1; i++) {
            sticks[i] = new Stick(points[i], points[i+1], (points[i].pos - points[i+1].pos).magnitude);
        }
    }

    // could also be done in update but using Time.deltaTime
    void Update() {
        UpdateRope();

        if (useLineRenderer)
        {
            UpdateLine();
        }
    }

    void SetupLine()
    {
        lr.enabled = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.positionCount = segmentCount;

        lr.material = lineMaterial;
        if (useColor)
        {
            lineMaterial.color = lineColor;
        }
    }

    void UpdateLine()
    {
        Vector3[] points = new Vector3[segmentCount];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.GetChild(i).position;
        }

        lr.SetPositions(points);
    }

    // handling the restrictions on the distances between segments
    void UpdateRope() {
        
        // forces on points are driven by their rigidbodies
        for (int i = 0; i < points.Length; i++) {
            points[i].pos = segmentTransforms[i].position;
        }

        // here is where the constraints happen
        for (int j = 0; j < iterationsPerUpdate; j++) {
            for (int i = 0; i < sticks.Length; i++) {
                Vector3 centre = (sticks[i].a.pos + sticks[i].b.pos)/2;
                Vector3 dir = (sticks[i].a.pos - sticks[i].b.pos).normalized;
                if (!sticks[i].a.isLocked)
                    sticks[i].a.pos = sticks[i].a.pos + (centre + dir * sticks[i].length / 2-sticks[i].a.pos) * sticks[i].a.mass;
                if (!sticks[i].b.isLocked)
                    sticks[i].b.pos = sticks[i].b.pos + (centre - dir * sticks[i].length / 2 - sticks[i].b.pos) * sticks[i].b.mass;
            }
        }

        // here we update the transforms based on the points
        for (int i = 0; i < segmentTransforms.Length; i++) {
            // "bounce"
            Vector3 v = segmentTransforms[i].GetComponent<Rigidbody>().linearVelocity;
            Vector3 upDown = Vector3.Project(segmentTransforms[i].GetComponent<Rigidbody>().linearVelocity, points[i].pos - segmentTransforms[i].position);
            Vector3 upDownFixed = (points[i].pos - segmentTransforms[i].position)*0.5f;

            segmentTransforms[i].GetComponent<Rigidbody>().linearVelocity = v - upDown*0.25f + upDownFixed;
            segmentTransforms[i].position = points[i].pos;
        }
    }
}

[System.Serializable]
public class Point {
    public Vector3 pos;
    public float mass;
    public bool isLocked;

    public Point(Vector3 _pos, bool _lock) {
        pos = _pos;
        isLocked = _lock;
    }
}

[System.Serializable]
public class Stick {
    public Point a, b;
    public float length;

    public Stick(Point _a, Point _b, float _length) {
        a = _a;
        b = _b; 
        length = _length;
    }
}