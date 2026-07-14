using UnityEngine;
using System.Collections.Generic;


//***
// FINALLY! THE MARCHING CUBES IMPLEMENTATION! REJOICE!
//***


// this script will have a lot of similarities to cbt_meshbody.cs

// I am NOT making a generic data class for now,
// planets that have both mesh and marched chunks (such as for a water layer) will simply have both components


public class cbt_marchedbody : MonoBehaviour
{
    public e_genericentity eComp;

    public bool enableChunkCulling;
    public float chunkCullingAngle; // anything at a greater angle will be culled
    public bool updateChunksPeriodically;

    [Header("TRACKING CONFIG")]
    public bool useDirectObject; // false in the main game, true when testing
    public Transform t_decidingObject;

    [Space(12)]
    public GameObject p_chunk;
    public Transform t_chunkContainer;

    // the resolution of one side of the chunk
    public int startingResolution = 10;

    private int bodyIndex;

    // ************ chunk data ************
    [HideInInspector]
    public List<cbt_marchedchunk> chunks;
    [HideInInspector]
    public List<cbt_marchedchunk> newChunks;
    [HideInInspector]
    public cbt_marchedchunk parentChunk; // the SINGLE original grid square

    // ************************


    // each bool is whether it has a collider
    public bool[] detailLevelThresholds;
    public int maxDetailLevels;


    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
    }


    public void Initialize(int bodyIndex)
    {
        this.bodyIndex = bodyIndex;
        Initialize();
    }

    public void Initialize()
    {
        if (!useDirectObject)
        {
            if (cb_solarsystem.Instance.monoBodies[bodyIndex].data.tConfig.equitorialRadius > 1000000f)
            {
                cb_solarsystem.Instance.monoBodies[bodyIndex].pose.data.SetDataEntry("scaleFactor", (100000f / cb_solarsystem.Instance.monoBodies[bodyIndex].data.tConfig.equitorialRadius).ToString());
            }
        }

        // **** just setting variables ****
        
        chunks = new List<cbt_marchedchunk>();
        newChunks = new List<cbt_marchedchunk>();
        // ****************************

        // unlike the mesh implementation,
        // here we have ONE parent chunk (not 6)
        // initializing the 6 parent chunks
        GameObject g_parentChunk = Instantiate(p_chunk, t_chunkContainer);
        g_parentChunk.GetComponent<cbt_marchedchunk>().body = this;
        parentChunk = g_parentChunk.GetComponent<cbt_marchedchunk>();
        
        // no direct radius for now
        // if (useDirectRadius)
        // {
        //     parentChunks[i].directRadius = directRadius;
        // }



        // build the chunk, basically
        parentChunk.Initialize(startingResolution, bodyIndex);
        // set the LOD to the minimum
        parentChunk.levelOfDetail = detailLevelThresholds.Length - 1; // higher number, lower detail
        // these chunks have no parents, they are the grandparents
        parentChunk.parent = null;

        // no such thing as a 'starting face' for marched cubes

        // we do still have hashcodes though
        parentChunk.hashCode = null;

        chunks.Add(parentChunk);
    }

    void FixedUpdate()
    {
        if (chunks != null && updateChunksPeriodically)
        {
            UpdateAllChunks();
        }
    }

    void UpdateAllChunks()
    {
        // here is where the LOD management gets done
        
        // note that this process is different from the quadtree system:
        // instead of checking distances to chunks we're simply checking which chunks the player is "inside" of,
        // and subdividing those ones

        foreach (cbt_marchedchunk current in chunks)
        {
            if (current.levelOfDetail > (detailLevelThresholds.Length-maxDetailLevels) && current.IsLocalPlayerInBounds(0.1f) && current.mcu.isVisible)
            {
                // player is within the bounds of the chunk, meaning we subdivide
                current.Subdivide();

                cmd.LogRaw("SUBDIVIDING...", Color.limeGreen);
                break;
            }
        }
    }
}
