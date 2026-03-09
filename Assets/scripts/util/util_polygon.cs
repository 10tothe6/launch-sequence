using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// struct representing a piece of a polygon,
// so that we can keep track of vertex indices
[System.Serializable]
public class util_polygoncomponent
{
    
}

public class util_polygon
{
    // my own custom triangulation algorithm

    // * cut up the concave shape into a bunch of convex shapes
    // * triangulate all the convex shapes
    // * combine those triangulations into one final thing
    public static int[] GenerateConcaveTriangulation(Vector2[] verts)
    {
        // gotta convert to 3D for our little angle-detection trick
        Vector3[] verts3D = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            verts3D[i] = new Vector3(verts[i].x, 0, verts[i].y);
        }

        // to be filled with all of the convex pieces of our concave shape
        List<Vector2[]> convexSlices = new List<Vector2[]>();

        Vector3 normal = Vector3.up;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v1 = GetPreceedingVertex(verts3D, i);
            Vector3 v2 = verts[i];
            Vector3 v3 = GetSucceedingVertex(verts3D, i);

            Vector3 a = v1 - v2;
            Vector3 b = v3 - v2;

            float cAngle = GetClockwiseAngle(a, b, normal);

            if (cAngle >= 180)
            {
                Vector2 a2D = new Vector2(a.x, a.z);
                Vector2 b2D = new Vector2(b.x, b.z);

                Vector2 bisector = (a2D+b2D).normalized;
                // we have found our reflex angle and can now make a cut

                List<int>[] slices = CutPolygon(bisector, verts);
            }
        }


        // now we have the slices array filled
        List<int[]> sliceTriangulations = new List<int[]>();
        for (int i = 0; i < convexSlices.Count; i++)
        {
            sliceTriangulations.Add(GenerateConvexTriangulation(convexSlices[i]));
        }

        // temp
        return new int[0];
    }

    // just easier to package up this 2-case logic
    public static Vector2 GetPreceedingVertex(Vector2[] verts, int index)
    {
        if (index == 0)
        {
            return verts[verts.Length - 1];
        } else
        {
            return verts[index - 1];
        }
    }

    public static Vector2 GetSucceedingVertex(Vector2[] verts, int index)
    {
        if (index == verts.Length - 1)
        {
            return verts[0];
        } else
        {
            return verts[index + 1];
        }
    }

    public static Vector3 GetPreceedingVertex(Vector3[] verts, int index)
    {
        if (index == 0)
        {
            return verts[verts.Length - 1];
        } else
        {
            return verts[index - 1];
        }
    }

    public static Vector3 GetSucceedingVertex(Vector3[] verts, int index)
    {
        if (index == verts.Length - 1)
        {
            return verts[0];
        } else
        {
            return verts[index + 1];
        }
    }

    // returns a 2 by n array that represents the two pieces of the cut polygon

    // very useful for turning a concave polygon into 2 convex gons
    public static List<int>[] CutPolygon(Vector2 cutLine, Vector2[] verts)
    {
        // using the idea that taking the negative rociprocal of the slope of a 2D line yields the perpindicular slope
        Vector2 perpLine = new Vector2(cutLine.y, -cutLine.x);
        // there are 2 possible perp lines, which one this ends up being doesn't matter


        // two pieces, never more
        List<int>[] result = new List<int>[2];

        for (int i = 0; i < verts.Length; i++)
        {
            if (Vector2.Dot(perpLine, verts[i]) > 0)
            {
                result[0].Add(i);
            } else if (Vector2.Dot(perpLine, verts[i]) < 0)
            {
                result[1].Add(i);
            } else // lies right on the line, so 2 copies (bc we need the resulting copies to be complete polygons)
            {
                result[0].Add(i);
                result[1].Add(i);
            }
        }

        return result;
    }

    // just a triangle fan, technically my own idea but many people have used this
    
    // not sure whether this is clockwise or not?
    public static int[] GenerateConvexTriangulation(Vector2[] verts)
    {
        int[] tris = new int[(verts.Length - 2) * 3];

        for (int i = 0, n = 1; i < tris.Length; i+= 3, n++)
        {
            tris[i] = 0;
            tris[i+1] = n;
            tris[i+2] = n+1;
        }

        return tris;
    }

    // returns degrees
    public static float GetClockwiseAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        float rawAngle = Vector3.Angle(a, b);
        if (Vector3.Dot(Vector3.Cross(a, b), normal) > 0)
        {
            return rawAngle;
        } else
        {
            return 360 - rawAngle;
        }
    }

    // returns degrees
    public static float GetCounterClockwiseAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        float rawAngle = Vector3.Angle(a, b);
        if (Vector3.Dot(Vector3.Cross(a, b), normal) > 0)
        {
            return 360 - rawAngle;
        } else
        {
            return rawAngle;
        }
    }
}
