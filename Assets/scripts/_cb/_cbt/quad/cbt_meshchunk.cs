using UnityEngine;

// replaces 'TerrainFace.cs'
// we are using the term 'chunk' not 'face' now

// [progress] ported, needs comments and cleanup

public class cbt_meshchunk : MonoBehaviour
{
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
    // let's consider the chunk as a square inscribed in a cirlcle
    // how much of the sphere's surface is covered by that circular region?
    public float spanningAngle; // deg

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
        v = v.normalized;

        // bad getcomp call
        if (transform.parent.parent.GetComponent<cbt_meshbody>().useTemporaryPerlin)
        {
            return (float)WorldManager.Instance.perlin.Noise(v.x * 50f, v.y * 50f, v.z * 50f) * 10f;
        } else
        {
            return (float)TemporaryPerlin.Instance.perlin.Noise(v.x * 20f, v.y * 20f, v.z * 20f) * 100f;
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
                if (x == 0 && y == 0)
                {
                    spanningAngle = 5 * Vector3.Angle(chunkMidpoint, (localUp + axisA * (0.5f + xOff - 0.5f) * 2 * scale + axisB * (0.5f + yOff - 0.5f) * 2 * scale).normalized * rad);
                }

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
                //normals[i] = normals[i].normalized;
                //Debug.Log((altVert1 - vertices[i]) + "     " + (altVertB - vertices[i])+ "     " + vertices[i] + "    " + altVert1);
                if (Vector3.Angle(normals[i], vertices[i]) > 90)
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

    public Vector3 GetLocalPosition()
    {
        return chunkMidpoint;
    }

    // ************************
    // TODO: find a better way of calculating dist to a chunk
    // ************************

    // public Vector3 GetClosestPositionToPlayer()
    // {
    //     Vector3 centerToChunk = chunkMidpoint;
    //     Vector3 centerToControl = cb_renderingmanager.GetControlPosition() - cb_solarsystem.Instance.monoBodies[bodyIndex].data.pConfig.GetPosition().ToVector3();

    //     Vector3 axis = Vector3.Cross(centerToChunk, centerToControl);

    //     Vector3 limit = util_math.RotateVector(centerToChunk, axis, spanningAngle);

    //     return cb_solarsystem.Instance.monoBodies[bodyIndex].data.pConfig.GetPosition().ToVector3() + limit;
    // }

    public float GetDistance()
    {
        if (cb_renderingmanager.GetControlPosition().ToVector3() == Vector3.zero) {return Mathf.Infinity;}
        return (float)cb_renderingmanager.GetControlPosition().Sub(new num_precisevector3(chunkMidpoint).Add(cb_solarsystem.Instance.monoBodies[bodyIndex].data.pConfig.GetPosition())).Mag().AsDouble();

        // float distanceToSurface = WorldManager.Instance.GetSeaLevelAltitude();
        

        // if (Vector3.Angle(chunkMidpoint, cb_renderingmanager.GetControlPosition() - cb_solarsystem.Instance.monoBodies[bodyIndex].data.pConfig.GetPosition().ToVector3()) < spanningAngle)
        // {
        //     return distanceToSurface;
        // } else
        // {
        //     return (cb_renderingmanager.GetControlPosition() - (chunkMidpoint + cb_solarsystem.Instance.monoBodies[bodyIndex].data.pConfig.GetPosition().ToVector3()).magnitude;
        // }
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
