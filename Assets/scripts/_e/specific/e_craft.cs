using System.Collections.Generic;
using UnityEngine;

public class e_craft : MonoBehaviour
{
    public Transform t_partContainer;
    private e_genericentity eComp;

    public List<crft_genericpart> parts;

    private List<mtrl_containedresource> cachedResources;

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
        cachedResources = new List<mtrl_containedresource>();
    }


    public void UpdateCachedResourceCounts()
    {
        cachedResources.Clear();

        for (int i = 0; i < parts.Count; i++)
        {
            crft_resourcecontainer comp = parts[i].GetComponent<crft_resourcecontainer>();
            if (comp != null)
            {
                AddResourcesToCache(comp.containedResources);
            }
        }
    }

    void AddResourcesToCache(List<mtrl_containedresource> resources)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            AddResourceToCache(resources[i]);
        }
    }
    void AddResourceToCache(mtrl_containedresource resource)
    {
        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource.resource_name)
            {
                cachedResources[i].max_capacity += resource.max_capacity;
                cachedResources[i].current_capacity += resource.current_capacity;
            }
        }

        cachedResources.Add(new mtrl_containedresource(resource.resource_name, resource.current_capacity, resource.max_capacity));
    }



    // these two functions search through all the parts in the craft
    public float GetResourceAmount(string resource_name)
    {
        // just using the cache for this
        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource_name)
            {
                return cachedResources[i].current_capacity;
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
                return cachedResources[i].max_capacity;
            }
        }

        return 0;
    }


    // TODO: some way of doing part connections
    public void Initialize(crft_genericpartdata[] partData)
    {
        for (int i = 0; i < partData.Length; i++)
        {
            AddPart(partData[i]);
        }
    }

    public void AddPart(crft_genericpartdata partData)
    {
        GameObject g_newPart = Instantiate(PartManager.Instance.GetPartPrefabFromName(partData.partName));
        g_newPart.transform.SetParent(t_partContainer);

        g_newPart.transform.localPosition = partData.position;

        parts.Add(g_newPart.GetComponent<crft_genericpart>());

        UpdateCachedResourceCounts();
    }
}
