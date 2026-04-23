using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class test_drawcatenary : MonoBehaviour
{
    public Transform t1;
    public Transform t2;

    public Vector3[] points;
    public List<Vector3> castPoints;
    public List<int> castPointIndices;

    public Vector3[] linearPoints;
    public float pointRadius;
    public float sag;

    public bool regen;
    
    void Update()
    {
        if (regen)
        {
            GenerateCableShape();

           //regen = false;
        }        
    }

    void GenerateCableShape()
    {
        linearPoints = util_geo.GetLinearPoints(t1.position, t2.position).ToArray();
        points = util_geo.GetCatenaryPoints(t1.position, t2.position, sag).ToArray();

        castPoints = new List<Vector3>();
        castPointIndices = new List<int>();

        castPoints.Add(t1.position);
        castPointIndices.Add(0);
        for (int i = 1; i < linearPoints.Length - 1; i++)
        {

            RaycastHit hit;
            if (Physics.Raycast(linearPoints[i], -Vector3.up, out hit, (points[i] - linearPoints[i]).magnitude))
            {
                castPoints.Add(hit.point);
                castPointIndices.Add(i);
            }
        }
        castPoints.Add(t2.position);
        castPointIndices.Add(linearPoints.Length - 1);

        List<int> toRemove = new List<int>();
        List<bool> dropsLeft = new List<bool>();
        List<bool> dropsRight = new List<bool>();

        dropsRight.Add(true);
        dropsLeft.Add(true);

        // figure out which cast points are on the border of non-hit points
        for (int i = 1; i < castPointIndices.Count - 1; i++)
        {
            bool isOk = false;
            if (castPointIndices[i + 1] > castPointIndices[i] + 1)
            {
                isOk = true;
                dropsRight.Add(true);
                
            } else
            {
                dropsRight.Add(false);
            }
            
            if (castPointIndices[i - 1] < castPointIndices[i] -1 )
            {
                isOk = true;
                dropsLeft.Add(true);

            } else
            {
                dropsLeft.Add(false);
            }
            
            if (!isOk)
            {
                toRemove.Add(i);
            }
        }

        dropsRight.Add(true);
        dropsLeft.Add(true);

        for (int i = toRemove.Count - 1; i >= 0; i --)
        {
            castPointIndices.RemoveAt(toRemove[i]);
            castPoints.RemoveAt(toRemove[i]);
            dropsRight.RemoveAt(toRemove[i]);
             dropsLeft.RemoveAt(toRemove[i]);
        }

        List<Vector3> finalPoints = new List<Vector3>();

        for (int i = 1; i < castPoints.Count; i++)
        {
            if (dropsLeft[i] && dropsRight[i-1])
            {
                List<Vector3> catenaryPoints = util_geo.GetCatenaryPoints(castPoints[i-1],castPoints[i], sag);
                for (int j = 0; j < catenaryPoints.Count; j++) {finalPoints.Add(catenaryPoints[j]);}
            }
        }

        points = finalPoints.ToArray();
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (i > 0)
            {
                Debug.DrawLine(points[i], points[i-1]);
            }
            Gizmos.DrawSphere(points[i], pointRadius);
        }
        Gizmos.color =Color.blue;
        for (int i = 0; i < linearPoints.Length; i++)
        {
            Gizmos.DrawSphere(linearPoints[i], pointRadius);
        }
    }
}
