using JetBrains.Annotations;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// calculates the bounds of a mesh (relative to the camera)
// and feeds them to a ui_boundingbox component

public class ui_boundsfrommesh : MonoBehaviour
{
    public MeshRenderer sourceMesh;
    public ui_boundingbox display;

    public Camera cam;

    public bool updatePeriodic;

    // to make it look more like the product of computer vision
    // just adds some jitter to the results
    public bool glitchyEffect; 
    public float jitterAmt;

    public bool delayEffect;
    public float timeBetweenUpdates;
    private float lastUpdateTime;

    void Update()
    {
        if (updatePeriodic)
        {
            if (delayEffect)
            {
                if (Time.time < lastUpdateTime + timeBetweenUpdates)
                {
                    return;
                }
            }
            // otherwise just run it
            lastUpdateTime = Time.time;

            Bounds b = sourceMesh.bounds;

            // first, get the AABB of the mesh
            Vector3[] points = new Vector3[8];

            points[0] = b.center + Vector3.right * b.extents.x + Vector3.forward * b.extents.z - Vector3.up * b.extents.y;
            points[1] = b.center - Vector3.right * b.extents.x + Vector3.forward * b.extents.z - Vector3.up * b.extents.y;
            points[2] = b.center + Vector3.right * b.extents.x - Vector3.forward * b.extents.z - Vector3.up * b.extents.y;
            points[3] = b.center - Vector3.right * b.extents.x - Vector3.forward * b.extents.z - Vector3.up * b.extents.y;


            points[4] = b.center + Vector3.right * b.extents.x + Vector3.forward * b.extents.z + Vector3.up * b.extents.y;
            points[5] = b.center - Vector3.right * b.extents.x + Vector3.forward * b.extents.z + Vector3.up * b.extents.y;
            points[6] = b.center + Vector3.right * b.extents.x - Vector3.forward * b.extents.z + Vector3.up * b.extents.y;
            points[7] = b.center - Vector3.right * b.extents.x - Vector3.forward * b.extents.z + Vector3.up * b.extents.y;

            // now convert to screen
            Vector3[] screenPoints = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                screenPoints[i] = cam.WorldToScreenPoint(points[i]);
            }

            // now find the max and min x and y
            float maxX = 0;
            float minX = 0;
            float maxY = 0;
            float minY = 0;

            for (int i = 0; i < screenPoints.Length; i++)
            {
                if (screenPoints[i].x > maxX|| maxX == 0)
                {
                    maxX = screenPoints[i].x;
                }
                if (screenPoints[i].x < minX|| minX == 0)
                {
                    minX = screenPoints[i].x;
                }


                if (screenPoints[i].y > maxY || maxY == 0)
                {
                    maxY = screenPoints[i].y;
                }
                if (screenPoints[i].y < minY || minY == 0)
                {
                    minY = screenPoints[i].y;
                }
            }

            if (glitchyEffect)
            {
                maxX += Random.Range(-jitterAmt,jitterAmt);
                minX += Random.Range(-jitterAmt,jitterAmt);
                maxY += Random.Range(-jitterAmt,jitterAmt);
                minY += Random.Range(-jitterAmt,jitterAmt);
            }

            // now assemble the bounds of our rect and pass it off
            rectbounds bounds = new rectbounds();
            bounds.center = new Vector3((maxX + minX) / 2f, (maxY + minY) / 2f, 0f);
            bounds.extents = new Vector3((maxX-minX)/2f, (maxY-minY)/2f, 0f);

            display.SetBounds(bounds);
        }
    }
}
