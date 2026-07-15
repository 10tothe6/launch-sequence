using System.Collections.Generic;
using UnityEngine;

public class util_mesh : MonoBehaviour
{
    void Awake()
    {
        cube = ins_cube;
        sphere = ins_sphere;

        m_unlit = ins_m_unlit;
    }

    public Mesh ins_cube;
    public static Mesh cube ;

    public Mesh ins_sphere;
    public static Mesh sphere ;

    public Material ins_m_unlit;
    public static Material m_unlit;

    public static Vector3[] CopyVectors(Vector3[] input)
    {
        Vector3[] result = new Vector3[input.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = input[i];
        }

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
}
