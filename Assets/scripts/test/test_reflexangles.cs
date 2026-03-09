using System.Collections.Generic;
using UnityEngine;

public class test_reflexangles : MonoBehaviour
{
    
    void Start()
    {
        Vector3[] verts = new Vector3[transform.childCount];
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = transform.GetChild(i).position;
        }

        Vector2[] test = util_polygon.Vector3ToVector2(verts);

        bool[] reflexAngles = util_polygon.IdentifyReflexAnglesWhenClockwise(util_polygon.Vector2ToVector3(test));
        for (int i = 0; i < reflexAngles.Length; i++)
        {
            transform.GetChild(i).gameObject.SetActive(reflexAngles[i]);
        }
    }
}
