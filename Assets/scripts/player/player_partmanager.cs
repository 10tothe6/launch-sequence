using System;
using System.Collections.Generic;
using UnityEngine;

// handles the player's craft,
// which is edited from the Characer Editor-thing

// also handles the player placing parts in the build menu, 
// because I don't want to add a 12th script to this fucking gameobject for that

public class player_partmanager : MonoBehaviour
{
    private e_craft craftComp;
    public string[] defaultPartNames;

    [Space(12)]
    [Header("BUILD MODE")]
    private crft_genericpart placing_part;
    public Transform t_previewPartSlot;

    #region BUILDING MODE

    public void StartPlacingPart(string part_name)
    {
        GameObject g_new = Instantiate(PartManager.Instance.GetPartPrefabFromName(part_name), t_previewPartSlot);

        crft_genericpart comp = g_new.GetComponent<crft_genericpart>();

        comp.SetMaterialAsPreview();
        comp.DisableAllColliders();

        comp.Initialize();

        placing_part = comp;
    }

    #endregion

    // also contains building mode logic, but it contains other stuff too
    // so i figured leave it out of the #region
    private void Update()
    {
        if (placing_part != null)
        {
            RaycastHit hit;

            // didn't want to bother creating a separate LayerMask variable here
            // in part because I'll be switching to a much more central system later (ideally)
            if (util_physics.LookRaycast(out hit, 10f, LayerMask.GetMask(new string[]{"IsWalkable"})))
            {
                placing_part.PositionPart(hit.point, hit.normal);
            }

            // clicking to actually place the part
            if (Input.mouseButtonDownLeft)
            {
                EntityManager.SpawnNewSinglePartSpaceCraft(placing_part.GetPartName(), placing_part.transform.position, placing_part.transform.eulerAngles);
                
                // now for the clean-up
                Destroy(placing_part.gameObject);
                placing_part = null;
            }
        }
    }

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
