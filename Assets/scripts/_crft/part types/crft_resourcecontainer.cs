using System.Collections.Generic;
using UnityEngine;

public class crft_resourcecontainer : MonoBehaviour
{
    public List<crft_resourcecompartement> compartements;
    
    // each resource container has a set of connections, 
    // and together they all make up a resource network

    // this is especially useful for batteries
    public List<crft_resourcecontainer> connected_containers;

    // wrapper
    public void AddResource(string name, float amt)
    {
        AddResource(new mtrl_containedresource(name, amt));
    }
    private void AddResource(mtrl_containedresource resource)
    {
        for (int i = 0; i < compartements.Count; i++)
        {
            mtrl_containedresource leftovers = compartements[i].AddResource(resource);

            resource.resource_amount = leftovers.resource_amount;

            if (resource.resource_amount <= 0)
            {
                return;
            }
        }
    }

    // wrapper
    public void RemoveResource(string name, float amt)
    {
        RemoveResource(new mtrl_containedresource(name, amt));
    }
    private void RemoveResource(mtrl_containedresource resource)
    {
        for (int i = 0; i < compartements.Count; i++)
        {
            mtrl_containedresource leftovers = compartements[i].RemoveResource(resource);

            resource.resource_amount = leftovers.resource_amount;

            if (resource.resource_amount <= 0)
            {
                return;
            }
        }
    }



    public float GetResourceAmount(string resource_name)
    {
        float sum = 0;

        for (int i = 0; i < compartements.Count; i++)
        {
            sum += compartements[i].GetCurrentResourceAmount(resource_name);
        }

        return sum;
    }

    public float GetResourceCapacity(string resource_name)
    {
        float sum = 0;





        return sum;
    }
}
