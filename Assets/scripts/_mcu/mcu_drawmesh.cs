using UnityEngine;
using System.Collections.Generic;
using System;

// ************  INFO ON THIS CLASS: ************

// just takes in a 3D array of points and draws a mesh using marching cubes
// does NOT calculate the points, that information is given to it

// the mesh is ALWAYS generated relative to the bottom-left corner of the object,
// extending in the +x,+y and +z directions

public class mcu_drawmesh : MonoBehaviour
{
    [Header("DEBUG")]
    public int indexToLog_x;
    public int indexToLog_y;
    public int indexToLog_z;
    public bool logPoint;

    public float pointRadius;

    [Space(20)]
    [Header("Config")]
    public bool showGridPoints;
    public bool showGridLines;

    public MeshFilter mf;
    public MeshRenderer mr;

    // these are more like resolutions
    public int xSize;
    public int ySize;
    public int zSize;
    public double[,,] points;

    // these are the actual sizes
    public double xSizeActual;
    public double ySizeActual;
    public double zSizeActual;
    public double xScaleFactor;

    public double yScaleFactor;

    public double zScaleFactor;

    // THESE INDICES ARE ALL ONE GREATER THAN THE ACTUAL INDEX
    // the first index is the beginning point,
    // the second is the ending point,
    // the value is the index
    private int[,] existingVertexIndices;

    List<int> tris;
    List<Vector3> verts;
    List<Vector3> norms;

