using System.Collections.Generic;
using UnityEngine;

public class crft_resourcecontainer : MonoBehaviour
{
    public List<mtrl_containedresource> containedResources;

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
}
