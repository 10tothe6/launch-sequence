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

    public static Vector3[] CopyVectors(Vector3[] input)
    {
        Vector3[] result = new Vector3[input.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = input[i];
        }

        return result;
    }
    public static Mesh GeneratePolygonMesh(Vector3[] points, Vector3 normalDirection, float uvScale)
    {
        Mesh result = new Mesh();

        Vector3[] verts = new Vector3[points.Length];
        Vector3[] norms = new Vector3[points.Length];
        Vector2[] uvs = new Vector2[points.Length];

        List<int> tris = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            verts[i] = points[i];
            norms[i] = normalDirection;
            uvs[i] = new Vector2(verts[i].x, verts[i].z) * uvScale;

            if (i < points.Length - 2)
            {
                tris.Add(0);
                tris.Add(i+1);
                tris.Add(i+2);
            }
        }

        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetUVs(0, uvs);
        result.SetTriangles(tris, 0);

        return result;
    }
    public static Mesh CombineMeshes(Mesh a, Mesh b) {
        Mesh combinedMesh = new Mesh();

        combinedMesh.SetVertices(CombineVector3Arrays(a.vertices, b.vertices));
        combinedMesh.SetNormals(CombineVector3Arrays(a.normals, b.normals));
        combinedMesh.SetUVs(0, CombineVector2Arrays(a.uv, b.uv));
        
        int[] triangles = CombineIntArrays(a.triangles, b.triangles);

        for (int i = a.triangles.Length; i < triangles.Length; i++) {
            triangles[i] += a.vertices.Length;
        }

        combinedMesh.SetTriangles(triangles, 0);

        return combinedMesh;
    }

    public static Vector2[] CombineVector2Arrays(Vector2[] first, Vector2[] second)
    {
        Vector2[] result = new Vector2[first.Length + second.Length];

        for (int i = 0; i < result.Length; i++)
        {
            if (i < first.Length)
            {
                result[i] = first[i];
            }
            else
            {
                result[i] = second[i - first.Length];
            }


        }

        return result;
    }

    public static Vector3[] CombineVector3Arrays(Vector3[] first, Vector3[] second) {
        Vector3[] result = new Vector3[first.Length + second.Length];

        for (int i = 0; i < result.Length; i++) {
            if (i < first.Length) {
                result[i] = first[i];
            }
            else {
                result[i] = second[i-first.Length];
            }

            
        }

        return result;
    }

    public static int[] CombineIntArrays(int[] first, int[] second) {
        int[] result = new int[first.Length + second.Length];

        for (int i = 0; i < result.Length; i++) {
            if (i < first.Length) {
                result[i] = first[i];
            }
            else {
                result[i] = second[i-first.Length];
            }

            
        }

        return result;
    }

    public static float DistanceToRect(Transform rect, Vector3 point)
    {
        Vector3 localPoint = rect.InverseTransformPoint(point);
        
        Vector3 clampedPoint = new Vector3(
            Mathf.Clamp(localPoint.x, -0.5f, 0.5f),
            Mathf.Clamp(localPoint.y, -0.5f, 0.5f),
            Mathf.Clamp(localPoint.z, -0.5f, 0.5f));

            Vector3 dist = point - rect.TransformPoint(clampedPoint);

        return new Vector3(dist.x, 0, dist.z).magnitude;
    }
    public static bool IsPointInsidePolygon(Vector3[] points, Vector3 point)
    {
        int leftCount = 0;
        int rightCount = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 toNext;
            Vector3 toPoint;
            if (i < points.Length-1)
            {
                toNext = points[i+1]-points[i];
                toNext = new Vector3(toNext.z, 0, -toNext.x);
                toPoint = point-points[i];
            } else
            {
                toNext = points[0]-points[i];
                toNext = new Vector3(toNext.z, 0, -toNext.x);
                toPoint = point-points[i];
            }

            if (Vector3.Dot(toNext, toPoint) < 0)
            {
                leftCount++;
            } else if (Vector3.Dot(toNext, toPoint) > 0) {rightCount++;}
        }

        if (leftCount == 0 || rightCount == 0)
        {
            return true;
        }
        return false;
    }

    public static float DistanceToPolygon(Vector3[] points1, Vector3 point1)
    {
        Vector3 point = new Vector3(point1.x, 0, point1.z);
        Vector3[] points = CopyVectors(points1);
        
        for (int i =0; i<points.Length; i++)
        {
            points[i] = points[i] - Vector3.up * points[i].y;
        }
        
        bool isInside = IsPointInsidePolygon(points, point);
        if (isInside) return 0;

        Vector3 vert = point;
        float dist = 999;
        for (int n = 0; n < points.Length; n++)
        {   
            Vector3 dir1;
            Vector3 dir2;

            if (n < points.Length - 1)
            {
                dir1 = vert - points[n];
                dir2 = points[n+1] - points[n];
            } else
            {
                dir1 = vert - points[n];
                dir2 = points[0] - points[n];
            }

            if (Vector3.Dot(dir1, dir2) > 0)
            {
                Vector3 projectedDir = Vector3.Project(dir1, dir2);
                projectedDir = projectedDir.normalized * Mathf.Min(dir2.magnitude, projectedDir.magnitude);

                Vector3 clampedPoint = points[n] + projectedDir;
                clampedPoint = new Vector3(clampedPoint.x, 0, clampedPoint.z);
                vert = new Vector3(vert.x, 0, vert.z);

                float distToLine = Vector3.Distance(clampedPoint, vert);
                if (distToLine < dist) dist = distToLine;
            }
        } 

        return dist;
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
