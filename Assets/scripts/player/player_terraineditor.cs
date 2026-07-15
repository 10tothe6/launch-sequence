using System.Collections.Generic;
using UnityEngine;

public class player_terraineditor : MonoBehaviour
{
    private cbt_marchedbody current_body;
    public LayerMask what_is_ground;

    void Update()
    {
        if (Input.mouseButtonDownLeft)
        {
            current_body = cb_solarsystem.Instance.monoBodies[WorldManager.Instance.GetSOIIndex()].GetComponent<cbt_marchedbody>();

            RaycastHit hit;

            if (util_physics.LookRaycast(out hit, 10f, what_is_ground))
            {
                if (hit.collider.GetComponent<mcu_drawmesh>() != null)
                {
                    cbt_marchedchunk interactedWith = hit.transform.parent.GetComponent<cbt_marchedchunk>();

                    List<cbt_marchedchunk> others = GetAdjacentChunks(interactedWith);

                    // hey look a foreach loop
                    foreach (cbt_marchedchunk c in others)
                    {
                        if (c.levelOfDetail == 0)
                        {
                            c.ShowChunkCenter();
                        }
                    }
                }
            }
        }
    }

    // returns the 26 chunks around whichever one you gave
    // well, SOMETIMES less than 26 if the chunk is on the very edge of editable space (but that should never happen)
    public List<cbt_marchedchunk> GetAdjacentChunks(cbt_marchedchunk chunk)
    {
        List<cbt_marchedchunk> toReturn = new List<cbt_marchedchunk>();

        string hash = chunk.hashCode;

        // x
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1,0,0))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1,0,0))));

        

        // y
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,-1,0))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,1,0))));

        
        // z
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,0,-1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,0,1))));


        // xz
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1,0,1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1,0,-1))));


        // x -z
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1,0,-1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1,0,1))));


        // xy
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1,1,0))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1,-1,0))));


        // x -y
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1,-1,0))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1,1,0))));


        // yz
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,1,1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,-1,-1))));

        // y -z
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,1,-1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(0,-1,1))));

        // xyz
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1,1,1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1, -1, -1))));


        // xy -z
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1, 1, -1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1, -1, 1))));


        // x -y z
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1, -1, 1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1,1, -1))));

        // x -y -z
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(1, -1, -1))));
        toReturn.Add(current_body.GetChunkFromHashCode(mcu_utils.GetAdjacentHashCode(hash, new Vector3(-1, 1, 1))));

        return toReturn;
    }
}
