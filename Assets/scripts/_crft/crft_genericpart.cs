using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class crft_genericpart : MonoBehaviour
{
    [Header("GENERAL CONFIG")]
    // used for thrust and general physics calculations
    public float part_mass; // in kg
    public bool isBuildable;



    // TODO: some sort of part data class to hold these sorts of things,
    // not on the monobehaviour scripts

    // implementing that is a prereq to having 3D rendered part icons, i suspect
    public Sprite part_icon; 

    [Space(10)]
    [Header("PART DATA")]
    // I must be allergic to dictionaries or something
    public List<string> partDataKeys;
    public List<string> partDataValues;

    public crft_genericpartdata data;
    public crft_genericpart[] connectedParts;



    public Transform t_snapPointContainer;
    public Transform[] t_snapPoints;
    [SerializeField]
    private Transform surfaceMount;
    
    [HideInInspector] 
    public e_craft eComp;

    public UnityEvent onInitialize;

    // telling all the components to go collect their part data
    public UnityEvent onRecievePartData;
    public List<Func<string>> partDataCollectors;
    

    // a sort of cache for default material information,
    // allowing us to restore a part's model after setting the material to a preview or something like that
    private List<Material> default_materials;
    private List<MeshRenderer> default_material_locations;

    #region PLACING LOGIC

    public void DisableAllColliders()
    {
        SetAllCollidersActive(false);
    }
    public void EnableAllColliders()
    {
        SetAllCollidersActive(true);
    }
    private void SetAllCollidersActive(bool active)
    {
        Collider[] c = GetComponentsInChildren<Collider>();

        for (int i = 0; i < c.Length; i++)
        {
            c[i].enabled = active;
        }
    }

    private void CacheDefaultMaterials()
    {
        default_materials = new List<Material>();
        default_material_locations = new List<MeshRenderer>();

        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            default_materials.Add(mr[i].sharedMaterial);
            default_material_locations.Add(mr[i]);
        }
    }
    public void SetMaterialBackToDefault()
    {
        // this one's a little bit more complex, because a part may have multiple materials and we want to support that

        for (int i = 0; i < default_materials.Count; i++)
        {
            default_material_locations[i].material = default_materials[i];
        }
    }

    // these two are wrappers for the below
    public void SetMaterialAsPreview()
    {
        SetMaterial(PartManager.Instance.m_preview);
    }
    public void SetMaterialAsInvalid()
    {
        SetMaterial(PartManager.Instance.m_badplacement);
    }
    public void SetMaterial(Material m)
    {
        // I should be safe in doing this???
        // I mean i don't see why there would be another MR in the children that's NOT part of the part model

        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].sharedMaterial = m;
        }
    }

    // places the part against a surface
    // basically does nothing if the part is not surface-mountable
    public void PositionPart(Vector3 point, Vector3 normal)
    {
        if (surfaceMount != null)
        {
            transform.forward = normal;
            transform.rotation *= surfaceMount.localRotation;

            transform.position = point - (surfaceMount.position - transform.position);
        } else
        {
            // the idea here is just to have the part follow the position
            transform.position = point;

            // but, have a sort of red "you can't place here" vibe
            // that part ^ will be done elsewhere
        }
    }

    #endregion

    public void Initialize()
    {
        partDataCollectors = new List<Func<string>>();

        PrepareSnappingPoints();

        onInitialize.Invoke();
    }

    private void PrepareSnappingPoints()
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

    public crft_genericpartdata AssemblePartData()
    {
        data.partName = GetPartName();

        data.position = transform.localPosition;
        data.euler_angles = transform.localEulerAngles;

        data.additional_part_data = MakeAdditionalPartData();

        return data;
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

    public string MakeAdditionalPartData()
    {
        string part_data = "";

        for (int i = 0; i < partDataCollectors.Count; i++)
        {
            part_data += partDataCollectors[i].Invoke();
        }

        return part_data;
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
