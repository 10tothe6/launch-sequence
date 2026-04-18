using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

// replaces 'TerrainFace.cs'
// we are using the term 'chunk' not 'face' now

// [progress] ported, needs comments and cleanup

public class cbt_meshchunk : MonoBehaviour
{
    public cbt_meshbody body;
    public bool isCulledByAngle;
    public bool isCulledByLOD;


    public Transform t_model;

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

    public int levelOfDetail; // the LOD level of the chunk

    public Vector3 chunkMidpoint;

    // the corner vertices of the chunk, stored for easy access during distance calculations
    // ************************************
    private Vector3 a;
    private Vector3 b;
    private Vector3 c;
    private Vector3 d;

    private Vector3 ab;
    private float abMag;
    private Vector3 ac;
    private float acMag;
    
    private Vector3 n; // plane normal
    // ************************************

    public void Initialize(int resolution, Vector3 localUp, Vector3 dims, int bodyIndex) {
        this.dims = dims;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        this.bodyIndex = bodyIndex;

        ConstructMesh(bodyIndex);

        a = mf.sharedMesh.vertices[0];
        b = mf.sharedMesh.vertices[resolution - 1];
        c = mf.sharedMesh.vertices[resolution * resolution - resolution];
        d = mf.sharedMesh.vertices[resolution * resolution - 1];
        
        ab = (b-a).normalized;
        abMag = (b-a).magnitude;
        ac = (c-a).normalized;
        acMag = (c-a).magnitude;

        n = Vector3.Cross(ab,ac).normalized;
    }

    public void SetDebugColor(Color col)
    {
        mr.material.color = col;
    }

    // POSITION IS NOT RELATIVE HERE
    public float GetDistanceToChunk(num_precisevector3 position)
    {
        if (directRadius != 0) {return 0;} // this means no cb_solarsystem is in use rn

        num_precisevector3 playerRelativePosition = position.Sub(cb_solarsystem.Instance.monoBodies[bodyIndex].pose.data.GetPosition());
        Vector3 clampedPosition = GetClampedPosition(playerRelativePosition.ToVector3());

        return num_precisevector3.Distance(playerRelativePosition, new num_precisevector3(clampedPosition)).AsFloat();
    }

    // position MUST BE RELATIVE TO THE MESHBODY OBJECT
    public float GetDistanceToChunk(Vector3 position)
    {
        Vector3 clampedPosition = GetClampedPosition(position);
        return Vector3.Distance(clampedPosition, position);
    }

    public Vector3 GetClampedPosition(Vector3 position)
    {
        Vector3 diff = position - a;
        //if (bodyIndex == 2) {Debug.Log(position);}
        // remove any component of the vector in/out of the plane
        diff -= Vector3.Project(diff, n);


        // adjusting the vector in the ab direction
        Vector3 abComponent = Vector3.Project(diff, ab);
        diff -= abComponent;
        if (Vector3.Dot(abComponent, ab) < 0)
        {
            abComponent = Vector3.zero;
        } else if (abComponent.magnitude > abMag)
        {
            abComponent *= abMag / abComponent.magnitude;
        }

        diff += abComponent;

        Vector3 acComponent = Vector3.Project(diff, ac);
        diff -= acComponent;
        if (Vector3.Dot(acComponent, ac) < 0)
        {
            acComponent = Vector3.zero;
        } else if (acComponent.magnitude > acMag)
        {
            acComponent *= acMag / acComponent.magnitude;
        }
        diff += acComponent;

        Vector3 clampedPosition = a + diff;
        
        if (directRadius != 0)
        {
            return clampedPosition.normalized * directRadius;
        } else
        {
            return clampedPosition.normalized * cb_solarsystem.Instance.monoBodies[bodyIndex].data.tConfig.equitorialRadius;
        }
    }

