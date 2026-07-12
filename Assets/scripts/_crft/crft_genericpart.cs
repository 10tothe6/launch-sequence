using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class crft_genericpart : MonoBehaviour
{

    // I must be allergic to dictionaries or something
    public List<string> partDataKeys;
    public List<string> partDataValues;

    public crft_genericpartdata data;
    public crft_genericpart[] connectedParts;



    public Transform t_snapPointContainer;
    public Transform[] t_snapPoints;
    private Transform surfaceMount;
    
    [HideInInspector] 
    public e_craft eComp;

    // telling all the components to go collect their part data
    public UnityEvent onRecievePartData;

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


    // whenever loading a craft, each part receieves a data string

    // this contains literally EVERY bit of data for the part, like inventory contents and so on
    public void AcceptPartData(string data)
    {
        string[] elements = util_string.SplitByChar(data, '|');

        partDataKeys.Clear();
        partDataValues.Clear();
        
        for (int i = 0; i < elements.Length; i++)
        {
            string[] split = util_string.SplitByChar(elements[i], ':');

            partDataKeys.Add(split[0]);
            partDataValues.Add(split[1]);
        }

        onRecievePartData.Invoke();
    }

    public string GetAdditionalPartData(string key)
    {
        if (partDataKeys.Contains(key))
        {
            return partDataValues[partDataKeys.IndexOf(key)];
        } else
        {
            return "";
        }
    }
}
