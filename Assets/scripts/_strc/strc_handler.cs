using System.Collections.Generic;
using UnityEngine;

// the script that manages the forces in a structure
// like the parent
public class strc_handler : MonoBehaviour
{
    List<strc_part> parts;

    void Start()
    {
        RecalculateLoadStructure();
    }

    public void RecalculateLoadStructure()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].RecalculatePathsToGround();
        }
    }

    public void ApplyForceToPart(strc_part part)
    {
        
    }

    // load from gravity
    public void CalculateStaticLoad()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].staticLoad = Vector3.zero;
        }
        for (int i = 0; i < parts.Count; i++)
        {
            Vector3 load = parts[i].GetGravityLoad();

            parts[i].ApplyLoad(load);
        }
    }
}
