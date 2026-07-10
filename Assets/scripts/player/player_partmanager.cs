using System;
using System.Collections.Generic;
using UnityEngine;

public class player_partmanager : MonoBehaviour
{
    private e_craft craftComp;
    public string[] defaultPartNames;

    void Awake()
    {
        craftComp = GetComponent<e_craft>();

        InitializeFirstTime();
    }

    public void InitializeFirstTime()
    {
        AddDefaultParts();
    }

    public Func<inv_inventorydata>[] GetInventorySources()
    {
        List<Func<inv_inventorydata>> toReturn = new List<Func<inv_inventorydata>>();

        for (int i = 0; i < craftComp.parts.Count; i++)
        {
            if (craftComp.parts[i].GetComponent<crft_inventory>() != null)
            {
                int j = i;
                toReturn.Add(() => {return craftComp.parts[j].GetComponent<crft_inventory>().data; });
            }
        }

        return toReturn.ToArray();
    }

    public void AddDefaultParts()
    {
        for (int i = 0; i < defaultPartNames.Length; i++)
        {
            // position doesn't really matter for these cuz they have no model
            crft_genericpartdata newPartData = new crft_genericpartdata(defaultPartNames[i], Vector3.zero);

            craftComp.AddPart(newPartData);

            // adding the default resources
            craftComp.FillAllResourceContainers();
        }
    }
}
