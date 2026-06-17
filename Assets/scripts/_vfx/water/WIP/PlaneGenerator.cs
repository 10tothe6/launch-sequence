using UnityEngine;

// this script creates a mesh and assigns it to the MeshFilter component
// that's it, just attatch it to a game object

// im using it to generate the water plane, 
// prob will use it in addition to smth else to generate underwater terrain so I don't dupe plane generation code

public class PlaneGenerator : MonoBehaviour
{
    public bool generateOnAwake;

    [Header("vertex count per side")]
    public int width;
    [Header("side length")]
    public float realScale;

    void Start()
    {
        // by default, generate facing up
        if (generateOnAwake) Generate(false);
    }

    public void Generate(bool reverse)
    {
        Mesh planeMesh = MeshUtils.GeneratePlane(width, realScale, reverse);
        planeMesh.RecalculateBounds();

        MeshFilter filterComp = GetComponent<MeshFilter>();
        MeshCollider colliderComp = GetComponent<MeshCollider>();

        if (filterComp != null) filterComp.sharedMesh = planeMesh;
        if (colliderComp != null) colliderComp.sharedMesh = planeMesh;
    }
}
