using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: use newtons method to have a scan layer right at the top, cuz thats smarter than just giving up

public class fluid_volume : MonoBehaviour
{
    float epsilon = 0.01f;

    public Transform t_model;
    public MeshFilter mf;
    public MeshRenderer mr;

    [Header("CONFIG")]
    public bool showScanPoints;

    public int maxVolumeSlices;
    public float volumeSliceSpacing;
    public float sliceUVScaleMultiplier;

    // ***
    public float volumeAmt; // current amount of fluid
    // ***
    public float maxVolume; // possible amount of fluid
    public List<float> volumeAreas;
    public List<float> stagedVolumeAreas;
    public List<Mesh> volumeSlices; // some extra data involved in storing it this way, but its convinient enough

    public bool showBasePoint;
    
    private Vector3 basePoint; // the bottom of the pool

    public List<Vector3> scanImpactPoints;

    public void Spawn(Vector3 spawnPosition)
    {
        basePoint = GetMinimumPoint(spawnPosition);

        RescanVolume();
        RecalculateVolume();
    }

    void Update()
    {
        // the code that actually draws the fluid

        // first, we figure out the fluid height
        float height = GetFluidHeight();

        t_model.position = basePoint + Vector3.up * height;
        mf.sharedMesh = GetAppropriateMesh();
    }

    public Mesh GetAppropriateMesh()
    {
        int min = 0;
        for (int i = 0; i < stagedVolumeAreas.Count-1;i++)
        {
            if (stagedVolumeAreas[i] > volumeAmt)
            {
                break;
            } else
            {
                min = i;
            }
        }

        float floor = stagedVolumeAreas[min];
        float lerpPercent = (volumeAmt - floor) / (stagedVolumeAreas[min+1] - floor);

        if (lerpPercent > 0.5f)
        {
            return volumeSlices[min + 1];
        } else
        {
            return volumeSlices[min];
        }
    }

    // sort of a cv from above but eh whatever
    public float GetFluidHeight()
    {
        int min = -1;
        for (int i = 0; i < stagedVolumeAreas.Count-1;i++)
        {
            if (stagedVolumeAreas[i] > volumeAmt)
            {
                break;
            } else
            {
                min = i;
            }
        }

        float floor = 0;
        if (min != -1)
        {
            floor = stagedVolumeAreas[min];
        }
        float lerpPercent = (volumeAmt - floor) / (stagedVolumeAreas[min+1] - floor);

        return (min+1) * volumeSliceSpacing + lerpPercent * volumeSliceSpacing;
    }
    
    void RecalculateVolume()
    {
        stagedVolumeAreas = new List<float>();

        // converting all the 2D areas to 3D areas
        maxVolume = 0;
        for (int i = 0; i < volumeSlices.Count; i++)
        {
            maxVolume += volumeAreas[i] * volumeSliceSpacing;
            stagedVolumeAreas.Add(maxVolume);
        }

        Debug.Log("New volume calculated to be " + maxVolume + " m^3.");
    }

    void RescanVolume()
    {
        scanImpactPoints = new List<Vector3>();

        for (int i = 0; i < maxVolumeSlices; i++)
        {
            Vector3 midPoint = basePoint + Vector3.up * volumeSliceSpacing * (i+1);
            List<Vector3> verts = new List<Vector3>();


            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            bool keepGoing = true;
            bool hasFailed = false;

            int safeIterations = 10;
            int iterationCount = 0;

            Vector3 currentPoint = midPoint;
            Vector3 currentDirection = Vector3.right;
            while(keepGoing && iterationCount < safeIterations)
            {
                iterationCount++;

                // the actual tracing part
                RaycastHit hit;
                // vector3.right is arbitrary
                if (Physics.Raycast(currentPoint, currentDirection, out hit, 25f))
                {
                    bool okToAdd = true;
                    for (int j = 0; j < verts.Count; j++)
                    {
                        if (Vector3.Distance(verts[j], hit.point) < 0.1f)
                        {
                            keepGoing = false;
                            okToAdd = false;
                        }
                    }

                    if (okToAdd)
                    {
                        // we add the impact point
                        verts.Add(hit.point);
                        currentPoint = hit.point - currentDirection.normalized * epsilon;
                        
                        currentDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
                    }
                } else
                {
                    // we missed, so no sense in trying again
                    keepGoing = false;
                    hasFailed = true;
                }
            }

            verts = util_polygon.RemoveCollinears(verts.ToArray()).ToList(); // one little performance loss for a gain later ig
            for (int v = 0; v < verts.Count; v++)
            {
                // for debugging
                scanImpactPoints.Add(verts[v]);

                // ADD SUPPORT FOR CHANGING UP VECTOR YOU ASS
                verts[v] = new Vector3(verts[v].x, 0, verts[v].z);
            }

            int[] triangles = util_polygon.GenerateConcaveTriangulation(util_polygon.Vector3ToVector2(verts.ToArray()));

            Mesh newSlice = new Mesh();
            newSlice.SetVertices(verts.ToArray());
            newSlice.SetNormals(norms.ToArray());
            newSlice.SetUVs(0,uvs.ToArray());
            newSlice.SetTriangles(triangles, 0);

            if (hasFailed)
            {
                Debug.Log("Volume scan has failed!");
            }
            else
            {
                volumeSlices.Add(newSlice);
                volumeAreas.Add(util_polygon.CalculateArea(newSlice));
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (showBasePoint)
        {
            Gizmos.DrawSphere(basePoint, 0.05f);
        }

        if (showScanPoints && scanImpactPoints != null)
        {
            for (int i = 0; i < scanImpactPoints.Count; i++)
            {
                Gizmos.DrawSphere(scanImpactPoints[i], 0.05f);
            }
        }
    }

    public Vector3 GetMinimumPoint(Vector3 startPoint)
    {
        int interationCount = 0;
        int safeIterations = 10;
        bool hasFailed = false;

        Vector3 currentPoint = startPoint;
        Vector3 currentDirection = -Vector3.up;

        while (interationCount < safeIterations)
        {
            interationCount++;

            RaycastHit hit;
            if (Physics.Raycast(currentPoint, currentDirection, out hit, 10f))
            {
                Vector3 oldDirection = currentDirection;
                currentPoint = hit.point - currentDirection.normalized * epsilon;
                
                Vector3 cross = Vector3.Cross(currentDirection, hit.normal);
                currentDirection = Vector3.Cross(hit.normal, cross).normalized;

                if (Vector3.Angle(hit.normal, oldDirection) > 180 - epsilon)
                {
                    break;
                }
            } else
            {
                hasFailed = true;
                break;
            }
        }
        //if (interationCount >= safeIterations) { hasFailed = true; }

        if (hasFailed)
        {
            Debug.Log("Flow simulation failed!");
        }

        return currentPoint;
    }
}
