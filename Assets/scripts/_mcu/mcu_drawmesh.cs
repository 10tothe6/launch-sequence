using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using Unity.VisualScripting;

// literally just takes in a 3D array of points and draws a mesh using marching cubes
// nothing else, yet
public class mcu_drawmesh : MonoBehaviour
{
    [Header("Config")]
    public bool showGridPoints;
    public bool showGridLines;

    public MeshFilter mf;
    
    public int xSize;
    public int ySize;
    public int zSize;
    public float[,,] points;

    // THESE INDICES ARE ALL ONE GREATER THAN THE ACTUAL INDEX
    // the first index is the beginning point,
    // the second is the ending point,
    // the value is the index
    private int[,] existingVertexIndices;

    List<int> tris;
    List<Vector3> verts;
    List<Vector3> norms;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();

        // float[] test = new float[] {0,0,0,0,0,0,0,0};
        // Debug.Log(GetConfigIndex(test));
    }

    public void Initialize(float[,,] points, int xSize, int ySize, int zSize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.zSize = zSize;
        this.points = points;

        DrawMesh();
    }
    
    // just making all the points 0
    public void InitializeEmpty(int xSize, int ySize, int zSize)
    {
        Initialize(new float[xSize,ySize,zSize], xSize, ySize, zSize);
    }

    void OnDrawGizmos()
    {
        if (showGridPoints)
        {
            DrawGridPoints();
        }
        if (showGridLines)
        {
            DrawCellLines();
        }
    }

    // draws the lines between each point in the 3D grid, aka. the bounds for each cell
    public void DrawCellLines()
    {
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int y = 0; y < ySize - 1; y++)
            {
                for (int z = 0; z < zSize - 1; z++)
                {
                    float s = 1;

                    // really hoping these lines are right

                    // 0 --> 1
                    Gizmos.DrawLine(new Vector3(x,y,z),new Vector3(x+s,y,z));
                    // 0 --> 3
                    Gizmos.DrawLine(new Vector3(x,y,z),new Vector3(x,y+s,z));
                    // 0 --> 4
                    Gizmos.DrawLine(new Vector3(x,y,z),new Vector3(x,y,z+s));
                    // 1 --> 2
                    Gizmos.DrawLine(new Vector3(x+s,y,z),new Vector3(x+s,y+s,z));
                    // 1 --> 5
                    Gizmos.DrawLine(new Vector3(x+s,y,z),new Vector3(x+s,y,z+s));
                    // 2 --> 3
                    Gizmos.DrawLine(new Vector3(x,y+s,z),new Vector3(x+s,y+s,z));
                    // 3 --> 7
                    Gizmos.DrawLine(new Vector3(x,y+s,z),new Vector3(x,y+s,z+s));
                    // 2 --> 6
                    Gizmos.DrawLine(new Vector3(x+s,y+s,z),new Vector3(x+s,y+s,z+s));
                    
                    // 4 --> 5
                    Gizmos.DrawLine(new Vector3(x,y,z+s),new Vector3(x+s,y,z+s));
                    // 4 --> 7
                    Gizmos.DrawLine(new Vector3(x,y,z+s),new Vector3(x,y+s,z+s));
                    // 5 --> 6
                    Gizmos.DrawLine(new Vector3(x+s,y,z+s),new Vector3(x+s,y+s,z+s));
                    // 6 --> 7
                    Gizmos.DrawLine(new Vector3(x+s,y+s,z+s),new Vector3(x,y+s,z+s));
                }
            }
        }
    }

    // draws the 3D grid of points that represents the area taken up by this component
    public void DrawGridPoints()
    {
        float rad = 0.15f;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    // TODO: offset this properly
                    Gizmos.DrawSphere(new Vector3(x,y,z), rad);
                }
            }
        }
    }

    public void DrawMesh()
    {
        Mesh result = new Mesh();

        tris = new List<int>();
        verts = new List<Vector3>();
        norms = new List<Vector3>();

        existingVertexIndices = new int[xSize*ySize*zSize,xSize*ySize*zSize];
        
        // looping through the CELLS, not points
        // each cell being a cube
        // there are n-1 cells, given n points
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int y = 0; y < ySize - 1; y++)
            {
                for (int z = 0; z < zSize - 1; z++)
                {
                    // might be a bad thing to initialize an array every step?
                    // ah what the hell this'll be a shader eventually anyways
                    float[] cellValues = new float[8];
                    cellValues[0] = points[x,y,z];
                    cellValues[1] = points[x+1,y,z];
                    cellValues[2] = points[x+1,y+1,z];
                    cellValues[3] = points[x,y+1,z];
                    cellValues[4] = points[x,y,z+1];
                    cellValues[5] = points[x+1,y,z+1];
                    cellValues[6] = points[x+1,y+1,z+1];
                    cellValues[7] = points[x,y+1,z+1];

                    Vector3[] cellVertices = new Vector3[]
                    {
                        new Vector3(x,y,z),
                        new Vector3(x+1,y,z),
                        new Vector3(x+1,y+1,z),
                        new Vector3(x,y+1,z),
                        new Vector3(x,y,z+1),
                        new Vector3(x+1,y,z+1),
                        new Vector3(x+1,y+1,z+1),
                        new Vector3(x,y+1,z+1),
                    };

                    int configurationIndex = GetConfigIndex(cellValues);
                    //Debug.Log(configurationIndex);
                    int[] triangulation = mcu_utils.triangulations[configurationIndex];

                    for (int ti = 0; ti < triangulation.Length; ti += 3)
                    {   
                        // skip over null triangles
                        if (triangulation[ti] == -1) {continue;}

                        // the vertices gathered from the edges described in the triangulation
                        int[] a = mcu_utils.edgeVertices[triangulation[ti]];
                        int[] b = mcu_utils.edgeVertices[triangulation[ti+1]];
                        int[] c = mcu_utils.edgeVertices[triangulation[ti+2]];

                        int i = GetPointIndexFromPosition(cellVertices[a[0]]);
                        int f = GetPointIndexFromPosition(cellVertices[a[1]]);
                        int aIndex = GetVertexIndex(i,f);
                        if (aIndex == 0) {aIndex = verts.Count;
                            AddVertex(i,f);
                        } else {aIndex--;}

                        i = GetPointIndexFromPosition(cellVertices[b[0]]);
                        f = GetPointIndexFromPosition(cellVertices[b[1]]);
                        int bIndex = GetVertexIndex(i,f);
                        if (bIndex == 0) {bIndex = verts.Count;
                            AddVertex(i,f);
                        } else {bIndex--;}

                        i = GetPointIndexFromPosition(cellVertices[c[0]]);
                        f = GetPointIndexFromPosition(cellVertices[c[1]]);
                        int cIndex = GetVertexIndex(i,f);
                        if (cIndex == 0) {cIndex = verts.Count;
                            AddVertex(i,f);
                        } else {cIndex--;}

                        tris.Add(aIndex);
                        tris.Add(bIndex);
                        tris.Add(cIndex);
                    }
                }
            }
        }

        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetTriangles(tris,0);

        mf.mesh = result;
    }

    void AddVertex(int initial, int final) {
        existingVertexIndices[initial,final] = verts.Count + 1;
        existingVertexIndices[final,initial] = verts.Count + 1;
        
        norms.Add(Vector3.up);

        Vector3 vi = GetPositionFromPointIndex(initial);
        Vector3 vf = GetPositionFromPointIndex(final);
        verts.Add(Vector3.Lerp(vi,vf, GetZero(points[Mathf.RoundToInt(vi.x),Mathf.RoundToInt(vi.y),Mathf.RoundToInt(vi.z)],points[Mathf.RoundToInt(vf.x),Mathf.RoundToInt(vf.y),Mathf.RoundToInt(vf.z)])));
    }

    public Vector3 GetPositionFromPointIndex(int pointIndex)
    {
        int z = Mathf.FloorToInt((float)pointIndex / ((float)xSize * (float)ySize));
        int y = Mathf.FloorToInt((pointIndex - z * xSize * ySize) / (float)xSize);
        int x = pointIndex - z * xSize * ySize - y * xSize;
        return new Vector3(x, y, z);
    }

    public int GetPointIndexFromPosition(Vector3 pos)
    {
        return Mathf.RoundToInt(pos.x) + Mathf.RoundToInt(pos.y) * xSize + Mathf.RoundToInt(pos.z) * xSize * ySize;
    }

    public int GetConfigIndex(float[] cellValues)
    {
        float surfaceHeight = 0.1f;

        int sum = 0;

        for (int i = 0; i < cellValues.Length; i++)
        {
            int current = cellValues[i] < surfaceHeight ? 1 : 0;

            current = current << i;

            sum = sum | current;
        }

        return sum;
    }

    // grabbing the VERTEX index of a vertex, if it exists, 
    // based on the points that the vertex is derived from
    public int GetVertexIndex(int initial, int final)
    {
        return existingVertexIndices[initial,final];
    }

    // given two values, figure out the percentage along the line a --> b where 0 is
    // just a small step I wanted to remove from the larger logic
    public float GetZero(float a, float b)
    {
        float total = Mathf.Abs(a) + Mathf.Abs(b);
        float dist = Mathf.Abs(a);

        
        return dist / total;
    }
}
