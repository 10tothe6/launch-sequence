using System.Collections.Generic;
using UnityEngine;

public class crft_resourcecontainer : MonoBehaviour
{
    public List<mtrl_containedresource> containedResources;

    public void AddResource(string resource_name, float amt)
    {
        for (int i = 0; i < containedResources.Count; i++)
        {
            if (containedResources[i].resource_name == resource_name)
            {
                containedResources[i].current_capacity += amt;
                containedResources[i].current_capacity = Mathf.Clamp(containedResources[i].current_capacity, 0f, containedResources[i].max_capacity);
            }
        }
    }
    public void RemoveResource(string resource_name, float amt)
    {
        for (int i = 0; i < containedResources.Count; i++)
        {
            if (containedResources[i].resource_name == resource_name)
            {
                containedResources[i].current_capacity -= amt;
                containedResources[i].current_capacity = Mathf.Clamp(containedResources[i].current_capacity, 0f, containedResources[i].max_capacity);
            }
        }
    }

    public float GetResourceAmount(string resource_name)
    {
        for (int i = 0; i < containedResources.Count; i++)
        {
            if (containedResources[i].resource_name == resource_name)
            {
                return containedResources[i].current_capacity;
            }
        }

        return 0;
    }
    public float GetResourceCapacity(string resource_name)
    {
        for (int i = 0; i < containedResources.Count; i++)
        {
            if (containedResources[i].resource_name == resource_name)
            {
                return containedResources[i].max_capacity;
            }
        }

        return 0;
    }
}
