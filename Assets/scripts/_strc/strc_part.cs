using System.Collections.Generic;
using UnityEngine;

// data relating to an element of a solid structure
public class strc_part : MonoBehaviour // yes its a mono script
{
    public float mass;

    public Vector3 staticLoad;

    public bool isConnectedToGround;
    public List<strc_part> immidiatelyConnectedParts;

    public List<strc_path> pathsToGround; // "ground" just means any supporting structure

    public Vector3 GetGravityLoad()
    {
        // not dealing with planets yet so thankfully down is down
        return -Vector3.up * 9.81f * mass;
    }

    public void ApplyLoad(Vector3 load)
    {
        
    }

    public void RecalculatePathsToGround()
    {
        pathsToGround = new List<strc_path>();

        pathsToGround.Add(new strc_path());
        strc_part target = this;
        
        while(target.immidiatelyConnectedParts.Count > 0)
        {
            
        }
    }
}
