using System.Collections.Generic;
using UnityEngine;

public class util_mesh : MonoBehaviour
{
    public static Mesh GenerateTunnel(BezierCurve curve, float radius, int numMajorPoints, int numMinorPoints)
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        
        Vector3[] centerPoints = curve.GetPointArray(numMajorPoints);

        Vector3 upDir = Vector3.Cross(centerPoints[2]-centerPoints[1],centerPoints[0]-centerPoints[1]).normalized;

        for (int i = 0; i < centerPoints.Length; i++)
        {
            Vector3 forwardDir;
            if (i < centerPoints.Length - 1)
            {
                forwardDir = centerPoints[i+1]-centerPoints[i];
            } else
            {
                forwardDir = centerPoints[i]-centerPoints[i-1];
            }
            forwardDir = forwardDir.normalized;

            // now we have both a forward and an up
            // so we make the right vector
            Vector3 rightDir = Vector3.Cross(forwardDir, upDir).normalized;

            // creating a loop around the center point
            
            // these points are centered around the origin
            Vector3[] loopPoints = util_geo.GenerateCirclePoints(upDir, rightDir, numMinorPoints, radius);
            // so we have to add to them as we set the vertices
            for (int j = 0; j < loopPoints.Length; j++)
            {
                verts.Add(loopPoints[j] + centerPoints[i]);
                norms.Add(loopPoints[j].normalized);
                uvs.Add(Vector2.one); // temp temp
            }
        }

        // TODO: triangles

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0,uvs);

        mesh.SetTriangles(tris,0);

        return mesh;
    }


    public static Mesh GeneratePlane(int width, float worldScale, bool isReversed)
    {
        Mesh planeMesh = new Mesh();

        Vector3[] verts = new Vector3[width * width];
        Vector2[] uvs = new Vector2[width * width];
        Vector3[] norms = new Vector3[width * width];

        int[] tris = new int[(width - 1) * (width - 1) * 6];

        float scaleFactor = worldScale / (width-1);

        int triangleIndex = 0;
        for (int x = 0, i = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++, i++)
            {
                verts[i] = new Vector3(x * scaleFactor - worldScale / 2, 0, y * scaleFactor - worldScale / 2);
                uvs[i] = new Vector2(x / width, y / width);

                norms[i] = Vector3.up;

                if (x > 0 && y > 0)
                {
                    if (!isReversed)
                    {
                        tris[triangleIndex] = i;
                        tris[triangleIndex + 1] = i - width - 1;
                        tris[triangleIndex + 2] = i - width;
                        tris[triangleIndex + 3] = i - 1;
                        tris[triangleIndex + 4] = i - width - 1;
                        tris[triangleIndex + 5] = i;
                    }
                    else
                    {
                        tris[triangleIndex] = i;
                        tris[triangleIndex + 1] = i - width;
                        tris[triangleIndex + 2] = i - width - 1;
                        tris[triangleIndex + 3] = i - 1;
                        tris[triangleIndex + 4] = i;
                        tris[triangleIndex + 5] = i - width - 1;
                    }

                    triangleIndex += 6;
                }
            }
        }

        planeMesh.SetVertices(verts);
        planeMesh.SetUVs(0, uvs);
        planeMesh.SetNormals(norms);

        planeMesh.SetTriangles(tris, 0);

        return planeMesh;
    }
}
