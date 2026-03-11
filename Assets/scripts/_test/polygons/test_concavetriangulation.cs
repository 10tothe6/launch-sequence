using UnityEngine;

public class test_concavetriangulation : MonoBehaviour
{
    public MeshFilter mf;

    void Start()
    {
        Vector3[] verts = new Vector3[transform.childCount];
        Vector3[] norms = new Vector3[transform.childCount];
        Vector2[] uvs = new Vector2[transform.childCount];

        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = transform.GetChild(i).position;
            norms[i] = Vector3.up;
            uvs[i] = new Vector2(transform.GetChild(i).position.x,transform.GetChild(i).position.z);
        }

        int[] tris = util_polygon.GenerateConcaveTriangulation(util_polygon.Vector3ToVector2(verts));

        Mesh m = new Mesh();
        m.SetVertices(verts);
        m.SetNormals(norms);
        m.SetUVs(0,uvs);
        m.SetTriangles(tris,0);

        mf.sharedMesh = m;
    }
}
