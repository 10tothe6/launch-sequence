using System.Collections.Generic;
using UnityEngine;

public class crft_resourcenetwork : MonoBehaviour
{
    // showing which resources we have, making it easier for other scripts to quickly check
    // makes it easy to see capacity and amount of the whole network too
    public List<mtrl_containedresource> cachedResources;

    // every part in the network
    public List<crft_resourcecontainer> network_elements;

    # region ADD/REMOVE

    public void AddResource(string resource_name, float amt)
    {
        // step 1: figure out how much of that resource we have, and CAN have
        float current_resource_amt = 0;
        float current_resource_cap = 0;

        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource_name)
            {
                current_resource_amt = cachedResources[i].current_capacity;
                current_resource_cap = cachedResources[i].max_capacity;
            }
        }

        if (current_resource_cap == 0 || current_resource_amt >= current_resource_cap) {return;}


        // step 2: modify the counts in every part
        for (int i = 0; i < network_elements.Count; i++)
        {
            float percentOfNetwork = (network_elements[i].GetResourceCapacity(resource_name) - network_elements[i].GetResourceAmount(resource_name)) / (current_resource_cap - current_resource_amt);

            network_elements[i].AddResource(resource_name, amt * percentOfNetwork);
        }


        OnNetworkModified();
    }

    public void RemoveResource(string resource_name, float amt)
    {
        // step 1: figure out how much of that resource we have, and CAN have
        float current_resource_amt = 0;
        float current_resource_cap = 0;

        for (int i = 0; i < cachedResources.Count; i++)
        {
            if (cachedResources[i].resource_name == resource_name)
            {
                current_resource_amt = cachedResources[i].current_capacity;
                current_resource_cap = cachedResources[i].max_capacity;
            }
        }

        if (current_resource_cap == 0 || current_resource_amt <= 0) {return;}

        // step 2: modify the counts in every part
        for (int i = 0; i < network_elements.Count; i++)
        {
            float percentOfNetwork = network_elements[i].GetResourceAmount(resource_name) / current_resource_amt;

            network_elements[i].RemoveResource(resource_name, amt * percentOfNetwork);
        }

        

        OnNetworkModified();
    }

    #endregion

    // called when a part is added or removed OR resources are added / removed
    private void OnNetworkModified()
    {
        UpdateCachedResources();
    }

    private void UpdateCachedResources()
    {
        
    }
}
