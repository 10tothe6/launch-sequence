using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// struct representing a piece of a polygon,
// so that we can keep track of vertex indices

// (UNUSED RIGHT NOW)
[System.Serializable]
public class util_polygoncomponent
{
    public Vector2[] vertices;
    public Vector2[] actualVertexIndices;

    public util_polygoncomponent() {}

    public util_polygoncomponent(Vector2[] vertices, Vector2[] actualVertexIndices)
    {
        this.vertices = vertices;
        this.actualVertexIndices = actualVertexIndices;
    }
}

public class util_polygon
{
    // my own custom triangulation algorithm

    // * cut up the concave shape into a bunch of convex shapes
    // * triangulate all the convex shapes
    // * combine those triangulations into one final thing

    // it was painful, but this is non-recursive (as in it doesn't call itself)
    public static int[] GenerateConcaveTriangulation(Vector2[] verts)
    {
        // gotta convert to 3D for our little angle-detection trick
        // pretty much everything up until the very end will reference this
        Vector3[] verts3D = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            verts3D[i] = new Vector3(verts[i].x, 0, verts[i].y);
        }

        // to be filled with all of the convex pieces of our concave shape
        // think of these as 'fully processed'
        List<int[]> convexSlices = new List<int[]>(); // ints are vertex indices

        List<int[]> concaveSlices = new List<int[]>();
        // we add the shape as a whole as a concave slice, and begin

        Vector3 normal = Vector3.up;

        while (concaveSlices.Count > 0)
        {
            // we loop backwards cuz things will be getting deleted and we don't wanna mess up the index math
            for (int n = concaveSlices.Count - 1; n >= 0; n--)
            {
                // our job for each concave slice is to find a reflex angle
                // if we CAN'T find one then the slice is convex and we transfer it
                // if we CAN, then we slice it
                // in EITHER case we delete the slice

                // the loop that looks for a reflex angle
                int reflexIndex = -1;
                Vector2 bisector = Vector2.zero;
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
                        reflexIndex = i;

                        Vector2 a2D = new Vector2(a.x, a.z);
                        Vector2 b2D = new Vector2(b.x, b.z);

                        bisector = (a2D+b2D).normalized;
                    }
                }

                if (reflexIndex == -1)
                {
                    // the slice is already convex so we just add it
                    convexSlices.Add(concaveSlices[n]);
                } else
                {
                    // here we have to cut the thing
                    // each slice contains original vertex indices
                    List<int>[] slices = CutPolygon(bisector, verts, concaveSlices[n]);
                    
                    concaveSlices.Add(slices[0].ToArray());
                    concaveSlices.Add(slices[1].ToArray());
                }
                // again, we delete the concave slice in either case
                concaveSlices.RemoveAt(n);
            }
        }

        // so now the concave slices list is at 0,
        // and all we have are convex slices
        List<int[]> sliceTriangulations = new List<int[]>();
        int triSum = 0;
        for (int i = 0; i < convexSlices.Count; i++)
        {
            int[] newTriangulation = GenerateConvexTriangulation(convexSlices[i].Length, concaveSlices[i]);

            triSum += newTriangulation.Length;
            sliceTriangulations.Add(newTriangulation);
        }

        // copying all of the different triangulations into one array
        int[] result = new int[triSum];

        int localIndex = 0;
        int sliceIndex = 0;
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = sliceTriangulations[sliceIndex][localIndex];

            if (localIndex == sliceTriangulations[sliceIndex].Length - 1)
            {
                localIndex = 0;
                sliceIndex++;
            } else {localIndex++;}
        }

        // temp
        return result;
    }

    // makes an array going [0, 1, 2, .... n] for n items
    public static int[] GenerateAscending(int length)
    {
        int[] result = new int[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = i;
        }
        return result;
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

    // we don't need to return a util_polygoncomponent[] because we can just keep the original indices
    public static List<int>[] CutPolygon(Vector2 cutLine, Vector2[] verts, int[] indexMap)
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
                result[0].Add(indexMap[i]);
            } else if (Vector2.Dot(perpLine, verts[i]) < 0)
            {
                result[1].Add(indexMap[i]);
            } else // lies right on the line, so 2 copies (bc we need the resulting copies to be complete polygons)
            {
                result[0].Add(indexMap[i]);
                result[1].Add(indexMap[i]);
            }
        }

        return result;
    }
    // with no map
    public static List<int>[] CutPolygon(Vector2 cutLine, Vector2[] verts)
    {
        return CutPolygon(cutLine, verts, GenerateAscending(verts.Length));
    }

    // just a triangle fan, technically my own idea but many people have used this
    
    // not sure whether this is clockwise or not?
    public static int[] GenerateConvexTriangulation(int length, int[] indexMap)
    {
        int[] tris = new int[(length - 2) * 3];

        for (int i = 0, n = 1; i < tris.Length; i+= 3, n++)
        {
            tris[i] = indexMap[0];
            tris[i+1] = indexMap[n];
            tris[i+2] = indexMap[n+1];
        }

        return tris;
    }
    // when there isn't a mapping
    public static int[] GenerateConvexTriangulation(int length)
    {
        return GenerateConvexTriangulation(length, GenerateAscending(length));
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
