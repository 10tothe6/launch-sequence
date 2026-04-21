using UnityEngine;

// allows the drawing of various mesh components individually, like vertices and normals

public class test_drawmesh : MonoBehaviour
{
    public Mesh m;
    // alternatively, you can provide the meshrenderer
    public MeshFilter mf;

    public bool drawVertices;
    public float vertexRadius;


    public bool drawNormals;
    public float normalLength;

    void Update()
    {
        if (mf != null) {m = mf.sharedMesh;}

        if (m == null) {return;} // can't draw a mesh if there is no mesh

        if (drawVertices)
        {
            for (int i = 0;i < m.vertices.Length; i++)
            {
                Gizmos.DrawSphere(m.vertices[i], vertexRadius);
            }
        }
        if (drawNormals)
        {
            for (int i = 0;i < m.vertices.Length; i++)
            {
                Debug.DrawLine(m.vertices[i], m.vertices[i] + m.normals[i] * normalLength);
            }
        }
    }
}
