using System.Collections.Generic;
using UnityEngine;

// does NOT include data for items as well, that's ItemManager.cs

public class PartManager : MonoBehaviour
{
    private static PartManager _instance;

    public static PartManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    public GameObject[] p_parts;



    // materials to be adopted by parts when in use in the editor or smth like that
    public Material m_preview;
    public Material m_badplacement;


    

    void Awake()
    {
        Instance = this;
    }
    
    public string[] GetAllPartNames()
    {
        string[] part_names = new string[p_parts.Length];

    

        return part_names;
    }


    // only the parts placeable from the build menu
    // (some are not directly buildable, like the ansible)
    public string[] GetBuildablePartNames()
    {
        List<string> toReturn = new List<string>();

        for (int i = 0; i < p_parts.Length; i++)
        {
            if (p_parts[i].GetComponent<crft_genericpart>().isBuildable)
            {
                toReturn.Add(p_parts[i].GetComponent<crft_genericpart>().GetPartName());
            }
        }

        return toReturn.ToArray();
    }

    public GameObject GetPartPrefabFromName(string partName)
    {
        for (int i = 0; i < p_parts.Length; i++)
        {
            if (p_parts[i].name == "part_" + partName)
            {
                return p_parts[i];
            }
        }

        return null;
    }
}
