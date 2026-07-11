using UnityEngine;

public class crft_genericpart : MonoBehaviour
{
    public crft_genericpartdata data;
    public crft_genericpart[] connectedParts;



    public Transform t_snapPointContainer;
    public Transform[] t_snapPoints;
    private Transform surfaceMount;
    
    [HideInInspector] 
    public e_craft eComp;

    void Awake()
    {
        if (t_snapPointContainer != null)
        {
            t_snapPoints = new Transform[t_snapPointContainer.childCount];


            for (int i = 0; i < t_snapPointContainer.childCount; i++)
            {
                t_snapPoints[i] = t_snapPointContainer.GetChild(i);

                if (t_snapPointContainer.GetChild(i).GetComponent<crft_snappingpoint>().canBeSurfaceMounted)
                {
                    surfaceMount = t_snapPointContainer.GetChild(i);
                }
            }
        }
    }

    public string GetPartName()
    {
        // gets rid of any suffixes, like "(clone)" that unity puts on
        // just making sure, y'know?
        string[] splitName = util_string.SplitByChar(gameObject.name, '(');

        // removing the "part_" prefix
        return splitName[0].Substring(5);
    }

    public void PositionPart(Vector3 point, Vector3 normal)
    {
        if (surfaceMount != null)
        {
            transform.forward = normal;
            transform.rotation *= surfaceMount.localRotation;

            transform.position = point - (surfaceMount.position - transform.position);
        }
    }
}
