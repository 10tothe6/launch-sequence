using UnityEngine;

// replaces 'TerrainFace.cs'
// we are using the term 'chunk' not 'face' now

// [progress] ported, needs comments and cleanup

public class cbt_meshchunk : MonoBehaviour
{
    [Header("TESTING")]
    // normally the radius is referenced from cb_solarsystem,
    // but the script checks for a non-zero value here and uses that if need be
    public float directRadius;
    [Space(12)]
    public MeshRenderer mr;
    public MeshFilter mf;

    public cbt_meshchunk parent;
    public cbt_meshchunk[] children;

    // there WAS some tree data on the TerrainFace script, but we're ignoring all that

    public int startingFace;
    public string hashCode;

    Mesh mesh;
    int resolution; // the number of vertices per side of the mesh (meshes are square)
    int bodyIndex;

    public Vector3 dims; // not sure
    public Vector3 localUp; // not sure
    Vector3 axisA; // not sure
    Vector3 axisB; // not sure

    public int level; // the LOD level of the chunk

    public void Initialize(int resolution, Vector3 localUp, Vector3 dims, int bodyIndex) {
        this.dims = dims;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        this.bodyIndex = bodyIndex;

        ConstructMesh(bodyIndex);
    }

    public void SetDebugColor(Color col)
    {
        mr.material.color = col;
    }

    public float GetHeightAt(Vector3 v) {
        return 1f;
    }

    public void ConstructMesh(int bodyIndex)
    {
        mesh = new Mesh();

        float rad = directRadius != 0 ? directRadius : cb_solarsystem.Instance.monoBodies[bodyIndex].data.tConfig.equitorialRadius;

        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector3[] normals = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[resolution * resolution];
        int[] indices = new int[(resolution - 1) * (resolution - 1) * 6];

        int triIndex = 0;
        float scale = dims.z / (float)resolution;

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                float xOff = dims.x / (float)resolution - (scale / 2) * (1 / scale - 1);
                float yOff = dims.y / (float)resolution - (scale / 2) * (1 / scale - 1);
                xOff /= scale;
                yOff /= scale;
                Vector3 pointOnUnitCube = localUp + axisA * (percent.x + xOff - 0.5f) * 2 * scale + axisB * (percent.y + yOff - 0.5f) * 2 * scale;
                
                //without normalizing it it would be a cube
                vertices[i] = pointOnUnitCube.normalized * rad;
                //vertices[i] += pointOnUnitCube.normalized * GetHeightAt(vertices[i]) * (rad / 1000f);
                // vertices[i] = cb_renderingmanager.Instance.AdjustVector(vertices[i].normalized, 0) * vertices[i].magnitude;

                normals[i] = pointOnUnitCube.normalized;
                // normals[i] = cb_renderingmanager.Instance.AdjustVector(pointOnUnitCube.normalized, 0);
                
                // the 23 and 24 is based off of resolution
                uvs[i] = new Vector2((float)x / 23 * scale + dims.x / 24, (float)y / 23 * scale + dims.y / 24);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    indices[triIndex] = i;
                    indices[triIndex + 1] = i + resolution + 1;
                    indices[triIndex + 2] = i + resolution;

                    indices[triIndex + 3] = i;
                    indices[triIndex + 4] = i + 1;
                    indices[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.normals = normals;
        mesh.uv = uvs;

        mf.mesh = mesh;

        //gameObject.layer = Sys.planetLayerMaskInt;
    }
}