    // what is the height of the terrain (from [0..1]) at a given point on the planet
    public float GetHeightAt(Vector3 v) {
        v = v.normalized;

        if (directRadius == 0)
        {
            // stars have flat terrain, there is no variation in height
            if (cb_solarsystem.Instance.monoBodies[bodyIndex].data.bodyType == (ushort)cb_bodytype.Stellar)
            {
                return 0;
            }
        }

        // bad getcomp call
        bool usingTempPerlin = false;
        if (transform.parent.parent.GetComponent<cbt_meshbody>() != null)
        {
            usingTempPerlin = transform.parent.parent.GetComponent<cbt_meshbody>().useTemporaryPerlin;
        }
        if (usingTempPerlin)
        {
            return (float)WorldManager.Instance.perlin.Noise(v.x * 50f, v.y * 50f, v.z * 50f) * 40f;
        } else
        {
            return (float)TemporaryPerlin.Instance.perlin.Noise(v.x * 20f, v.y * 20f, v.z * 20f) * 25f;
        }
    }

    public void ConstructMesh(int bodyIndex)
    {
        mesh = new Mesh();

        float rad = directRadius != 0 ? directRadius : cb_solarsystem.Instance.monoBodies[bodyIndex].data.tConfig.equitorialRadius;

        if (rad > 100000f)
        {
            rad = 10000f;
        }

        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector3[] normals = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[resolution * resolution];
        int[] indices = new int[(resolution - 1) * (resolution - 1) * 6];

        int triIndex = 0;
        float scale = dims.z / (float)resolution;

        float xOff = dims.x / (float)resolution - (scale / 2) * (1 / scale - 1);
        float yOff = dims.y / (float)resolution - (scale / 2) * (1 / scale - 1);
        xOff /= scale;
        yOff /= scale;

        chunkMidpoint = (localUp + axisA * (0.5f + xOff - 0.5f) * 2 * scale + axisB * (0.5f + yOff - 0.5f) * 2 * scale).normalized * rad;

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                
                Vector3 pointOnUnitCube = localUp + axisA * (percent.x + xOff - 0.5f) * 2 * scale + axisB * (percent.y + yOff - 0.5f) * 2 * scale;
                
                //without normalizing it it would be a cube
                vertices[i] = pointOnUnitCube.normalized * rad;

                Vector3 altVertA1 = (vertices[i] - axisA * 0.05f).normalized * rad;
                Vector3 altVertA2 = (vertices[i] + axisA * 0.05f).normalized * rad;

                Vector3 altVertB1 = (vertices[i] - axisB * 0.05f).normalized * rad;
                Vector3 altVertB2 = (vertices[i] + axisB * 0.05f).normalized * rad;
                
                vertices[i] += pointOnUnitCube.normalized * GetHeightAt(vertices[i].normalized) * (rad / 1000f);

                altVertA1 += altVertA1.normalized * GetHeightAt(altVertA1.normalized) * (rad / 1000f);
                altVertA2 += altVertA2.normalized * GetHeightAt(altVertA2.normalized) * (rad / 1000f);
                altVertB1 += altVertB1.normalized * GetHeightAt(altVertB1.normalized) * (rad / 1000f);
                altVertB2 += altVertB2.normalized * GetHeightAt(altVertB2.normalized) * (rad / 1000f);
                
                normals[i] = Vector3.Cross((altVertA2 - altVertA1).normalized, (altVertB2 - altVertB1).normalized);
                if (Vector3.Angle(normals[i], vertices[i]) > 100)
                {
                    normals[i] *= -1;
                }

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

    public void UpdateRenderStatus()
    {
        if (!isCulledByAngle && !isCulledByLOD)
        {
            Show();
        } else {Hide();}
    }

    public void Hide()
    {
        mr.gameObject.SetActive(false);
    }
    public void Show()
    {
        mr.gameObject.SetActive(true);
    }
    
    public void SetAsActive()
    {
        isCulledByLOD = false;
        UpdateRenderStatus();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].isCulledByLOD = true;
            children[i].UpdateRenderStatus();
        }
    }

    public bool IsGrandChild()
    {
        return children.Length == 0;
    }
}
