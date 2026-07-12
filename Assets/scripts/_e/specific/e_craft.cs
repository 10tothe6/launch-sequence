using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class e_craft : MonoBehaviour
{
    public Transform t_partContainer;
    private e_genericentity eComp;

    public List<crft_genericpart> parts;

    private List<mtrl_resourcecapacity> cachedResources;
    

    // cached lists of all part types for easy access
    public List<crft_inventory> inventories;
    public List<crft_antenna> antennas;


    // resource networks, basically trying to do what KSP did with its resource system but cleaner
    // (most significant difference being that we can have many resource networks on a single craft)
    public List<crft_resourcenetwork> resource_networks;

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
        cachedResources = new List<mtrl_resourcecapacity>();

        antennas = new List<crft_antenna>();
        inventories = new List<crft_inventory>();
    }

    public void DisablePhysics()
    {
        GetComponent<e_applyphysics>().Freeze();
    }
    public void EnablePhysics()
    {
        GetComponent<e_applyphysics>().UnFreeze();
    }

    #region RESOURCES


    private void UpdateCachedResourceCounts()
    {
        cachedResources.Clear();

        for (int i = 0; i < parts.Count; i++)
        {
            crft_resourcecontainer comp = parts[i].GetComponent<crft_resourcecontainer>();
            if (comp != null)
            {
                AddCompartementsToCache(comp.compartements);
            }
        }
    }

    private void AddCompartementsToCache(List<crft_resourcecompartement> compartements)
    {
        for (int i = 0; i < compartements.Count; i++)
        {
            AddResourceToCache(compartements[i].GetCapacityInformation());
        }
    }
    private void AddResourceToCache(mtrl_resourcecapacity resource)
    {
        bool foundExistingResourceEntry = false;
        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource.resource_name)
            {
                cachedResources[i].max_available_space += resource.max_available_space;
                cachedResources[i].resource_amount += resource.resource_amount;

                foundExistingResourceEntry = true;
            }
        }

        if (!foundExistingResourceEntry)
        {
            cachedResources.Add(new mtrl_resourcecapacity(resource.resource_name, resource.resource_amount, resource.max_available_space));
        }
    }


    public void FillAllResourceContainers()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            crft_resourcecontainer comp = parts[i].GetComponent<crft_resourcecontainer>();

            if (comp == null) {continue;}

            for (int j = 0; j  < comp.compartements.Count; j++)
            {
                comp.compartements[j].SetAsFull();
            }
        }
    }



    // TODO: this
    public void AddResource(string resource_name, float resource_amount)
    {
        
    }



    // these two functions search through all the parts in the craft
    public float GetResourceAmount(string resource_name)
    {
        // just using the cache for this
        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource_name)
            {
                return cachedResources[i].resource_amount;
            }
        }

        return 0;
    }
    public float GetResourceCapacity(string resource_name)
    {
        // just using the cache for this
        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource_name)
            {
                return cachedResources[i].max_available_space;
            }
        }

        return 0;
    }

    #endregion





    #region PARTS

    // called whenever a part is added or removed (or moved?? )
    private void OnPartListModified()
    {
        UpdateCachedResourceCounts();
        UpdateCachedPartLists();
    }


    // TODO: add some sort of way to only modify the parts in the lists that were changed, instead of rescanning the whole ship every time
    // a simple and easy optimization, esp. for large ships
    private void UpdateCachedPartLists()
    {
        antennas.Clear();
        inventories.Clear();

        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].GetComponent<crft_antenna>() != null)
            {
                antennas.Add(parts[i].GetComponent<crft_antenna>());
            }
            if (parts[i].GetComponent<crft_inventory>() != null)
            {
                inventories.Add(parts[i].GetComponent<crft_inventory>());
            }
        }
    }


    // TODO: some way of doing part connections
    public void Initialize(crft_craftdata data)
    {
        for (int i = 0; i < data.parts.Length; i++)
        {
            AddPart(data.parts[i]);
        }
    }

    public void RemovePart(GameObject part)
    {
        parts.Remove(part.GetComponent<crft_genericpart>());

        Destroy(part);

        OnPartListModified();
    }


    // wrapper function
    public GameObject AddPart(string partName)
    {
        crft_genericpartdata data = new crft_genericpartdata(partName, Vector3.zero);
        return AddPart(data);
    }

    public GameObject AddPart(crft_genericpartdata partData)
    {
        GameObject g_newPart = Instantiate(PartManager.Instance.GetPartPrefabFromName(partData.partName));
        g_newPart.transform.SetParent(t_partContainer);

        g_newPart.transform.localPosition = partData.position;

        parts.Add(g_newPart.GetComponent<crft_genericpart>());

        OnPartListModified();

        return g_newPart;
    }

    #endregion
}
