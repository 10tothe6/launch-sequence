using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtils : MonoBehaviour
{
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
