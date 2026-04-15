using UnityEngine;
using System.Collections.Generic;

// this script will have a lot of similarities to cbt_meshbody.cs
// TODO: make a generic data class?
public class cbt_marchedbody : MonoBehaviour
{
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

    // ************************



    public float[] detailLevelThresholds;

    // TODO: call this from a more managerial script
    void FixedUpdate()
    {
        if (chunks != null && updateChunksPeriodically)
        {
            UpdateAllChunks();
        }
    }

    void UpdateAllChunks()
    {
        
    }
}