    private Vector3 offset;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if(logPoint)
        {
            logPoint = false;
            Debug.Log(points[indexToLog_x, indexToLog_y, indexToLog_z]);
        }
    }

    public void Initialize(double[,,] points, 
    int xSize, int ySize, int zSize, 
    float xSizeActual, float ySizeActual, float zSizeActual)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.zSize = zSize;
        this.points = points;

        this.xSizeActual = xSizeActual;
        this.ySizeActual = ySizeActual;
        this.zSizeActual = zSizeActual;

        xScaleFactor = xSizeActual / ((float)xSize-1);
        yScaleFactor = ySizeActual / ((float)ySize-1);
        zScaleFactor = zSizeActual / ((float)zSize-1);

        DrawMesh();
    }

    public void ReInitialize()
    {
        DrawMesh();
    }

    public void SetOffset(Vector3 offset)
    {
        this.offset = offset;
    }
    
    // just making all the points 0
    public void InitializeEmpty(int xSize, int ySize, int zSize,
    float xSizeActual, float ySizeActual, float zSizeActual)
    {
        Initialize(new double[xSize,ySize,zSize], xSize, ySize, zSize, xSizeActual,ySizeActual,zSizeActual);
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
        // for (int x = 0; x < xSize - 1; x++)
        // {
        //     for (int y = 0; y < ySize - 1; y++)
        //     {
        //         for (int z = 0; z < zSize - 1; z++)
        //         {
        //             float s = 1;

        //             // really hoping these lines are right

        //             // 0 --> 1
        //             Gizmos.DrawLine(new Vector3(x,y,z)*xScaleFactor,new Vector3(x+s,y,z)*xScaleFactor);
        //             // 0 --> 3
        //             Gizmos.DrawLine(new Vector3(x,y,z)*xScaleFactor,new Vector3(x,y+s,z)*xScaleFactor);
        //             // 0 --> 4
        //             Gizmos.DrawLine(new Vector3(x,y,z)*xScaleFactor,new Vector3(x,y,z+s)*xScaleFactor);
        //             // 1 --> 2
        //             Gizmos.DrawLine(new Vector3(x+s,y,z)*xScaleFactor,new Vector3(x+s,y+s,z)*xScaleFactor);
        //             // 1 --> 5
        //             Gizmos.DrawLine(new Vector3(x+s,y,z)*xScaleFactor,new Vector3(x+s,y,z+s)*xScaleFactor);
        //             // 2 --> 3
        //             Gizmos.DrawLine(new Vector3(x,y+s,z)*xScaleFactor,new Vector3(x+s,y+s,z)*xScaleFactor);
        //             // 3 --> 7
        //             Gizmos.DrawLine(new Vector3(x,y+s,z)*xScaleFactor,new Vector3(x,y+s,z+s)*xScaleFactor);
        //             // 2 --> 6
        //             Gizmos.DrawLine(new Vector3(x+s,y+s,z)*xScaleFactor,new Vector3(x+s,y+s,z+s)*xScaleFactor);
                    
        //             // 4 --> 5
        //             Gizmos.DrawLine(new Vector3(x,y,z+s)*xScaleFactor,new Vector3(x+s,y,z+s)*xScaleFactor);
        //             // 4 --> 7
        //             Gizmos.DrawLine(new Vector3(x,y,z+s)*xScaleFactor,new Vector3(x,y+s,z+s)*xScaleFactor);
        //             // 5 --> 6
        //             Gizmos.DrawLine(new Vector3(x+s,y,z+s)*xScaleFactor,new Vector3(x+s,y+s,z+s)*xScaleFactor);
        //             // 6 --> 7
        //             Gizmos.DrawLine(new Vector3(x+s,y+s,z+s)*xScaleFactor,new Vector3(x,y+s,z+s)*xScaleFactor);
        //         }
        //     }
        // }
    }

    // draws the 3D grid of points that represents the area taken up by this component
    public void DrawGridPoints()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    // TODO: offset this properly
                    Gizmos.DrawSphere(transform.position + new Vector3(x,y,z)*(float)xScaleFactor + offset, pointRadius);
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
                    double[] cellValues = new double[8];
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
                        new Vector3(x,y,z)*(float)xScaleFactor,
                        new Vector3(x+1,y,z)*(float)xScaleFactor,
                        new Vector3(x+1,y+1,z)*(float)xScaleFactor,
                        new Vector3(x,y+1,z)*(float)xScaleFactor,
                        new Vector3(x,y,z+1)*(float)xScaleFactor,
                        new Vector3(x+1,y,z+1)*(float)xScaleFactor,
                        new Vector3(x+1,y+1,z+1)*(float)xScaleFactor,
                        new Vector3(x,y+1,z+1)*(float)xScaleFactor,
                    };

                    Vector3[] cellVertices_raw = new Vector3[]
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
                            AddVertex(i,f, cellVertices_raw[a[0]], cellVertices_raw[a[1]]);
                        } else {aIndex--;}

                        i = GetPointIndexFromPosition(cellVertices[b[0]]);
                        f = GetPointIndexFromPosition(cellVertices[b[1]]);
                        int bIndex = GetVertexIndex(i,f);
                        if (bIndex == 0) {bIndex = verts.Count;
                            AddVertex(i,f, cellVertices_raw[b[0]], cellVertices_raw[b[1]]);
                        } else {bIndex--;}

                        i = GetPointIndexFromPosition(cellVertices[c[0]]);
                        f = GetPointIndexFromPosition(cellVertices[c[1]]);
                        int cIndex = GetVertexIndex(i,f);
                        if (cIndex == 0) {cIndex = verts.Count;
                            AddVertex(i,f, cellVertices_raw[c[0]], cellVertices_raw[c[1]]);
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

        if (GetComponent<MeshCollider>() != null)
        {
            GetComponent<MeshCollider>().sharedMesh = mf.mesh;
        }
    }

    void AddVertex(int initial, int final, Vector3 initial_pos, Vector3 final_pos) {
        existingVertexIndices[initial,final] = verts.Count + 1;
        existingVertexIndices[final,initial] = verts.Count + 1;
        
        norms.Add(Vector3.up);

        Vector3 vi = initial_pos;
        Vector3 vf = final_pos;
        verts.Add(offset + Vector3.Lerp(vi,vf, (float)GetZero(points[Mathf.RoundToInt(vi.x),Mathf.RoundToInt(vi.y),Mathf.RoundToInt(vi.z)],points[Mathf.RoundToInt(vf.x),Mathf.RoundToInt(vf.y),Mathf.RoundToInt(vf.z)]))*(float)xScaleFactor);
    }

    public int GetPointIndexFromPosition(Vector3 pos)
    {
        return Mathf.RoundToInt(pos.x/(float)xScaleFactor) + Mathf.RoundToInt(pos.y/(float)xScaleFactor) * xSize + Mathf.RoundToInt(pos.z/(float)xScaleFactor) * xSize * ySize;
    }

    public int GetConfigIndex(double[] cellValues)
    {
        float surfaceHeight = 0;

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
    public double GetZero(double a, double b)
    {
        double total = Math.Abs(a) + Math.Abs(b);
        double dist = Math.Abs(a);

        
        return dist / total;
    }
}
