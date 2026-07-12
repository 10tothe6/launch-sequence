using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

[System.Serializable]
public class crft_resourcecompartement : MonoBehaviour
{
    // this is basically a whitelist
    // if empty, the compartement can hold any resource

    // entries here can also be tags, those entries have a t: at the start of them
    // (e.g. {water, ice}     or   {t:fluid, t:solid}    or {electricity})
    public List<string> allowed_resources;

    public float max_capacity;
    public float cached_fill_level; // the total amount of resources in the compartment right now

    public List<mtrl_containedresource> contained_resources;

    private void OnResourceCountsUpdated()
    {
        UpdateCachedValues();
    }

    private void UpdateCachedValues()
    {
        float fill_level = 0;

        for (int i = 0; i < contained_resources.Count; i++)
        {
            fill_level += contained_resources[i].resource_amount;
        }

        cached_fill_level = fill_level;
    }

    public float GetCurrentResourceAmount(string resource_name)
    {
        for (int i = 0; i < contained_resources.Count; i++)
        {
            if (contained_resources[i].resource_name == resource_name)
            {
                return contained_resources[i].resource_amount;
            }
        }

        return 0;
    }

    public mtrl_resourcecapacity GetCapacityInformation()
    {
        List<string> allowed = GetALlAllowedResources();

        if (allowed.Count > 1) {return null;}

        mtrl_resourcecapacity toReturn = new mtrl_resourcecapacity();

        toReturn.resource_name = allowed[0];
        toReturn.resource_amount = GetCurrentResourceAmount(allowed[0]);
        toReturn.max_available_space = 0;

        return toReturn;
    }
    public mtrl_resourcecapacity GetCapacityInformation(string resource_name)
    {
        mtrl_resourcecapacity toReturn = new mtrl_resourcecapacity();

        toReturn.resource_name = resource_name;
        toReturn.resource_amount = cached_fill_level;
        toReturn.max_available_space = max_capacity;

        return toReturn;
    }


    // returns the amount left over, if all of it was added it returns null
    public mtrl_containedresource AddResource(mtrl_containedresource resource)
    {
        mtrl_containedresource leftToAdd = new mtrl_containedresource(resource);


        float space_left = max_capacity - cached_fill_level;
        float amount_to_add = Mathf.Min(space_left, leftToAdd.resource_amount);

        bool foundExistingResourceEntry = false;
        for (int i = 0; i < contained_resources.Count; i++)
        {
            if (contained_resources[i].resource_name == leftToAdd.resource_name)
            {
                
                contained_resources[i].resource_amount += Mathf.Min(space_left, leftToAdd.resource_amount);
                leftToAdd.resource_amount -= amount_to_add;

                foundExistingResourceEntry = true;
                break;
            }
        }

        if (!foundExistingResourceEntry)
        {
            contained_resources.Add(new mtrl_containedresource(leftToAdd.resource_name, amount_to_add));
            leftToAdd.resource_amount -= amount_to_add;
        }


        OnResourceCountsUpdated();

        return leftToAdd;
    }
    // returns the amount that COULDN'T be removed, if null then the removal worked
    public mtrl_containedresource RemoveResource(mtrl_containedresource resource)
    {
        mtrl_containedresource leftToRemove = new mtrl_containedresource(resource);



        for (int i = 0; i < contained_resources.Count; i++)
        {
            if (contained_resources[i].resource_name == leftToRemove.resource_name)
            {
                float amount_to_remove = Mathf.Min(contained_resources[i].resource_amount, leftToRemove.resource_amount);
                contained_resources[i].resource_amount -= amount_to_remove;
                leftToRemove.resource_amount -= amount_to_remove;
                
                break;
            }
        }


        OnResourceCountsUpdated();

        return leftToRemove;
    }

    public bool IsResourceAllowed(string resource_name)
    {
        return GetALlAllowedResources().Contains(resource_name);
    }

    // using the whitelist, it grabs all the resources that either are directly listed or have a tag thats listed
    public List<string> GetALlAllowedResources()
    {
        List<string> toReturn = new List<string>();


        for (int i = 0; i < allowed_resources.Count; i++)
        {
            if (allowed_resources[i].Length < 2)
            {
                toReturn.Add(allowed_resources[i]);
            }
            else if (allowed_resources[i].Substring(0, 2) == "t:")
            {
                List<string> resources_with_tag = ItemManager.GetAllResourceNamesWithTag(allowed_resources[i].Substring(2));

                // not worrying about duplicates here, we worry about those at the end
                for (int j = 0; j < resources_with_tag.Count; j++)
                {
                    toReturn.Add(resources_with_tag[j]);
                }
            } else
            {
                toReturn.Add(allowed_resources[i]);
            }
        }

        // because of tags, we may have listed certain resource twice
        return util_string.RemoveDuplicatesFromList(toReturn);
    }

    // this really only means anything if only ONE resource is allowed
    // but it is useful for things like batteries
    public void SetAsFull()
    {
        List<string> allowed = GetALlAllowedResources();

        if (allowed.Count > 1) {return;}

        contained_resources.Clear();

        AddResource(new mtrl_containedresource(allowed[0], max_capacity));
    }
}
