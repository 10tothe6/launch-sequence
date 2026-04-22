using UnityEngine;

public class test_drawcatenary : MonoBehaviour
{
    public Transform t1;
    public Transform t2;

    public Vector3[] points;
    public float pointRadius;
    public float sag;

    public bool regen;
    
    void Update()
    {
        if (regen)
        {
            points = util_geo.GetCatenaryPoints(t1.position, t2.position, sag).ToArray();
           //regen = false;
        }        
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], pointRadius);
        }
    }
}
