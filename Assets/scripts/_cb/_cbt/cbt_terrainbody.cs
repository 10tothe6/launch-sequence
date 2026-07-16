using UnityEngine;

// essentially combining the cbt_marchedbody and cbt_meshbody scripts,
// into one great collective horror

public class cbt_terrainbody : MonoBehaviour
{
    private int body_index;



    public cbt_meshbody mesh_terrain;
    public cbt_marchedbody marched_terrain; // technically also mesh terrain when you think abt it

    private cb_trackedbody body_data;


    // I was thinking of putting data such as "does the planet have an ocean here"
    // but centralizing it in cb_trackedbody seems like a smarter plan

    // as such, I'm doing that
    // I'll just pull from that script

    void Awake()
    {
        body_data = GetComponent<cb_trackedbody>();
    }

    public void InitializeAll(int body_index)
    {
        bool shouldUseMeshLayer = body_data.data.hasWater;
        // this is the only thing I can think to use this system for atm,
        // a water layer
        if (shouldUseMeshLayer)
        {
            mesh_terrain.Initialize(body_index);
        }

        bool shouldUseMarchedLayer = body_data.data.hasSurface;
        if (shouldUseMarchedLayer)
        {
            marched_terrain.Initialize(body_index);
        }
    }
}
